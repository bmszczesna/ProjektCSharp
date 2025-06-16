using System;
using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Logic
{
    internal class Ball : IBall
    {
        private readonly Dimensions tableDimensions;
        private const double Margin = 4.0;

        public Ball(Data.IBall ball, Dimensions tableDimensions)
        {
            this.tableDimensions = tableDimensions;
            this.DataBall = ball;
            ball.NewPositionNotification += RaisePositionChangeEvent;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        public Data.IBall DataBall { get; }

        public double Diameter => DataBall.Diameter;

        #endregion IBall

        #region private

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
            var newPosition = new Position(e.x, e.y);

            HandleEdgeCollision(newPosition);

            DataBall.Position = new Data.Vector(newPosition.x, newPosition.y);

            NewPositionNotification?.Invoke(this, newPosition);
        }

        private void HandleEdgeCollision(Position position)
        {
            double radius = DataBall.Diameter / 2.0;

            if (position.x - radius < Margin)
            {
                position.x = radius + Margin;
                DataBall.Velocity = new Data.Vector(-DataBall.Velocity.x, DataBall.Velocity.y);
                DiagnosticLogger.LogWallCollision("LEFT", DataBall);
            }
            else if (position.x + radius > tableDimensions.TableWidth)
            {
                position.x = tableDimensions.TableWidth - radius;
                DataBall.Velocity = new Data.Vector(-DataBall.Velocity.x, DataBall.Velocity.y);
                DiagnosticLogger.LogWallCollision("RIGHT", DataBall);
            }

            if (position.y - radius < Margin)
            {
                position.y = radius + Margin;
                DataBall.Velocity = new Data.Vector(DataBall.Velocity.x, -DataBall.Velocity.y);
                DiagnosticLogger.LogWallCollision("TOP", DataBall);
            }
            else if (position.y + radius > tableDimensions.TableHeight)
            {
                position.y = tableDimensions.TableHeight - radius;
                DataBall.Velocity = new Data.Vector(DataBall.Velocity.x, -DataBall.Velocity.y);
                DiagnosticLogger.LogWallCollision("BOTTOM", DataBall);
            }
        }


        public bool AreBallsColliding(IPosition a, IPosition b, double diameter)
        {
            double dx = a.x - b.x;
            double dy = a.y - b.y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return distance < diameter;
        }

        public void ResolveElasticCollision(Data.IBall a, Data.IBall b)
        {
            var dx = a.Position.x - b.Position.x;
            var dy = a.Position.y - b.Position.y;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            if (distance == 0) return;

            var normal = new Data.Vector(dx / distance, dy / distance);
            var relativeVelocity = new Data.Vector(a.Velocity.x - b.Velocity.x, a.Velocity.y - b.Velocity.y);
            double speed = relativeVelocity.x * normal.x + relativeVelocity.y * normal.y;

            if (speed > 0) return;

            double impulse = (2 * speed) / (a.Mass + b.Mass);
            a.Velocity = new Data.Vector(
                a.Velocity.x - impulse * b.Mass * normal.x,
                a.Velocity.y - impulse * b.Mass * normal.y
            );
            b.Velocity = new Data.Vector(
                b.Velocity.x + impulse * a.Mass * normal.x,
                b.Velocity.y + impulse * a.Mass * normal.y
            );
        }

        #endregion private
    }
}
