namespace ConcurrentProgramming.Logic.Test
{
    [TestClass]
    public class BusinessLogicAbstractAPIUnitTest
    {


        [TestMethod]
        public void GetDimensionsTestMethod()
        {
            Assert.AreEqual(new Dimensions(10.0, 420.0, 400.0), BusinessLogicAbstractAPI.GetDimensions);
        }

    }
}
