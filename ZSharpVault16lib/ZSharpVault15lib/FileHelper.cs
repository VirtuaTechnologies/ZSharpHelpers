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
using System.Linq;
using ZGHCC = ZSharpVault16lib.GenVaultHelper;
using ZSharpVault16lib.Global;


namespace ZSharpVault16lib
{
    public class FileHelper
    {
        private static string resultVal;
        private static string statusMess;
        private static bool boolRes = false;

        public static bool checkifFileExists(VDF.Vault.Currency.Connections.Connection connection, string filePath)
        {
            try
            {
                boolRes = false;
                string path = filePath.Replace(System.IO.Path.GetFileName(filePath), "");
                File[] files = getFilesfromVaultFolder(connection, path);
                foreach(File file in files)
                {
                    if (file.Name == System.IO.Path.GetFileName(filePath))
                        boolRes = true;
                }
                connection.WebServiceManager.DocumentService.FindLatestFilesByPaths(filePath.ToSingleArray()).First();
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
                //MessageBox.Show(ex.ToString());
            }
            return boolRes;
        }

        public static bool checkifFileExistsusingFolderID(VDF.Vault.Currency.Connections.Connection connection, string fileID, string folderID)
        {
            try
            {
                
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
                //MessageBox.Show(ex.ToString());
            }
            return boolRes;
        }

