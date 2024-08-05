using CSharpGameServer.Logger;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Reflection;

namespace CSharpGameServer.DB.SPObjects
{
    public abstract class SPBase
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

            return GenerateSPQuery(values);
        }

        public bool GenerateSPQuery(params object[] values)
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

    public abstract class SPWithResult<ResultType> : SPBase
    {
        protected List<ResultType> resultList = new List<ResultType>();
        Type resultType = typeof(ResultType);

        public override void AddResultRows(DbDataReader reader)
        {
            while (reader.Read())
            {
                var item = (ResultType?)Activator.CreateInstance(resultType);
                if (item == null)
                {
                    throw new Exception("AddResultRows() : Failed to create instance of ResultType");
                }

                foreach (var prop in typeof(ResultType).GetProperties())
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
