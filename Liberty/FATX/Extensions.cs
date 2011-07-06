using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FATX;
using System.Windows.Controls;

namespace Extensions
{
    static class Extensions
    {
        public static Entry Entry(ComboBoxItem node)
        {
            return (Entry)node.Tag;
        }

        public static string ToHexString(this byte[] ByteArray)
        {
            string r = "";
            for (int i = 0; i < ByteArray.Length; i++)
            {
                r += ByteArray[i].ToString("X2");
            }
            return r;
        }

        public static string ToASCIIString(this byte[] ByteArray)
        {
            string r = "";
            for (int i = 0; i < ByteArray.Length; i++)
            {
                r += ByteArray[i].ToString();
            }
            return r;
        }

        /// <summary>
        /// Rounds a number down to the nearest 0x200 byte boundary
        /// </summary>
        public static long DownToNearest200(this long val)
        {
            return (val -= (val % 0x200));
        }

        /// <summary>
        /// Rounds a number up to the nearest 0x200 byte boundary
        /// </summary>
        public static long UpToNearest200(this long val)
        {
            long valToAdd = 0x200 - (val % 0x200);
            if (valToAdd == 0x200)
            {
                return val;
            }
            return val + valToAdd;
        }

        /// <summary>
        /// Rounds a number up to the nearest cluster boundary
        /// </summary>
        public static long UpToNearestCluster(this long val, long ClusterSize)
        {
            long valToAdd = ClusterSize - (val % ClusterSize);
            if (valToAdd == ClusterSize)
            {
                return val;
            }
            return val + valToAdd;
        }

        /// <summary>
        /// Rounds a number up to the nearest cluster boundary -- doesn't pay
        /// attention to whether or not we're adding the cluster size (so
        /// if it's already on the proper boundary, it's rounded up anyway)
        /// </summary>
        public static long UpToNearestClusterForce(this long val, long ClusterSize)
        {
            long valToAdd = ClusterSize - (val % ClusterSize);
            return val + valToAdd;
        }

        /// <summary>
        /// Rounds a number down to the nearest cluster value
        /// </summary>
        public static long DownToNearestCluster(this long val, long ClusterSize)
        {
            return (val -= (val % ClusterSize));
        }
    }
}
