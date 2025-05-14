using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Data
{
    /// <summary>
    ///  Two dimensions immutable vector
    /// </summary>
    public record Vector : IVector
    {
        #region IVector

        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public double x { get; set; }
        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        public double y { get; set; }

        #endregion IVector

        /// <summary>
        /// Creates new instance of <seealso cref="Vector"/> and initialize all properties
        /// </summary>
        public Vector(double XComponent, double YComponent)
        {
            x = XComponent;
            y = YComponent;
        }

    }
}