using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.MathUtil
{
    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Returns the cross-product between two vectors.
        /// </summary>
        /// <param name="one">The first vector</param>
        /// <param name="two">The second vector</param>
        /// <returns>The cross-product</returns>
        public static Vector3 Cross(Vector3 one, Vector3 two)
        {
            Vector3 result;
            result.X = one.Y * two.Z - one.Z * two.Y;
            result.Y = one.Z * two.X - one.X * two.Z;
            result.Z = one.X * two.Y - one.Y * two.X;
            return result;
        }
    }
}
