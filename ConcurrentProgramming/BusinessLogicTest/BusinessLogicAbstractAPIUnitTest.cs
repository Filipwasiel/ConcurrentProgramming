using FW_LJ_CP.BusinessLogic;

namespace FW_LJ_CP.BusinessLogic.Test
{
    public class BusinessLogicAbstractAPIUnitTest
    {
        [Fact]
        public void BusinessLogicConstructorTestMethod()
        {
            BusinessLogicAbstractAPI instance1 = BusinessLogicAbstractAPI.GetBusinessLogicLayer();
            BusinessLogicAbstractAPI instance2 = BusinessLogicAbstractAPI.GetBusinessLogicLayer();
            Assert.Same(instance1, instance2);
            instance1.Dispose();
            Assert.Throws<ObjectDisposedException>(() => instance2.Dispose());
        }

        [Fact]
        public void GetDimensionsTestMethod()
        {
            Assert.Equal<Dimensions>(new(10.0, 10.0, 10.0), BusinessLogicAbstractAPI.GetDimensions);
        }
    }
}