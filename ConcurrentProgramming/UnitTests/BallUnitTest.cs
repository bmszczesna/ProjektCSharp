using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ConcurrentProgramming.Data;

namespace ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Vector testinVector = new Vector(0.0, 0.0);
            double diameter = 5.0;  // Dodajemy wartość średnicy
            Ball newInstance = new(testinVector, testinVector, diameter);  // Przekazujemy 3 argumenty
        }


        [TestMethod]
        public void MoveTestMethod()
        {
            Vector initialPosition = new(10.0, 10.0);
            double diameter = 5.0; // Dodajemy wartość dla średnicy
            Ball newInstance = new(initialPosition, new Vector(0.0, 0.0), diameter); // Przekazujemy średnicę do konstruktora
            IVector currentPosition = new Vector(0.0, 0.0);
            int numberOfCallBackCalled = 0;

            // Rejestracja callbacka, który będzie sprawdzał, czy pozycja została zaktualizowana
            newInstance.NewPositionNotification += (sender, position) =>
            {
                Assert.IsNotNull(sender);
                currentPosition = position;
                numberOfCallBackCalled++;
            };

            // Wywołanie metody Move
            newInstance.Move(new Vector(0.0, 0.0));

            // Sprawdzenie, czy callback został wywołany
            Assert.AreEqual<int>(1, numberOfCallBackCalled);

            // Sprawdzenie, czy pozycja nie została zmieniona
            Assert.AreEqual<IVector>(initialPosition, currentPosition);
        }

    }
}
