using CSharpGameServer.PacketBase;
using MessageQueueId = int;

namespace CSharpGameServer.Core
{
    internal class MessageQueue(MessageQueueId inMessageQueueId)
    {
        private readonly Queue<RequestPacket> messageQueue = new();
        private readonly Lock queueLock = new();
        private readonly MessageQueueId messageQueueId = inMessageQueueId;

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

    public class MessageQueueManager(int inQueueListSize, int inLogicThreadSize)
    {
        private static MessageQueueManager? _instance;
        private static readonly Lock ConstructorLock = new();
        private readonly MessageQueue[] queueList = new MessageQueue[inQueueListSize];

        public static MessageQueueManager Instance(int inQueueListSize, int inLogicThreadSize)
        {
            if (_instance != null)
            {
                return _instance;
            }

            lock (ConstructorLock)
            {
                _instance ??= new MessageQueueManager(inQueueListSize, inLogicThreadSize);
            }

            return _instance;
        }

        public void PushToQueue(ulong clientSessionId, RequestPacket packet)
        {
            var divided = (clientSessionId / (ulong)inLogicThreadSize);
            var logicThreadId = (int)(clientSessionId - (divided * (ulong)(inLogicThreadSize)));
            
            queueList[logicThreadId].PushToQueue(packet);
        }

        public RequestPacket PopFromQueue(int logicThreadId)
        {
            return queueList[logicThreadId].PopFromQueue();
        }
    }
}
