namespace ConcurrentProgramming.Logic.Test
{
    [TestClass]
    public class BusinessLogicAbstractAPIUnitTest
    {


        [TestMethod]
        public void GetDimensionsTestMethod()
        {
            Assert.AreEqual<Dimensions>(new(10.0, 10.0, 10.0), BusinessLogicAbstractAPI.GetDimensions);
        }
    }
}
