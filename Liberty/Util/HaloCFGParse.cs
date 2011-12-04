using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Liberty.Util.HaloCFGParse
{
    public class HCEXParser
    {
        IList<CFGData> cfgData = new List<CFGData>();

        public void Parse(string s)
        {
            string[] file = s.Replace("\t", "").Replace("\"", "").Replace("    ","").Split('\n');

            cfgData.Clear();
            CFGData dta = new CFGData();

            for (int i = 9; i < (int)file.Length; i++)
            {
                if (file[i] != "}" || file[i] != "{" || file[i] != "")
                {
                    if (file[i].StartsWith("mode"))
                        dta.Mode = file[i].Replace("mode=","");

                    if (file[i].StartsWith("level_id"))
                        dta.LevelID = file[i].Replace("level_id=", "");

                    if (file[i].StartsWith("checkpoint_id"))
                        dta.CheckpointID = file[i].Replace("checkpoint_id=", "");

                    if (file[i].StartsWith("difficulty"))
                        dta.DifficultyID = int.Parse(file[i].Replace("difficulty=", ""));

                    if (dta.DifficultyID == 0)
                        dta.Difficulty = "Easy";
                    else if (dta.DifficultyID == 1)
                        dta.Difficulty = "Normal";
                    else if (dta.DifficultyID == 2)
                        dta.Difficulty = "Heroic";
                    else if (dta.DifficultyID == 3)
                        dta.Difficulty = "Legendary";
                }
            }

            cfgData.Add(dta);
        }

        public CFGData getParsedData()
        {
            if (cfgData.Count == 0)
                throw new ArgumentException("No CFG Data has been parsed. Cannot output.");
            else
                return cfgData[0];
        }

        public class CFGData
        {
            public string Mode { get; set; }

            public string LevelID { get; set; }
            public string CheckpointID { get; set; }

            public int DifficultyID { get; set; }
            public string Difficulty { get; set; }
        }
    }
}
