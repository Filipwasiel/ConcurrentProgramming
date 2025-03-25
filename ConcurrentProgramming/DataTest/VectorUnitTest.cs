namespace FW_LJ_CP.Data.Test
{
    public class VectorUnitTest
    {
        [Fact]
        public void ConstructorTestMethod()
        {
            Random randomGenerator = new();
            double XComponent = randomGenerator.NextDouble();
            double YComponent = randomGenerator.NextDouble();
            Vector newInstance = new(XComponent, YComponent);
            Assert.Equal<double>(XComponent, newInstance.x);
            Assert.Equal<double>(YComponent, newInstance.y);
        }
    }
}