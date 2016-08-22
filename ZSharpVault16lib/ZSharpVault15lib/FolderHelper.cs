using Autodesk;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDF = Autodesk.DataManagement.Client.Framework;
using VaultAPI.Data;
using System.Drawing;
using ZGHCC = ZSharpVault16lib.GenVaultHelper;
using ZVFH = ZSharpVault16lib.FileHelper;
using ZSharpVault16lib.Global;
using System.Diagnostics;

namespace ZSharpVault16lib
{
    public class FolderHelper
    {
        public static Autodesk.Connectivity.WebServices.Folder gefolderbyfolderPath(VDF.Vault.Currency.Connections.Connection connection, string folderPath)
        {
            Autodesk.Connectivity.WebServices.Folder selectedFolder = null;
            try
            {
                selectedFolder = connection.WebServiceManager.DocumentService.FindFoldersByPaths(folderPath.ToSingleArray()).First();
            }
            catch (SystemException ex)
            {

                Debug.Write(ex.ToString());
                //MessageBox.Show(ex.ToString());
            }
            return selectedFolder;
        }

        public static Dictionary<string, VaultAPI.Data.folderInfo> getAllVaultFolders(VDF.Vault.Currency.Connections.Connection connection)
        {
            Dictionary<string, VaultAPI.Data.folderInfo> fodlerDict = new Dictionary<string, VaultAPI.Data.folderInfo>();
            try
            {
                //get folder list from vault
                foreach (var folder in connection.FolderManager.GetChildFolders(connection.FolderManager.RootFolder, false, false))
                {
                    VaultAPI.Data.folderInfo rootFI = new VaultAPI.Data.folderInfo();
                    rootFI = getFolderInfo(connection, folder);

                    if (folder.NumberOfChildren > 0)
                    {
                        List<folderInfo> childFoldersList = new List<folderInfo>();
                        foreach (var sfolder1 in connection.FolderManager.GetChildFolders(folder, false, false))
                        {
                            folderInfo subFI = new folderInfo();
                            subFI = (VaultAPI.Data.folderInfo)getFolderInfo(connection, sfolder1);
                            childFoldersList.Add(subFI);
                            
                        }
                        rootFI.childFolders = childFoldersList;
                    }
                    fodlerDict.Add(folder.EntityName, rootFI);
                }
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return fodlerDict;
        }

        private static Folder fld;
        public static Folder getFolderusingID(VDF.Vault.Currency.Connections.Connection connection, string folderID)
        {
            
            try
            {
                long masterID = Convert.ToInt64(folderID);
                IDictionary<long, VDF.Vault.Currency.Entities.Folder> folderIdsToFolderEntities = connection.FolderManager.GetFoldersByIds(new long[] { masterID });
                fld = folderIdsToFolderEntities[masterID];
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return fld;
        }

        public static Dictionary<string, VaultAPI.Data.folderInfo> getAllVaultSubFolderFolders(VDF.Vault.Currency.Connections.Connection connection, string parentID)
        {
            Dictionary<string, VaultAPI.Data.folderInfo> fodlerDict = new Dictionary<string, VaultAPI.Data.folderInfo>();
            try
            {
                //get folder list from vault
                
                foreach (var folder in connection.FolderManager.GetChildFolders(getFolderusingID(connection, parentID), false, false))
                {
                    VaultAPI.Data.folderInfo rootFI = new VaultAPI.Data.folderInfo();
                    rootFI = getFolderInfo(connection, folder);
                    Color col = folder.Category.Color;
                    //if (folder.NumberOfChildren > 0)
                    //{
                    //    foreach (var sfolder1 in connection.FolderManager.GetChildFolders(folder, false, false))
                    //    {
                    //        VaultAPI.Data.folderInfo subFI = new VaultAPI.Data.folderInfo();
                    //        subFI = (VaultAPI.Data.folderInfo)getFolderInfo(sfolder1);
                    //        rootFI.childFolders.Add(subFI);
                    //    }
                    //}
                    fodlerDict.Add(folder.EntityName, rootFI);
                }
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return fodlerDict;
        }

        public static VaultAPI.Data.folderInfo getFolderInfo(VDF.Vault.Currency.Connections.Connection connection, Folder folder)
        {
            VaultAPI.Data.folderInfo FI = new VaultAPI.Data.folderInfo();
            try
            {
                FI.Category = folder.Category.Name;
                FI.catID = folder.Category.ID;
                FI.CreateDate = folder.CreateDate.ToString();
                FI.CreateUserId = folder.CreateUserId;
                FI.EntityName = folder.EntityName;
                FI.FolderPath = folder.FolderPath;
                FI.FullName = folder.FullName;
                FI.NumberOfChildren = folder.NumberOfChildren;
                FI.CreateUserName = folder.CreateUserName;
                FI.EntityClass = folder.EntityClass.ToString();
                FI.EntityIterationId = folder.EntityIterationId;
                FI.EntityMasterId = folder.EntityMasterId;
                FI.Id = folder.Id;
                FI.FullUncName = folder.FullUncName;
                FI.IsLibraryFolder = folder.IsLibraryFolder;
                FI.IsVaultRoot = folder.IsVaultRoot;
                FI.Locked = folder.Locked;
                FI.Color = ZGHCC.HexConverter(folder.Category.Color);
                FI.fileCount = ZVFH.getFilesinFolderCountID(connection, folder.Id.ToString());
                FI.ParentId = folder.ParentId;
            }
            catch (SystemException ex)
            { }

            return FI;
        }
    }
}
