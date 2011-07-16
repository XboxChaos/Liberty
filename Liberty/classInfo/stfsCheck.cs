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
using X360;
using X360.STFS;
using X360.IO;
using Microsoft.Win32;
using X360.FATX;
using Liberty.classInfo.storage;

namespace Liberty.classInfo
{
    class stfsCheck
    {
        /// <summary>
        /// Check if the STFS package is a valid campaign gamesave
        /// </summary>
        /// <param name="fileName">STFS Package File Directory</param>
        public static string checkSTFSPackage(string fileName)
        {
            STFSPackage package = null;
            try
            {
                package = new STFSPackage(fileName, null);
                X360.STFS.FileEntry file = package.GetFile("mmiof.bmf");
                string extractedDirec = extraIO.createNewBackup(file);
                storage.fileInfoStorage.fileExtractDirectory = extractedDirec;
                storage.fileInfoStorage.fileOriginalDirectory = fileName;
                storage.fileInfoStorage.saveData = new Reach.CampaignSave(storage.fileInfoStorage.fileExtractDirectory);
                package.CloseIO();
                return "yes";
            }
            catch (Exception ex)
            {
                if (package != null)
                    package.CloseIO();
                return ex.ToString();
            }
        }
        public static string finishFileEditing()
        {
            storage.fileInfoStorage.saveData.Update();
            storage.fileInfoStorage.saveData.Close();

            STFSPackage package = null;
            try
            {
                package = new STFSPackage(storage.fileInfoStorage.fileOriginalDirectory, null);
                X360.STFS.FileEntry file = package.GetFile("mmiof.bmf");
                file.Inject(storage.fileInfoStorage.fileExtractDirectory);
                string KV = extraIO.createTempKV();
                package.FlushPackage(new X360.STFS.RSAParams(KV));
                extraIO.deleteTempKV();
                package.CloseIO();

                return "yes";
            }
            catch (Exception ex)
            {
                if (package != null)
                    package.CloseIO();
                return ex.ToString();
            }
        }
    }
}
