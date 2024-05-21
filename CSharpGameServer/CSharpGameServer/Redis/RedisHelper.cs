using StackExchange.Redis;

namespace CSharpGameServer.Redis
{
    public class RedisHelper : IDisposable
    {
        private readonly ConnectionMultiplexer redis;
        private readonly IDatabase database;

        public RedisHelper(string connectionString)
        {
            redis = ConnectionMultiplexer.Connect(connectionString);
            database = redis.GetDatabase();
        }

        public void SetValue(string key, string value)
        {
            database.StringAppend(key, value);
        }

        public string GetValue(string key)
        {
            return database.StringGet(key);
        }

        public bool DeleteKey(string key)
        {
            return database.KeyDelete(key);
        }

        public void Dispose()
        {
            redis?.Dispose();
        }
    }
}
