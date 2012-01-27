using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

namespace Liberty.Util
{
    /// <summary>
    /// An INI-style taglist that contains tag definitions for multiple maps.
    /// </summary>
    public sealed class INITagList
    {
        public static TagList LoadFromString(string contents)
        {
            TagList result = new TagList();
            string currentGroup = null;

            string[] lines = contents.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // If there is a comment, only look at everything before it
                int commentPos = line.IndexOf(';');
                if (commentPos != -1)
                    line = line.Substring(0, commentPos);

                line = line.Trim();
                if (line.Length > 0)
                {
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        // Enter a new group
                        currentGroup = line.Trim(groupLineTrim).ToLower();
                        if (currentGroup.Length == 0)
                            throw new ArgumentException("The taglist is invalid at line " + (i + 1) + ":\r\nInvalid group name.");
                    }
                    else if (currentGroup != null)
                    {
                        // Read a key = value pair
                        int equalsPos = line.IndexOf('=');
                        if (equalsPos == -1)
                            throw new ArgumentException("The taglist is invalid at line " + (i + 1) + ":\r\nTags must be stored as key = value pairs.");
                        string idStr = line.Substring(0, equalsPos);
                        string name = line.Substring(equalsPos + 1).TrimStart(null);

                        // Parse it
                        uint id;
                        if (idStr.StartsWith("0x", true, null))
                        {
                            // Parse a hex number
                            if (!uint.TryParse(idStr.Substring(2), NumberStyles.HexNumber, null, out id))
                                throw new ArgumentException("The taglist is invalid at line " + (i + 1) + ":\r\nTag IDs starting with 0x must be hexadecimal integers.");
                        }
                        else
                        {
                            // Parse a decimal number
                            if (!uint.TryParse(idStr, out id))
                                throw new ArgumentException("The taglist is invalid at line " + (i + 1) + ":\r\nTag IDs without a 0x prefix must be decimal integers.");
                        }

                        // Store it
                        result.Add(currentGroup, id, name);
                    }
                }
            }

            return result;
        }

        public static TagList LoadFromFile(string path)
        {
            return LoadFromString(File.ReadAllText(path, Encoding.UTF8));
        }

        private static char[] groupLineTrim = new char[] { '[', ']', ' ', '\t' };
    }
}