        public static File gefilebyfilePath(VDF.Vault.Currency.Connections.Connection connection, string filePath)
        {
            Autodesk.Connectivity.WebServices.File selectedFile = null;
            try
            {
                if (checkifFileExists(connection, filePath))
                {
                    selectedFile = connection.WebServiceManager.DocumentService.FindLatestFilesByPaths(filePath.ToSingleArray()).First();
                }
                else
                {
                    return null;
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
                //MessageBox.Show(ex.ToString());
            }
            return selectedFile;
        }
        public static string getPathbyFileID(VDF.Vault.Currency.Connections.Connection connection, long fileID)
        {
            try
            {
                Autodesk.Connectivity.WebServices.File selectedFile = null;
                selectedFile = connection.WebServiceManager.DocumentService.GetLatestFileByMasterId(fileID);
                Autodesk.Connectivity.WebServices.Folder fld = connection.WebServiceManager.DocumentService.GetFolderById(selectedFile.FolderId);
                resultVal = fld.FullName + "/" + selectedFile.Name;
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
                //MessageBox.Show(ex.ToString());
            }
            return resultVal;
        }

        public static File geFilebyID(VDF.Vault.Currency.Connections.Connection connection, long fileID)
        {
            Autodesk.Connectivity.WebServices.File selectedFile = null;
            try
            {
                
                selectedFile = connection.WebServiceManager.DocumentService.GetLatestFileByMasterId(fileID);
                
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
                //MessageBox.Show(ex.ToString());
            }
            return selectedFile;
        }

        public static string downloadVaultFilebyID(VDF.Vault.Currency.Connections.Connection connection, string path, long selectedFileID)
        {
            try
            {
                using (WebServiceManager serviceManager = connection.WebServiceManager)
                {
                    Autodesk.Connectivity.WebServices.File localFile = serviceManager.DocumentService.GetFileById(selectedFileID);
                    var FileDownloadTicket = serviceManager.DocumentService.GetDownloadTicketsByFileIds(new long[] { selectedFileID });
                    FilestoreService fileStoreService = serviceManager.FilestoreService;
                    var fileBytes = fileStoreService.DownloadFilePart(FileDownloadTicket[0].Bytes, 0, localFile.FileSize, false);
                    //MessageBox.Show(path + "\\" + localFile.Name);
                    System.IO.File.WriteAllBytes(path + "\\" + localFile.Name, fileBytes);


                    //Check if file exist
                    if (!System.IO.File.Exists(path + "\\" + localFile.Name))
                    {
                        //failedFiles.Add(file);
                        statusMess = "File Download Failed!";
                        ZGHCC.writeLog(statusMess);
                    }
                    else
                    {
                        statusMess = path + "\\" + localFile.Name;
                        //fileExt = System.IO.Path.GetExtension(localFile.Name);
                        ZGHCC.writeLog(statusMess);
                    }

                }

                //statusMess = "Success";
            }
            catch (SystemException ex)
            {
                statusMess = null;
                Debug.Write("\nError: " + ex.ToString());
                statusMess = "Error: \n" + ex.ToString();
            }
            return statusMess;
            //Autodesk.Connectivity.WebServices.File localFile = serviceManager.DocumentService.GetFileById(file.Id);
        }

        public static string downloadVaultFile(VDF.Vault.Currency.Connections.Connection connection, string path, List<ISelection> filesColl)
        {
            try
            {
                //var settings = new VDF.Vault.Settings.AcquireFilesSettings(Global.variables.connection, updateFileReferences: false);
                //settings.LocalPath = new VDF.Currency.FolderPathAbsolute(Global.variables.fileStore);

                if (filesColl.Count > 0)
                {
                    foreach (var file in filesColl)
                    {
                        File selectedFile = null;
                        // Look of the File object. 
                        if (file.TypeId == SelectionTypeId.File)
                        {
                            // our ISelection.Id is really a File.MasterId
                            selectedFile = connection.WebServiceManager.DocumentService.GetLatestFileByMasterId(file.Id);
                        }
                        else if (file.TypeId == SelectionTypeId.FileVersion)
                        {
                            // our ISelection.Id is really a File.Id
                            selectedFile = connection.WebServiceManager.DocumentService.GetFileById(file.Id);

                        }
                        if (selectedFile != null)
                        {
                           using (WebServiceManager serviceManager = connection.WebServiceManager)
                            {
                                Autodesk.Connectivity.WebServices.File localFile = serviceManager.DocumentService.GetFileById(selectedFile.Id);
                                var FileDownloadTicket = serviceManager.DocumentService.GetDownloadTicketsByFileIds(new long[] { selectedFile.Id });
                                FilestoreService fileStoreService = serviceManager.FilestoreService;
                                var fileBytes = fileStoreService.DownloadFilePart(FileDownloadTicket[0].Bytes, 0, localFile.FileSize, false);
                                //MessageBox.Show(path + "\\" + selectedFile.Name);
                                System.IO.File.WriteAllBytes(path + "\\" + selectedFile.Name, fileBytes);
                                

                                //Check if file exist
                                if (!System.IO.File.Exists(path + "\\" + selectedFile.Name))
                                {
                                    //failedFiles.Add(file);
                                    statusMess += "\n" + path + "\\" + selectedFile.Name + " - File Check Issue";
                                }
                                else
                                {
                                    statusMess += "\n" + path + "\\" + selectedFile.Name + " - Success";
                                }

                            }

                        }
                        //MessageBox.Show(String.Format("Hello World! The file {0} size is: {1} bytes", selectedFile.Name, selectedFile.FileSize));
                    }
                    statusMess = "Success";
                }
            }
            catch (SystemException ex)
            {
                statusMess = null;
                Debug.Write("\nError: " + ex.ToString());
                statusMess = "Error: \n" + ex.ToString();
            }
            return statusMess;
            //Autodesk.Connectivity.WebServices.File localFile = serviceManager.DocumentService.GetFileById(file.Id);
        }
        
        public static string getFilePropertyValuebyIndex(VDF.Vault.Currency.Connections.Connection connection, File selectedFile, int index)
        {
            try
            {
                var propDefs = connection.PropertyManager.GetPropertyDefinitions(VDF.Vault.Currency.Entities.EntityClassIds.Files, null, VDF.Vault.Currency.Properties.PropertyDefinitionFilter.IncludeAll);
                var propSvc = connection.WebServiceManager.PropertyService;
                var properties = propSvc.GetPropertiesByEntityIds("FILE", new long[] { selectedFile.Id });
                Dictionary<PropInst, PropInst> dict = properties.ToDictionary(x => x, x => x);
                PropInst val = dict.ElementAt(index-1).Value;
                resultVal = val.ToString();
            }
            catch (SystemException ex)
            {
                Debug.Write("\nError: " + ex.ToString());
            }
            return resultVal;
        }

        public static string getFilePropertyValuebyKey(VDF.Vault.Currency.Connections.Connection connection, File selectedFile, string key)
        {
            Dictionary<string, string> dict = new Dictionary<string,string>();
            try
            {
                var propDefs = connection.PropertyManager.GetPropertyDefinitions(VDF.Vault.Currency.Entities.EntityClassIds.Files, null, VDF.Vault.Currency.Properties.PropertyDefinitionFilter.IncludeAll);
                var propSvc = connection.WebServiceManager.PropertyService;
                var properties = propSvc.GetPropertiesByEntityIds("FILE", new long[] { selectedFile.Id });
                //Dictionary<PropInst, PropInst> dict = properties.ToDictionary(x => x, x => x);
                foreach (var item in properties)
                {
                    string keyItem = propDefs.Keys.ToArray()[item.PropDefId - 1];
                    string val = item.Val.ToString();
                    dict.Add(keyItem, val);
                    ZGHCC.writeLog("==LIB ==" + propDefs.Keys.ToArray()[item.PropDefId - 1] + " | " + item.PropDefId.ToString() + " | " + item.Val.ToString());
                    if (keyItem == key)
                    {
                        resultVal = val;
                    }
                    else
                        resultVal = null;
                }

            }
            catch (SystemException ex)
            {
                Debug.Write("\nError: " + ex.ToString());
            }
            return resultVal;
        }

        public static Dictionary<string, string> PrintProperties(VDF.Vault.Currency.Connections.Connection connection, File selectedFile)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                ZGHCC.writeLog("DesignVisAttmtStatus: " + selectedFile.DesignVisAttmtStatus.ToString());
                VDF.Vault.Currency.Entities.FileIteration fileInteration = new FileIteration(connection, selectedFile);
                var propDefs = connection.PropertyManager.GetPropertyDefinitions(VDF.Vault.Currency.Entities.EntityClassIds.Files, null, VDF.Vault.Currency.Properties.PropertyDefinitionFilter.IncludeAll);
                foreach (var key in propDefs.Keys)
                {
                    // Print the Name from the Definition and the Value from the Property
                    object propValue = connection.PropertyManager.GetPropertyValue(fileInteration, propDefs[key], null);
                    dict.Add(propDefs[key].DisplayName.ToString(), propValue == null ? "" : propValue.ToString());
                    ZGHCC.writeLog(string.Format("== LIB 1 DICT == >>   '{0}' = '{1}'", propDefs[key].DisplayName.ToString(), propValue == null ? "" : propValue.ToString()));
                }
                dict.Add("Id", fileInteration.EntityMasterId.ToString());
                dict.Add("EntityClass", "File");
            }
            catch (SystemException ex)
            {
                ZGHCC.writeLog(ex.ToString());
            }
            return dict;
        }

