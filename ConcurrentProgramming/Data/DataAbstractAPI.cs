namespace FW_LJ_CP.Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        #region Layer Factory

        public static DataAbstractAPI GetDataLayer()
        {
            return modelInstance.Value;
        }

        #endregion Layer Factory

        #region public API

        public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);

        #endregion public API

        #region IDisposable

        public abstract void Dispose();

        #endregion IDisposable

        #region private

        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

        #endregion private
    }

    public interface IVector
    {
        /// <summary>
        /// The X component of the vector.
        /// </summary>
        double x { get; init; }

        /// <summary>
        /// The y component of the vector.
        /// </summary>
        double y { get; init; }
    }

    public interface IBall
    {
        event EventHandler<IVector> NewPositionNotification;
        double Diameter { get; }

        IVector Velocity { get; set; }
    }
}