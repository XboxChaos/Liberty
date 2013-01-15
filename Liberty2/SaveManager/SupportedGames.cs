using Liberty.Backend;
using Liberty.SaveManager.Flexibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Liberty.SaveManager
{
    public static class Manager
    {
        public static SupportedGames SupportedGame;

        public class SupportedGames
        {
            public SupportedGames() { LoadGames(); }
            private IList<SupportedGame>_games = new List<SupportedGame>();
            public IList<SupportedGame> Games
            {
                get { return _games; }
                private set { _games = value; }
            }

            public void LoadGames()
            {
                string applicationFormatsPath = VariousFunctions.GetApplicationLocation() + "Formats\\";

                // Load XContainer
                if (!File.Exists(applicationFormatsPath + "SupportedGames.xml"))
                    return;
                XDocument gameInfo = XDocument.Load(applicationFormatsPath + "SupportedGames.xml");
                XContainer supportedGames = gameInfo.Element("games");

                // Find the first game
                IList<XElement> games = supportedGames.Elements("game").ToList();

                if (games == null)
                    return;

                // Loop though games, getting their info
                foreach (XElement game in games)
                {
                    XAttribute nameAttrib = game.Attribute("name");
                    XAttribute safeNameAttrib = game.Attribute("safeName");
                    XAttribute diffucultyShieldAttrib = game.Attribute("diffucultyShield");
                    XAttribute titleIDAttrib = game.Attribute("titleID");
                    XAttribute subPackageNameAttrib = game.Attribute("subPackageName");
                    XAttribute hasGameStateAttrib = game.Attribute("hasGameState");
                    XAttribute stfsStartsWithAttrib = game.Attribute("stfsStartsWith");
                    XAttribute securityTypeAttrib = game.Attribute("securityType");
                    XAttribute securitySaltAttrib = game.Attribute("securitySalt");
                    XAttribute filenameAttrib = game.Attribute("filename");

                    // Check compulsary fields
                    if (nameAttrib != null && safeNameAttrib != null &&
                        diffucultyShieldAttrib != null && titleIDAttrib != null &&
                        subPackageNameAttrib != null && hasGameStateAttrib != null &&
                        stfsStartsWithAttrib != null && securityTypeAttrib != null && 
                        filenameAttrib != null)
                    {
                        SupportedGame supportedGame = new SupportedGame();
                        supportedGame.Name = nameAttrib.Value;
                        supportedGame.SafeName = safeNameAttrib.Value;
                        supportedGame.TitleID = long.Parse(titleIDAttrib.Value.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
                        supportedGame.SubPackageName = subPackageNameAttrib.Value;
                        supportedGame.HasGamestate = Convert.ToBoolean(hasGameStateAttrib.Value);
                        supportedGame.STFSStartsWith = stfsStartsWithAttrib.Value;
                        supportedGame.SecurityType = securityTypeAttrib.Value.ToLower() == "SHA1" ? 
                            SecurityType.SHA1 : SecurityType.CRC32;
                        if (securitySaltAttrib != null)
                            supportedGame.SecuritySalt = VariousFunctions.StringToByteArray(securitySaltAttrib.Value.Replace("0x", "").Replace(" ", "").Replace(",", ""));
                        supportedGame.FileName = filenameAttrib.Value;
                        LoadGameLayout(applicationFormatsPath + supportedGame.FileName, supportedGame);

                        _games.Add(supportedGame);
                    }
                }
            }

            public void LoadGameLayout(string path, SupportedGame game)
            {
                // Load XContainer
                if (!File.Exists(path))
                    return;

                XDocument layoutDocument = XDocument.Load(path);

                // Make sure there is a root <layouts> tag
                XContainer layoutContainer = layoutDocument.Element("layouts");
                if (layoutContainer == null)
                    throw new ArgumentException("Invalid layout document");

                // Layout tags have the format:
                // <layout for="(layout's purpose)">(structure fields)</layout>
                foreach (XElement layout in layoutContainer.Elements("layout"))
                {
                    XAttribute forAttrib = layout.Attribute("for");
                    if (forAttrib == null)
                        throw new ArgumentException("Layout tags must have a \"for\" attribute");
                    game.AddLayout(forAttrib.Value, XMLLayoutLoader.LoadLayout(layout));
                }
            }

            public enum SecurityType
            {
                SHA1,
                CRC32
            }
            public class SupportedGame
            {
                public string Name { get; set; }
                public string SafeName { get; set; }
                public string DifficultyShield { get; set; }
                public long TitleID { get; set; }
                public string SubPackageName { get; set; }
                public bool HasGamestate { get; set; }
                public string STFSStartsWith { get; set; }
                public SecurityType SecurityType { get; set; }
                public byte[] SecuritySalt { get; set; }
                public string FileName { get; set; }

                public void AddLayout(string name, StructureLayout layout)
                {
                    _layouts[name] = layout;
                }
                public StructureLayout GetLayout(string name)
                {
                    return _layouts[name];
                }
                public bool HasLayout(string name)
                {
                    return _layouts.ContainsKey(name);
                }

                private Dictionary<string, StructureLayout> _layouts = new Dictionary<string, StructureLayout>();
            }
        }
    }
}
