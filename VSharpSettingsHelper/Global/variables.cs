//This is an Intelectual Property of Zcodia Technologies and Raghulan Gowthaman.
//www.zcodiatechnologies.com.au
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSharpSettingsHelper.Global
{
    class variables
    {
        #region GeneralVars
        public static string appName = "AppName";
        public static bool appName_HardCoded = true; //if set to true it wouldnt get the app name from setting.xml
        public static string tempPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        public static string appPath = tempPath.Substring(6, tempPath.Length - 6);
        public static string settingsFile = appPath + @"\Data\VSSH_Lib_Settings.xml";
        public static string logFile = @"C:\A2KApps\Transmittal\Data\log.txt";
        public static string fileStore;
        public static string dlpath;
        public static bool logSwitch = true;
        public static string delimiter = "~";
        #endregion
    }
}
