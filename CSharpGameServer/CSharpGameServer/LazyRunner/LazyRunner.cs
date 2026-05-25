using CSharpGameServer.DB;
using CSharpGameServer.DB.SPObjects;

namespace CSharpGameServer.LazyRunner
{
    public class LazyRunner
    {
        private readonly Action action;
        private readonly int delayMilliSeconds;
        private readonly Timer timer;

        public LazyRunner(Action inAction, int inDelayMilliSeconds)
        {
            action = inAction;
            delayMilliSeconds = inDelayMilliSeconds;
            timer = new Timer(Run, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            timer.Change(delayMilliSeconds, Timeout.Infinite);
        }

        private void Run(object? _)
        {
            action();
            timer.Dispose();
        }
    }

    public class SpLazyRunner(int inDelayMilliSeconds, SpBase spObject) : LazyRunner(() =>
    {
        var connection = DbConnectionManager.Instance.GetConnection();
        if (connection == null)
        {
            return;
        }

        var isSuccess = false;
        try
        {
            isSuccess = connection.Execute(spObject);
            if (isSuccess)
            {
                return;
            }

            var queryString = spObject.GetQueryString();
            if (queryString != null)
            {
                Logger.LoggerManager.Instance.WriteLogError("SPLazyRunner {0} failed", queryString);
            }
        }
        finally
        {
            DbConnectionManager.Instance.ReleaseConnection(connection, isSuccess);
        }
    }, inDelayMilliSeconds);

    public class BatchSpLazyRunner(int inDelayMilliSeconds, BatchSpObject batchSpObject) : LazyRunner(() =>
    {
        var connection = DbConnectionManager.Instance.GetConnection();
        if (connection == null)
        {
            return;
        }

        var isSuccess = false;
        try
        {
            isSuccess = connection.ExecuteBatch(batchSpObject.GetSpList());
        }
        finally
        {
            DbConnectionManager.Instance.ReleaseConnection(connection, isSuccess);
        }
    }, inDelayMilliSeconds);
}
