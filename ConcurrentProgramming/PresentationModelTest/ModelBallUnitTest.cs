using FW_LJ_CP.BusinessLogic;

namespace FW_LJ_CP.Presentation.Model.Test
{
    public class ModelBallUnitTest
    {
        [Fact]
        public void ConstructorTestMethod()
        {
            ModelBall ball = new ModelBall(0.0, 0.0, new BusinessLogicIBallFixture());
            Assert.Equal<double>(0.0, ball.Top);
            Assert.Equal<double>(0.0, ball.Left);
        }

        [Fact]
        public void PositionChangeNotificationTestMethod()
        {
            int notificationCounter = 0;
            ModelBall ball = new ModelBall(0, 0.0, new BusinessLogicIBallFixture());
            ball.PropertyChanged += (sender, args) => notificationCounter++;
            Assert.Equal(0, notificationCounter);
            ball.SetLeft(1.0);
            Assert.Equal<int>(1, notificationCounter);
            Assert.Equal<double>(1.0, ball.Left);
            Assert.Equal<double>(0.0, ball.Top);
            ball.SettTop(1.0);
            Assert.Equal(2, notificationCounter);
            Assert.Equal<double>(1.0, ball.Left);
            Assert.Equal<double>(1.0, ball.Top);
        }

        #region testing instrumentation

        private class BusinessLogicIBallFixture : BusinessLogic.IBall
        {
            public event EventHandler<IPosition>? NewPositionNotification;

            public double Diameter => 20.0;
            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        #endregion testing instrumentation
    }
}