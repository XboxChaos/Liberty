using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace Liberty.Util
{
    /// <summary>
    /// Holds a collection of grouped datum indices which can be translated into friendly names
    /// </summary>
    public class TagList
    {
        /// <summary>
        /// Adds a tag to the TagList
        /// </summary>
        /// <param name="group">The group to add to</param>
        /// <param name="id">The datum index</param>
        /// <param name="name">The index's name</param>
        public void Add(string group, uint id, string name)
        {
            Dictionary<uint, string> groupKeys;
            if (!_tags.TryGetValue(group, out groupKeys))
            {
                groupKeys = new Dictionary<uint, string>();
                _tags[group] = groupKeys;
            }
            groupKeys[id] = name;
        }

        /// <summary>
        /// Translates a tag ID into a name string and returns it.
        /// </summary>
        /// <param name="group">The group in the taglist to read from.</param>
        /// <param name="id">The tag ident to translate</param>
        /// <returns>The name of the tag, or null if none.</returns>
        public string Translate(string group, uint id)
        {
            Dictionary<uint, string> groupKeys;
            if (_tags.TryGetValue(group, out groupKeys))
            {
                string name;
                if (groupKeys.TryGetValue(id, out name))
                    return name;
            }
            
            //return "0x" + id.ToString("X");
            return null;
        }

        private Dictionary<string, Dictionary<uint, string>> _tags = new Dictionary<string, Dictionary<uint, string>>();
    }
}
