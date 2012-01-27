using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

namespace Liberty.Util
{
    public sealed class AscensionTagList
    {
        public static TagList LoadFromString(string map, string contents)
        {
            TagList result = new TagList();

            // I borrowed these hardcoded string indices from the Ascension source code
            // I know, the code sucks and can allow for very misleading taglists, but it's not my problem
            string[] lines = contents.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                line = line.Trim();

                uint id;
                if (!uint.TryParse(line.Substring(2, 8), NumberStyles.HexNumber, null, out id))
                    throw new ArgumentException("The taglist is invalid at line " + (i + 1) + ":\r\nTag IDs must be hexadecimal integers.");

                string path = line.Substring(11);
                result.Add(map, id, path);
            }

            return result;
        }

        public static TagList LoadFromFile(string path)
        {
            string map = Path.GetFileNameWithoutExtension(path);
            return LoadFromString(map, File.ReadAllText(path, Encoding.UTF8));
        }
    }
}
