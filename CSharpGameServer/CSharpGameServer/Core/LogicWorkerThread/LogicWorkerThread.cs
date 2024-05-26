using CSharpGameServer.Protocol;
using System.Diagnostics;

namespace CSharpGameServer.Core.LogicWorkerThread
{
    internal class LogicWorker
    {
        private ManualResetEvent doWorkThreadEvent = new ManualResetEvent(false);
        private ManualResetEvent stopThreadEvent = new ManualResetEvent(false);
        private Thread? thread = null;
        private int threadId;
        private Queue<Tuple<Client, RequestPacket>> ItemStoreQueue = new Queue<Tuple<Client, RequestPacket>>();
        private object itemStoreQueueLock = new object();

        public LogicWorker(int inThreadId)
        {
            threadId = inThreadId;
            thread = new Thread(() => StartWorkerThread());
            thread.Start();
        }

        private void StartWorkerThread()
        {
            WaitHandle[] threadEventes = new WaitHandle[] { doWorkThreadEvent, stopThreadEvent };
            List<Tuple<Client, RequestPacket>> processList = new List<Tuple<Client, RequestPacket>>();

            while (true)
            {
                int eventIndex = WaitHandle.WaitAny(threadEventes);
                if (eventIndex == 1)
                {
                    break;
                }

                lock (itemStoreQueueLock)
                {
                    foreach(var packet in ItemStoreQueue)
                    {
                        processList.Add(packet);
                    }

                    ItemStoreQueue.Clear();
                }

                foreach (var processItem in processList)
                {
                    PacketHandlerManager.Instance.CallHandler(processItem.Item1, processItem.Item2);
                }
                processList.Clear();
            }
        }

        public void PushPacket(Client targetClient, RequestPacket packet)
        {
            lock (itemStoreQueueLock)
            {
                ItemStoreQueue.Enqueue(Tuple.Create(targetClient, packet));
            }
        }

        public void DoWork()
        {
            if (thread == null)
            {
                return;
            }

            doWorkThreadEvent.Set();
        }

        public void StopThread()
        {
            if (thread == null)
            {
                return;
            }

            stopThreadEvent.Set();
            thread.Join();

            Console.WriteLine("Thread {0} is stopped", threadId);
        }
    }

    public class LogicWorkerThreadManager
    {
        private List<LogicWorker> workerThreadList;
        private int threadSize;

        public LogicWorkerThreadManager()
        {
            workerThreadList = new List<LogicWorker>();
        }

        public void MakeThreads(int inThreadSize)
        {
            threadSize = inThreadSize;

            for (int threadId = 0; threadId < inThreadSize; threadId++)
            {
                workerThreadList.Add(new LogicWorker(threadId));
            }
        }

        public void PushPacket(Client targetClient, RequestPacket packet)
        {
            workerThreadList[GetThreadId(targetClient.clientSessionId)].PushPacket(targetClient, packet);
        }

        public void DoWork(ulong ownerId)
        {
            workerThreadList[GetThreadId(ownerId)].DoWork();
        }

        public void StopAllLogicThreads()
        {
            foreach (LogicWorker workerThread in workerThreadList)
            {
                workerThread.StopThread();
            }
            workerThreadList.Clear();
            Console.WriteLine("All logic threads are stopped");
        }

        private int GetThreadId(ulong ownerId)
        {
            ulong devided = (ownerId / (ulong)threadSize);
            return (int)(ownerId - (devided * (ulong)(threadSize)));
        }
    }
}
