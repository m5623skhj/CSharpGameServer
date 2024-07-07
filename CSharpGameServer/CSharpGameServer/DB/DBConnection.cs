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

        public bool Execute(string inQuery)
        {
            if (connection == null)
            {
                return false;
            }

            try
            {
                using (var command = new MySqlCommand(inQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.LoggerManager.Instance.WriteLogError("Query error {0} / {1}",
                    inQuery, ex.Message);
                
                return false;
            }

            Logger.LoggerManager.Instance.WriteLogInfo("Query successed {0}", inQuery);
            return true;
        }
    }
}
