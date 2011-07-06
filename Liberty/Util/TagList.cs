using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace Liberty.Util
{
    /// <summary>
    /// Provides methods for reading INI-style taglists.
    /// </summary>
    class TagList
    {
        public static TagList FromFile(string path)
        {
            return new TagList(File.OpenRead(path));
        }

        public static TagList FromString(string data)
        {
            return new TagList(new MemoryStream(System.Text.ASCIIEncoding.Default.GetBytes(data)));
        }

        /// <summary>
        /// Translates a tag ID into a name string and returns it.
        /// </summary>
        /// <param name="group">The group in the taglist to read from.</param>
        /// <param name="id">The tag ident to translate</param>
        /// <returns>The name of the tag, or the ID in hex form if none.</returns>
        public string Translate(string group, uint id)
        {
            TagKey key = new TagKey { group = group.ToLower(), id = id };
            if (_tags.ContainsKey(key))
                return _tags[key];
            else
                return "0x" + id.ToString("X");
        }

        private struct TagKey
        {
            public string group;
            public uint id;
        }
        
        private TagList(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            bool inBlock = false;
            string currentGroup = "";
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().Trim();
                if (line.Length == 0 || line.StartsWith(";"))
                    continue;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    // Enter a new block
                    inBlock = true;
                    currentGroup = line.Trim(new char[3] { '[', ']', ' ' }).ToLower();
                    if (currentGroup.Length == 0)
                        throw new ArgumentException("The taglist is invalid.\r\nGroup names must contain at least one non-whitespace character.");
                }
                else if (inBlock)
                {
                    // Read a key = value pair
                    int equalsPos = line.IndexOf('=');
                    if (equalsPos == -1)
                        throw new ArgumentException("The taglist is invalid.\r\nExpressions in a block must be key = value pairs.");
                    string idStr = line.Substring(0, equalsPos).TrimStart(null);
                    string name = line.Substring(equalsPos + 1).Trim();
                    
                    // Parse it
                    uint id;
                    if (idStr.StartsWith("0x"))
                    {
                        // Parse a hex number
                        if (!uint.TryParse(idStr.Substring(2), NumberStyles.HexNumber, null, out id))
                            throw new ArgumentException("The taglist is invalid.\r\nKeys starting with 0x must be valid hex numbers.");
                    }
                    else
                    {
                        // Parse a decimal number
                        if (!uint.TryParse(idStr, out id))
                            throw new ArgumentException("The taglist is invalid.\r\nKeys with no prefix must be valid decimal integers.");
                    }

                    // Store it
                    _tags[new TagKey { group = currentGroup, id = id }] = name;
                }
            }
            reader.Close();
        }

        private Dictionary<TagKey, string> _tags = new Dictionary<TagKey, string>();
    }
}
