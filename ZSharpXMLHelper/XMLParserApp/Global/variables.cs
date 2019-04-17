using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLParserApp.Global
{
    class variables
    {
        #region GeneralVars
        public static string appName = "AppName";
        public static bool appName_HardCoded = true; //if set to true it wouldnt get the app name from setting.xml
        public static string devDetails = "Raghulan Gowthaman";
        public static string fileStore;
        public static string dlpath;
        public static string delimiter = "~";
        #endregion

        #region Switch
        public static bool logSwitch;
        public static bool debug = true;
        public static bool errorBoxSwitch = false;
        #endregion

        #region - Path
        public static string dataPath;
        public static string appPath;
        public static string settingsFile;
        public static string logFile;
        #endregion

    }
}
