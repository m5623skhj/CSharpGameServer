﻿using CSharpGameServer.Logger;
using CSharpGameServer.Packet;
using CSharpGameServer.PacketBase;

namespace CSharpGameServer.Core.LogicWorkerThread
{
    internal class LogicWorker
    {
        private ManualResetEvent doWorkThreadEvent = new ManualResetEvent(false);
        private ManualResetEvent stopThreadEvent = new ManualResetEvent(false);
        private Thread? thread;
        private int threadId;
        private Queue<Tuple<Client, RequestPacket>> itemStoreQueue = new Queue<Tuple<Client, RequestPacket>>();
        private object itemStoreQueueLock = new object();
        private int isRunning;
        private readonly int isTrue = 1;
        private readonly int isFalse = 0;

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
            SetIsRunning(isTrue);

            while (true)
            {
                int eventIndex = WaitHandle.WaitAny(threadEventes);
                if (eventIndex == 1)
                {
                    break;
                }

                lock (itemStoreQueueLock)
                {
                    foreach (var packet in itemStoreQueue)
                    {
                        processList.Add(packet);
                    }

                    itemStoreQueue.Clear();
                    SetIsRunning(isFalse);
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
            var threadId = GetThreadId(targetClient.clientSessionId);
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
            foreach (LogicWorker workerThread in workerThreadList)
            {
                workerThread.StopThread();
            }
            workerThreadList.Clear();
            LoggerManager.Instance.WriteLogDebug("All logic threads are stopped");
        }

        private int GetThreadId(ulong ownerId)
        {
            ulong devided = (ownerId / (ulong)threadSize);
            return (int)(ownerId - (devided * (ulong)(threadSize)));
        }
    }
}
