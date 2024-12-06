﻿namespace CSharpGameServer.DB
{
    public class DbConnectionManager
    {
        private static DbConnectionManager? instance = null;
        private readonly string connectionString;
        private readonly int maxPoolSize = 10;
        private Queue<DbConnection?> connectionPool = new Queue<DbConnection?>();
        private object connectionPoolLock = new object();

        private DbConnectionManager(string server, string db, string userId, string password)
        {
            connectionString = $"Server={server};Database={db};Uid={userId};Pwd={password};";
        }

        public static void Initialize(string server, string db, string userId, string password)
        {
            if (instance == null)
            {
                if (instance == null)
                {
                    instance = new DbConnectionManager(server, db, userId, password);
                }
            }
        }

        public static DbConnectionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("DBConnectionManager is not initialized. Call Initialize first.");
                }
                return instance;
            }
        }

        public DbConnection? GetConnection()
        {
            lock (connectionPoolLock)
            {
                if (connectionPool.Count > 0)
                {
                    return connectionPool.Dequeue();
                }
            }

            return CreateConnection();
        }

        private DbConnection CreateConnection()
        {
            var connection = new DbConnection();
            connection.CreateConnection(connectionString);

            return connection;
        }

        public void ReleaseConnection(DbConnection? connection)
        {
            if (connection == null)
            {
                return;
            }

            lock (connectionPoolLock)
            {
                if (connectionPool.Count < maxPoolSize)
                {
                    connectionPool.Enqueue(connection);
                }
                else
                {
                    connection.CloseConnection();
                }
            }
        }
    }
}
