using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace ConcurrentProgramming.Data
{
    public static class DiagnosticLogger
    {
        private static readonly BlockingCollection<string> logQueue = new();
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
                try
                {
                    using StreamWriter writer = new(logFilePath, append: true);
                    foreach (var log in logQueue.GetConsumingEnumerable())
                    {
                        writer.WriteLine(log);
                        writer.Flush(); // Wymuszamy zapis na dysk po każdym wpisie
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Logger exception: {ex}");
                }
            });
        }

        public static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine("LOG >> " + message);
            if (!logQueue.IsAddingCompleted)
            {
                try
                {
                    logQueue.Add($"[{DateTime.Now:O}] {message}");
                }
                catch (InvalidOperationException)
                {
                    // Kolejka została zamknięta - ignorujemy
                }
            }
        }

        public static void Shutdown()
        {
            if (isDisposed) return;

            isDisposed = true;
            logQueue.CompleteAdding();
            try
            {
                loggingTask.Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    System.Diagnostics.Debug.WriteLine($"Logger shutdown exception: {inner}");
                }
            }
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
