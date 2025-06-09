using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentProgramming.Data
{
    internal class DiagnosticLogger : IDisposable
    {
        private readonly BlockingCollection<string> logQueue = new();
        private readonly CancellationTokenSource cts = new();
        private readonly Task loggingTask;

        public DiagnosticLogger(string filePath)
        {
            loggingTask = Task.Run(() =>
            {
                using StreamWriter writer = new(filePath, append: true);
                foreach (string log in logQueue.GetConsumingEnumerable(cts.Token))
                {
                    writer.WriteLine(log);
                }
            });
        }

        public void Log(string message)
        {
            if (!logQueue.IsAddingCompleted)
                logQueue.Add(message);
        }

        public void Dispose()
        {
            logQueue.CompleteAdding();
            cts.Cancel();
            loggingTask.Wait();
            logQueue.Dispose();
            cts.Dispose();
        }
    }
}
