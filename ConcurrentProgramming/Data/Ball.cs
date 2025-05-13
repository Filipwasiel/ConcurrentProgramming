

namespace FW_LJ_CP.Data
{
  internal class Ball : IBall
{
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity, double mass)
    {
        Position = initialPosition;
        Velocity = initialVelocity;
            Mass = mass;
            Diameter = ComputeDiameter(mass);
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }
        public double Mass { get; private set; }
        public double Diameter { get; private set; }

        #endregion IBall

        #region private

        internal Vector Position { get; private set; }

        public const double TableWidth = 400;
    public const double TableHeight = 420;

        private void RaiseNewPositionChangeNotification()
    {
        NewPositionNotification?.Invoke(this, Position);
    }

    internal void Move(Vector delta)
    {
            double newX = Math.Clamp(Position.x + delta.x, 0, TableWidth - Diameter);
            double newY = Math.Clamp(Position.y + delta.y, 0, TableHeight - Diameter);

            if (newX == 0 || newX == TableWidth - Diameter)
            {
                Velocity = new Vector(-Velocity.x, Velocity.y);
            }
            if (newY == 0 || newY == TableHeight - Diameter)
            {
                Velocity = new Vector(Velocity.x, -Velocity.y);
            }
            Position = new Vector(newX, newY);
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