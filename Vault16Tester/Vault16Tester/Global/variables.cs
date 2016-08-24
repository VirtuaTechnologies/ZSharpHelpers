using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Autodesk.Connectivity.WebServices;
using VDF = Autodesk.DataManagement.Client.Framework;
using System.Drawing;
using System.Windows.Media;
using ZSharpGeneralHelper;
using Autodesk.Connectivity.WebServicesTools;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using System.Collections.ObjectModel;
using ZSharpVault16lib;

namespace Vault16Tester.Global
{
    class variables
    {
        public static VDF.Vault.Currency.Connections.Connection connection;
        public static CustEnt[] custEntities;
        public static DataTable dataTable;
        public static string server = "A2K_NSW_L012";
        public static string vaultStore = "VaultDS";
        public static string username = "Administrator";
        public static string pass = "";
        public static List<string> objType = new List<string>();
        public static Dictionary<string, NumSchemes> NSLIst = new Dictionary<string, NumSchemes>();
        public static WebServiceManager wServ;
        public static FileIteration selectedFile = null;
        public static Dictionary<string, string> vaultfilePropsString = new Dictionary<string, string>();
        public static Dictionary<PropDef, object> vaultfileProps = new Dictionary<PropDef, object>();
        public static Dictionary<PropDef, object> vaultfilePropsUPd = new Dictionary<PropDef, object>();

        #region Create Local Fodlers
        public static Autodesk.Connectivity.WebServices.Folder selectedFodler;
        public static VDF.Vault.Currency.Entities.Folder selectedFodlerEntity;
        public static IEnumerable<VDF.Vault.Currency.Entities.Folder> subFolders;
        public static System.Windows.Media.Brush _brush_set_folder_exists = System.Windows.Media.Brushes.Aqua;
        public static System.Windows.Media.Brush _brush_set_folder_dontexists = System.Windows.Media.Brushes.Yellow;
        public static ObservableCollection<FolderStructureModel> folderList = new ObservableCollection<FolderStructureModel>();
        #endregion
    }

    class CategoryData
    {
        public CategoryData(string catName, string desc, string sysName, int colorCode, long catID)
        {
            CATName = catName;
            Desc = desc;
            ZSharpGeneralHelper.ColorConverter con = new ZSharpGeneralHelper.ColorConverter();
            ColorCode = new SolidColorBrush(con.ToMediaColor(System.Drawing.Color.FromArgb(colorCode)));
            CatID = catID;
            SysName = sysName;
        }

        public string CATName { get; set; }
        public string Desc { get; set; }
        public string SysName { get; set; }
        public SolidColorBrush ColorCode { get; set; }
        public long CatID { get; set; }
        
    }


    class NumSchemes
    {
        public NumSchemes(NumSchmField[] fieldArray, bool isActive, bool isDefault, bool isInUse, bool isSys, string name, string sysName, long schemeID)
        {
            FieldArray = fieldArray;
            isActive = ISActive;
            isDefault = ISDefault;
            isInUse = ISInUse;
            isSys = ISSys;
            Name = name;
            SysName = sysName;
            SchemeID = schemeID;
        }

        public NumSchmField[] FieldArray { get; set; }
        public bool ISActive { get; set; }
        public bool ISDefault { get; set; }
        public bool ISInUse { get; set; }
        public bool ISSys { get; set; }
        public string SysName { get; set; }
        public string Name { get; set; }
        public long SchemeID { get; set; }

    }

    public class folderInfo
    {
        public string EntityName { get; set; }
        public string FolderPath { get; set; }
        public string FullName { get; set; }
        public int NumberOfChildren { get; set; }
        public string Category { get; set; }
        public long catID { get; set; }
        public string CreateDate { get; set; }
        public long CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public string EntityClass { get; set; }
        public long EntityIterationId { get; set; }
        public long EntityMasterId { get; set; }
        public long Id { get; set; }
        public string FullUncName { get; set; }
        public bool IsLibraryFolder { get; set; }
        public bool IsVaultRoot { get; set; }
        public bool Locked { get; set; }
        public long ParentId { get; set; }

    }

}
