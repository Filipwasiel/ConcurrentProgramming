namespace FW_LJ_CP.Data.Test
{
    public class DataImplementationUnitTest
    {
        [Fact]
        public void ConstructorTestMethod()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                IEnumerable<IBall>? ballsList = null;
                newInstance.CheckBallsList(x => ballsList = x);
                Assert.NotNull(ballsList);
                int numberOfBalls = 0;
                newInstance.CheckNumberOfBalls(x => numberOfBalls = x);
                Assert.Equal<int>(0, numberOfBalls);
            }
        }

        [Fact]
        public void DisposeTestMethod()
        {
            DataImplementation newInstance = new DataImplementation();
            bool newInstanceDisposed = false;
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.False(newInstanceDisposed);
            newInstance.Dispose();
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.True(newInstanceDisposed);
            IEnumerable<IBall>? ballsList = null;
            newInstance.CheckBallsList(x => ballsList = x);
            Assert.NotNull(ballsList);
            newInstance.CheckNumberOfBalls(x => Assert.Equal<int>(0, x));
            Assert.Throws<ObjectDisposedException>(() => newInstance.Dispose());
            Assert.Throws<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
        }

        [Fact]
        public void StartTestMethod()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                int numberOfCallbackInvoked = 0;
                int numberOfBalls2Create = 10;
                newInstance.Start(
                  numberOfBalls2Create,
                  (startingPosition, ball) =>
                  {
                      numberOfCallbackInvoked++;
                      Assert.True(startingPosition.x >= 0);
                      Assert.True(startingPosition.y >= 0);
                      Assert.NotNull(ball);
                  });
                Assert.Equal<int>(numberOfBalls2Create, numberOfCallbackInvoked);
                newInstance.CheckNumberOfBalls(x => Assert.Equal<int>(10, x));
            }
        }

        [Fact]
        public async Task BallMovementHappensOverTime()
        {
            using (DataImplementation impl = new DataImplementation())
            {
                IVector? lastPosition = null;
                IBall? ball = null;
                ManualResetEventSlim positionUpdated = new(false);

                impl.Start(1, (pos, b) =>
                {
                    ball = b;
                    b.NewPositionNotification += (sender, vec) =>
                    {
                        lastPosition = vec;
                        positionUpdated.Set();
                    };
                });

                bool updated = positionUpdated.Wait(500);
                Assert.True(updated);
                Assert.NotNull(lastPosition);
            }
        }

        [Fact]
        public async Task CancellationStopsTasks()
        {
            DataImplementation impl = new();

            int notificationCount = 0;

            impl.Start(1, (pos, b) =>
            {
                b.NewPositionNotification += (sender, vec) => notificationCount++;
            });

            await Task.Delay(200);
            impl.Dispose();
            int countAfterDispose = notificationCount;

            await Task.Delay(200);
            Assert.Equal(countAfterDispose, notificationCount);
        }

        [Fact]
        public void Logger_WritesCollisionToFile()
        {
            string tempLogPath = Path.GetTempFileName();

            using var logger = new CollisionLogger(tempLogPath);

            logger.LogBall2BallCollision(
                x1: 10, y1: 10, vx1: 1, vy1: 1, ballId1: 1,
                x2: 20, y2: 20, vx2: -1, vy2: -1, ballId2: 2
            );

            logger.Stop();

            string[] logLines = File.ReadAllLines(tempLogPath);

            Assert.Single(logLines);
            Assert.Contains("Wykryto kolizje kula–kula", logLines[0]);
            Assert.Contains("Ball1 (ID=1", logLines[0]);
            Assert.Contains("Ball2 (ID=2", logLines[0]);

            File.Delete(tempLogPath);
        }

    }
}