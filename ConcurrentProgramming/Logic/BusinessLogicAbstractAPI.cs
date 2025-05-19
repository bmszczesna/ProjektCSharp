using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Logic
{
    public abstract class BusinessLogicAbstractAPI : IDisposable
    {
        #region Layer Factory

        public static BusinessLogicAbstractAPI GetBusinessLogicLayer()
        {
            return modelInstance.Value;
        }

        #endregion Layer Factory

        #region Layer API

        public static readonly Dimensions GetDimensions = new(420.0, 400.0);

        public abstract void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler);

        #region IDisposable

        public abstract void Dispose();

        #endregion IDisposable

        #endregion Layer API

        #region private

        private static Lazy<BusinessLogicAbstractAPI> modelInstance = new Lazy<BusinessLogicAbstractAPI>(() => new BusinessLogicImplementation());

        #endregion private
    }
    /// <summary>
    /// Immutable type representing table dimensions
    /// </summary>
    /// <param name="TableHeight"></param>
    /// <param name="TableWidth"></param>
    /// <remarks>
    /// Must be abstract
    /// </remarks>
    public record Dimensions(double TableHeight, double TableWidth);

    public interface IPosition
    {
        double x { get; set; }
        double y { get; set; }
    }

    public interface IBall
    {
        event EventHandler<IPosition> NewPositionNotification;
        double Diameter { get; }

    }
}