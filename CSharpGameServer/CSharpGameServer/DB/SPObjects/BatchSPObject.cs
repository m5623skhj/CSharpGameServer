using CSharpGameServer.Logger;

namespace CSharpGameServer.DB.SPObjects
{
    public class BatchSPObject
    {
        List<SPBase> batchSPObjects = new List<SPBase>();

        public void AddSPObject(SPBase spObject)
        {
            batchSPObjects.Add(spObject);
        }

        public List<SPBase> GetSPList() { return batchSPObjects; }

        public bool Execute()
        {
            var connection = DBConnectionManager.Instance.GetConnection();
            if (connection == null)
            {
                LoggerManager.Instance.WriteLogError("BatchSPObject failed, connection is null",
                    string.Join(", ", batchSPObjects.Select(sp => sp.GetQueryString())));

                return false;
            }

            return connection.Execute(batchSPObjects);
        }
    }
}
