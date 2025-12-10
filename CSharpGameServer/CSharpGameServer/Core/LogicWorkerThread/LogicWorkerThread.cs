using CSharpGameServer.Logger;
using CSharpGameServer.Packet;
using CSharpGameServer.PacketBase;

namespace CSharpGameServer.Core.LogicWorkerThread
{
    internal class LogicWorker
    {
        private readonly ManualResetEvent doWorkThreadEvent = new(false);
        private readonly ManualResetEvent stopThreadEvent = new(false);
        private readonly Thread? thread;
        private readonly int threadId;
        private readonly Queue<Tuple<Client, RequestPacket>> itemStoreQueue = new();
        private readonly object itemStoreQueueLock = new();
        private int isRunning;
        private const int IsTrue = 1;
        private const int IsFalse = 0;

        public LogicWorker(int inThreadId)
        {
            threadId = inThreadId;
            thread = new Thread(StartWorkerThread);
            thread.Start();
        }

        private void StartWorkerThread()
        {
            WaitHandle[] threadEvents = [doWorkThreadEvent, stopThreadEvent];
            List<Tuple<Client, RequestPacket>> processList = [];
            SetIsRunning(IsTrue);

            while (true)
            {
                var eventIndex = WaitHandle.WaitAny(threadEvents);
                if (eventIndex == 1)
                {
                    break;
                }

                lock (itemStoreQueueLock)
                {
                    processList.AddRange(itemStoreQueue);

                    itemStoreQueue.Clear();
                    SetIsRunning(IsFalse);
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
                if (IsRunningThread())
                {
                    return;
                }

                itemStoreQueue.Enqueue(Tuple.Create(targetClient, packet));
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

            LoggerManager.Instance.WriteLogDebug("Thread {threadId} is stopped", threadId);
        }

        private void SetIsRunning(int setValue)
        {
            Interlocked.Exchange(ref isRunning, setValue);
        }

        public bool IsRunningThread()
        {
            return Interlocked.CompareExchange(ref isRunning, 0, 0) == 1;
        }
    }

    public class LogicWorkerThreadManager
    {
        private readonly List<LogicWorker> workerThreadList = [];
        private int threadSize;

        public void MakeThreads(int inThreadSize)
        {
            threadSize = inThreadSize;

            for (var threadId = 0; threadId < inThreadSize; threadId++)
            {
                workerThreadList.Add(new LogicWorker(threadId));
            }
        }

        public void PushPacket(Client targetClient, RequestPacket packet)
        {
            var threadId = GetThreadId(targetClient.ClientSessionId);
            if (workerThreadList[threadId].IsRunningThread())
            {
                workerThreadList[threadId].PushPacket(targetClient, packet);
            }
        }

        public void DoWork(ulong ownerId)
        {
            workerThreadList[GetThreadId(ownerId)].DoWork();
        }

        public void StopAllLogicThreads()
        {
            foreach (var workerThread in workerThreadList)
            {
                workerThread.StopThread();
            }
            workerThreadList.Clear();
            LoggerManager.Instance.WriteLogDebug("All logic threads are stopped");
        }

        private int GetThreadId(ulong ownerId)
        {
            var divided = (ownerId / (ulong)threadSize);
            return (int)(ownerId - (divided * (ulong)(threadSize)));
        }
    }
}
