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
        public static string tempPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        public static string appPath = tempPath.Substring(6, tempPath.Length - 6);
        public static string logFile = appPath + @"\Data\log2.txt";
        public static bool logSwitch = true;
    }
}
