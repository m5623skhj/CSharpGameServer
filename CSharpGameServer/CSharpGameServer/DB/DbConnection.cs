using CSharpGameServer.DB.SPObjects;
using CSharpGameServer.Logger;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace CSharpGameServer.DB
{
    public class DbConnection
    {
        private MySqlConnection? connection;

        public void CreateConnection(string connectionString)
        {
            if (connection != null)
            {
                return;
            }

            connection = new MySqlConnection(connectionString);
            connection.Open();
        }

        public void CloseConnection()
        {
            if (connection == null)
            {
                return;
            }

            connection.Close();
            connection = null;
        }

        public bool Execute(SpBase spObject)
        {
            if (connection == null)
            {
                return false;
            }

            try
            {
                using var command = spObject.CreateCommand(connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LoggerManager.Instance.WriteLogError("Query error {0} / {1}",
                    spObject.GetQueryString() ?? "<null>", ex.Message);

                spObject.OnRollback();
                return false;
            }

            spObject.OnCommit();
            return true;
        }

        public bool ExecuteWithResult(SpBase spObject)
        {
            if (connection == null)
            {
                return false;
            }

            try
            {
                using var command = spObject.CreateCommand(connection);
                using var reader = command.ExecuteReader();
                spObject.AddResultRows(reader);
            }
            catch (Exception ex)
            {
                LoggerManager.Instance.WriteLogError("Query error {0} / {1}",
                    spObject.GetQueryString() ?? "<null>", ex.Message);

                spObject.OnRollback();
                return false;
            }

            spObject.OnCommit();
            return true;
        }

        public bool ExecuteBatch(List<SpBase> batchSpObjects)
        {
            if (connection == null)
            {
                return false;
            }

            using (var batch = connection.CreateBatch())
            {
                if (MakeBatchCommand(batchSpObjects, batch) == false)
                {
                    return false;
                }

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        batch.Transaction = transaction;
                        batch.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        LoggerManager.Instance.WriteLogError("BatchSPObject failed, connection is null",
                            string.Join(", ", batchSpObjects.Select(sp => sp.GetQueryString())), e.Message);

                        transaction.Rollback();
                        foreach (var sp in batchSpObjects.AsEnumerable().Reverse())
                        {
                            sp.OnRollback();
                        }
                        return false;
                    }
                }
            }

            foreach (var sp in batchSpObjects)
            {
                sp.OnCommit();
            }
            return true;
        }

        public bool ExecuteBatchWithResult(List<SpBase> batchSpObjects)
        {
            if (connection == null)
            {
                return false;
            }

            using (var batch = connection.CreateBatch())
            {
                if (MakeBatchCommand(batchSpObjects, batch) == false)
                {
                    return false;
                }

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        batch.Transaction = transaction;
                        using (var reader = batch.ExecuteReader())
                        {
                            var spListIndex = 0;
                            do
                            {
                                batchSpObjects[spListIndex].AddResultRows(reader);
                                ++spListIndex;
                            } while (reader.NextResult());
                        }

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        LoggerManager.Instance.WriteLogError("BatchSPObject failed, connection is null",
                            string.Join(", ", batchSpObjects.Select(sp => sp.GetQueryString())), e.Message);

                        transaction.Rollback();
                        foreach (var sp in batchSpObjects.AsEnumerable().Reverse())
                        {
                            sp.OnRollback();
                        }
                        return false;
                    }
                }
            }

            foreach (var sp in batchSpObjects)
            {
                sp.OnCommit();
            }
            return true;
        }

        private static bool MakeBatchCommand(List<SpBase> batchSpObjects, DbBatch batch)
        {
            foreach (var spObject in batchSpObjects)
            {
                var queryString = spObject.GetQueryString();
                if (queryString == null)
                {
                    return false;
                }

                var batchCommand = batch.CreateBatchCommand();
                batchCommand.CommandType = System.Data.CommandType.Text;
                batchCommand.CommandText = queryString;

                foreach (var parameter in spObject.GetParameters())
                {
                    var dbParameter = batchCommand.CreateParameter();
                    dbParameter.ParameterName = parameter.Name;
                    dbParameter.Value = parameter.Value;
                    batchCommand.Parameters.Add(dbParameter);
                }

                batch.BatchCommands.Add(batchCommand);
            }

            return true;
        }
    }
}
