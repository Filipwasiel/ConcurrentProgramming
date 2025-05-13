// See https://aka.ms/new-console-template for more information
namespace FW_LJ_CP.BusinessLogic
{
  internal class Ball : IBall
{
    public Ball(Data.IBall ball)
    {
            _ball = ball;
            ball.NewPositionNotification += RaisePositionChangeEvent;
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

        public double Diameter => _ball.Diameter;

        #endregion IBall

        #region private

        private readonly Data.IBall _ball;

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
        NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }

    #endregion private
}
}