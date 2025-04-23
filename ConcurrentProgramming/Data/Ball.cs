namespace FW_LJ_CP.Data
{
  internal class Ball : IBall
{
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity)
    {
        Position = initialPosition;
        Velocity = initialVelocity;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }

    #endregion IBall

    #region private

    private Vector Position;

    private const double BallDiameter = 20.0;
    public const double TableWidth = 400;
    public const double TableHeight = 420;

        private void RaiseNewPositionChangeNotification()
    {
        NewPositionNotification?.Invoke(this, Position);
    }

    internal void Move(Vector delta)
    {
            double newX = Math.Clamp(Position.x + delta.x, 0, TableWidth - BallDiameter);
            double newY = Math.Clamp(Position.y + delta.y, 0, TableHeight - BallDiameter);

            if (newX == 0 || newX == TableWidth - BallDiameter)
            {
                Velocity = new Vector(-Velocity.x, Velocity.y);
            }
            if (newY == 0 || newY == TableHeight - BallDiameter)
            {
                Velocity = new Vector(Velocity.x, -Velocity.y);
            }
            Position = new Vector(newX, newY);
        RaiseNewPositionChangeNotification();
    }

    #endregion private
}
}