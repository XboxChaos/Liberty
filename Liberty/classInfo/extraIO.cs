/*
* Liberty - http://xboxchaos.com/
*
* Copyright (C) 2011 XboxChaos
* Copyright (C) 2011 ThunderWaffle/AMD
* Copyright (C) 2011 Xeraxic
*
* Liberty is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published
* by the Free Software Foundation; either version 3 of the License,
* or (at your option) any later version.
*
* Liberty is distributed in the hope that it will be useful, but
* WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
* General Public License for more details.
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X360.STFS;
using System.IO;
using System.Security.Cryptography;

namespace Liberty.classInfo
{
    class extraIO
    {
        public static string createNewBackup(FileEntry saveFile)
        {
            Random ran = new Random();
            string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\reachSaveBackup\\" + ran.Next(0, 1000) + "\\";
            Directory.CreateDirectory(temp);
            temp = temp + "reach.lib";
            if (!saveFile.Extract(temp))
                return null;
            return temp;
        }

        public static string createTempKV()
        {
            Random ran = new Random();
            string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\tempItems\\";
            if (!Directory.Exists(temp)) { Directory.CreateDirectory(temp); }
            temp = temp + "KV.bin";
            File.WriteAllBytes(temp, Liberty.Properties.Resources.KV);

            return temp;
        }

        public static void deleteTempKV()
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\tempItems\\KV.bin")) { File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\tempItems\\KV.bin"); }
        }

        public static string getMD5Hash(string file)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 8192);
            md5.ComputeHash(stream);
            stream.Close();

            byte[] hash = md5.Hash;
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(string.Format("{0:X2}", b));
            }
            return sb.ToString();
        }

        public static string getMD5HashCatch(string file)
        {
            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 8192);
                md5.ComputeHash(stream);
                stream.Close();

                byte[] hash = md5.Hash;
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.Append(string.Format("{0:X2}", b));
                }
                return sb.ToString();
            }
            catch { return ""; }
        }
    }
}
