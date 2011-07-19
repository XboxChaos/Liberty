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
using X360.FATX;
using System.Windows.Controls;

namespace Liberty.classInfo.storage
{
    class fileInfoStorage
    {
        public static string fileExtractDirectory = null;
        public static string fileOriginalDirectory = null;

        public static byte[] saveFileArray = null;
        public static Reach.CampaignSave saveData = null;

        public static Reach.TagGroup massCordMoveType;
        public static float _massCordX = 0;
        public static float _massCordY = 0;
        public static float _massCordZ = 0;

        public static bool connectedToUpdate;

        public static bool saveIsLocal;
        public static FATX.Folder folderToInjectDevice;
        public static FATX.File oldFileInFolder;
        public static FATX.FATXDrive xChosenDrive;
        public static string driveName;
        public static int driveType;

        public static Util.TagList tagList = null;

        public static bool messageOpt = false;
        public static bool leavingStep2 = false;

        public static bool updStart = false;

        public static List<ListBoxItem> listboxItems = null;
        public static ListBoxItem selectedListboxItem = null;
        public static string replaceObjectName;
    }
}
