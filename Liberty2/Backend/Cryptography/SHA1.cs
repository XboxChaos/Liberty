using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Liberty.Backend.Cryptography
{
    public class SHA1Crypto
    {
        public static string ComputeHashToString(string str)
        {
            Encoder enc = System.Text.Encoding.Unicode.GetEncoder();
            byte[] unicodeText = new byte[str.Length * 2];
            enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] result = sha1.ComputeHash(unicodeText);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }
            return sb.ToString();
        }
        public static string ComputeHashToString(byte[] byteArr)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] result = sha1.ComputeHash(byteArr);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static byte[] ComputeHashToBA(string str)
        {
            Encoder enc = System.Text.Encoding.Unicode.GetEncoder();
            byte[] unicodeText = new byte[str.Length * 2];
            enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

            SHA1 sha1 = new SHA1CryptoServiceProvider();
            return sha1.ComputeHash(unicodeText);
        }
        public static byte[] ComputeHashToBA(byte[] byteArr, bool useSalt = false)
        {
            if (useSalt)
            {
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                sha1.TransformBlock(_salt, 0, _salt.Length, _salt, 0);
                sha1.TransformFinalBlock(byteArr, 0, byteArr.Length);
                return sha1.Hash;
            }
            else
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                return sha1.ComputeHash(byteArr);
            }
        }

        /// <summary>
        /// The salt used to verify the validity of saves
        /// </summary>
        private static byte[] _salt = new byte[]
        {
            0xED, 0xD4, 0x30, 0x09, 0x66, 0x6D, 0x5C, 0x4A, 0x5C, 0x36, 0x57,
            0xFA, 0xB4, 0x0E, 0x02, 0x2F, 0x53, 0x5A, 0xC6, 0xC9, 0xEE, 0x47,
            0x1F, 0x01, 0xF1, 0xA4, 0x47, 0x56, 0xB7, 0x71, 0x4F, 0x1C, 0x36,
            0xEC
        };
    }
} 
