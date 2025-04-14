namespace ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        #region ctor

        internal Ball(Vector initialPosition, Vector initialVelocity, double diameter)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
            Diameter = diameter;
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }

        public double Diameter { get; }

        #endregion IBall

        #region private

        private Vector Position;

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        internal void Move(Vector delta)
        {
            double canvasWidth = 400;
            double canvasHeight = 420;
            double radius = Diameter / 2;
            double frameWidth = 4;

            double newX = Position.x + delta.x;
            double newY = Position.y + delta.y;

            // Left edge
            if (newX - radius <= frameWidth)
            {
                delta = new Vector(-delta.x, delta.y); 
                newX = radius + frameWidth;
            }
            // Right edge
            else if (newX + radius >= canvasWidth - frameWidth)
            {
                delta = new Vector(-delta.x, delta.y);
                newX = canvasWidth - radius - frameWidth;
            }

            // Top edge
            if (newY - radius <= frameWidth) 
            {
                delta = new Vector(delta.x, -delta.y); 
                newY = radius + frameWidth; 
            }
            // Down edge
            else if (newY + radius >= canvasHeight - frameWidth) 
            {
                delta = new Vector(delta.x, -delta.y);
                newY = canvasHeight - radius - frameWidth;
            }

            Position = new Vector(newX, newY);

            RaiseNewPositionChangeNotification();
        }



        #endregion private
    }
}