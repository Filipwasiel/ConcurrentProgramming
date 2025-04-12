using FW_LJ_CP.Presentation.Model;

namespace FW_LJ_CP.PresentationModelTest
{
    public class ModelAbstractAPITest
    {
        [Fact]
        public void SingletonConstructorTestMethod()
        {
            ModelAbstractApi instance1 = ModelAbstractApi.CreateModel();
            ModelAbstractApi instance2 = ModelAbstractApi.CreateModel();
            Assert.Same(instance1, instance2);
            instance1.Dispose();
            Assert.Throws<ObjectDisposedException>(() => instance2.Dispose());
        }
    }
}