using System.Timers;
using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor

        public DataImplementation()
        {
            MoveTimer = new System.Timers.Timer(8);
            MoveTimer.Elapsed += OnMoveTimerElapsed;
            MoveTimer.Start();
        }

        #endregion ctor

        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            Random random = new Random();
            double defaultDiameter = 20;

            lock (ballsLock)
            {
                for (int i = 0; i < numberOfBalls; i++)
                {
                    Vector startingPosition = new Vector(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
                    Vector startingVelocity = new Vector((random.NextDouble() * 20 - 10), (random.NextDouble() * 20 - 10));
                    Ball newBall = new Ball(startingPosition, startingVelocity, 1, defaultDiameter);
                    upperLayerHandler(startingPosition, newBall);
                    BallsList.Add(newBall);
                }
            }
        }

        #endregion DataAbstractAPI

        #region IDisposable

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
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }


        public override void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private bool Disposed = false;
        private readonly System.Timers.Timer MoveTimer;
        private List<Ball> BallsList = new List<Ball>();
        private void OnMoveTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            double deltaTime = e.SignalTime.Subtract(LastMoveTime).TotalSeconds;
            LastMoveTime = e.SignalTime;

            List<Ball> ballsCopy;
            lock (ballsLock)
            {
                ballsCopy = new List<Ball>(BallsList); // bezpieczna kopia
            }

            foreach (Ball ball in ballsCopy)
            {
                ball.Move(deltaTime);
            }
        }



        private DateTime LastMoveTime = DateTime.Now;
        private readonly object ballsLock = new object();

        #endregion private
    }
}
