/*
* Liberty - http://xboxchaos.com/
*
* Copyright (C) 2011 XboxChaos
* Copyright (C) 2011 ThunderWaffle/AMD
* Copyright (C) 2011 Xeraxic
*
* Liberty is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published
* by the Free Software Foundation; either version 3 of the License,
* or (at your option) any later version.
*
* Liberty is distributed in the hope that it will be useful, but
* WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
* General Public License for more details.
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Liberty.classInfo
{
    class updating
    {
        public static void startUpdate()
        {
            string temp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\update\\";
            try { Directory.CreateDirectory(temp); }
            catch { }

            iniFile ini = new iniFile(temp + "update.ini");

            ini.IniWriteValue("AppInfo", "appName", "Liberty");
            ini.IniWriteValue("AppInfo", "appVer", Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".", ""));
            ini.IniWriteValue("AppInfo", "appDir", Assembly.GetExecutingAssembly().Location);
        }
    }
}