        public static string getFileIDsinCSV(VDF.Vault.Currency.Connections.Connection connection, List<ISelection> filesColl)
        {
            
            resultVal = null;
            try
            {
                //var settings = new VDF.Vault.Settings.AcquireFilesSettings(Global.variables.connection, updateFileReferences: false);
                //settings.LocalPath = new VDF.Currency.FolderPathAbsolute(Global.variables.fileStore);
                if (filesColl.Count > 0)
                {
                    foreach (var file in filesColl)
                    {
                        File selectedFile = connection.WebServiceManager.DocumentService.GetLatestFileByMasterId(file.Id);
                        // Look of the File object. 
                        if (selectedFile != null)
                        {
                            resultVal += selectedFile.Id + "-";
                        }
                        //MessageBox.Show(String.Format("Hello World! The file {0} size is: {1} bytes", selectedFile.Name, selectedFile.FileSize));
                    }
                    char last = resultVal[resultVal.Length - 1];
                    if(last.Equals("-"))
                        resultVal = resultVal.Remove(resultVal.Length - 1);
                    ZGHCC.writeLog("FileIDS CSV: " + resultVal);
                    statusMess = "Success";
                }
            }
            catch (SystemException ex)
            {
                ZGHCC.writeLog("\nError: " + ex.ToString());
            }
            return resultVal;
        }


