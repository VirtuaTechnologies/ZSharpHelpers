using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSharpQLogger
{
    public class LogIT
    {
        private static string logFile;
        private static bool logSwitch;

        public void CreateLogFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath); //@"C:\CSIApp\Log"
        }

        public static void write(logSetting logsettings, string data)
        {
            try
            {
                logFile = logsettings.logFile;
                logSwitch = logsettings.logSwitch;

                if (logSwitch)
                {
                    if (new FileInfo(logFile).Length == 0)
                    {
                        log_header(logsettings);
                    }
                 
                    if (!File.Exists(logFile))
                    {
                        var myFile = File.Create(logFile);
                        myFile.Close();
                    }
                    using (StreamWriter sw = File.AppendText(logFile))
                    {
                        sw.WriteLine("\n::" + DateTime.Now + " :: " + data);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        public static void log_header(logSetting logsettings)
        {
            logFile = logsettings.logFile;
            logSwitch = logsettings.logSwitch;
            using (System.IO.StreamWriter StreamWriter1 = new System.IO.StreamWriter(logFile, true))
            {
                StreamWriter1.WriteLine("\n:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                StreamWriter1.WriteLine("\n:" + logsettings.LogTitle);
                StreamWriter1.WriteLine("\n:Developer::Raghulan Gowthaman:::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                //StreamWriter1.WriteLine("\n:::::::::::::::::" + log_name + " :::::::::::::::::::::::::::::::::::::");
                StreamWriter1.WriteLine("Date and Time : " + DateTime.Now);
                //StreamWriter1.WriteLine("Drawing : " + Helper.CAD_Helper.GET_current_dwg());
                StreamWriter1.WriteLine("\n:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                //StreamWriter1.WriteLine("User Name : " + Global.variables.first_name + " " + Global.variables.last_name);
                //StreamWriter1.WriteLine("Designation : " + Global.variables.User_designation);
                //StreamWriter1.WriteLine("Email : " + Global.variables.User_email);
                //StreamWriter1.WriteLine("Phone : " + Global.variables.User_phone);
                StreamWriter1.WriteLine("\n:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
            }
        }

    }

    public class logSetting
    {
        public string LogTitle { get; set; }
        public string DevDetail { get; set; }
        public string LogDetails { get; set; }
        public string logFile { get; set; }
        public bool logSwitch { get; set; }

        public logSetting(string _LogTitle, string _DevDetail, string _LogDetails, string _logFile, bool _logSwitch)
        {
            LogTitle = _LogTitle;
            DevDetail = _DevDetail;
            LogDetails = _LogDetails;
            logFile = _logFile;
            logSwitch = _logSwitch;
        }

        public string RetLogTitle()
        {
            return LogTitle;
        }


    }
}
