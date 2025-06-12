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

        public static void LogWallCollision(string wall, double x, double y)
        {
            Log($"Ball hit {wall} wall at ({x:F2}, {y:F2})");
        }

        public static void LogBallPosition(int id, double x, double y, double vx, double vy)
        {
            Log($"Ball #{id} Pos: ({x:F2}, {y:F2}) Vel: ({vx:F2}, {vy:F2})");
        }

        public static void LogBallCollision(double ax, double ay, double bx, double by)
        {
            Log($"Collision: Ball A at ({ax:F2}, {ay:F2}) vs Ball B at ({bx:F2}, {by:F2})");
        }

    }
}
