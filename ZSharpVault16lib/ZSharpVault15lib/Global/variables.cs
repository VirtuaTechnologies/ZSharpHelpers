using Autodesk.Connectivity.WebServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDF = Autodesk.DataManagement.Client.Framework;
using System.Windows;


namespace ZSharpVault16lib.Global
{
    class variables
    {
        public static VDF.Vault.Currency.Connections.Connection m_connection;

        public static string appName = "ACAD Content Manager";
        public static string devDetails = "Raghulan Gowthaman";

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

        #region Create Local Fodlers
        public static Folder selectedFodler;
        public static VDF.Vault.Currency.Entities.Folder selectedFodlerEntity;
        public static IEnumerable<VDF.Vault.Currency.Entities.Folder> subFolders;
        public static System.Windows.Media.Brush _brush_set_folder_exists = System.Windows.Media.Brushes.Aqua;
        public static System.Windows.Media.Brush _brush_set_folder_dontexists = System.Windows.Media.Brushes.Yellow;
        public static ObservableCollection<FolderStructureModel> folderList = new ObservableCollection<FolderStructureModel>();
        public static string CLF_App_Title = "Vault ADV Tools";
        #endregion
    }

    

}
