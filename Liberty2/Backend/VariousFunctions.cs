using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Liberty.Backend
{
    public class VariousFunctions
    {
        /// <summary>
        /// Create a filename of a file that doesn't exist in temporary storage
        /// </summary>
        /// <param name="directory">The directory to create the file in</param>
        public static string CreateTemporaryFile(string directory)
        {
            string file = "";
            Random ran = new Random();
            while (true)
            {
                string filename = ran.Next(0, Int32.MaxValue).ToString() + ".ext";
                file = directory + filename;

                if (!File.Exists(file))
                    break;
            }

            return file;
        }
        /// <summary>
        /// Clean up all temporary files stored by Assembly (Images, Error Logs)
        /// </summary>
        public static void CleanUpTemporaryFiles()
        {
            List<FileInfo> files = new List<FileInfo>();

            DirectoryInfo tmpExt = new DirectoryInfo(GetTemporaryExtractionLocation());
            files.AddRange(new List<FileInfo>(tmpExt.GetFiles("*.ext")));
            DirectoryInfo tmpLog = new DirectoryInfo(GetTemporaryErrorLogs());
            files.AddRange(new List<FileInfo>(tmpLog.GetFiles("*.tmp")));

            foreach (FileInfo fi in files)
                try { fi.Delete(); } catch { }
        }
        /// <summary>
        /// Gets the parent directory of the application's exe, with trailing backslash
        /// </summary>
        public static string GetApplicationLocation()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
        }
        /// <summary>
        /// Gets the location of the applications assembly (lulz, assembly.exe)
        /// </summary>
        public static string GetApplicationAssemblyLocation()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }
        /// <summary>
        /// Get the temporary location to save extracted content
        /// </summary>
        public static string GetTemporaryExtractionLocation()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XboxChaos\\Liberty\\content_extraction\\";
        }
        /// <summary>
        /// Get the temporary location to save update stuff
        /// </summary>
        public static string GetTemporaryInstallerLocation()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XboxChaos\\Liberty\\update_storage\\";
        }
        /// <summary>
        /// Get the temporary location to save error_logs
        /// </summary>
        public static string GetTemporaryErrorLogs()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\XboxChaos\\Liberty\\error_logs\\";
        }
        /// <summary>
        /// Create the temporary Directories
        /// </summary>
        public static void CreateTemporaryDirectories()
        {
            if (!Directory.Exists(GetTemporaryExtractionLocation()))
                Directory.CreateDirectory(GetTemporaryExtractionLocation());

            if (!Directory.Exists(GetTemporaryErrorLogs()))
                Directory.CreateDirectory(GetTemporaryErrorLogs());

            if (!Directory.Exists(GetTemporaryInstallerLocation()))
                Directory.CreateDirectory(GetTemporaryInstallerLocation());
        }

        /// <summary>
        /// Write a Stream to a chosen filepath
        /// </summary>
        public void SaveStreamToFile(string fileFullPath, Stream stream)
        {
            if (stream.Length == 0) return;

            // Create a FileStream object to write a stream to a file
            using (FileStream fileStream = System.IO.File.Create(fileFullPath, (int)stream.Length))
            {
                // Fill the bytes[] array with the stream data
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static char[] DisallowedPluginChars = new char[] { ' ', '>', '<', ':', '-', '_', '/', '\\', '&', ';', '!', '?', '|', '*', '"' };
    }
}