        private static Autodesk.Connectivity.WebServices.File[] filecoll;

        public static Autodesk.Connectivity.WebServices.File[] getFilesfromVaultFolderID(VDF.Vault.Currency.Connections.Connection connection, string folderID)
        {
            try
            {
                filecoll = null;
                using (WebServiceManager wServ = connection.WebServiceManager)
                {
                    string bookmark = string.Empty;
                    SrchStatus status = null;
                    filecoll = wServ.DocumentService.FindFilesBySearchConditions(null, null, new long[] { Convert.ToInt64(folderID) }, false, true, ref bookmark, out status);
                    //MessageBox.Show("Count" + filecoll.ToList<Autodesk.Connectivity.WebServices.File>().Count().ToString());
                }
            }
            catch (SystemException ex)
            {
                //statusMess = null;
                Debug.Write("\nError: " + ex.ToString());
                statusMess += "Error: \n" + ex.ToString();
            }
            return filecoll;
        }

        private static int count;
        public static int getFilesinFolderCountID(VDF.Vault.Currency.Connections.Connection connection, string folderID)
        {
            
            try
            {
                filecoll = null;
                using (WebServiceManager wServ = connection.WebServiceManager)
                {
                    string bookmark = string.Empty;
                    SrchStatus status = null;
                    filecoll = wServ.DocumentService.FindFilesBySearchConditions(null, null, new long[] { Convert.ToInt64(folderID) }, false, true, ref bookmark, out status);
                    if (filecoll == null)
                        count = 0;
                    else
                        count = filecoll.Count();
                    //MessageBox.Show("Count" + filecoll.ToList<Autodesk.Connectivity.WebServices.File>().Count().ToString());
                }
            }
            catch (SystemException ex)
            {
                //statusMess = null;
                Debug.Write("\nError: " + ex.ToString());
                statusMess += "Error: \n" + ex.ToString();
            }
            return count;
        }

        public static Autodesk.Connectivity.WebServices.File[] getFilesfromVaultFolder(VDF.Vault.Currency.Connections.Connection connection, string vaultFolder)
        {
            try
            {
                filecoll = null;
                using (WebServiceManager wServ = connection.WebServiceManager)
                {
                    string bookmark = string.Empty;
                    SrchStatus status = null;
                    filecoll = wServ.DocumentService.FindFilesBySearchConditions(null, null, new long[] { wServ.DocumentService.GetFolderByPath(vaultFolder).Id }, false, true, ref bookmark, out status);
                    //MessageBox.Show("Count" + filecoll.ToList<Autodesk.Connectivity.WebServices.File>().Count().ToString());
                }
            }
            catch (SystemException ex)
            {
                //statusMess = null;
                Debug.Write("\nError: " + ex.ToString());
                statusMess += "Error: \n" + ex.ToString();
            }
            return filecoll;
        }

        private static Dictionary<int, Autodesk.Connectivity.WebServices.File> filesDict = new Dictionary<int, Autodesk.Connectivity.WebServices.File>();
        public Dictionary<int, Autodesk.Connectivity.WebServices.File> getFilesDictfromVaultFolder(VDF.Vault.Currency.Connections.Connection connection, string vaultFolder)
        {
            Autodesk.Connectivity.WebServices.File[] fileColls = null;

            fileColls = getFilesfromVaultFolder(connection, vaultFolder);
            filesDict.Clear();
            int i = 0;
            foreach (var tFile in fileColls)
            {
                //MessageBox.Show(tFile.Id + " | " + tFile.Name);
                filesDict.Add(i, tFile);
                i++;
                //lView_Log.Items.Add("Template files: " + tFiles.Name);
            }

            foreach (var f in filesDict)
            {
                //MessageBox.Show(f.Key + " | " + f.Value.Name);

                //lView_Log.Items.Add("Template files: " + tFiles.Name);
            }

            return filesDict;
        }

