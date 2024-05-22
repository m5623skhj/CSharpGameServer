using CSharpGameServer.Protocol;
using MessageQueueId = int;

namespace CSharpGameServer.Core
{
    internal class MessageQueue
    {
        private Queue<RequestPacket> messageQueue = new Queue<RequestPacket>();
        private readonly object queueLock = new object();
        private readonly MessageQueueId messageQueueId;

        public MessageQueue(MessageQueueId inMessageQueueId)
        {
            messageQueueId = inMessageQueueId;
        }

        public void PushToQueue(RequestPacket packet)
        {
            lock (queueLock)
            {
                messageQueue.Enqueue(packet);
            }
        }

        public RequestPacket PopFromQueue()
        {
            lock (queueLock)
            {
                return messageQueue.Dequeue();
            }
        }
    }

    public class MessageQueueManager
    {
        private static MessageQueueManager? instance = null;
        private static readonly object constructorLock = new object();
        private MessageQueue[] queueList;
        private readonly int logicThreadSize;

        public static MessageQueueManager Instance(int inQueueListSize, int inLogicThreadSize)
        {
            if (instance == null)
            {
                lock (constructorLock)
                {
                    if (instance == null)
                    {
                        instance = new MessageQueueManager(inQueueListSize, inLogicThreadSize);
                    }
                }
            }

            return instance;
        }

        public MessageQueueManager(int inQueueListSize, int inLogicThreadSize)
        {
            queueList = new MessageQueue[inQueueListSize];
            logicThreadSize = inLogicThreadSize;
        }

        public void PushToQueue(ulong clientSessionId, RequestPacket packet)
        {
            ulong devided = (clientSessionId / (ulong)logicThreadSize);
            int logicThreadId = (int)(clientSessionId - (devided * (ulong)(logicThreadSize)));
            
            queueList[logicThreadId].PushToQueue(packet);
        }

        public RequestPacket PopFromQueue(int logicThreadId)
        {
            return queueList[logicThreadId].PopFromQueue();
        }
    }
}
