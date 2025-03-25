namespace FW_LJ_CP.BusinessLogic
{
  internal class Ball : IBall
{
    public Ball(Data.IBall ball)
    {
        ball.NewPositionNotification += RaisePositionChangeEvent;
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    #endregion IBall

    #region private

    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
        NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }

    #endregion private
}
}