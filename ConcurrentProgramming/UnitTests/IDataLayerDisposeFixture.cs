using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Logic.Test
{
    public interface IDataLayerDisposeFixture
    {
        void Dispose();
        void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);
        void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler);
    }
}