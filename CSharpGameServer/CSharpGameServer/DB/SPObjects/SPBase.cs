using CSharpGameServer.Logger;
using System.Data.Common;
using System.Reflection;

namespace CSharpGameServer.DB.SPObjects
{
    public abstract class SpBase
    {
        protected string? Query;

        public bool SetParams(object paramObject)
        {
            var paramType = paramObject.GetType();
            var properties = paramType.GetProperties();
            var values = new object[properties.Length];

            for (var i = 0; i < properties.Length; i++)
            {
                var value = properties[i].GetValue(paramObject);
                if (value == null)
                {
                    return false;
                }

                values[i] = value;
            }

            return GenerateSpQuery(values);
        }

        public bool GenerateSpQuery(params object[] values)
        {
            if (Query == null)
            {
                return false;
            }

            Query = string.Format(Query, values);
            return true;
        }

        public string? GetQueryString()
        {
            return Query;
        }

        public abstract void OnCommit();
        public abstract void OnRollback();

        public virtual void AddResultRows(DbDataReader reader) { }
    }

    public abstract class SpWithResult<TResultType> : SpBase
    {
        protected List<TResultType> ResultList = [];
        private readonly Type resultType = typeof(TResultType);

        public override void AddResultRows(DbDataReader reader)
        {
            while (reader.Read())
            {
                var item = (TResultType?)Activator.CreateInstance(resultType);
                if (item == null)
                {
                    throw new Exception("AddResultRows() : Failed to create instance of ResultType");
                }

                foreach (var prop in typeof(TResultType).GetProperties())
                {
                    if (reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                    {
                        LoggerManager.Instance.WriteLogError("Column has DBNull value: " + prop.Name);
                        continue;
                    }

                    prop.SetValue(item, reader[prop.Name]);
                }
                ResultList.Add(item);
            }
        }
    }
}
