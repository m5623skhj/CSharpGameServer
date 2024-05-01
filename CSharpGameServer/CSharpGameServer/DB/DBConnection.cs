using MySql.Data.MySqlClient;

namespace CSharpGameServer.DB
{
    public class DBConnection
    {
        private MySqlConnection connection;

        public void CreateConnection(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public MySqlCommand MakeQueryCommand(string query)
        {
            return new MySqlCommand(query, connection);
        }
    }
}
