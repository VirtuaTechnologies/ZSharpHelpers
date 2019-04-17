using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GV = XMLParserApp.Global.variables;
using ZSharpXMLHelper;
using ZSharpQLogger;
using System.Reflection;

namespace XMLParserApp.Helper
{
    class GenHelper
    {
        public static void initializeREST()
        {
            getPath();
            getSettings();
            setLogFiles();
        }

        public static void getSettings()
        {
            try
            {
                if (File.Exists(Global.variables.settingsFile))
                {
                    GV.logSwitch = bool.Parse(xmlParser.getXMLValue(Global.variables.settingsFile, "Settings", "name", "logSwitch"));
                    GV.logFile = xmlParser.getXMLValue(Global.variables.settingsFile, "Settings", "name", "logFile");
                    GV.errorBoxSwitch = bool.Parse(xmlParser.getXMLValue(Global.variables.settingsFile, "Settings", "name", "errorBoxSwitch"));
                    GV.debug = bool.Parse(xmlParser.getXMLValue(Global.variables.settingsFile, "Settings", "name", "debug"));
                    GV.fileStore = xmlParser.getXMLValue(Global.variables.settingsFile, "Settings", "name", "FileStore");
                }

            }
            catch (SystemException ex)
            {
                //MessageBox.Show(ex.ToString());
            }

        }

        public static bool checkNullString(string inputString)
        {

            bool result;
            if (inputString == null || inputString == string.Empty || string.IsNullOrEmpty(inputString) || inputString.Length == 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            //logit.logger("-----------Null Check: " + inputString + " | " + inputString.Length.ToString() + " | " + result);
            return result;
        }

        public static void createDir(string fodler)
        {
            try
            {
                if (!Directory.Exists(fodler))
                    Directory.CreateDirectory(fodler);
            }
            catch (SystemException ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public static void setLogFiles()
        {
            if (File.Exists(GV.logFile))
            {
                File.Delete(GV.logFile);
                writeLog("Deleting Log");
            }
            File.Create(GV.logFile).Close();
            writeLog("Creating Log");
        }

        public static logSetting logSet;
        public static void writeLog(string mess)
        {
            if (GV.logSwitch)
            {
                logSet = new logSetting(GV.appName, GV.devDetails, "", GV.logFile, true);
                LogIT.write(logSet, mess);
            }
        }

        public static void getPath()
        {
            GV.appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            GV.dataPath = GV.appPath + "\\Data\\";
            GV.settingsFile = GV.appPath + @"\Data\Settings.xml";
            //GV.logFile = GV.appPath + @"\Data\CMWCF.log";
        }

        public static string DecodeFrom64(string encodedData)
        {
            string returnValue = null;
            try
            {
                byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
                returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            }
            catch (SystemException e)
            {
            }
            return returnValue;
        }

        public static string Split_csv_get_specific(string csv_value, int part)
        {
            string result;
            try
            {
                result = csv_value.Split(',')[part];

            }
            catch (IndexOutOfRangeException ex)
            {
                result = "no";
            }
            return result;

        }

    }
}
