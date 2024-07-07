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
}
