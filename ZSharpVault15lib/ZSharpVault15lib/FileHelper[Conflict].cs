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
using logit = ZSharpLogger.Log;

namespace ZSharpVault15lib
{
    public class FileHelper
    {
        private static string resultVal;
        private static string statusMess;
        public static string downloadVaultFiles(VDF.Vault.Currency.Connections.Connection connection, string path, List<ISelection> filesColl)
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
                    logit.logger("==LIB ==" + propDefs.Keys.ToArray()[item.PropDefId - 1] + " | " + item.PropDefId.ToString() + " | " + item.Val.ToString());
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
                logit.logger("DesignVisAttmtStatus: " + selectedFile.DesignVisAttmtStatus.ToString());
                VDF.Vault.Currency.Entities.FileIteration fileInteration = new FileIteration(connection, selectedFile);
                var propDefs = connection.PropertyManager.GetPropertyDefinitions(VDF.Vault.Currency.Entities.EntityClassIds.Files, null, VDF.Vault.Currency.Properties.PropertyDefinitionFilter.IncludeAll);
                foreach (var key in propDefs.Keys)
                {
                    // Print the Name from the Definition and the Value from the Property
                    object propValue = connection.PropertyManager.GetPropertyValue(fileInteration, propDefs[key], null);
                    dict.Add(key.ToString(), propValue == null ? "" : propValue.ToString());
                    logit.logger(string.Format("== LIB 1 DICT == >>   '{0}' = '{1}'", key.ToString(), propValue == null ? "" : propValue.ToString()));
                }
            }
            catch (SystemException ex)
            {
                logit.logger(ex.ToString());
            }
            return dict;
        }

        public static Dictionary<string, string> getFileProperties(VDF.Vault.Currency.Connections.Connection connection, File selectedFile)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
                {
                    VDF.Vault.Currency.Entities.FileIteration fileInteration = new FileIteration(connection, selectedFile);
                    PropDef[] propDefs = serviceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("FILE");
                    var propDefsAlt = connection.PropertyManager.GetPropertyDefinitions(VDF.Vault.Currency.Entities.EntityClassIds.Files, null, VDF.Vault.Currency.Properties.PropertyDefinitionFilter.IncludeAll);
                    foreach (PropDef pdef in propDefs)
                    {
                        object propValue = connection.PropertyManager.GetPropertyValue(fileInteration, propDefsAlt[pdef.Id], null);
                        //PropInst propinst = propcoll[0];
                        //string val = propinst.Val == null ? "" : propinst.Val.ToString();
                        dict.Add(pdef.DispName.ToString(), propValue == null ? "" : propValue.ToString());
                        logit.logger(string.Format("== LIB pro DICT == >>   '{0}' = '{1}'", pdef.DispName.ToString(), propValue == null ? "" : propValue.ToString()));

                        //PropInst[] propcoll = serviceManager.PropertyService.GetProperties("FILE", new long[] { selectedFile.Id }, new long[] { pdef.Id });
                       
                    }
                }
            }
            catch (SystemException ex)
            {
                logit.logger(ex.ToString());
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
                    logit.logger("FileIDS CSV: " + resultVal);
                    statusMess = "Success";
                }
            }
            catch (SystemException ex)
            {
                logit.logger("\nError: " + ex.ToString());
            }
            return resultVal;
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
