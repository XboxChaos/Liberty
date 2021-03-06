﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.classInfo.storage.settings
{
    class applicationSettings
    {
        public static bool checkUpdatesOL;
        public static bool showChangeLog;

        public static bool displaySplash;
        public static bool checkDLL;
        public static bool enableEasterEggs;
        public static bool noWarnings;
        public static int splashTimer;

        public static bool getLatestTagLst;
        public static bool storeTaglistNoMem;
        public static bool extTaglistFrmAsc;
        public static string extTaglistFromAscDirec;

        public static int AccentColour;
        public static int ThemeColour;

        public class gameIdent
        {
            public static Util.SaveType gameID;
        }
    }
}
