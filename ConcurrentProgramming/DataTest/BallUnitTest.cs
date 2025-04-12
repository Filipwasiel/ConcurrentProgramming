namespace FW_LJ_CP.Data.Test
{
    public class BallUnitTest
    {

        [Fact]
        public void MoveTestMethod()
        {
            Vector initialPosition = new(10.0, 10.0);
            Ball newInstance = new(initialPosition, new Vector(0.0, 0.0));
            IVector curentPosition = new Vector(0.0, 0.0);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) => { Assert.NotNull(sender); curentPosition = position; numberOfCallBackCalled++; };
            newInstance.Move(new Vector(0.0, 0.0));
            newInstance.Move(new Vector(1.0, 1.0));
            newInstance.Move(new Vector(10.0, 10.0));
            Assert.Equal<int>(3, numberOfCallBackCalled);
            Assert.Equal<IVector>(new Vector(21.0, 21.0), curentPosition);
            Console.WriteLine(initialPosition);
            Console.WriteLine(curentPosition);
        }
    }
}