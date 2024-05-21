using StackExchange.Redis;
using System.Diagnostics.Metrics;

namespace CSharpGameServer.Redis
{
    public class RedisHelper : IDisposable
    {
        private static RedisHelper? instance = null;
        private static ConnectionMultiplexer? redis = null;
        private static readonly object constructorLock = new object();
        private readonly IDatabase? database = null;

        public static RedisHelper Instance(string connectionString)
        {
            if (instance == null)
            {
                lock (constructorLock)
                {
                    if(instance == null)
                    {
                        instance = new RedisHelper(connectionString);
                    }
                }
            }

            return instance;
        }

        private RedisHelper(string connectionString)
        {
            var connection = ConnectionMultiplexer.Connect(connectionString);
            database = connection.GetDatabase();
        }

        public void SetValue(string key, string value)
        {
            if (database != null)
            {
                database.StringAppend(key, value);
            }
        }

        public string? GetValue(string key)
        {
            if (database != null)
            {
                return database.StringGet(key);
            }

            return null;
        }

        public bool DeleteKey(string key)
        {
            if (database != null)
            {
                return database.KeyDelete(key);
            }

            return false;
        }

        public void Dispose()
        {
            redis?.Dispose();
        }
    }
}
