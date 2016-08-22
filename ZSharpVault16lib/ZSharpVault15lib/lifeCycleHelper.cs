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
using ZSharpVault16lib.Global;

namespace ZSharpVault16lib
{
    public class lifeCycleHelper
    {
        private static string result;

        #region File
        public static Dictionary<string, LfCycState> getFileLifeCycleStates(VDF.Vault.Currency.Connections.Connection connection, string filePath)
        {
            Dictionary<string, LfCycState> lifeCycleDict = new Dictionary<string, LfCycState>();
            try
            {
                File selectedFile = FileHelper.gefilebyfilePath(connection, filePath);
                if (selectedFile != null)
                {
                    if (selectedFile.FileLfCyc != null)
                    {
                        LfCycDef lifeCycleDef = connection.WebServiceManager.LifeCycleService.GetLifeCycleDefinitionsByIds(
                        selectedFile.FileLfCyc.LfCycDefId.ToSingleArray()).First();

                        lifeCycleDict = lifeCycleDef.StateArray.ToDictionary(n => n.DispName);
                    }
                    else
                    {
                        return null;
                    }
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
            return lifeCycleDict;
        }

        private static FileLfCyc resultLC;
        public static FileLfCyc getFileCurrentlifeCycle(VDF.Vault.Currency.Connections.Connection connection, string filePath)
        {
            try
            {
                File selectedFile = FileHelper.gefilebyfilePath(connection, filePath);
                if (selectedFile != null)
                {
                    if (selectedFile.FileLfCyc != null)
                    {
                        resultLC = selectedFile.FileLfCyc;
                    }
                    else
                    {
                        return null;
                    }
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
            return resultLC;
        }

        public static bool updateFileLifeCycle(VDF.Vault.Currency.Connections.Connection connection, string filePath, long stateID, string comment)
        {
            bool result;
            try
            {
                File selectedFile = FileHelper.gefilebyfilePath(connection, filePath);
                if (selectedFile.FileLfCyc != null)
                {
                    connection.WebServiceManager.DocumentServiceExtensions.UpdateFileLifeCycleStates(selectedFile.MasterId.ToSingleArray(), stateID.ToSingleArray(), (comment));
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
                //MessageBox.Show(ex.ToString());
                result = false;
            }
            return result;
        }
        #endregion

        #region Folder
        private static EntLfCyc resultFLC;
        public static EntLfCyc getFolderCurrentlifeCycle(VDF.Vault.Currency.Connections.Connection connection, string folderPath)
        {
            try
            {
                Autodesk.Connectivity.WebServices.Folder fld = FolderHelper.gefolderbyfolderPath(connection, folderPath);
                if (fld.LfCyc != null)
                {
                    resultFLC = fld.LfCyc;
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
            return resultFLC;
        }
        private static Dictionary<string, LfCycState> lifeCycleDict = new Dictionary<string, LfCycState>();
        public static Dictionary<string, LfCycState> getFolderLifeCycleStates(VDF.Vault.Currency.Connections.Connection connection, string folderPath)
        {
            try
            {
                Autodesk.Connectivity.WebServices.Folder fld = FolderHelper.gefolderbyfolderPath(connection, folderPath);
                if (fld.LfCyc != null)
                {
                    LfCycDef lifeCycleDef = connection.WebServiceManager.LifeCycleService.GetLifeCycleDefinitionsByIds(
                    fld.LfCyc.LfCycDefId.ToSingleArray()).First();

                    lifeCycleDict = lifeCycleDef.StateArray.ToDictionary(n => n.DispName);
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
            return lifeCycleDict;
        }

        
        public static bool updateFolderLifeCycle(VDF.Vault.Currency.Connections.Connection connection, string folderPath, long stateID, string comment)
        {
            bool result;
            try
            {
                Autodesk.Connectivity.WebServices.Folder fld = FolderHelper.gefolderbyfolderPath(connection, folderPath);
                if (fld.LfCyc != null)
                {
                    connection.WebServiceManager.DocumentServiceExtensions.UpdateFolderLifeCycleStates(fld.Id.ToSingleArray(), stateID.ToSingleArray(), (comment));
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
                //MessageBox.Show(ex.ToString());
                result = false;
            }
            return result;
        }
        #endregion

        #region ITEM
        private static EntLfCyc resultELC;
        public static EntLfCyc getEntityLifeCycleStates(VDF.Vault.Currency.Connections.Connection connection, string folderPath)
        {
            try
            {
                Autodesk.Connectivity.WebServices.Folder fld = FolderHelper.gefolderbyfolderPath(connection, folderPath);
                LfCycDef lifeCycleDef = connection.WebServiceManager.LifeCycleService.GetLifeCycleDefinitionsByIds(
                fld.LfCyc.LfCycDefId.ToSingleArray()).First();

                lifeCycleDict = lifeCycleDef.StateArray.ToDictionary(n => n.DispName);

            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
                //MessageBox.Show(ex.ToString());
            }
            return resultELC;
        }
        #endregion
    }

    public class TransItem
    {
        public LfCycTrans Transition { get; private set; }
        public LfCycState ToState { get; private set; }

        public TransItem(LfCycTrans trans, LfCycState toState)
        {
            this.Transition = trans;
            this.ToState = toState;
        }

        public override string ToString()
        {
            return ToState.DispName;
        }
    }
}
