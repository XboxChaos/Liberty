using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Liberty.HCEX
{
    public class SaveCFG
    {
        public SaveCFG(string data)
        {
            string[] file = data.Replace("\t", "").Replace("\"", "").Replace("    ","").Split('\n');

            for (int i = 0; i < (int)file.Length; i++)
            {
                if (file[i] != "}" || file[i] != "{" || file[i] != "")
                {
                    if (file[i].StartsWith("mode"))
                        Mode = file[i].Replace("mode=","");

                    if (file[i].StartsWith("level_id"))
                        Level = file[i].Replace("level_id=", "");

                    if (file[i].StartsWith("checkpoint_id"))
                        CheckpointID = file[i].Replace("checkpoint_id=", "");

                    if (file[i].StartsWith("difficulty"))
                        DifficultyID = int.Parse(file[i].Replace("difficulty=", ""));

                    if (DifficultyID == 0)
                        Difficulty = "Easy";
                    else if (DifficultyID == 1)
                        Difficulty = "Normal";
                    else if (DifficultyID == 2)
                        Difficulty = "Heroic";
                    else if (DifficultyID == 3)
                        Difficulty = "Legendary";
                }
            }

        }

        public string Mode { get; set; }

        public string Level { get; set; }
        public string CheckpointID { get; set; }

        public int DifficultyID { get; set; }
        public string Difficulty { get; set; }
    }
}