        public List<Autodesk.Connectivity.WebServices.File> getFilesListfromVaultFolder(VDF.Vault.Currency.Connections.Connection connection, string vaultFolder)
        {
            Autodesk.Connectivity.WebServices.File[] fileColls = getFilesfromVaultFolder(connection, vaultFolder);

            return fileColls.ToList<Autodesk.Connectivity.WebServices.File>();
        }

        public static bool uploadFiletoVaultFolder(VDF.Vault.Currency.Connections.Connection connection, string destinationTempFolder, string folderID, string comment, string fileName, byte[] fileBytes)
        {
            try
            {
                VDF.Vault.Currency.Entities.Folder fld = FolderHelper.getFolderusingID(connection, folderID);
                List<FileAssocParam> fileAssocParamList = new List<FileAssocParam>();
                FileAssocParam fileAssocArray = new FileAssocParam();
                FileAssocParam[] paramArray = fileAssocParamList.ToArray();
                BOM _bom = new BOM();
                //string destinationFile = System.IO.Path.GetTempPath() + fileName;
                System.IO.File.WriteAllBytes(destinationTempFolder, fileBytes);
                VDF.Currency.FilePathAbsolute FPA = new VDF.Currency.FilePathAbsolute(destinationTempFolder);
                FileIteration FI = connection.FileManager.AddFile(fld, comment, paramArray, _bom, FileClassification.None, false, FPA);

                //cehck if file exists
                if (FI.FolderId.ToString() == folderID)
                {
                    ZGHCC.writeLog("File uplaoded and it Exists: " + folderID + " | " + comment);
                    boolRes = true;
                }
                else
                {
                    boolRes = false;
                }
                
            }
            catch (SystemException ex)
            {
                ZGHCC.writeLog("uploadFiletoVaultFolder EX: " +  ex.ToString());
                statusMess = null;
                Debug.Write("\nError: " + ex.ToString());
                statusMess = "Error: \n" + ex.ToString();
                boolRes = false;
            }
            return boolRes;
        }
        /*
        public static Dictionary<string, string> getFileAllVals(VDF.Vault.Currency.Connections.Connection connection, File selectedFile)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Clear();
            try
            {
                var propDefs = connection.PropertyManager.GetPropertyDefinitions(VDF.Vault.Currency.Entities.EntityClassIds.Files, null, VDF.Vault.Currency.Properties.PropertyDefinitionFilter.IncludeAll);
                var propSvc = connection.WebServiceManager.PropertyService;
                var properties = propSvc.GetPropertiesByEntityIds("FILE", new long[] { selectedFile.Id });
                Dictionary<PropInst, PropInst> dictProp = properties.ToDictionary(x => x, x => x);
                foreach (var item in dictProp)
                {
                    
                    
                    //string keyItem = item.Key.
                    //string val = item.Value
                    //logit.logger("== LIB 1 DICT == >> " + propDefs.Keys.ToArray()[item.PropDefId - 1] + " | " + +" | " + item.Key.Val + " | " + item.Value.EntityId + " | " + item.Value.PropDefId + " | " + item.Value.Val);
                }
                for (int i = 0; i < properties.ToDictionary(x => x, x => x).Count(); i++)
                {
                    logit.logger("==LIB == ARR" + properties.ToArray()[i] + " | " + properties.ToArray()[i].Val.ToString());
                }
                foreach (var item in properties)
                {
                    //item.
                    string keyItem = propDefs.Keys.ToArray()[item.PropDefId];
                    string val = item.Val.ToString();
                    if(!dict.ContainsKey(keyItem))
                        dict.Add(keyItem, val);
                    //logit.logger("==LIB ==" + propDefs.Keys.ToArray()[item.PropDefId] + " | " + item.PropDefId.ToString() + " | " + item.Val.ToString());
                }

            }
            catch (SystemException ex)
            {
                Debug.Write("\nError: " + ex.ToString());
            }
            return dict;
        }
         */
    }
}
