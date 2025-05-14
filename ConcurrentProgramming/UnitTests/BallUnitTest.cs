using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void Constructor_CreatesBallCorrectly()
        {
            var position = new Vector(0.0, 0.0);
            var velocity = new Vector(1.0, 1.0);
            double mass = 2.0;
            double diameter = 5.0;

            var ball = new Ball(position, velocity, mass, diameter);

            Assert.AreEqual(position.x, ball.Position.x);
            Assert.AreEqual(velocity.y, ball.Velocity.y);
            Assert.AreEqual(mass, ball.Mass);
            Assert.AreEqual(diameter, ball.Diameter);
        }

        [TestMethod]
        public void Move_ChangesPositionAndFiresEvent()
        {
            var position = new Vector(10.0, 10.0);
            var velocity = new Vector(1.0, 0.0);
            var ball = new Ball(position, velocity, 1.0, 5.0);

            int eventCount = 0;
            ball.NewPositionNotification += (_, pos) =>
            {
                Assert.AreEqual(11.0, pos.x, 0.001);
                Assert.AreEqual(10.0, pos.y, 0.001);
                eventCount++;
            };

            ball.Move(1.0);
            Assert.AreEqual(1, eventCount);
        }
    }
}