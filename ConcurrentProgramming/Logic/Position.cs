namespace ConcurrentProgramming.Logic
{
    internal record Position : IPosition
    {
        #region IPosition

        public double x { get; set; }
        public double y { get; set; }

        #endregion IPosition

        /// <summary>
        /// Creates new instance of <seealso cref="IPosition"/> and initialize all properties
        /// </summary>
        public Position(double posX, double posY)
        {
            x = posX;
            y = posY;
        }
    }
}

