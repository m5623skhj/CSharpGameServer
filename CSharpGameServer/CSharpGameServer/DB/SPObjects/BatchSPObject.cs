using CSharpGameServer.Logger;

namespace CSharpGameServer.DB.SPObjects
{
    public abstract class BatchSpObject
    {
        List<SpBase> batchSpObjects = new List<SpBase>();

        public void AddSpObject(SpBase spObject)
        {
            batchSpObjects.Add(spObject);
        }

        public List<SpBase> GetSpList() { return batchSpObjects; }

        public bool Execute()
        {
            var connection = DbConnectionManager.Instance.GetConnection();
            if (connection == null)
            {
                LoggerManager.Instance.WriteLogError("BatchSPObject failed, connection is null",
                    string.Join(", ", batchSpObjects.Select(sp => sp.GetQueryString())));

                return false;
            }

            return connection.ExecuteBatch(batchSpObjects);
        }
    }
}
