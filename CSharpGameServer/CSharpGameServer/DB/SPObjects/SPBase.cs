using CSharpGameServer.Logger;
using System.Data.Common;
using System.Reflection;

namespace CSharpGameServer.DB.SPObjects
{
    public abstract class SpBase
    {
        protected string? query = null;

        public bool SetParams(object paramObject)
        {
            Type paramType = paramObject.GetType();
            PropertyInfo[] properties = paramType.GetProperties();
            object[] values = new object[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                object? value = properties[i].GetValue(paramObject);
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
            if (query == null)
            {
                return false;
            }

            query = string.Format(query, values);
            return true;
        }

        public string? GetQueryString()
        {
            return query;
        }

        public abstract void OnCommit();
        public abstract void OnRollback();

        public virtual void AddResultRows(DbDataReader reader) { }
    }

    public abstract class SpWithResult<TResultType> : SpBase
    {
        protected List<TResultType> resultList = new List<TResultType>();
        Type resultType = typeof(TResultType);

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
                resultList.Add(item);
            }
        }
    }
}
