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

            lock (ballsLock)
            {
                for (int i = 0; i < numberOfBalls; i++)
                {
                    double mass = random.NextDouble() * 4 + 1;
                    double diameter = 10 + mass * 5;

                    Vector startingPosition = new Vector(random.Next(100, 300), random.Next(100, 300));
                    Vector startingVelocity = new Vector((random.NextDouble() * 100 - 20), (random.NextDouble() * 100 - 20));

                    Ball newBall = new Ball(startingPosition, startingVelocity);
                    upperLayerHandler(startingPosition, newBall);
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
                DiagnosticLogger.LogBallPosition(index, ball.Position.x, ball.Position.y, ball.Velocity.x, ball.Velocity.y);
                index++;
            }
        }
    }
}
