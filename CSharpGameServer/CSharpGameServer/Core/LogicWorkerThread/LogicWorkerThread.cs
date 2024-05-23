namespace CSharpGameServer.Core.LogicWorkerThread
{
    internal class LogicWorker
    {
        private ManualResetEvent doWorkThreadEvent = new ManualResetEvent(false);
        private ManualResetEvent stopThreadEvent = new ManualResetEvent(false);
        private Thread? thread = null;
        private int threadId;

        public LogicWorker(int inThreadId)
        {
            threadId = inThreadId;
            thread = new Thread(() => StartWorkerThread());
            thread.Start();
        }

        private void StartWorkerThread()
        {
            WaitHandle[] threadEventes = new WaitHandle[] { doWorkThreadEvent, stopThreadEvent };
            while (true)
            {
                int eventIndex = WaitHandle.WaitAny(threadEventes);
                if(eventIndex == 1)
                {
                    break;
                }

                // do something
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
        }
    }

    public class LogicWorkerThreadManager
    {
        private static LogicWorkerThreadManager? instance = null;
        private static readonly object constructorLock = new object();
        private List<LogicWorker> workerThreadList;
        private int threadSize;

        public static LogicWorkerThreadManager Instance(int inThreadSize)
        {
            if (instance == null)
            {
                lock (constructorLock)
                {
                    if (instance == null)
                    {
                        instance = new LogicWorkerThreadManager(inThreadSize);
                    }
                }
            }

            return instance;
        }

        public LogicWorkerThreadManager(int inThreadSize)
        {
            threadSize = inThreadSize;
            workerThreadList = new List<LogicWorker>();

            for (int threadId = 0; threadId < inThreadSize; threadId++)
            {
                workerThreadList.Add(new LogicWorker(threadId));
            }
        }

        public void DoWork(ulong ownerId)
        {
            ulong devided = (ownerId / (ulong)threadSize);
            int logicThreadId = (int)(ownerId - (devided * (ulong)(threadSize)));

            workerThreadList[logicThreadId].DoWork();
        }
    }
}
