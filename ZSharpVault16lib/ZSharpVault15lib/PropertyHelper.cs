using System;
using System.Collections.Generic;
using System.Data;
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

namespace ZSharpVault16lib
{
    public class PropertyHelper
    {
        /*
        "Contains"= 1
        "DoesNotContain"= 2
        "IsExactly"= 3
        "IsEmpty"= 4
        "IsNotEmpty"= 5
        "GreaterThan"= 6
        "GreaterThanOrEqualTo" or ">=" = 7;
        "LessThan"or "<"= 8
        "LessThanOrEqualTo" or"<="= 9
        "NotEqualTo" or "!=" = 10
                 * 
         */
        private static CustomEntityService CES;
        private static CustEnt[] custEntities;
        //private static DataTable dataTable = new DataTable();
        private static DataRow row;
        private static Dictionary<long, string> cDefDict = new Dictionary<long, string>();
        private static Dictionary<string, string> custPropDict = new Dictionary<string, string>();
        private static CustEnt ent;
        private static string result;
        private static bool boolresult;
        private static bool allProps;
        private static long longresult;
        private static object resultObj;
        private static PropDef propinfo;
        private static long[] rID;
        private static VDF.Vault.Currency.Connections.Connection connection;

        public static object getItemPropValue(VDF.Vault.Currency.Connections.Connection connection, string entityClassIDs, Item item, string propName)
        {
            try
            {
                using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
                {
                    var propSvc = serviceManager.PropertyService;
                    PropDef propDef = getPropertyDefDetails(connection, entityClassIDs, propName);
                    //MessageBox.Show(">>> :: " + propDef.DispName + propDef.Id + " | " );
                    var properties = propSvc.GetProperties("ITEM", new long[] { item.Id }, new long[] { propDef.Id });

                    if (properties != null)
                    {
                        //MessageBox.Show(" | " + properties);
                        foreach (PropInst prop in properties)
                        {
                            if (item != null)
                            {
                                resultObj = prop.Val == null ? "" : prop.Val.ToString();
                                if (string.IsNullOrEmpty(resultObj.ToString()))
                                {
                                    //MessageBox.Show(propName + " | " +  resultObj.GetType().ToString() + " | " + resultObj.ToString());
                                    resultObj = "null";
                                }
                                else
                                {
                                    //MessageBox.Show(">>> " + propName + resultObj.GetType().ToString() + " | " + resultObj.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                
                resultObj = ex.ToString();
            }
            return resultObj;
        }

        public static Dictionary<long, PropDef> getPropertiesbyClass(VDF.Vault.Currency.Connections.Connection connection, string entityClassIDs)
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

        public static void updateITEMPropbyName(VDF.Vault.Currency.Connections.Connection connection, Item item, string PropName, string propValue)
        {
            try
            {
                using (WebServiceManager wServ = connection.WebServiceManager) //using will log out after usage
                {
                    ItemService IS = wServ.ItemService;
                    PropDef propDefs = getPropertyDefDetails(connection, "ITEM", PropName);
                    var editITEM = IS.EditItems(new long[] { item.RevId })[0];
                    GenVaultHelper.LibNotes();
                    rID = new[] { editITEM.RevId };
                    PropInstParam PIP = new PropInstParam() { PropDefId = propDefs.Id, Val = propValue };
                    Item[] uitem = IS.UpdateItemProperties(rID, new PropInstParamArray[] { new PropInstParamArray() { Items = new PropInstParam[] { PIP } } });
                    IS.UpdateAndCommitItems(uitem); //IS.UpdateAndCommitItems(new Item[] { item });
                    //IS.DeleteUncommittedItems(false);
                }
            }
            catch (System.Exception ex)
            {
                //return ex.ToString();
            }
        }

        public static PropDef getPropertyDefDetails(VDF.Vault.Currency.Connections.Connection connection, string entityClassIDs, string propertyName)
        {
            Dictionary<long, string> props = new Dictionary<long, string>();
            try
            {
                using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
                {
                    var propDefs = serviceManager.PropertyService.GetPropertyDefinitionsByEntityClassId(entityClassIDs);

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

        public static Dictionary<string, CustEntDef> getCustomObjDefs(VDF.Vault.Currency.Connections.Connection connection)
        {
            Dictionary<string, CustEntDef> cDefDictColl = new Dictionary<string, CustEntDef>();
            try
            {
                CES = connection.WebServiceManager.CustomEntityService;
                CustEntDef[] ceDefs = CES.GetAllCustomEntityDefinitions();
                foreach (var ceDef in ceDefs)
                {
                    cDefDictColl.Add(ceDef.DispName.ToString(), ceDef);
                }
            }
            catch (SystemException ex)
            {

            }
            return cDefDictColl;
        }

        public static CustEntDef getCustomObjDefbyName(VDF.Vault.Currency.Connections.Connection connection, string custObjDefName)
        {
            CustEntDef cDef = new CustEntDef();
            try
            {
                
                CES = connection.WebServiceManager.CustomEntityService;
                CustEntDef[] ceDefs = CES.GetAllCustomEntityDefinitions();
                //MessageBox.Show("inside,," + ceDefs.ToList().Count);
                foreach (var ceDef in ceDefs)
                {
                    //MessageBox.Show(custObjDefName + " --" + ceDef.Id.ToString() + " | " + ceDef.DispName);
                    if (ceDef.DispName.Equals(custObjDefName))
                    {
                        //MessageBox.Show(ceDef.Id.ToString() + " | | ||" + ceDef.DispName);
                        cDef = ceDef;
                    }
                }
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return cDef;
        }

        public static Dictionary<long, string> getCustomObjDefNames(VDF.Vault.Currency.Connections.Connection connection)
        {
            try
            {
                CES = connection.WebServiceManager.CustomEntityService;
                CustEntDef[] ceDefs = CES.GetAllCustomEntityDefinitions();
                foreach (var ceDef in ceDefs)
                {
                    cDefDict.Add(ceDef.Id, ceDef.DispName.ToString());
                }
            }
            catch (SystemException ex)
            {

            }
            return cDefDict;
        }

        public Dictionary<string, string> getCUSTENTPropertyVal(VDF.Vault.Currency.Connections.Connection connection, string entityClassIDs, CustEnt custEnt)
        {
            try
            {
                
                using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
                {
                    var propDefs = connection.PropertyManager.GetPropertyDefinitions(entityClassIDs, null, VDF.Vault.Currency.Properties.PropertyDefinitionFilter.IncludeAll);
                    VDF.Vault.Currency.Entities.CustomObject custObj = new VDF.Vault.Currency.Entities.CustomObject(connection, custEnt); 
                    foreach (var key in propDefs.Keys)
                    {
                        
                        object propValue = connection.PropertyManager.GetPropertyValue(custObj, propDefs[key], null);
                        custPropDict.Add(propDefs[key].DisplayName.ToString(), propValue == null ? "" : propValue.ToString());
                    }
                }
            }
            catch (SystemException ex)
            {
                result = ex.ToString();
            }
            return custPropDict;
        }

        private static DataTable setProptoCol(VDF.Vault.Currency.Properties.PropertyDefinitionDictionary propDefs, List<string> properties)
        {
            DataTable dataTable = new DataTable();
            foreach (var key in propDefs.Keys)
            {
                if (allProps)
                {
                    dataTable.Columns.Add(propDefs[key].DisplayName.ToString());
                }
                else
                {
                    if (properties.Contains(propDefs[key].DisplayName.ToString()))
                    {
                        dataTable.Columns.Add(propDefs[key].DisplayName.ToString());
                    }
                }
            }
            return dataTable;
        }

        public DataTable getCENTCollPropertyValinDataTable(VDF.Vault.Currency.Connections.Connection connection, string entityClassIDs, CustEnt[] custEntscoll, List<string> properties)
        {
            DataTable dataTable = new DataTable();
            try
            {
                dataTable.Clear();
                using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
                {
                    var propDefs = connection.PropertyManager.GetPropertyDefinitions(entityClassIDs, null, VDF.Vault.Currency.Properties.PropertyDefinitionFilter.IncludeAll);
                    //set datatable columns
                    Debug.Write(properties.Count);
                    if (properties.Count == 0 || properties == null)
                        allProps = true;
                    else
                        allProps = false;

                    dataTable = setProptoCol(propDefs,properties);
                    
                    foreach (CustEnt custEnt in custEntscoll)
                    {
                        VDF.Vault.Currency.Entities.CustomObject custObj = new VDF.Vault.Currency.Entities.CustomObject(connection, custEnt);
                        row = dataTable.NewRow();
                        foreach (var key in propDefs.Keys)
                        {
                            if (allProps)
                            {
                                object propValue = connection.PropertyManager.GetPropertyValue(custObj, propDefs[key], null);
                                
                                //custPropDict.Add(propDefs[key].DisplayName.ToString(), propValue == null ? "" : propValue.ToString());
                                if (propValue != null)
                                {
                                    Debug.Write("\n" + key + " | " + propValue.ToString() + "\n");
                                    row[propDefs[key].DisplayName.ToString()] = propValue;
                                }
                            }
                            else
                            {
                                if (properties.Contains(propDefs[key].DisplayName.ToString()))
                                {
                                    object propValue = connection.PropertyManager.GetPropertyValue(custObj, propDefs[key], null);
                                    
                                    //custPropDict.Add(propDefs[key].DisplayName.ToString(), propValue == null ? "" : propValue.ToString());
                                    if (propValue != null)
                                    {
                                        Debug.Write("\n" + key + " | " + propValue.ToString() + "\n");
                                        row[propDefs[key].DisplayName.ToString()] = propValue;
                                    }
                                }
                            }
                        }
                        dataTable.Rows.Add(row);
                    }
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return dataTable;
        }

        public long createCustomObj(VDF.Vault.Currency.Connections.Connection connection, long PropDefID, string objName)
        {
            
            
            using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
            {
                ent = CES.AddCustomEntity(PropDefID, objName);
            }
            return ent.Id;
        }

        public string createCustomObjTest(string server, string vault, string uName, string pass)
        {
            try
            {

                connection = getVaultConnection(server, vault, uName, pass);
                using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
                {
                    ent = CES.AddCustomEntity(1, "TEST123");
                    result = ent.ToString();
                }
            }
            catch (System.Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }

        public static VDF.Vault.Currency.Connections.Connection getVaultConnection(string server, string vault, string uName, string pass)
        {
            VDF.Vault.Results.LogInResult results = VDF.Vault.Library.ConnectionManager.LogIn(server, vault, uName, pass, VDF.Vault.Currency.Connections.AuthenticationFlags.Standard, null);
            connection = results.Connection;
            return connection;
 
        }

        public VDF.Vault.Currency.Connections.Connection getpsVaultConn(string server, string vault, string uName, string pass)
        {
            VDF.Vault.Results.LogInResult results = VDF.Vault.Library.ConnectionManager.LogIn(server, vault, uName, pass, VDF.Vault.Currency.Connections.AuthenticationFlags.Standard, null);
            connection = results.Connection;
            return connection;
        }

        public static long createCustomObj(VDF.Vault.Currency.Connections.Connection connection, int PropDefID, string objName)
        {
            
            using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
            {
                ent = CES.AddCustomEntity(PropDefID, objName);
            }
            return ent.Id;
        }

        public static void updateCustomObjProp(VDF.Vault.Currency.Connections.Connection connection, long PropID, long entID, object value)
        {
            using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
            {
                List<long> entitiesList = new List<long>();
                entitiesList.Clear();
                entitiesList.Add(entID);
                CES.UpdateCustomEntityProperties(entitiesList.ToArray(), new PropInstParamArray[] { new PropInstParamArray() { Items = new PropInstParam[] { new PropInstParam() { PropDefId = PropID, Val = value } } } });

            }
        }

        public static void updateFileProperty(VDF.Vault.Currency.Connections.Connection connection, string propName, string propValue)
        {
            try
            {
                using (WebServiceManager wServ = connection.WebServiceManager)
                {
                    
                }
            }
            catch (SystemException ex)
            {
                //statusMess = null;
                Debug.Write("\nError: " + ex.ToString());
                
            }
        }

        public void updateCustomObjPropbyID(Autodesk.Connectivity.WebServices.CustomEntityService vces, long PropID, long entID, string value)
        {
            
            List<long> entitiesList = new List<long>();
            entitiesList.Clear();
            entitiesList.Add(entID);
            vces.UpdateCustomEntityProperties(entitiesList.ToArray(), new PropInstParamArray[] { new PropInstParamArray() { Items = new PropInstParam[] { new PropInstParam() { PropDefId = PropID, Val = value } } } });
        }

        public string updateCustomObjPropbyPropName(VDF.Vault.Currency.Connections.Connection connection, string PropName, long entID, string ObjType, object value) //CUSTENT
        {
            try
            {
                
                using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
                {
                    var propDefs = getPropertyDefDetails(connection, ObjType, PropName);
                    CES = connection.WebServiceManager.CustomEntityService;
                    //check value format
                    List<long> entitiesList = new List<long>();
                    entitiesList.Clear();
                    entitiesList.Add(entID);
                    CES.UpdateCustomEntityProperties(entitiesList.ToArray(), new PropInstParamArray[] { new PropInstParamArray() { Items = new PropInstParam[] { new PropInstParam() { PropDefId = propDefs.Id, Val = value } } } });
                    result = propDefs.DispName + " | " + propDefs.IsSys + " | " + propDefs.Typ + " | " + propDefs.Id;
                }
            }
            catch (SystemException ex)
            {
                result = ex.ToString();
            }

            return result;
        }

        public CustEnt[] getCustomentitiesbyID(VDF.Vault.Currency.Connections.Connection connection, long PropDefID, string srchop, string searchText)
        {
            try
            {
                
                using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
                {
                    //get property details
                    
                    result = connection.Vault.ToString();
                    DocumentService docServ = serviceManager.DocumentService;

                    SrchCond srchCond = new SrchCond()
                    {
                        PropDefId = PropDefID,
                        PropTyp = PropertySearchType.AllProperties,
                        SrchOper = SrchOperator(srchop), // is equal
                        SrchRule = SearchRuleType.Must,
                        SrchTxt = searchText
                    };
                    string bookmark = string.Empty;
                    SrchStatus status = null;
                    custEntities = connection.WebServiceManager.CustomEntityService.FindCustomEntitiesBySearchConditions(new SrchCond[] { srchCond }, null, ref bookmark, out status);
                    
                }
            }
            catch (SystemException ex)
            {
                result = ex.ToString();
            }
            return custEntities;
        }

        public CustEnt[] getCustomentitiesbyPropName(VDF.Vault.Currency.Connections.Connection connection, string ObjType, string PropName, string srchop, string searchText)
        {
            try
            {
                //
                using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
                {
                    //get property details
                    var propDefs = getPropertyDefDetails(connection, ObjType, PropName);
                    result = connection.Vault.ToString();
                    DocumentService docServ = serviceManager.DocumentService;

                    SrchCond srchCond = new SrchCond()
                    {
                        PropDefId = propDefs.Id,
                        PropTyp = PropertySearchType.AllProperties,
                        SrchOper = SrchOperator(srchop), // is equal
                        SrchRule = SearchRuleType.Must,
                        SrchTxt = searchText.ToString()
                    };

                    //result = propDefs.Id.ToString();
                    string bookmark = string.Empty;
                    SrchStatus status = null;
                    custEntities = connection.WebServiceManager.CustomEntityService.FindCustomEntitiesBySearchConditions(new SrchCond[] { srchCond }, null, ref bookmark, out status);

                }
            }
            catch (SystemException ex)
            {
                result = ex.ToString();
            }
            return custEntities;
        }

        public CustEnt[] getCustomentitiesbyPropNameMulti(VDF.Vault.Currency.Connections.Connection connection, SrchCond[]  srchConds)
        {
            try
            {
                //
                using (WebServiceManager serviceManager = connection.WebServiceManager) //using will log out after usage
                {
                    string bookmark = string.Empty;
                    SrchStatus status = null;
                    custEntities = connection.WebServiceManager.CustomEntityService.FindCustomEntitiesBySearchConditions(srchConds, null, ref bookmark, out status);
                }
            }
            catch (SystemException ex)
            {
                result = ex.ToString();
            }
            return custEntities;
        }

        private static bool checkVaultDataType(DataType dataType, object value)
        {
            try
            {
                switch (dataType)
                {
                    case DataType.String:
                        {
                            boolresult = true;
                            break;
                        }
                    case DataType.Numeric:
                        {

                            break;
                        }
                    case DataType.DateTime:
                        {

                            break;
                        }
                    case DataType.Bool:
                        {

                            break;
                        }
                }
            }
            catch (SystemException ex)
            {
                result = ex.ToString();
            }

            return boolresult;
        }

        public long SrchOperator(string operator_val)
        {
            try
            {
                switch (operator_val)
                {
                    case "Contains":
                        {
                            longresult = 1;
                            break;
                        }
                    case "DoesNotContain":
                        {
                            longresult = 2;
                            break;
                        }
                    case "IsExactly":
                    case "=":
                        {
                            longresult = 3;
                            break;
                        }
                    case "IsEmpty":
                        {
                            longresult = 4;
                            break;
                        }
                    case "IsNotEmpty":
                        {
                            longresult = 5;
                            break;
                        }
                    case "GreaterThan":
                    case ">":
                        {
                            longresult = 6;
                            break;
                        }
                    case "GreaterThanOrEqualTo":
                    case ">=":
                        {
                            longresult = 7;
                            break;
                        }
                    case "LessThan":
                    case "<":
                        {
                            longresult = 8;
                            break;
                        }
                    case "LessThanOrEqualTo":
                    case "<=":
                        {
                            longresult = 9;
                            break;
                        }
                    case "NotEqualTo":
                    case "!=":
                        {
                            longresult = 10;
                            break;
                        }
                    default:
                        {
                            return 0;
                        }
                }
            }
            catch (SystemException ex)
            {
                result = ex.ToString();
            }
            return longresult;
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

        public static Dictionary<PropDef, object> getFilePropertiesDefs(VDF.Vault.Currency.Connections.Connection connection, File selectedFile )
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
