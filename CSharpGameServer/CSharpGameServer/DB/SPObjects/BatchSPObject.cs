using CSharpGameServer.Logger;

namespace CSharpGameServer.DB.SPObjects
{
    public abstract class BatchSpObject
    {
        private readonly List<SpBase> batchSpObjects = [];

        public void AddSpObject(SpBase spObject)
        {
            batchSpObjects.Add(spObject);
        }

        public List<SpBase> GetSpList() { return batchSpObjects; }

        public bool Execute()
        {
            var connection = DbConnectionManager.Instance.GetConnection();
            if (connection != null)
            {
                return connection.ExecuteBatch(batchSpObjects);
            }

            LoggerManager.Instance.WriteLogError("BatchSPObject failed, connection is null",
                string.Join(", ", batchSpObjects.Select(sp => sp.GetQueryString())));

            return false;

        }
    }
}
