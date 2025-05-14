namespace ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        #region ctor


        internal Ball(Vector initialPosition, Vector initialVelocity, double mass, double diameter)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
            Mass = mass;
            Diameter = diameter;
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Position { get; set; }
        public IVector Velocity { get; set; }
        public double Mass { get; }
        public double Diameter { get; }


        #endregion IBall

        #region private

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        internal void Move(double deltaTime)
        {
            Position = new Vector(Position.x + Velocity.x * deltaTime, Position.y + Velocity.y * deltaTime);
            RaiseNewPositionChangeNotification();
        }

        #endregion private
    }
}