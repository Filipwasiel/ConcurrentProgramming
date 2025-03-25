namespace FW_LJ_CP.BusinessLogic.Test
{
    public class BallUnitTest
    {
        [Fact]
        public void MoveTestMethod()
        {
            DataBallFixture dataBallFixture = new DataBallFixture();
            Ball newInstance = new(dataBallFixture);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) => { Assert.NotNull(sender); Assert.NotNull(position); numberOfCallBackCalled++; };
            dataBallFixture.Move();
            Assert.Equal<int>(1, numberOfCallBackCalled);
        }

        #region testing instrumentation

        private class DataBallFixture : Data.IBall
        {
            public Data.IVector Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public event EventHandler<Data.IVector>? NewPositionNotification;
            public void Move()
            {
                NewPositionNotification?.Invoke(this, new VectorFixture(0.0, 0.0));
            }
        }

        private class VectorFixture : Data.IVector
        {
            internal VectorFixture(double X, double Y)
            {
                x = X;
                y = Y;
            }
            public double x { get; init; }
            public double y { get; init; }
        }
        #endregion testing instrumentation
    }
}