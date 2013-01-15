using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.Backend
{
    public class LibertyProtocol
    {
        public static void UpdateProtocol()
        {
            RegistryKey keyProtoBase = Registry.ClassesRoot.CreateSubKey("liberty\\");
            keyProtoBase.SetValue("", "URL:Liberty Application Manager");
            keyProtoBase.SetValue("AppUserModelID", "XboxChaos.Liberty.Default");
            keyProtoBase.SetValue("FriendlyTypeName", "Liberty Application Manager");
            keyProtoBase.SetValue("SourceFilter", "");
            keyProtoBase.SetValue("URL Protocol", "");

            RegistryKey keyProtoDefaultIcon = Registry.ClassesRoot.CreateSubKey("liberty\\DefaultIcon\\");
            keyProtoDefaultIcon.SetValue("", VariousFunctions.GetApplicationLocation() + "LibertyIconLibrary.dll,100");

            RegistryKey keyProtoExtensions = Registry.ClassesRoot.CreateSubKey("liberty\\Extensions\\");
            RegistryKey keyProtoShell = Registry.ClassesRoot.CreateSubKey("liberty\\shell\\");
            RegistryKey keyProtoShellOpen = Registry.ClassesRoot.CreateSubKey("liberty\\shell\\open\\");
            RegistryKey keyProtoShellOpenCommand = Registry.ClassesRoot.CreateSubKey("liberty\\shell\\open\\command\\");
            keyProtoShellOpenCommand.SetValue("", string.Format("\"{0}\" %1", VariousFunctions.GetApplicationAssemblyLocation()));
        }
    }
}
