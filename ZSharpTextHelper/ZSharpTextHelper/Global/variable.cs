using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSharpTextHelper.Global
{
    class variable
    {
        public static string appName = "ZSharpTextHelper";
        public static string devDetails = "Raghulan Gowthaman";
        public static string tempPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

        #region - Path
        public static string dataPath;
        public static string appPath;
        public static string settingsFile;
        public static string logFile;
        #endregion

        #region Switch
        public static bool logSwitch;
        public static bool debug = true;
        public static bool errorBoxSwitch = false;
        #endregion
    }
}
