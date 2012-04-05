using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.Reach
{
    /// <summary>
    /// Provides methods for managing taglists.
    /// </summary>
    public class TagListManager
    {
        public TagListManager(Util.SaveManager<CampaignSave> saveManager)
        {
            _saveManager = saveManager;
        }

        /// <summary>
        /// Attempts to identify an object in the campaign save.
        /// </summary>
        /// <param name="obj">The object to identify</param>
        /// <returns>The object's name</returns>
        public string Identify(Reach.GameObject obj)
        {
            string mapName = _saveManager.SaveData.Map;
            mapName = mapName.Substring(mapName.LastIndexOf('\\') + 1);

            // Generic taglists + name guessing
            foreach (Util.TagList tagList in _genericTaglists)
            {
                string name = tagList.Translate(mapName, obj.MapID);
                if (!string.IsNullOrEmpty(name))
                    return name;
            }

            // Map-specific taglists
            foreach (Util.TagList tagList in _mapTaglists)
            {
                string name = tagList.Translate(mapName, obj.MapID);
                if (!string.IsNullOrEmpty(name))
                    return name;
            }

            // Nothing found, just convert the ID to hex
            return "0x" + obj.MapID.ToString("X");
        }

        /// <summary>
        /// Adds a generic taglist (applies to all maps)
        /// </summary>
        /// <param name="tagList">The Util.TagList to add</param>
        public void AddGenericTaglist(Util.TagList tagList)
        {
            _genericTaglists.Add(tagList);
        }

        /// <summary>
        /// Adds a map-specific taglist
        /// </summary>
        /// <param name="tagList">The Util.TagList to add</param>
        public void AddMapSpecificTaglist(Util.TagList tagList)
        {
            _mapTaglists.Add(tagList);
        }

        /// <summary>
        /// Removes all generic taglists
        /// </summary>
        public void RemoveGenericTaglists()
        {
            _genericTaglists.Clear();
        }

        /// <summary>
        /// Removes all map-specific taglists
        /// </summary>
        public void RemoveMapSpecificTaglists()
        {
            _mapTaglists.Clear();
        }

        private Util.SaveManager<CampaignSave> _saveManager;
        private List<Util.TagList> _genericTaglists = new List<Util.TagList>();
        private List<Util.TagList> _mapTaglists = new List<Util.TagList>();
    }
}
