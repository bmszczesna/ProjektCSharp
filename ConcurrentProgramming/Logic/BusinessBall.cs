namespace ConcurrentProgramming.Logic
{
    internal class Ball : IBall
    {
        private readonly Dimensions tableDimensions;
        private const double Margin = 4.0;

        public Ball(Data.IBall ball, Dimensions tableDimensions)
        {
            this.tableDimensions = tableDimensions;
            ball.NewPositionNotification += RaisePositionChangeEvent;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
            var newPosition = new Position(e.x, e.y);

            newPosition = HandleEdgeCollision(newPosition);

            NewPositionNotification?.Invoke(this, newPosition);
        }

        private Position HandleEdgeCollision(Position position)
        {
            if (position.x - tableDimensions.BallDimension < Margin)
            {
                position.x = tableDimensions.BallDimension + Margin;
            }
            else if (position.x + tableDimensions.BallDimension > tableDimensions.TableWidth)
            {
                position.x = tableDimensions.TableWidth - tableDimensions.BallDimension;
            }

            if (position.y - tableDimensions.BallDimension < Margin)
            {
                position.y = tableDimensions.BallDimension + Margin;
            }
            else if (position.y + tableDimensions.BallDimension > tableDimensions.TableHeight)
            {
                position.y = tableDimensions.TableHeight - tableDimensions.BallDimension;
            }

            return position;
        }

        #endregion private
    }
}

