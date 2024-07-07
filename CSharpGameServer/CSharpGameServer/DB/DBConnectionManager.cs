namespace CSharpGameServer.DB
{
    public class DBConnectionManager
    {
        private static DBConnectionManager? instance = null;
        private readonly string connectionString;
        private readonly int maxPoolSize = 10;
        private Queue<DBConnection> connectionPool = new Queue<DBConnection>();
        private object connectionPoolLock = new object();

        private DBConnectionManager(string server, string db, string userId, string password)
        {
            connectionString = $"Server={server};Database={db};Uid={userId};Pwd={password};";
        }
        public static void Initialize(string server, string db, string userId, string password)
        {
            if (instance == null)
            {
                if (instance == null)
                {
                    instance = new DBConnectionManager(server, db, userId, password);
                }
            }
        }

        public static DBConnectionManager Instance
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

        public DBConnection GetConnection()
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

        private DBConnection CreateConnection()
        {
            var connection = new DBConnection();
            connection.CreateConnection(connectionString);

            return connection;
        }

        public void ReleaseConnection(DBConnection connection)
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
