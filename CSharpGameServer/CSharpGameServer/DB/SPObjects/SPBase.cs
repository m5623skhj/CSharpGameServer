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
    }
}
