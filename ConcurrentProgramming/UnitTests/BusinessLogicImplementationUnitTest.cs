using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConcurrentProgramming.Logic;
using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Logic.Test
{
    [TestClass]
    public class BusinessLogicImplementationUnitTest
    {
        [TestMethod]
        public void Start_CallsUnderlyingDataLayerCorrectly()
        {
            // Arrange
            var fixture = new DataLayerStartFixture();
            using var logic = new BusinessLogicImplementation(fixture);

            int callbackCount = 0;

            // Act
            logic.Start(3, (pos, ball) =>
            {
                callbackCount++;
                Assert.IsNotNull(pos);
                Assert.IsNotNull(ball);
            });

            // Assert
            Assert.AreEqual(1, callbackCount); // Jeden raz wywołane
            Assert.IsTrue(fixture.StartCalled);
            Assert.AreEqual(3, fixture.NumberOfBallsCreated); // Sprawdzamy liczbę utworzonych piłek
        }

        [TestMethod]
        public void Start_ThrowsIfDisposed()
        {
            // Arrange
            var fixture = new DataLayerStartFixture();
            var logic = new BusinessLogicImplementation(fixture);

            // Act
            logic.Dispose();

            // Assert
            Assert.ThrowsException<ObjectDisposedException>(() =>
                logic.Start(3, (_, _) => { })
            );
        }

        [TestMethod]
        public void Dispose_SetsFlags_AndCleansUp()
        {
            // Arrange
            var fixture = new DataLayerDisposeFixture();
            var logic = new BusinessLogicImplementation(fixture);

            bool isDisposed = false;
            logic.CheckObjectDisposed(d => isDisposed = d);

            // Assert przed Dispose
            Assert.IsFalse(isDisposed, "Logic powinien być niezużyty przed Dispose");

            // Act
            logic.Dispose();

            logic.CheckObjectDisposed(d => isDisposed = d);

            // Assert po Dispose
            Assert.IsTrue(isDisposed, "Logic nie ustawił flagi Disposed");
            Assert.IsTrue(fixture.Disposed, "Fixture nie ustawił flagi Disposed");
        }


        #region Fixtures

        // Klasa symulująca dane warstwy
        private class DataLayerStartFixture : DataAbstractAPI
        {
            public bool StartCalled = false;
            public int NumberOfBallsCreated = -1;

            // Poprawnie zaimplementowana metoda Start
            public override void Start(int numberOfBalls, Action<ConcurrentProgramming.Data.IVector, ConcurrentProgramming.Data.IBall>
 upperLayerHandler)
            {
                StartCalled = true;
                NumberOfBallsCreated = numberOfBalls;

                // Tworzymy odpowiednie obiekty dla Vector i Ball
                var vectorFixture = new VectorFixture();
                var ballFixture = new BallFixture();

                // Wywołujemy akcję (callback) z odpowiednimi obiektami
                upperLayerHandler(vectorFixture, ballFixture);
            }

            public override void Dispose()
            {
                // Możesz zaimplementować metodę Dispose, jeśli jest to wymagane
            }

            // Fixture dla Vektora
            private class VectorFixture : IVector
            {
                public double x { get; set; } = 100.0;
                public double y { get; set; } = 200.0;
            }

            // Fixture dla Piłki (IBall)
            private class BallFixture : ConcurrentProgramming.Data.IBall

            {
                public event EventHandler<IVector>? NewPositionNotification;

                public IVector Position { get; set; } = new Vector(100, 200);
                public IVector Velocity { get; set; } = new Vector(1, 0);
                public double Mass => 1.0;
                public double Diameter => 10.0;

                // Zdarzenie powiadamiające o zmianie pozycji
                public void RaiseNewPositionNotification()
                {
                    NewPositionNotification?.Invoke(this, Position); // Używamy IVector
                }
            }
        }



        // Fixture dla Dispose
        private class DataLayerDisposeFixture : DataAbstractAPI
        {
            public bool Disposed = false;

            // Implementacja metody Start (nie jest używana w tym teście)
            public override void Start(int numberOfBalls, Action<ConcurrentProgramming.Data.IVector, ConcurrentProgramming.Data.IBall> upperLayerHandler)
            {
                // Ta metoda nie jest potrzebna w tym teście
            }

            // Implementacja metody Dispose
            public override void Dispose() => Disposed = true;
        }

        #endregion
    }
}
