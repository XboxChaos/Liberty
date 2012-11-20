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
        /// Adds the components of two vectors to create a new vector.
        /// </summary>
        /// <param name="one">The first vector</param>
        /// <param name="two">The second vector</param>
        /// <returns>A vector containing the components added together.</returns>
        public static Vector3 Add(Vector3 one, Vector3 two)
        {
            float x = one.X + two.X;
            float y = one.Y + two.Y;
            float z = one.Z + two.Z;
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Subtracts the components of a vector from the components of another.
        /// </summary>
        /// <param name="one">The vector to subtract from</param>
        /// <param name="two">The vector to subtract</param>
        /// <returns>A vector containing the subtracted components.</returns>
        public static Vector3 Subtract(Vector3 one, Vector3 two)
        {
            float x = one.X - two.X;
            float y = one.Y - two.Y;
            float z = one.Z - two.Z;
            return new Vector3(x, y, z);
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

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Vector3 other = (Vector3)obj;
            return (X == other.X && Y == other.Y && Z == other.Z);
        }
    }
}
