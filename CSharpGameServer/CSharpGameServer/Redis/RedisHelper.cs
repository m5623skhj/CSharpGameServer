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
                    if (instance == null)
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

        public async Task SetValueAsync(string key, string value)
        {
            if(database != null)
            {
                await database.StringAppendAsync(key, value);
            }
        }

        public RedisValue? GetValue(string key)
        {
            if (database != null)
            {
                return database.StringGet(key);
            }

            return null;
        }

        public async Task<RedisValue?> GetValueAsync(string key)
        {
            if (database != null)
            {
                return await database.StringGetAsync(key);
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

        public async Task<bool> DeleteKeyAsync(string key)
        {
            if (database != null)
            {
                return await database.KeyDeleteAsync(key);
            }

            return false;
        }

        public bool SetKeyExpiry(string key, TimeSpan time)
        {
            if (database != null)
            {
                return database.KeyExpire(key, time);
            }

            return false;
        }

        public async Task<bool> SetKeyExpiryAsync(string key, TimeSpan expiry)
        {
            if (database != null)
            {
                return await database.KeyExpireAsync(key, expiry);
            }

            return false;
        }

        public bool RefreshKeyExpiry(string key, TimeSpan time)
        {
            if (database != null)
            {
                return database.KeyExpire(key, time);
            }

            return false;
        }

        public async Task<bool> RefreshKeyExpiryAsync(string key, TimeSpan expiry)
        {
            if (database != null)
            {
                return await database.KeyExpireAsync(key, expiry);
            }

            return false;
        }

        public void Dispose()
        {
            redis?.Dispose();
        }
    }
}
