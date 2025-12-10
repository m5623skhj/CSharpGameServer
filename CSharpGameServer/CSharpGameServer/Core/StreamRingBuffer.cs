namespace CSharpGameServer.Core
{
    public class StreamRingBuffer
    {
        public static readonly uint DefaultBufferSize = 8192;

        private byte[] buffer;
        private uint bufferSize;
        
        private uint head;
        private uint tail;

        public StreamRingBuffer()
        {
            bufferSize = DefaultBufferSize;
            buffer = new byte[bufferSize];
        }

        public StreamRingBuffer(uint inBufferSize)
        {
            bufferSize = inBufferSize;
            buffer = new byte[bufferSize];
        }

        public void Resize(uint inBufferSize)
        {
            bufferSize = inBufferSize;
            buffer = new byte[inBufferSize];
        }

        public bool PushData(byte[] inputData)
        {
            var inputSize = (uint)inputData.Length;
            if (inputSize > GetUseSize() ||
                inputSize == 0)
            {
                return false;
            }

            if (tail + inputSize > buffer.Length)
            {
                var tailToBufferEnd = bufferSize - tail;
                var tailPos = inputSize - tailToBufferEnd;

                Array.Copy(buffer, tail, inputData, 0, tailToBufferEnd);
                Array.Copy(buffer, 0, inputData, tailToBufferEnd, tailPos);
                tail = tailPos;
            }
            else
            {
                Array.Copy(buffer, tail, inputData, 0, inputSize);
                tail += inputSize;
            }

            return true;
        }

        public byte[]? PopData(uint popSize)
        {
            var data = PeekData(popSize);
            if (data == null)
            {
                return data;
            }

            var headToBufferEnd = bufferSize - head;
            if (headToBufferEnd < popSize)
            {
                head = popSize - headToBufferEnd;
            }
            else
            {
                head += popSize;
            }

            return data;
        }

        public byte[]? PeekData(uint peekSize)
        {
            return GetUseSize() < peekSize ? null : GetData(peekSize);
        }

        public byte[] PeekAllData()
        {
            return GetData(GetUseSize());
        }

        public bool EraseData(uint eraseSize)
        {
            if (GetUseSize() < eraseSize)
            {
                return false;
            }

            if (head > tail)
            {
                head = head + eraseSize - bufferSize;
            }
            else
            {
                head += eraseSize;
            }

            return true;
        }

        public uint GetUseSize()
        {
            if (head > tail)
            {
                return bufferSize - head + tail;
            }
            else if (head < tail) 
            {
                return tail - head;
            }

            return 0;
        }

        public bool IsEmpty()
        {
            return head == tail;
        }

        private byte[] GetData(uint dataSize)
        {
            var data = new byte[dataSize];
            if (head < tail)
            {
                Array.Copy(buffer, head, data, 0, head + dataSize);
            }
            else
            {
                var tailToEndSize = bufferSize - tail;
                if (tailToEndSize >= dataSize)
                {
                    Array.Copy(buffer, tail, data, 0, dataSize);
                }
                else
                {
                    Array.Copy(buffer, tail, data, 0, tailToEndSize);
                    Array.Copy(buffer, tail, data, tailToEndSize, dataSize - tailToEndSize);
                }
            }

            return data;
        }
    }
}
