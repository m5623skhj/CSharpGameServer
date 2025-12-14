using System.Text;

namespace CSharpGameServer.etc
{
    public static class FixedStringUtil
    {
        public static unsafe void Write(string? src, byte* dest, int maxLen)
        {
            if (src != null)
            {
                var bytes = Encoding.UTF8.GetBytes(src);

                if (bytes.Length >= maxLen)
                {
                    throw new ArgumentException("String length exceeds fixed buffer size.");
                }

                for (var i = 0; i < maxLen; i++)
                {
                    dest[i] = 0;
                }

                for (var i = 0; i < bytes.Length; i++)
                {
                    dest[i] = bytes[i];
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(src));
            }
        }

        public static unsafe string Read(byte* src, int maxLen)
        {
            var len = 0;
            while (len < maxLen && src[len] != 0)
            {
                len++;
            }

            return Encoding.UTF8.GetString(src, len);
        }
    }
}
