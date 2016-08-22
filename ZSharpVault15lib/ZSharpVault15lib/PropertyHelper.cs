using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.Connectivity.Explorer.ExtensibilityTools;
using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Properties;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace ZSharpVault15lib
{
    public class PropertyHelper
    {
        public static Dictionary<long, string> getPropertyNamesbyClass(VDF.Vault.Currency.Connections.Connection connection, string entityClassIDs)
        {
            Dictionary<long, string> props = new Dictionary<long, string>();
            try
            {
                using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
                {
                    var propDefs = serviceManager.PropertyService.GetPropertyDefinitionsByEntityClassId(entityClassIDs);

                    foreach (var item in propDefs)
                    {
                        props.Add(item.Id, item.DispName);
                    }
                }
            }
            catch (SystemException ex)
            {

            }
            return props;
        }

        public static Dictionary<long, PropDef> getPropertyDefsbyClass(VDF.Vault.Currency.Connections.Connection connection, string entityClassIDs)
        {
            Dictionary<long, PropDef> props = new Dictionary<long, PropDef>();
            try
            {
                using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
                {
                    var propDefs = serviceManager.PropertyService.GetPropertyDefinitionsByEntityClassId(entityClassIDs);

                    foreach (var item in propDefs)
                    {
                        props.Add(item.Id, item);
                    }
                }
            }
            catch (SystemException ex)
            {

            }
            return props;
        }

        private static PropDef propinfo;
        public static PropDef getPropertyDefDetails(VDF.Vault.Currency.Connections.Connection connection, string entityClassIDs, string propertyName)
        {
            Dictionary<long, string> props = new Dictionary<long, string>();
            try
            {
                using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
                {
                    var propDefs = serviceManager.PropertyService.GetPropertyDefinitionsByEntityClassId(entityClassIDs.ToString());

                    foreach (var item in propDefs)
                    {
                        //MessageBox.Show(propertyName + " == " + item.DispName + " | | | " + item.Id);
                        if (item.DispName.Equals(propertyName))
                        {
                            //MessageBox.Show(item.DispName + " | " + item.Id);
                            propinfo = item;
                        }

                    }
                }
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return propinfo;
        }
    }

    public class FilePropertiesHelper
    {
        public static Dictionary<string, string> getFileProperties(VDF.Vault.Currency.Connections.Connection connection, File selectedFile)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                using (WebServiceManager manager = connection.WebServiceManager)
                {
                    FileIteration entity = new FileIteration(connection, selectedFile);
                    PropDef[] propertyDefinitionsByEntityClassId = manager.PropertyService.GetPropertyDefinitionsByEntityClassId("FILE");
                    PropertyDefinitionDictionary dictionary2 = connection.PropertyManager.GetPropertyDefinitions("FILE", null, PropertyDefinitionFilter.IncludeAll);
                    foreach (PropDef def in propertyDefinitionsByEntityClassId)
                    {
                        object obj2 = connection.PropertyManager.GetPropertyValue(entity, dictionary2[def.Id], null);
                        dictionary.Add(def.DispName.ToString(), (obj2 == null) ? "" : obj2.ToString());
                        //Log.logger(string.Format("== LIB pro DICT == >>   '{0}' = '{1}'", def.DispName.ToString(), (obj2 == null) ? "" : obj2.ToString()));
                    }
                }
            }
            catch (SystemException exception)
            {
                //Log.logger(exception.ToString());
            }
            return dictionary;
        }

        public static Dictionary<PropDef, object> getFilePropertiesDefs(VDF.Vault.Currency.Connections.Connection connection, File selectedFile)
        {
            Dictionary<PropDef, object> dictionary = new Dictionary<PropDef, object>();
            try
            {
                using (WebServiceManager manager = connection.WebServiceManager)
                {
                    FileIteration entity = new FileIteration(connection, selectedFile);
                    PropDef[] propertyDefinitionsByEntityClassId = manager.PropertyService.GetPropertyDefinitionsByEntityClassId("FILE");
                    PropertyDefinitionDictionary dictionary2 = connection.PropertyManager.GetPropertyDefinitions("FILE", null, PropertyDefinitionFilter.IncludeAll);
                    foreach (PropDef def in propertyDefinitionsByEntityClassId)
                    {
                        object obj2 = connection.PropertyManager.GetPropertyValue(entity, dictionary2[def.Id], null);
                        dictionary.Add(def, (obj2 == null) ? "" : obj2);
                        //Log.logger(string.Format("== LIB pro DICT == >>   '{0}' = '{1}'", def.DispName.ToString(), (obj2 == null) ? "" : obj2.ToString()));
                    }
                }
            }
            catch (SystemException exception)
            {
                //Log.logger(exception.ToString());
            }
            return dictionary;
        }

        public static void updateFilePropertiesBulk(VDF.Vault.Currency.Connections.Connection connection, File selectedFile, Dictionary<PropDef, object> vaultfileProps) //string propName, string propValue
        {
            try
            {
                var EXPUtil = ExplorerLoader.LoadExplorerUtil(connection.Server, connection.Vault, connection.UserID, connection.Ticket);// ticket);
                EXPUtil.UpdateFileProperties(selectedFile, vaultfileProps);
            }
            catch (SystemException ex)
            {
                //statusMess = null;
                Debug.Write("\nError: " + ex.ToString());

            }
        }

        public static void updateFilePropertybyFileId(VDF.Vault.Currency.Connections.Connection connection, long fileMasterID, string propName, object value)
        {
            try
            {
                Dictionary<PropDef, object> vaultfileProps = new Dictionary<PropDef, object>();
                vaultfileProps.Clear();
                vaultfileProps.Add(PropertyHelper.getPropertyDefDetails(connection, "FILE", propName), value);
                Autodesk.Connectivity.WebServices.File[] selFile = connection.WebServiceManager.DocumentService.GetFilesByMasterId(fileMasterID);
                updateFilePropertiesBulk(connection, selFile[0], vaultfileProps);
            }
            catch (SystemException ex)
            {
                //statusMess = null;
                Debug.Write("\nError: " + ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }

        public static void updateFilePropertyBulkbyFileId(VDF.Vault.Currency.Connections.Connection connection, long fileMasterID, Dictionary<string, string> vaultfilePropsString)
        {
            try
            {
                Dictionary<PropDef, object> vaultfileProps = new Dictionary<PropDef, object>();
                vaultfileProps.Clear();
                foreach (var prop in vaultfilePropsString)
                {
                    vaultfileProps.Add(PropertyHelper.getPropertyDefDetails(connection, "FILE", prop.Key), prop.Value);
                }


                Autodesk.Connectivity.WebServices.File[] selFile = connection.WebServiceManager.DocumentService.GetFilesByMasterId(fileMasterID);
                updateFilePropertiesBulk(connection, selFile[0], vaultfileProps);
            }
            catch (SystemException ex)
            {
                //statusMess = null;
                Debug.Write("\nError: " + ex.ToString());
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
