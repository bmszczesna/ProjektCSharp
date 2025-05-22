using System.Diagnostics;
using UnderneathLayerAPI = ConcurrentProgramming.Data.DataAbstractAPI;

namespace ConcurrentProgramming.Logic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        #region ctor

        public BusinessLogicImplementation() : this(null)
        { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
        }

        #endregion ctor

        #region BusinessLogicAbstractAPI

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            layerBellow.Start(numberOfBalls, (startingPosition, dataBall) =>
            {
                var logicBall = new Ball(dataBall, GetDimensions);
                logicBalls.Add(logicBall);

                dataBall.NewPositionNotification += (_, _) => HandleCollisions(logicBall);

                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), logicBall);
            });
        }


        private void HandleCollisions(Ball movingBall)
        {
            lock (collisionLock)
            {
                var ballsCopy = new List<Ball>(logicBalls);

                foreach (var other in ballsCopy)
                {
                    if (other == movingBall) continue;

                    var aPos = new Position(movingBall.DataBall.Position.x, movingBall.DataBall.Position.y);
                    var bPos = new Position(other.DataBall.Position.x, other.DataBall.Position.y);
                    var diameter = (other.DataBall.Diameter + movingBall.Diameter) / 2;

                    if (movingBall.AreBallsColliding(aPos, bPos, diameter))
                    {
                        movingBall.ResolveElasticCollision(movingBall.DataBall, other.DataBall);
                    }
                }
            }
        }

        #endregion BusinessLogicAbstractAPI

        #region private

        private bool Disposed = false;

        private readonly UnderneathLayerAPI layerBellow;

        private List<Ball> logicBalls = new();

        private readonly object collisionLock = new();

        #endregion private

        #region TestingInfrastructure

        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)

        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}