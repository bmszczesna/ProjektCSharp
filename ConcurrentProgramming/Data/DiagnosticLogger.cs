using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentProgramming.Data
{
    public class DiagnosticLogger
    {
        private static readonly BlockingCollection<string> logQueue = new();
        private static readonly CancellationTokenSource cts = new();
        private static readonly Task loggingTask;
        private static readonly string logFilePath;

        private static bool isDisposed = false;

        static DiagnosticLogger()
        {
            string folder = "Logs";
            Directory.CreateDirectory(folder);
            logFilePath = Path.Combine(folder, $"simulator_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

            loggingTask = Task.Run(() =>
            {
                using StreamWriter writer = new(logFilePath, append: true);
                try
                {
                    foreach (var log in logQueue.GetConsumingEnumerable(cts.Token))
                    {
                        writer.WriteLine(log);
                    }
                }
                catch (OperationCanceledException) { }
            });
        }

        public static void Log(string message)
        {
            if (!logQueue.IsAddingCompleted)
            {
                logQueue.Add($"[{DateTime.Now:O}] {message}");
            }
        }

        public static void Shutdown()
        {
            if (isDisposed) return;
            isDisposed = true;

            logQueue.CompleteAdding();
            cts.Cancel();
            try { loggingTask.Wait(); } catch { }
        }

        public static void LogWallCollision(string wall, IBall ball)
        {
            Log($"Ball hit {wall} wall at ({ball.Position.x:F2}, {ball.Position.y:F2})");
        }

        public static void LogBallPosition(int id, IBall ball)
        {
            Log($"Ball #{id} Pos: ({ball.Position.x:F2}, {ball.Position.y:F2}) Vel: ({ball.Velocity.x:F2}, {ball.Velocity.y:F2})");
        }

        public static void LogBallCollision(IBall ballA, IBall ballB)
        {
            Log($"Collision: Ball A at ({ballA.Position.x:F2}, {ballA.Position.y:F2}) vs Ball B at ({ballB.Position.x:F2}, {ballB.Position.y:F2})");
        }

    }
}
