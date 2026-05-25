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
            head = 0;
            tail = 0;
        }

        public bool PushData(byte[] inputData)
        {
            var inputSize = (uint)inputData.Length;
            if (inputSize > GetFreeSize() || inputSize == 0)
            {
                return false;
            }

            if (tail + inputSize > bufferSize)
            {
                var tailToBufferEnd = bufferSize - tail;
                var tailPos = inputSize - tailToBufferEnd;

                Array.Copy(inputData, 0, buffer, tail, tailToBufferEnd);
                Array.Copy(inputData, tailToBufferEnd, buffer, 0, tailPos);
                tail = tailPos;
            }
            else
            {
                Array.Copy(inputData, 0, buffer, tail, inputSize);
                tail += inputSize;
            }

            if (tail == bufferSize)
            {
                tail = 0;
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

            head = (head + popSize) % bufferSize;

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

            head = (head + eraseSize) % bufferSize;

            return true;
        }

        public uint GetUseSize()
        {
            if (head > tail)
            {
                return bufferSize - head + tail;
            }
            
            if (head < tail) 
            {
                return tail - head;
            }

            return 0;
        }

        public uint GetFreeSize()
        {
            return bufferSize - GetUseSize() - 1;
        }

        public bool IsEmpty()
        {
            return head == tail;
        }

        private byte[] GetData(uint dataSize)
        {
            var data = new byte[dataSize];
            if (dataSize == 0)
            {
                return data;
            }

            if (head < tail)
            {
                Array.Copy(buffer, head, data, 0, dataSize);
            }
            else
            {
                var headToEndSize = bufferSize - head;
                if (headToEndSize >= dataSize)
                {
                    Array.Copy(buffer, head, data, 0, dataSize);
                }
                else
                {
                    Array.Copy(buffer, head, data, 0, headToEndSize);
                    Array.Copy(buffer, 0, data, headToEndSize, dataSize - headToEndSize);
                }
            }

            return data;
        }
    }
}
