

namespace FW_LJ_CP.Data
{
  internal class Ball : IBall
{
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity, double mass, int Id)
    {
        Position = initialPosition;
        Velocity = initialVelocity;
            Mass = mass;
            Diameter = ComputeDiameter(mass);
            ballId = Id;
        }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }
        public double Mass { get; private set; }
        public double Diameter { get; private set; }

        public int ballId { get; }

        #endregion IBall

        #region private

        internal Vector Position { get; private set; }

        private void RaiseNewPositionChangeNotification()
    {
        NewPositionNotification?.Invoke(this, Position);
    }

    internal void Move(Vector delta, double tableWidth, double tableHeight, CollisionLogger? logger = null)
    {
            bool wallHit = false;
            double newX = Math.Clamp(Position.x + delta.x, 0, tableWidth - Diameter);
            double newY = Math.Clamp(Position.y + delta.y, 0, tableHeight - Diameter);

            if (newX == 0 || newX == tableWidth - Diameter)
            {
                Velocity = new Vector(-Velocity.x, Velocity.y);
                wallHit = true;
            }
            if (newY == 0 || newY == tableHeight - Diameter)
            {
                Velocity = new Vector(Velocity.x, -Velocity.y);
                wallHit = true;
            }
            Position = new Vector(newX, newY);

            if(logger != null && wallHit)
            {
                logger.LogBall2WallCollision(
                    Position.x, Position.y, Velocity.x, Velocity.y, ballId,
                    Diameter / 2.0, tableWidth, tableHeight);
            }

            RaiseNewPositionChangeNotification();
    }

        private static double ComputeDiameter(double mass)
        {
            const double baseDiameter = 20.0;
            const double scalingFactor = 20.0;
            return baseDiameter + (mass - 1.0) * scalingFactor;
        }

        #endregion private
    }
}