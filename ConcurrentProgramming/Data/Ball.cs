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

    private void RaiseNewPositionChangeNotification()
    {
        NewPositionNotification?.Invoke(this, Position);
    }

    internal void Move(Vector delta)
    {
        Position = new Vector(Position.x + delta.x, Position.y + delta.y);
        RaiseNewPositionChangeNotification();
    }

    #endregion private
}
}