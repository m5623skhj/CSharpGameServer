using MySql.Data.MySqlClient;

namespace CSharpGameServer.DB
{
    public class DBConnectionManager
    {
        private readonly string connectionString;
        private readonly int maxPoolSize = 10;
        private Queue<MySqlConnection> connectionPool = new Queue<MySqlConnection>();
        private object connectionPoolLock = new object();

        public DBConnectionManager(string server, string db, string userId, string password)
        {
            connectionString = $"Server={server};Database={db};Uid={userId};Pwd={password};";
        }

        public MySqlConnection GetConnection()
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

        private MySqlConnection CreateConnection()
        {
            var connection = new MySqlConnection(connectionString);
            connection.Open();

            return connection;
        }

        public void ReleaseConnection(MySqlConnection connection)
        {
            if (connection == null)
            {
                return;
            }

            lock (connectionPoolLock)
            {
                if(connectionPool.Count < maxPoolSize)
                {
                    connectionPool.Enqueue(connection);
                }
                else
                {
                    connection.Close();
                }
            }
        }
    }
}
