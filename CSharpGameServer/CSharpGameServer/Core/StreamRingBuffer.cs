namespace CSharpGameServer.Core
{
    public class StreamRingBuffer
    {
        static public readonly uint defaultBufferSize = 8192;

        private byte[] buffer;
        private uint bufferSize;
        
        private uint head = 0;
        private uint tail = 0;

        public StreamRingBuffer()
        {
            bufferSize = defaultBufferSize;
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
            uint inputSize = (uint)inputData.Length;
            if (inputSize > GetUseSize() ||
                inputSize == 0)
            {
                return false;
            }

            if (tail + inputSize > buffer.Length)
            {
                uint tailToBufferEnd = bufferSize - tail;
                uint tailPos = inputSize - tailToBufferEnd;

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
            byte[]? data = PeekData(popSize);
            if (data != null)
            {
                uint headToBufferEnd = bufferSize - head;
                if (headToBufferEnd < popSize)
                {
                    head = popSize - headToBufferEnd;
                }
                else
                {
                    head += popSize;
                }
            }

            return data;
        }

        public byte[]? PeekData(uint peekSize)
        {
            if (GetUseSize() < peekSize)
            {
                return null;
            }

            return GetData(peekSize);
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
            if (head != tail)
            {
                return false;
            }

            return true;
        }

        private byte[] GetData(uint dataSize)
        {
            byte[] data = new byte[dataSize];
            if (head < tail)
            {
                Array.Copy(buffer, head, data, 0, head + dataSize);
            }
            else
            {
                uint tailToEndSize = bufferSize - tail;
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
