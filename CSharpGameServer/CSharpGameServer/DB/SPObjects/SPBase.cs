using CSharpGameServer.Logger;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace CSharpGameServer.DB.SPObjects
{
    public readonly record struct QueryParameter(string Name, object Value);

    public abstract class SpBase
    {
        protected string? Query;
        private readonly List<QueryParameter> parameters = [];

        protected void ClearParameters()
        {
            parameters.Clear();
        }

        protected void AddParameter(string name, object? value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            parameters.Add(new QueryParameter(name, value));
        }

        public string? GetQueryString()
        {
            return Query;
        }

        public IReadOnlyList<QueryParameter> GetParameters()
        {
            return parameters;
        }

        public MySqlCommand CreateCommand(MySqlConnection connection)
        {
            var queryString = GetQueryString();
            if (queryString == null)
            {
                throw new InvalidOperationException("Query string is null");
            }

            var command = new MySqlCommand(queryString, connection)
            {
                CommandType = System.Data.CommandType.Text
            };

            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Name, parameter.Value);
            }

            return command;
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
