using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConcurrentProgramming.Data;
using ConcurrentProgramming.Logic;

namespace ConcurrentProgramming.Logic.Test
{
    [TestClass]
    public class CollisionTests
    {
        [TestMethod]
        public void BallsCollide_WhenWithinRadius()
        {
            var logicBall = new Ball(new DataBallFixture(), new Dimensions(10, 400, 400));
            var a = new Position(100, 100);
            var b = new Position(110, 100);

            bool result = logicBall.AreBallsColliding(a, b, 20);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ResolveElasticCollision_SwapsVelocity()
        {
            var a = new Data.Ball(new Vector(0, 0), new Vector(1, 0), 1.0, 10.0);
            var b = new Data.Ball(new Vector(10, 0), new Vector(-1, 0), 1.0, 10.0);
            var logic = new Ball(a, new Dimensions(10, 400, 400));

            logic.ResolveElasticCollision(a, b);

            Assert.AreEqual(-1, a.Velocity.x, 0.001);
            Assert.AreEqual(1, b.Velocity.x, 0.001);
        }

        [TestMethod]
        public void Ball_BouncesOffLeftWall()
        {
            var dataBall = new Data.Ball(new Vector(5, 200), new Vector(-1, 0), 1.0, 10.0);
            var logicBall = new Ball(dataBall, new Dimensions(10.0, 400.0, 400.0));

            dataBall.Position = new Vector(5, 200);
            logicBall.GetType()
                .GetMethod("RaisePositionChangeEvent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(logicBall, new object[] { null, dataBall.Position });

            Assert.IsTrue(dataBall.Velocity.x > 0);
        }

        private class DataBallFixture : Data.IBall
        {
            public event EventHandler<IVector>? NewPositionNotification;
            public IVector Position { get; set; } = new Vector(100, 100);
            public IVector Velocity { get; set; } = new Vector(0, 0);
            public double Mass => 1.0;
            public double Diameter => 10.0;
        }
    }
}
