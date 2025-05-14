namespace ConcurrentProgramming.Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        #region Layer Factory

        public static DataAbstractAPI GetDataLayer()
        {
            return modelInstance.Value;
        }

        #endregion Layer Factory

        #region public API

        public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);

        #endregion public API

        #region IDisposable

        public abstract void Dispose();

        #endregion IDisposable

        #region private

        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

        #endregion private
    }

    public interface IVector
    {
        /// <summary>
        /// The X component of the vector.
        /// </summary>
        double x { get; set; }

        /// <summary>
        /// The y component of the vector.
        /// </summary>
        double y { get; set; }
    }

    public interface IBall
    {
        event EventHandler<IVector> NewPositionNotification;
        IVector Position { get; set; }
        IVector Velocity { get; set; }
        double Mass { get; }
        double Diameter { get; }

    }

}
