namespace FW_LJ_CP.BusinessLogic.Test
{
    public class PositionUnitTest
    {
        [Fact]
        public void ConstructorTestMethod()
        {
            Random random = new Random();
            double initialX = random.NextDouble();
            double initialY = random.NextDouble();
            IPosition position = new Position(initialX, initialY);
            Assert.Equal<double>(initialX, position.x);
            Assert.Equal<double>(initialY, position.y);
        }
    }
}