using System.Diagnostics;

namespace FW_LJ_CP.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            lock (_lock)
            {
                BallsList.Clear();
            }
            _cts = new CancellationTokenSource();
            for (int i=0; i < numberOfBalls; i++)
            {
                Ball newBall = CreateBall();
                upperLayerHandler(newBall.Position, newBall);
                lock (_lock)
                {
                    BallsList.Add(newBall);
                }
                _ballTasks.Add(Task.Run(async () =>
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        lock (_lock)
                        {
                            newBall.Move(new Vector(newBall.Velocity.x, newBall.Velocity.y));
                            CheckCollisions(newBall);
                        }
                        await Task.Delay(16, _cts.Token);
                    }
                }, _cts.Token));
            }
        }

        #endregion DataAbstractAPI

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    _cts?.Cancel();
                    _cts?.Dispose();
                    BallsList.Clear();
                    _logger.Dispose();
                }
                Disposed = true;
            }
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private readonly object _lock = new();
        private CancellationTokenSource _cts;
        private bool Disposed = false;
        private Random RandomGenerator = new();
        private List<Ball> BallsList = [];
        private List<Task> _ballTasks = new();
        private int idIterator = 0;
        private readonly CollisionLogger _logger = new();

        private void CheckCollisions(Ball currentBall)
        {
            for (int i = 0; i < BallsList.Count; i++)
            {
                for (int j = i + 1; j < BallsList.Count; j++)
                {
                    HandleElasticCollision(BallsList[i], BallsList[j]);
                }
            }
        }
        private Ball CreateBall()
        {
            Vector startingPosition = new(RandomGenerator.Next(100, 400 - 100), RandomGenerator.Next(100, 400 - 100));
            Vector startingVelocity = new((RandomGenerator.NextDouble() - 0.5) * 10, (RandomGenerator.NextDouble() - 0.5) * 10);
            double randomMass = 1.0 + RandomGenerator.NextDouble() * 0.5;
            Ball newBall = new(startingPosition, startingVelocity, randomMass, idIterator);
            idIterator++;
            return newBall;
        }
        private void HandleElasticCollision(Ball ball1, Ball ball2)
        {
            Vector position1 = ball1.Position;
            Vector position2 = ball2.Position;
            Vector velocity1 = (Vector) ball1.Velocity;
            Vector velocity2 = (Vector) ball2.Velocity;
            double mass1 = ball1.Mass;
            double mass2 = ball2.Mass;
            Vector delta = position1 - position2;
            double distance = delta.Length();
            if (distance == 0) return;
            double minDistance = (ball1.Diameter + ball2.Diameter) / 2.0;
            if (distance < minDistance)
            {
                Vector normalizedDelta = delta / distance;
                Vector relativeVelocity = velocity1 - velocity2;
                double velocityAlongNormal = Vector.Dot(relativeVelocity, normalizedDelta);
                if (velocityAlongNormal > 0) return;
                double impulse = (2 * velocityAlongNormal) / (mass1 + mass2);
                Vector impulseVector = impulse * normalizedDelta;
                ball1.Velocity = velocity1 - impulseVector * mass2;
                ball2.Velocity = velocity2 + impulseVector * mass1;

                _logger.LogBall2BallCollision(
                    ball1.Position.x, ball1.Position.y, ((Vector)ball1.Velocity).x, ((Vector)ball1.Velocity).y, ball1.ballId,
                    ball2.Position.x, ball2.Position.y, ((Vector)ball2.Velocity).x, ((Vector)ball2.Velocity).y, ball2.ballId);
            }
        }
        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}
