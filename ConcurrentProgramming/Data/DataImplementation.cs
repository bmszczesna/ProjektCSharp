using System.Timers;
using System.Collections.Generic;
using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        public DataImplementation()
        {
            MoveTimer = new System.Timers.Timer(4);
            MoveTimer.Elapsed += OnMoveTimerElapsed;
            MoveTimer.Start();
        }

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            Random random = new Random();

            const double tableWidth = 400;  
            const double tableHeight = 420;
            const double margin = 10;
            const int placementTries = 30;

            lock (ballsLock)
            {
                BallsList.Clear();

                for (int i = 0; i < numberOfBalls; i++)
                {
                    double mass = random.NextDouble() * 4 + 1;
                    double diameter = 10 + mass * 5;
                    double radius = diameter / 2;

                    double minX = radius + margin;
                    double maxX = tableWidth - radius - margin;
                    double minY = radius + margin;
                    double maxY = tableHeight - radius - margin;

                    if (maxX <= minX) maxX = minX + 1;
                    if (maxY <= minY) maxY = minY + 1;

                    Vector bestPosition = new Vector(0, 0);
                    double bestMinDistance = -1;

                    for (int t = 0; t < placementTries; t++)
                    {
                        double candidateX = random.NextDouble() * (maxX - minX) + minX;
                        double candidateY = random.NextDouble() * (maxY - minY) + minY;

                        Vector candidate = new Vector(candidateX, candidateY);

                        if (BallsList.Count == 0)
                        {
                            bestPosition = candidate;
                            break;
                        }

                        double minDist = double.MaxValue;
                        foreach (var otherBall in BallsList)
                        {
                            double dx = candidate.x - otherBall.Position.x;
                            double dy = candidate.y - otherBall.Position.y;
                            double dist = Math.Sqrt(dx * dx + dy * dy);
                            minDist = Math.Min(minDist, dist);
                        }

                        if (minDist > bestMinDistance)
                        {
                            bestMinDistance = minDist;
                            bestPosition = candidate;
                        }
                    }

                    Vector startingVelocity = new Vector(
                        random.NextDouble() * 100 - 50,
                        random.NextDouble() * 100 - 50
                    );

                    Ball newBall = new Ball(bestPosition, startingVelocity);
                    upperLayerHandler(bestPosition, newBall);
                    BallsList.Add(newBall);
                }
            }
        }




        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                MoveTimer.Dispose();
                lock (ballsLock)
                {
                    BallsList.Clear();
                }
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private readonly System.Timers.Timer MoveTimer;
        private List<Ball> BallsList = new();
        private readonly object ballsLock = new();
        private DateTime LastMoveTime = DateTime.Now;
        private bool Disposed = false;

        private void OnMoveTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            double deltaTime = e.SignalTime.Subtract(LastMoveTime).TotalSeconds;
            LastMoveTime = e.SignalTime;

            List<Ball> ballsCopy;
            lock (ballsLock)
            {
                ballsCopy = new List<Ball>(BallsList);
            }

            int index = 0;
            foreach (Ball ball in ballsCopy)
            {
                ball.Move(deltaTime);
                //DiagnosticLogger.LogBallPosition(index, ball);
                index++;
            }
        }
    }
}
