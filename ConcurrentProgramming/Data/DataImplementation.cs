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

            lock (ballsLock)
            {
                for (int i = 0; i < numberOfBalls; i++)
                {
                    double mass = random.NextDouble() * 4 + 1; 
                    double diameter = 10 + mass * 5; 

                    Vector startingPosition = new Vector(random.Next(100, 300), random.Next(100, 300));
                    Vector startingVelocity = new Vector((random.NextDouble() * 50 - 10), (random.NextDouble() * 50 - 10));

                    Ball newBall = new Ball(startingPosition, startingVelocity);
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

            Disposed = true; 
        }



        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));

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
                ballsCopy = new List<Ball>(BallsList);
            }

            foreach (Ball ball in ballsCopy)
            {
                ball.Move(deltaTime);
            }
        }



        private DateTime LastMoveTime = DateTime.Now;
        private readonly object ballsLock = new object();

        #endregion private

#if DEBUG
        // Zwraca liczbę kul
        internal void CheckNumberOfBalls(Action<int> callback)
        {
            lock (ballsLock)
            {
                callback(BallsList.Count);
            }
        }

        // Zwraca listę kul
        internal void CheckBallsList(Action<IEnumerable<IBall>> callback)
        {
            lock (ballsLock)
            {
                callback(BallsList.ToList());
            }
        }

        // Zwraca stan flagi Disposed
        internal void CheckObjectDisposed(Action<bool> callback)
        {
            callback(Disposed);
        }
#endif

    }

}
