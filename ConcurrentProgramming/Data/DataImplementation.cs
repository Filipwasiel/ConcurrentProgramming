using System.Diagnostics;

namespace FW_LJ_CP.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor

        public DataImplementation()
        {
            MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16.67));
        }

        #endregion ctor

        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            Random random = new Random();
            lock (_lock)
            {
                BallsList.Clear();
            }
            for (int i = 0; i < numberOfBalls; i++)
            {
                Vector startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
                Vector startingVelocity = new((random.NextDouble() - 0.5) * 10, (random.NextDouble() - 0.5) * 10);
                double randomMass = 1.0 + random.NextDouble() * 0.5; 
                Ball newBall = new(startingPosition, startingVelocity, randomMass);
                upperLayerHandler(startingPosition, newBall);
                lock (_lock)
                {
                    BallsList.Add(newBall);
                }
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
                    MoveTimer.Dispose();
                    BallsList.Clear();
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

        //private bool disposedValue;
        private bool Disposed = false;

        private readonly Timer MoveTimer;
        private Random RandomGenerator = new();
        private List<Ball> BallsList = [];

        private void Move(object? x)
        {
            lock (_lock)
            {
                for (int i = 0; i < BallsList.Count; i++)
                {
                    for (int j = i + 1; j < BallsList.Count; j++)
                    {
                        HandleElasticCollision(BallsList[i], BallsList[j]);
                    }
                }
                foreach (Ball ball in BallsList)
                {
                    ball.Move(new Vector(ball.Velocity.x, ball.Velocity.y));
                }
            }

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
