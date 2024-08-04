using CSharpGameServer.DB.SPObjects;
using CSharpGameServer.Logger;
using MySql.Data.MySqlClient;

namespace CSharpGameServer.DB
{
    public class DBConnection
    {
        private MySqlConnection? connection = null;

        public void CreateConnection(string connectionString)
        {
            if (connection == null)
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (connection != null)
            {
                connection.Close();
                connection = null;
            }
        }

        public MySqlCommand MakeQueryCommand(string query)
        {
            return new MySqlCommand(query, connection);
        }

        public bool Execute(SPBase spObject)
        {
            if (connection == null)
            {
                return false;
            }

            var query = spObject.GetQueryString();
            if (query == null)
            {
                return false;
            }
            
            try
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LoggerManager.Instance.WriteLogError("Query error {0} / {1}",
                    query, ex.Message);
                spObject.OnRollback();

                return false;
            }

            spObject.OnCommit();
            return true;
        }

        public bool Execute<ResultType>(SPBase spObject, out List<ResultType> resultList) 
            where ResultType : new()
        {
            resultList = new List<ResultType>();

            if (connection == null)
            {
                return false;
            }

            var query = spObject.GetQueryString();
            if (query == null)
            {
                return false;
            }

            try
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new ResultType();
                            foreach (var prop in typeof(ResultType).GetProperties())
                            {
                                if(reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                                {
                                    LoggerManager.Instance.WriteLogError("Invalid sp result prop name " + prop.Name);
                                    continue;
                                }

                                prop.SetValue(item, reader[prop.Name]);
                            }
                            resultList.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.Instance.WriteLogError("Query error {0} / {1}",
                    query, ex.Message);
                spObject.OnRollback();

                return false;
            }

            return true;
        }

        public bool Execute(List<SPBase> batchSPObjects)
        {
            if (connection == null)
            {
                return false;
            }

            using (var batch = connection.CreateBatch())
            {
                foreach (SPBase spObject in batchSPObjects)
                {
                    string? queryString = spObject.GetQueryString();
                    if (queryString == null)
                    {
                        return false;
                    }

                    var batchCommand = batch.CreateBatchCommand();
                    batchCommand.CommandType = System.Data.CommandType.Text;
                    batchCommand.CommandText = queryString;

                    batch.BatchCommands.Add(batchCommand);
                }

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        batch.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        LoggerManager.Instance.WriteLogError("BatchSPObject failed, connection is null",
                            string.Join(", ", batchSPObjects.Select(sp => sp.GetQueryString())), e.Message);

                        transaction.Rollback();
                        foreach (var sp in batchSPObjects)
                        {
                            sp.OnRollback();
                        }
                        return false;
                    }
                }

                foreach (var sp in batchSPObjects)
                {
                    sp.OnCommit();
                }
                return true;
            }
        }
    }
}
