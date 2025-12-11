using StackExchange.Redis;

namespace CSharpGameServer.Redis
{
    public class RedisHelper : IDisposable
    {
        private static RedisHelper? _instance;
        private static readonly ConnectionMultiplexer? Redis = null;
        private static readonly Lock ConstructorLock = new();
        private readonly IDatabase? database;
        private bool disposed;

        public static RedisHelper Instance(string connectionString)
        {
            if (_instance != null)
            {
                return _instance;
            }

            lock (ConstructorLock)
            {
                _instance ??= new RedisHelper(connectionString);
            }

            return _instance;
        }

        private RedisHelper(string connectionString)
        {
            var connection = ConnectionMultiplexer.Connect(connectionString);
            database = connection.GetDatabase();
        }

        public void SetValue(string key, string value)
        {
            database?.StringAppend(key, value);
        }

        public async Task SetValueAsync(string key, string value)
        {
            if (database != null)
            {
                await database.StringAppendAsync(key, value);
            }
        }

        public RedisValue? GetValue(string key)
        {
            return database?.StringGet(key);
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
            return database != null && database.KeyDelete(key);
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
            return database != null && database.KeyExpire(key, time);
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
            return database != null && database.KeyExpire(key, time);
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                Redis?.Dispose();
            }

            disposed = true;
        }
    }
}
