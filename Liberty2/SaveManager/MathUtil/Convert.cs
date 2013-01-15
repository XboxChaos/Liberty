using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.SaveManager.MathUtil
{
    public class Convert
    {
        /// <summary>
        /// Takes right, forward, and up vectors and converts them to yaw, pitch, and roll values.
        /// Useful for editing the rotation values in objects.
        /// </summary>
        /// <param name="right">The right vector</param>
        /// <param name="forward">The forward vector (can be computed as Vector3.Cross(up, right))</param>
        /// <param name="up">The up vector</param>
        /// <param name="yaw">The resulting yaw, in radians</param>
        /// <param name="pitch">The resulting pitch, in radians</param>
        /// <param name="roll">The resulting roll, in radians</param>
        public static void ToYawPitchRoll(Vector3 right, Vector3 forward, Vector3 up, out float yaw, out float pitch, out float roll)
        {
            // These formulas were written by Whirligig and converted to C# by me.
            // I take no credit for this.
            // -- AMD

            pitch = (float)Math.Atan2(-up.X, Math.Sqrt(up.Y * up.Y + up.Z * up.Z));

            try
            {
                roll = (float)Math.Atan2(up.Y, up.Z);
                yaw = (float)Math.Atan2(forward.X, right.X);
            }
            catch (DivideByZeroException)
            {
                // gimbal lock :O
                roll = (float)-Math.Atan2(forward.Z, forward.Y);
                yaw = 0;
            }
        }

        /// <summary>
        /// Converts a yaw, a pitch, and a roll into a right vector.
        /// </summary>
        /// <param name="yaw">The yaw, in radians</param>
        /// <param name="pitch">The pitch, in radians</param>
        /// <param name="roll">The roll, in radians</param>
        /// <returns>The computed right vector</returns>
        public static Vector3 ToRightVector(float yaw, float pitch, float roll)
        {
            // These formulas were written by Whirligig and converted to C# by me.
            // I take no credit for this.
            // -- AMD

            Vector3 result;
            result.X = (float)Math.Cos(pitch) * (float)Math.Cos(yaw);
            result.Y = (float)Math.Sin(pitch) * (float)Math.Sin(roll) * (float)Math.Cos(yaw) - (float)Math.Sin(yaw) * (float)Math.Cos(roll);
            result.Z = (float)Math.Sin(pitch) * (float)Math.Cos(roll) * (float)Math.Cos(yaw) + (float)Math.Sin(roll) * (float)Math.Sin(yaw);
            return result;
        }

        /// <summary>
        /// Converts a yaw, a pitch, and a roll into an up vector.
        /// </summary>
        /// <param name="yaw">The yaw, in radians</param>
        /// <param name="pitch">The pitch, in radians</param>
        /// <param name="roll">The roll, in radians</param>
        /// <returns>The computed up vector</returns>
        public static Vector3 ToUpVector(float yaw, float pitch, float roll)
        {
            // These formulas were written by Whirligig and converted to C# by me.
            // I take no credit for this.
            // -- AMD

            Vector3 result;
            result.X = (float)-Math.Sin(pitch);
            result.Y = (float)Math.Sin(roll) * (float)Math.Cos(pitch);
            result.Z = (float)Math.Cos(pitch) * (float)Math.Cos(roll);
            return result;
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">Radians</param>
        /// <returns>Degrees</returns>
        public static float ToDegrees(float radians)
        {
            return radians * (float)(180.0 / Math.PI);
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">Degrees</param>
        /// <returns>Radians</returns>
        public static float ToRadians(float degrees)
        {
            return degrees * (float)(Math.PI / 180.0);
        }
    }
}
