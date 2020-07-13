using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GV = ZSharpTextHelper.Global.variable;
using ZSharpQLogger;
using System.Windows.Forms;
using ZSharpXMLHelper;
using VSharpXMLHelper;

namespace ZSharpTextHelper
{
    public class GenTextHelper
    {
        public static void getSettings()
        {
            if (System.IO.File.Exists(GV.settingsFile))
            {
                GV.logSwitch = bool.Parse(xmlParser.getXMLValue(GV.settingsFile, "Settings", "name", "logSwitch"));
                GV.logFile = xmlParser.getXMLValue(GV.settingsFile, "Settings", "name", "logFile");
                GV.errorBoxSwitch = bool.Parse(xmlParser.getXMLValue(GV.settingsFile, "Settings", "name", "errorBoxSwitch"));
                GV.debug = bool.Parse(xmlParser.getXMLValue(GV.settingsFile, "Settings", "name", "debug"));
            }
        }

        public static string Split_csv_get_specific1(string csv_value, int part)
        {
            part -= 1;
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

        public static string Split_csv_get_specific_dl(string csv_value, int part, char delimiter)
        {
            part -= 1;
            string result;
            try
            {
                result = csv_value.Split(delimiter)[part];

            }
            catch (IndexOutOfRangeException ex)
            {
                result = "no";
            }

            return result;
        }

        public static string Split_string_before_After(string stringVal, int part, string delimiter)
        {
            string result;
            try
            {
                if(part ==1) //before
                    result = stringVal.Substring(0, stringVal.IndexOf(delimiter));
                else //after
                    result = stringVal.Split(new string[] { delimiter }, StringSplitOptions.None).Last();
            }
            catch (IndexOutOfRangeException ex)
            {
                result = "no";
            }

            return result;
        }

        public static string ReplaceString(string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        public static void setLogFiles()
        {
            if (System.IO.File.Exists(GV.logFile))
            {
                System.IO.File.Delete(GV.logFile);
                writeLog("Deleting Log");
            }
            System.IO.File.Create(GV.logFile).Close();
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

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private static string result ;
        public static Int64 ExtractNumberfromString(string originalString)
        {
            try
            {
                result = new string(originalString.Where(c => Char.IsDigit(c)).ToArray());
                Debug.Write("\nExtractNumberfromString: " + result);
                
            }
            catch(SystemException ex)
            {
                Debug.Write("\nZSharpTextHelper - ExtractNumberfromString : " + ex.ToString());
            }
            return Convert.ToInt64(result);
        }

        public static List<string> convertCSVtoList(string csvString, char delimiter)
        {
            List<string> result = csvString.Split(delimiter).ToList();
            return result;
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

        public static string convertListtoCSV(List<string> listcoll)
        {
            string res = string.Join(",", listcoll.Select(x => x.ToString()).ToArray());
            return res;
        }

        static public string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static void getPath()
        {
            GV.appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            GV.dataPath = GV.appPath + "\\Data\\";
            GV.settingsFile = GV.appPath + @"\Data\VCRestSettings.xml";
            //GV.logFile = GV.appPath + @"\Data\CMWCF.log";
        }

    }
}
