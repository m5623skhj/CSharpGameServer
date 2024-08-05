using CSharpGameServer.DB.SPObjects;
using CSharpGameServer.Logger;
using MySql.Data.MySqlClient;
using System.Data.Common;

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

        public bool ExecuteWithResult(SPBase spObject)
        {
            if (connection == null)
            {
                return false;
            }

            var queryString = spObject.GetQueryString();
            if (queryString == null)
            {
                return false;
            }

            try
            {
                using (var command = new MySqlCommand(queryString, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    using (var reader = command.ExecuteReader())
                    {
                        spObject.AddResultRows(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.Instance.WriteLogError("Query error {0} / {1}",
                    queryString, ex.Message);

                spObject.OnRollback();
                return false;
            }

            spObject.OnCommit();
            return true;
        }

        public bool ExecuteBatch(List<SPBase> batchSPObjects)
        {
            if (connection == null)
            {
                return false;
            }

            using (var batch = connection.CreateBatch())
            {
                if (MakeBatchCommand(batchSPObjects, batch) == false)
                {
                    return false;
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
                        foreach (var sp in batchSPObjects.AsEnumerable().Reverse())
                        {
                            sp.OnRollback();
                        }
                        return false;
                    }
                }
            }

            foreach (var sp in batchSPObjects)
            {
                sp.OnCommit();
            }
            return true;
        }

        public bool ExecuteBatchWithResult(List<SPBase> batchSPObjects)
        {
            if (connection == null)
            {
                return false;
            }

            using (var batch = connection.CreateBatch())
            {
                if (MakeBatchCommand(batchSPObjects, batch) == false)
                {
                    return false;
                }

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var reader = batch.ExecuteReader())
                        {
                            int spListIndex = 0;
                            do
                            {
                                batchSPObjects[spListIndex].AddResultRows(reader);
                                ++spListIndex;
                            } while (reader.NextResult());
                        }

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        LoggerManager.Instance.WriteLogError("BatchSPObject failed, connection is null",
                            string.Join(", ", batchSPObjects.Select(sp => sp.GetQueryString())), e.Message);

                        transaction.Rollback();
                        foreach (var sp in batchSPObjects.AsEnumerable().Reverse())
                        {
                            sp.OnRollback();
                        }
                        return false;
                    }
                }
            }

            foreach (var sp in batchSPObjects)
            {
                sp.OnCommit();
            }
            return true;
        }

        private bool MakeBatchCommand(List<SPBase> batchSPObjects, DbBatch batch)
        {
            foreach (var spObject in batchSPObjects)
            {
                var queryString = spObject.GetQueryString();
                if (queryString == null)
                {
                    return false;
                }

                var batchCommand = batch.CreateBatchCommand();
                batchCommand.CommandType = System.Data.CommandType.Text;
                batchCommand.CommandText = queryString;

                batch.BatchCommands.Add(batchCommand);
            }
            return true;
        }
    }
}
