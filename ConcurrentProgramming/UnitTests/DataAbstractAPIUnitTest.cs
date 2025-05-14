namespace ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class DataAbstractAPIUnitTest
    {
        [TestMethod]
        public void ConstructorTestTestMethod()
        {
            DataAbstractAPI instance1 = DataAbstractAPI.GetDataLayer();
            DataAbstractAPI instance2 = DataAbstractAPI.GetDataLayer();

            // Sprawdzamy, czy instancje są takie same
            Assert.AreSame<DataAbstractAPI>(instance1, instance2);

            // Wywołujemy Dispose na instancji 1
            instance1.Dispose();

            // Sprawdzamy, czy po wywołaniu Dispose na instance2 występuje wyjątek
            Assert.ThrowsException<ObjectDisposedException>(() => instance2.Dispose());
        }

    }
}
