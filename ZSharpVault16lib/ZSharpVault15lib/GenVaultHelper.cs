using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Connectivity.WebServicesTools;
using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.Explorer.Extensibility;
using VDF = Autodesk.DataManagement.Client.Framework;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using System.Windows;
using System.Reflection;
using System.IO;
using ZSharpQLogger;
using GV = ZSharpVault16lib.Global.variables;
using ZSharpXMLHelper;

namespace ZSharpVault16lib
{
    public class GenVaultHelper
    {
        public static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static String RGBConverter(System.Drawing.Color c)
        {
            return "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
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

        public static void getSettings()
        {
            if (System.IO.File.Exists(Global.variables.settingsFile))
            {
                GV.logSwitch = bool.Parse(xmlParser.getXMLValue(Global.variables.settingsFile, "Settings", "name", "logSwitch"));
                GV.logFile = xmlParser.getXMLValue(Global.variables.settingsFile, "Settings", "name", "logFile");
                GV.errorBoxSwitch = bool.Parse(xmlParser.getXMLValue(Global.variables.settingsFile, "Settings", "name", "errorBoxSwitch"));
                GV.debug = bool.Parse(xmlParser.getXMLValue(Global.variables.settingsFile, "Settings", "name", "debug"));
            }
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

        public static void getPath()
        {
            GV.appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            GV.dataPath = GV.appPath + "\\Data\\";
            GV.settingsFile = GV.appPath + @"\Data\VCRestSettings.xml";
            //GV.logFile = GV.appPath + @"\Data\CMWCF.log";
        }

        public static void LibNotes()
        {
            try
            {
                string libFile = AssemblyDirectory + @"\" + Assembly.GetExecutingAssembly().GetName().Name + ".txt";
                //System.Windows.Forms.MessageBox.Show(libFile);
                string message = "This Library was developed as an accessory for Autodesk Vault API Developers. Please keep this document for copyright purposes.";
                if (System.IO.File.Exists(libFile))
                    System.IO.File.Delete(libFile);
                System.IO.File.Create(libFile).Close();

                using (System.IO.StreamWriter StreamWriter1 = new System.IO.StreamWriter(libFile, true))
                {
                    StreamWriter1.WriteLine("\n:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    StreamWriter1.WriteLine("\n" + Assembly.GetExecutingAssembly().FullName);
                    StreamWriter1.WriteLine("\n:Developer::Raghulan Gowthaman:::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    StreamWriter1.WriteLine("\n:www.raghulangowthaman.com:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    StreamWriter1.WriteLine("\n:Zcodia Technologies:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    StreamWriter1.WriteLine("\n:Zcodia Technologies:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    StreamWriter1.WriteLine("\n:www.zcodia.com::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    StreamWriter1.WriteLine("\n:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    StreamWriter1.WriteLine("\n" + message);
                    StreamWriter1.WriteLine("\n:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                }
            }
            catch (System.Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private static object[] nSchemes;
        public static object[] getNumberSchemes(VDF.Vault.Currency.Connections.Connection connection, string objType, NumSchmType numSchemeType)
        {
            try
            {
                LibNotes();
                switch (objType)
                {
                    case "File":
                    {
                        DocumentService fserv = connection.WebServiceManager.DocumentService;
                        nSchemes = fserv.GetAllFileNamingSchemes();
                        return nSchemes;
                    }
                    case "Change Order":
                    {
                        ChangeOrderService cOrder = connection.WebServiceManager.ChangeOrderService;
                        nSchemes = cOrder.GetNumberingSchemesByType(numSchemeType);
                        return nSchemes;
                    }
                    case "Item":
                    {
                        ItemService iServ = connection.WebServiceManager.ItemService;
                        nSchemes = iServ.GetNumberingSchemesByType(numSchemeType);
                        return nSchemes;
                    }
                    case "All":
                    {
                        DocumentService fserv = connection.WebServiceManager.DocumentService;
                        nSchemes = fserv.GetAllFileNamingSchemes();
                        ChangeOrderService cOrder = connection.WebServiceManager.ChangeOrderService;
                        nSchemes = cOrder.GetNumberingSchemesByType(numSchemeType);
                        ItemService iServ = connection.WebServiceManager.ItemService;
                        nSchemes = iServ.GetNumberingSchemesByType(numSchemeType);
                        break;
                    }
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return nSchemes;
        }

        
    }
}
