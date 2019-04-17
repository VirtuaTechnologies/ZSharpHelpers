//This is an Intelectual Property of Virtua Technologies and Raghulan Gowthaman.
//www.virtuatechnoliges.com.au
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GV = VSharpSettingsHelper.Global.variables;
using logit = ZSharpLogger.Log;

namespace VSharpSettingsHelper.Helper
{
    class GenHelper
    {
        public static void getSettings()
        {
            try
            {
                //carete log dir
                createDir(Path.GetDirectoryName(GV.logFile));

                //if settings file exists then get the settings
                if (File.Exists(Global.variables.settingsFile))
                {
                    GV.fileStore = xmlParser.getXMLValue(Global.variables.settingsFile, "Settings", "name", "FileStore");
                    GV.logFile = xmlParser.getXMLValue(Global.variables.settingsFile, "Settings", "name", "logFile");
                    GV.logSwitch = bool.Parse(xmlParser.getXMLValue(Global.variables.settingsFile, "Settings", "name", "logSwitch"));
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
            logit.logger("-----------Null Check: " + inputString + " | " + inputString.Length.ToString() + " | " + result);
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

        public static void setLogger()
        {
            try
            {
                //delete existing log file.
                if (File.Exists(GV.logFile))
                    File.Delete(GV.logFile);
                //setup log settings
                ZSharpLogger.logSetting logSettings = new ZSharpLogger.logSetting(GV.appName, null, null, GV.logFile, GV.logSwitch);

                logit.log_header(logSettings);
            }
            catch (SystemException ex)
            {
                //MessageBox.Show(ex.ToString());
            }

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
