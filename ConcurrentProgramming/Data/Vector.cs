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

        public double Length () => Math.Sqrt(x * x + y * y);
        public static double Dot(Vector a, Vector b) => a.x * b.x + a.y * b.y;
        public static Vector operator +(Vector a, Vector b) => new Vector(a.x + b.x, a.y + b.y);
        public static Vector operator -(Vector a, Vector b) => new Vector(a.x - b.x, a.y - b.y);
        public static Vector operator *(double scalar, Vector v) => new Vector(scalar * v.x, scalar * v.y);
        public static Vector operator *(Vector v, double scalar) => scalar * v;
        public static Vector operator /(Vector v, double scalar)
        {
            if (scalar == 0)
                throw new DivideByZeroException("Cannot divide by zero");
            return new Vector(v.x / scalar, v.y / scalar);
        }

    }
}
