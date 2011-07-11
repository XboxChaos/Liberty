using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Liberty.classInfo.SkinningEngine
{
    class loadSettings
    {
        public static string exportTheme(string[] stepForms, string[] progressBar,
            string[] headerBar, string[] dialogs, string[] mainWindow, string Developer, string Name, string Desc, string creationDate, bool edited)
        {

            string tempPath = IO.PathData.createTempThemeINI();
            iniFile ini = new iniFile(tempPath);

            //Write Header
            ini.IniWriteValue("Header", "Developer", Developer);
            ini.IniWriteValue("Header", "ThemeName", Name);
            ini.IniWriteValue("Header", "ThemeDesc", Desc);
            if (edited == false)
                ini.IniWriteValue("Header", "CreationDate", DateTime.Today.ToString());
            else
                ini.IniWriteValue("Header", "CreationDate", creationDate);
            ini.IniWriteValue("Header", "EditDate", DateTime.Today.ToString());

            //WriteStepForms
            #region stepForms
            ini.IniWriteValue("stepForms", "HeaderColour", stepForms[0]);
            ini.IniWriteValue("stepForms", "HeaderFontWeight", stepForms[1]);
            ini.IniWriteValue("stepForms", "SubHeaderColour", stepForms[2]);
            ini.IniWriteValue("stepForms", "SubHeaderFontWeight", stepForms[3]);
            ini.IniWriteValue("stepForms", "StepTitleMainColour", stepForms[4]);
            ini.IniWriteValue("stepForms", "StepTitleMainFontWeight", stepForms[5]);
            ini.IniWriteValue("stepForms", "SubStepTitleMainColour", stepForms[6]);
            ini.IniWriteValue("stepForms", "SubStepTitleMainFontWeight", stepForms[7]);
            ini.IniWriteValue("stepForms", "StepInfoBoldColour", stepForms[8]);
            ini.IniWriteValue("stepForms", "StepInfoBoldFontWeight", stepForms[9]);
            ini.IniWriteValue("stepForms", "StepOutroBoldColour", stepForms[10]);
            ini.IniWriteValue("stepForms", "StepOutroFontWeight", stepForms[11]);
            #endregion

            #region progressBar
            ini.IniWriteValue("progressBar", "HeaderSelectedColour", progressBar[0]);
            ini.IniWriteValue("progressBar", "HeaderSelectedFontWeight", progressBar[1]);
            ini.IniWriteValue("progressBar", "HeaderUnSelectedColour", progressBar[2]);
            ini.IniWriteValue("progressBar", "HeaderUnSelectedFontWeight", progressBar[3]);
            ini.IniWriteValue("progressBar", "ProgressbarColour", progressBar[4]);
            ini.IniWriteValue("progressBar", "ProgressbarSubColour", progressBar[5]);
            #endregion

            #region headerBar
            ini.IniWriteValue("headerBar", "InactiveColour", headerBar[0]);
            ini.IniWriteValue("headerBar", "InactiveFontWeight", headerBar[1]);
            ini.IniWriteValue("headerBar", "OnHoverColour", headerBar[2]);
            ini.IniWriteValue("headerBar", "OnHoverFontWeight", headerBar[3]);
            ini.IniWriteValue("headerBar", "OnClickColour", headerBar[4]);
            ini.IniWriteValue("headerBar", "OnClickFontWeight", headerBar[5]);
            ini.IniWriteValue("headerBar", "SeperatorColour", headerBar[6]);
            ini.IniWriteValue("headerBar", "SeperatorFontWeight", headerBar[7]);
            #endregion

            #region dialogs
            ini.IniWriteValue("dialogs", "backgroundGradient_Top", dialogs[0]);
            ini.IniWriteValue("dialogs", "backgroundGradient_Bottom", dialogs[1]);
            ini.IniWriteValue("dialogs", "HeaderColour", dialogs[2]);
            ini.IniWriteValue("dialogs", "HeaderFontWeight", dialogs[3]);
            ini.IniWriteValue("dialogs", "SubHeaderColour", dialogs[4]);
            ini.IniWriteValue("dialogs", "SubHeaderFontWeight", dialogs[5]);
            ini.IniWriteValue("dialogs", "ContentColour", dialogs[6]);
            ini.IniWriteValue("dialogs", "ContentFontWeight", dialogs[7]);
            ini.IniWriteValue("dialogs", "ContentBoldColour", dialogs[8]);
            ini.IniWriteValue("dialogs", "ContentBoldFontWeight", dialogs[9]);
            #endregion

            #region mainWindow
            ini.IniWriteValue("mainWindow", "WindowBackground", mainWindow[0]);
            ini.IniWriteValue("mainWindow", "WindowDropShadow", mainWindow[1]);
            #endregion

            return tempPath;
        }

        public static string loadFromCurrentTheme()
        {
            string[] stepForms = new string[12];
            string[] progressBar = new string[6];
            string[] headerBar = new string[8];
            string[] dialogs = new string[10];
            string[] mainWindow = new string[2];

            string iniPath = IO.PathData.loadIniPath();
            if (iniPath != null)
            {
                iniFile ini = new iniFile(iniPath);

                #region stepForms
                stepForms[0] = Convert.ToString(ini.IniReadValue("stepForms", "HeaderColour"));
                stepForms[1] = Convert.ToString(ini.IniReadValue("stepForms", "HeaderFontWeight"));
                stepForms[2] = Convert.ToString(ini.IniReadValue("stepForms", "SubHeaderColour"));
                stepForms[3] = Convert.ToString(ini.IniReadValue("stepForms", "SubHeaderFontWeight"));
                stepForms[4] = Convert.ToString(ini.IniReadValue("stepForms", "StepTitleMainColour"));
                stepForms[5] = Convert.ToString(ini.IniReadValue("stepForms", "StepTitleMainFontWeight"));
                stepForms[6] = Convert.ToString(ini.IniReadValue("stepForms", "SubStepTitleMainColour"));
                stepForms[7] = Convert.ToString(ini.IniReadValue("stepForms", "SubStepTitleMainFontWeight"));
                stepForms[8] = Convert.ToString(ini.IniReadValue("stepForms", "StepInfoBoldColour"));
                stepForms[9] = Convert.ToString(ini.IniReadValue("stepForms", "StepInfoBoldFontWeight"));
                stepForms[10] = Convert.ToString(ini.IniReadValue("stepForms", "StepOutroBoldColour"));
                stepForms[11] = Convert.ToString(ini.IniReadValue("stepForms", "StepOutroFontWeight"));
                #endregion

                #region progressBar
                progressBar[0] = Convert.ToString(ini.IniReadValue("progressBar", "HeaderSelectedColour"));
                progressBar[1] = Convert.ToString(ini.IniReadValue("progressBar", "HeaderSelectedFontWeight"));
                progressBar[2] = Convert.ToString(ini.IniReadValue("progressBar", "HeaderUnSelectedColour"));
                progressBar[3] = Convert.ToString(ini.IniReadValue("progressBar", "HeaderUnSelectedFontWeight"));
                progressBar[4] = Convert.ToString(ini.IniReadValue("progressBar", "ProgressbarColour"));
                progressBar[5] = Convert.ToString(ini.IniReadValue("progressBar", "ProgressbarSubColour"));
                #endregion

                #region headerBar
                headerBar[0] = Convert.ToString(ini.IniReadValue("headerBar", "InactiveColour"));
                headerBar[1] = Convert.ToString(ini.IniReadValue("headerBar", "InactiveFontWeight"));
                headerBar[2] = Convert.ToString(ini.IniReadValue("headerBar", "OnHoverColour"));
                headerBar[3] = Convert.ToString(ini.IniReadValue("headerBar", "OnHoverFontWeight"));
                headerBar[4] = Convert.ToString(ini.IniReadValue("headerBar", "OnClickColour"));
                headerBar[5] = Convert.ToString(ini.IniReadValue("headerBar", "OnClickFontWeight"));
                headerBar[6] = Convert.ToString(ini.IniReadValue("headerBar", "SeperatorColour"));
                headerBar[7] = Convert.ToString(ini.IniReadValue("headerBar", "SeperatorFontWeight"));
                #endregion

                #region dialogs
                dialogs[0] = Convert.ToString(ini.IniReadValue("dialogs", "backgroundGradient_Top"));
                dialogs[1] = Convert.ToString(ini.IniReadValue("dialogs", "backgroundGradient_Bottom"));
                dialogs[2] = Convert.ToString(ini.IniReadValue("dialogs", "HeaderColour"));
                dialogs[3] = Convert.ToString(ini.IniReadValue("dialogs", "HeaderFontWeight"));
                dialogs[4] = Convert.ToString(ini.IniReadValue("dialogs", "SubHeaderColour"));
                dialogs[5] = Convert.ToString(ini.IniReadValue("dialogs", "SubHeaderFontWeight"));
                dialogs[6] = Convert.ToString(ini.IniReadValue("dialogs", "ContentColour"));
                dialogs[7] = Convert.ToString(ini.IniReadValue("dialogs", "ContentFontWeight"));
                dialogs[8] = Convert.ToString(ini.IniReadValue("dialogs", "ContentBoldColour"));
                dialogs[9] = Convert.ToString(ini.IniReadValue("dialogs", "ContentBoldFontWeight"));
                #endregion

                #region mainWindow
                mainWindow[0] = Convert.ToString(ini.IniReadValue("mainWindow", "WindowBackground"));
                mainWindow[1] = Convert.ToString(ini.IniReadValue("mainWindow", "WindowDropShadow"));
                #endregion

                if (stepForms.Contains(null) || progressBar.Contains(null) || 
                    headerBar.Contains(null) || dialogs.Contains(null) || mainWindow.Contains(null))
                {
                    return "MV";
                }
                else
                {
                    classInfo.SkinningEngine.RuntimeStorage.stepForms.HeaderColour = stepForms[0];
                    classInfo.SkinningEngine.RuntimeStorage.stepForms.HeaderFontWeight = stepForms[1];
                    classInfo.SkinningEngine.RuntimeStorage.stepForms.StepInfoBoldColour = stepForms[2];
                    classInfo.SkinningEngine.RuntimeStorage.stepForms.StepInfoBoldFontWeight = stepForms[3];
                    classInfo.SkinningEngine.RuntimeStorage.stepForms.StepOutroBoldColour = stepForms[4];
                    classInfo.SkinningEngine.RuntimeStorage.stepForms.StepOutroFontWeight = stepForms[5];
                    classInfo.SkinningEngine.RuntimeStorage.stepForms.StepTitleMainColour = stepForms[6];
                    classInfo.SkinningEngine.RuntimeStorage.stepForms.StepTitleMainFontWeight = stepForms[7];
                    classInfo.SkinningEngine.RuntimeStorage.stepForms.SubHeaderColour = stepForms[8];
                    classInfo.SkinningEngine.RuntimeStorage.stepForms.SubHeaderFontWeight = stepForms[9];
                    classInfo.SkinningEngine.RuntimeStorage.stepForms.SubStepTitleMainColour = stepForms[10];
                    classInfo.SkinningEngine.RuntimeStorage.stepForms.SubStepTitleMainFontWeight = stepForms[11];

                    classInfo.SkinningEngine.RuntimeStorage.progressBar.HeaderSelectedColour = progressBar[0];
                    classInfo.SkinningEngine.RuntimeStorage.progressBar.HeaderSelectedFontWeight = progressBar[1];
                    classInfo.SkinningEngine.RuntimeStorage.progressBar.HeaderUnSelectedColour = progressBar[2];
                    classInfo.SkinningEngine.RuntimeStorage.progressBar.HeaderUnSelectedFontWeight = progressBar[3];
                    classInfo.SkinningEngine.RuntimeStorage.progressBar.ProgressbarColour = progressBar[4];
                    classInfo.SkinningEngine.RuntimeStorage.progressBar.ProgressbarSubColour = progressBar[5];

                    classInfo.SkinningEngine.RuntimeStorage.headerBar.InactiveColour = headerBar[0];
                    classInfo.SkinningEngine.RuntimeStorage.headerBar.InactiveFontWeight = headerBar[1];
                    classInfo.SkinningEngine.RuntimeStorage.headerBar.OnClickColour = headerBar[2];
                    classInfo.SkinningEngine.RuntimeStorage.headerBar.OnClickFontWeight = headerBar[3];
                    classInfo.SkinningEngine.RuntimeStorage.headerBar.OnHoverColour = headerBar[4];
                    classInfo.SkinningEngine.RuntimeStorage.headerBar.OnHoverFontWeight = headerBar[5];
                    classInfo.SkinningEngine.RuntimeStorage.headerBar.SeperatorColour = headerBar[6];
                    classInfo.SkinningEngine.RuntimeStorage.headerBar.SeperatorFontWeight = headerBar[7];

                    classInfo.SkinningEngine.RuntimeStorage.dialogs.backgroundGradient_Bottom = dialogs[0];
                    classInfo.SkinningEngine.RuntimeStorage.dialogs.backgroundGradient_Top = dialogs[1];
                    classInfo.SkinningEngine.RuntimeStorage.dialogs.ContentBoldColour = dialogs[2];
                    classInfo.SkinningEngine.RuntimeStorage.dialogs.ContentBoldFontWeight = dialogs[3];
                    classInfo.SkinningEngine.RuntimeStorage.dialogs.ContentColour = dialogs[4];
                    classInfo.SkinningEngine.RuntimeStorage.dialogs.ContentFontWeight = dialogs[5];
                    classInfo.SkinningEngine.RuntimeStorage.dialogs.HeaderColour = dialogs[6];
                    classInfo.SkinningEngine.RuntimeStorage.dialogs.HeaderFontWeight = dialogs[7];
                    classInfo.SkinningEngine.RuntimeStorage.dialogs.SubHeaderColour = dialogs[8];
                    classInfo.SkinningEngine.RuntimeStorage.dialogs.SubHeaderFontWeight = dialogs[9];

                    classInfo.SkinningEngine.RuntimeStorage.mainWindow.WindowBackground = mainWindow[0];
                    classInfo.SkinningEngine.RuntimeStorage.mainWindow.WindowDropShadow = mainWindow[1];
                    return null;
                }
            }
            else
            {
                return "NV";
            }

        }

        public static void restoreTheme()
        {
            #region dialog
            classInfo.SkinningEngine.RuntimeStorage.dialogs.backgroundGradient_Bottom =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.dialogs.backgroundGradient_Bottom;

            classInfo.SkinningEngine.RuntimeStorage.dialogs.backgroundGradient_Top =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.dialogs.backgroundGradient_Top;

            classInfo.SkinningEngine.RuntimeStorage.dialogs.ContentBoldColour =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.dialogs.ContentBoldColour;

            classInfo.SkinningEngine.RuntimeStorage.dialogs.ContentBoldFontWeight =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.dialogs.ContentBoldFontWeight;

            classInfo.SkinningEngine.RuntimeStorage.dialogs.ContentColour =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.dialogs.ContentColour;

            classInfo.SkinningEngine.RuntimeStorage.dialogs.ContentFontWeight =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.dialogs.ContentFontWeight;

            classInfo.SkinningEngine.RuntimeStorage.dialogs.HeaderColour =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.dialogs.HeaderColour;

            classInfo.SkinningEngine.RuntimeStorage.dialogs.HeaderFontWeight =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.dialogs.HeaderFontWeight;

            classInfo.SkinningEngine.RuntimeStorage.dialogs.SubHeaderColour =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.dialogs.SubHeaderColour;

            classInfo.SkinningEngine.RuntimeStorage.dialogs.SubHeaderFontWeight =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.dialogs.SubHeaderFontWeight;
            #endregion

            #region headerBar
            classInfo.SkinningEngine.RuntimeStorage.headerBar.InactiveColour =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.headerBar.InactiveColour;

            classInfo.SkinningEngine.RuntimeStorage.headerBar.InactiveFontWeight =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.headerBar.InactiveFontWeight;

            classInfo.SkinningEngine.RuntimeStorage.headerBar.OnClickColour =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.headerBar.OnClickColour;

            classInfo.SkinningEngine.RuntimeStorage.headerBar.OnClickFontWeight =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.headerBar.OnClickFontWeight;

            classInfo.SkinningEngine.RuntimeStorage.headerBar.OnHoverColour =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.headerBar.OnHoverColour;

            classInfo.SkinningEngine.RuntimeStorage.headerBar.OnHoverFontWeight =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.headerBar.OnHoverFontWeight;

            classInfo.SkinningEngine.RuntimeStorage.headerBar.SeperatorColour =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.headerBar.SeperatorColour;

            classInfo.SkinningEngine.RuntimeStorage.headerBar.SeperatorFontWeight =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.headerBar.SeperatorFontWeight;
            #endregion

            #region mainWindow
            classInfo.SkinningEngine.RuntimeStorage.mainWindow.WindowBackground =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.mainWindow.WindowBackground;

            classInfo.SkinningEngine.RuntimeStorage.mainWindow.WindowDropShadow =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.mainWindow.WindowDropShadow;
            #endregion

            #region stepForms
            classInfo.SkinningEngine.RuntimeStorage.stepForms.HeaderColour =
               classInfo.SkinningEngine.RuntimeStorage.defaultSettings.stepForms.HeaderColour;

            classInfo.SkinningEngine.RuntimeStorage.stepForms.HeaderFontWeight =
               classInfo.SkinningEngine.RuntimeStorage.defaultSettings.stepForms.HeaderFontWeight;

            classInfo.SkinningEngine.RuntimeStorage.stepForms.StepInfoBoldColour =
               classInfo.SkinningEngine.RuntimeStorage.defaultSettings.stepForms.StepInfoBoldColour;

            classInfo.SkinningEngine.RuntimeStorage.stepForms.StepInfoBoldFontWeight =
               classInfo.SkinningEngine.RuntimeStorage.defaultSettings.stepForms.StepInfoBoldFontWeight;

            classInfo.SkinningEngine.RuntimeStorage.stepForms.StepOutroBoldColour =
               classInfo.SkinningEngine.RuntimeStorage.defaultSettings.stepForms.StepOutroBoldColour;

            classInfo.SkinningEngine.RuntimeStorage.stepForms.StepOutroFontWeight =
               classInfo.SkinningEngine.RuntimeStorage.defaultSettings.stepForms.StepOutroFontWeight;

            classInfo.SkinningEngine.RuntimeStorage.stepForms.StepTitleMainColour =
               classInfo.SkinningEngine.RuntimeStorage.defaultSettings.stepForms.StepTitleMainColour;

            classInfo.SkinningEngine.RuntimeStorage.stepForms.StepTitleMainFontWeight =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.stepForms.StepTitleMainFontWeight;

            classInfo.SkinningEngine.RuntimeStorage.stepForms.SubHeaderColour =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.stepForms.SubHeaderColour;

            classInfo.SkinningEngine.RuntimeStorage.stepForms.SubHeaderFontWeight =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.stepForms.SubHeaderFontWeight;

            classInfo.SkinningEngine.RuntimeStorage.stepForms.SubStepTitleMainColour =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.stepForms.SubStepTitleMainColour;

            classInfo.SkinningEngine.RuntimeStorage.stepForms.SubStepTitleMainFontWeight =
                classInfo.SkinningEngine.RuntimeStorage.defaultSettings.stepForms.SubStepTitleMainFontWeight;
            #endregion

            #region progressBar
            classInfo.SkinningEngine.RuntimeStorage.progressBar.HeaderSelectedColour =
               classInfo.SkinningEngine.RuntimeStorage.defaultSettings.progressBar.HeaderSelectedColour;

            classInfo.SkinningEngine.RuntimeStorage.progressBar.HeaderSelectedFontWeight =
               classInfo.SkinningEngine.RuntimeStorage.defaultSettings.progressBar.HeaderSelectedFontWeight;

            classInfo.SkinningEngine.RuntimeStorage.progressBar.HeaderUnSelectedColour =
               classInfo.SkinningEngine.RuntimeStorage.defaultSettings.progressBar.HeaderUnSelectedColour;

            classInfo.SkinningEngine.RuntimeStorage.progressBar.HeaderUnSelectedFontWeight =
               classInfo.SkinningEngine.RuntimeStorage.defaultSettings.progressBar.HeaderUnSelectedFontWeight;

            classInfo.SkinningEngine.RuntimeStorage.progressBar.ProgressbarColour =
               classInfo.SkinningEngine.RuntimeStorage.defaultSettings.progressBar.ProgressbarColour;

            classInfo.SkinningEngine.RuntimeStorage.progressBar.ProgressbarSubColour =
               classInfo.SkinningEngine.RuntimeStorage.defaultSettings.progressBar.ProgressbarSubColour;
            #endregion
        }

        public static bool validateTheme(string filename)
        {
            string[] stepForms = new string[12];
            string[] progressBar = new string[6];
            string[] headerBar = new string[8];
            string[] dialogs = new string[10];
            string[] mainWindow = new string[2];

            iniFile ini = new iniFile(filename);

            #region stepForms
            stepForms[0] = Convert.ToString(ini.IniReadValue("stepForms", "HeaderColour"));
            stepForms[1] = Convert.ToString(ini.IniReadValue("stepForms", "HeaderFontWeight"));
            stepForms[2] = Convert.ToString(ini.IniReadValue("stepForms", "SubHeaderColour"));
            stepForms[3] = Convert.ToString(ini.IniReadValue("stepForms", "SubHeaderFontWeight"));
            stepForms[4] = Convert.ToString(ini.IniReadValue("stepForms", "StepTitleMainColour"));
            stepForms[5] = Convert.ToString(ini.IniReadValue("stepForms", "StepTitleMainFontWeight"));
            stepForms[6] = Convert.ToString(ini.IniReadValue("stepForms", "SubStepTitleMainColour"));
            stepForms[7] = Convert.ToString(ini.IniReadValue("stepForms", "SubStepTitleMainFontWeight"));
            stepForms[8] = Convert.ToString(ini.IniReadValue("stepForms", "StepInfoBoldColour"));
            stepForms[9] = Convert.ToString(ini.IniReadValue("stepForms", "StepInfoBoldFontWeight"));
            stepForms[10] = Convert.ToString(ini.IniReadValue("stepForms", "StepOutroBoldColour"));
            stepForms[11] = Convert.ToString(ini.IniReadValue("stepForms", "StepOutroFontWeight"));
            #endregion

            #region progressBar
            progressBar[0] = Convert.ToString(ini.IniReadValue("progressBar", "HeaderSelectedColour"));
            progressBar[1] = Convert.ToString(ini.IniReadValue("progressBar", "HeaderSelectedFontWeight"));
            progressBar[2] = Convert.ToString(ini.IniReadValue("progressBar", "HeaderUnSelectedColour"));
            progressBar[3] = Convert.ToString(ini.IniReadValue("progressBar", "HeaderUnSelectedFontWeight"));
            progressBar[4] = Convert.ToString(ini.IniReadValue("progressBar", "ProgressbarColour"));
            progressBar[5] = Convert.ToString(ini.IniReadValue("progressBar", "ProgressbarSubColour"));
            #endregion

            #region headerBar
            headerBar[0] = Convert.ToString(ini.IniReadValue("headerBar", "InactiveColour"));
            headerBar[1] = Convert.ToString(ini.IniReadValue("headerBar", "InactiveFontWeight"));
            headerBar[2] = Convert.ToString(ini.IniReadValue("headerBar", "OnHoverColour"));
            headerBar[3] = Convert.ToString(ini.IniReadValue("headerBar", "OnHoverFontWeight"));
            headerBar[4] = Convert.ToString(ini.IniReadValue("headerBar", "OnClickColour"));
            headerBar[5] = Convert.ToString(ini.IniReadValue("headerBar", "OnClickFontWeight"));
            headerBar[6] = Convert.ToString(ini.IniReadValue("headerBar", "SeperatorColour"));
            headerBar[7] = Convert.ToString(ini.IniReadValue("headerBar", "SeperatorFontWeight"));
            #endregion

            #region dialogs
            dialogs[0] = Convert.ToString(ini.IniReadValue("dialogs", "backgroundGradient_Top"));
            dialogs[1] = Convert.ToString(ini.IniReadValue("dialogs", "backgroundGradient_Bottom"));
            dialogs[2] = Convert.ToString(ini.IniReadValue("dialogs", "HeaderColour"));
            dialogs[3] = Convert.ToString(ini.IniReadValue("dialogs", "HeaderFontWeight"));
            dialogs[4] = Convert.ToString(ini.IniReadValue("dialogs", "SubHeaderColour"));
            dialogs[5] = Convert.ToString(ini.IniReadValue("dialogs", "SubHeaderFontWeight"));
            dialogs[6] = Convert.ToString(ini.IniReadValue("dialogs", "ContentColour"));
            dialogs[7] = Convert.ToString(ini.IniReadValue("dialogs", "ContentFontWeight"));
            dialogs[8] = Convert.ToString(ini.IniReadValue("dialogs", "ContentBoldColour"));
            dialogs[9] = Convert.ToString(ini.IniReadValue("dialogs", "ContentBoldFontWeight"));
            #endregion

            #region mainWindow
            mainWindow[0] = Convert.ToString(ini.IniReadValue("mainWindow", "WindowBackground"));
            mainWindow[1] = Convert.ToString(ini.IniReadValue("mainWindow", "WindowDropShadow"));
            #endregion

            if (stepForms.Contains(null) || progressBar.Contains(null) ||
                headerBar.Contains(null) || dialogs.Contains(null) || mainWindow.Contains(null))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string[] loadAllInstalledThemes()
        {
            string themes = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Liberty\\themes\\";
            string themesInDirec = "";
            DirectoryInfo di = new DirectoryInfo(themes);
            FileInfo[] rgFiles = di.GetFiles("*.ini");
            foreach (FileInfo fi in rgFiles)
            {
                themesInDirec += fi.FullName + "\n";
            }

            string[] themesInDirecArray = themesInDirec.Split('\n');

            return themesInDirecArray;
        }
    }
}
