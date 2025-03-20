namespace FW_LJ_CP.BusinessLogic
{
    internal record Position : IPosition
    {
        #region IPosition

        public double x { get; init; }
        public double y { get; init; }

        #endregion IPosition

        /// <summary>
        /// Creates new instance of <seealso cref="IPosition"/> and initialize all properties
        /// </summary>
        public Position(double posX, double posY)
        {
            x = posX;
            y = posY;
        }
    }
}
