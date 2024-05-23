using MySql.Data.MySqlClient;

namespace CSharpGameServer.DB
{
    public class DBConnection
    {
        private MySqlConnection? connection = null;

        public void CreateConnection(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
        }

        public void CloseConnection()
        {
            if(connection != null)
            {
                connection.Close();
            }
        }

        public MySqlCommand MakeQueryCommand(string query)
        {
            return new MySqlCommand(query, connection);
        }
    }
}
