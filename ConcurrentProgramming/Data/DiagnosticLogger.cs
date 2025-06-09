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
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory); // UPEWNIA się, że folder istnieje
            }

            loggingTask = Task.Run(() =>
            {
                using StreamWriter writer = new(filePath, append: true);
                try
                {
                    foreach (var message in logQueue.GetConsumingEnumerable(cts.Token))
                    {
                        writer.WriteLine(message);
                    }
                }
                catch (OperationCanceledException) { }
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
