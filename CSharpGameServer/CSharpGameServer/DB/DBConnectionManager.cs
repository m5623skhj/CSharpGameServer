namespace CSharpGameServer.DB
{
    public class DBConnectionManager
    {
        private readonly string connectionString;
        private readonly int maxPoolSize = 10;
        private Queue<DBConnection> connectionPool = new Queue<DBConnection>();
        private object connectionPoolLock = new object();

        public DBConnectionManager(string server, string db, string userId, string password)
        {
            connectionString = $"Server={server};Database={db};Uid={userId};Pwd={password};";
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
