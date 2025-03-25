namespace FW_LJ_CP.Data.Test
{
    public class DataAbstractAPIUnitTest
    {
        [Fact]
        public void ConstructorTestTestMethod()
        {
            DataAbstractAPI instance1 = DataAbstractAPI.GetDataLayer();
            DataAbstractAPI instance2 = DataAbstractAPI.GetDataLayer();
            Assert.Same(instance1, instance2);
            instance1.Dispose();
            Assert.Throws<ObjectDisposedException>(() => instance2.Dispose());
        }
    }
}