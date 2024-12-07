using CSharpGameServer.DB;
using CSharpGameServer.DB.SPObjects;

namespace CSharpGameServer.LazyRunner
{
    public class LazyRunner
    {
        private readonly Action action;
        private readonly int delayMilliSeconds;
        private Timer timer;

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

    public class SpLazyRunner : LazyRunner
    {
        public SpLazyRunner(int inDelayMilliSeconds, SpBase spObject)
            : base(() => 
            {
                var connection = DbConnectionManager.Instance.GetConnection();
                if (connection != null && connection.Execute(spObject) == false)
                {
                    string? queryString = spObject.GetQueryString();
                    if (queryString != null)
                    {
                        Logger.LoggerManager.Instance.WriteLogError("SPLazyRunner {0} failed", queryString);
                    }
                }
            }, inDelayMilliSeconds)
        {
        }
    }

    public class BatchSpLazyRunner : LazyRunner
    {
        public BatchSpLazyRunner(int inDelayMilliSeconds, BatchSpObject batchSpObject)
            : base(() =>
            {
                var connection = DbConnectionManager.Instance.GetConnection();
                connection?.ExecuteBatch(batchSpObject.GetSpList());
            }, inDelayMilliSeconds)
        {
        }
    }
}
