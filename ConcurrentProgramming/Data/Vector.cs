namespace FW_LJ_CP.Data
{
    /// <summary>
    ///  Two dimensions immutable vector
    /// </summary>
    internal record Vector : IVector
    {
        #region IVector

        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public double x { get; init; }
        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        public double y { get; init; }

        #endregion IVector

        /// <summary>
        /// Creates new instance of <seealso cref="Vector"/> and initialize all properties
        /// </summary>
        public Vector(double XComponent, double YComponent)
        {
            x = XComponent;
            y = YComponent;
        }
    }
}
