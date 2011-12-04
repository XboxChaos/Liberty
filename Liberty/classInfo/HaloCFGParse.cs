/*
 * Denis Bekman 2009
 * www.youpvp.com/blog
 --
 * This code is licensed under a Creative Commons Attribution 3.0 United States License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/3.0/us/
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Liberty.classInfo
{
    public class HaloCFGParse
    {
        IList<CFGData> cfgData = new List<CFGData>();

        public void Parse(string s)
        {
            char[] finder = s.ToCharArray();
            for(int i=0; i < s.Length;i++)
            {
                if (finder[i] == 'm' && finder[i + 1] == 'o' && finder[i + 2] == 'd' && finder[i + 3] == 'e')
                    s = s.Remove(0, i - 1);
            }
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
