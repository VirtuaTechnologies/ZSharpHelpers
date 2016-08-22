using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GV = ZSharpVault16lib.Global.variables;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace ZSharpVault16lib
{
    public class FolderStructureModel : INotifyPropertyChanged
    {
        FolderStructureModel(string name, string _folderPath, System.Windows.Media.Brush _folderExists, bool _folderCreationEnabled)
        {
            //string _nameTip;
            Name = name;
            folderPath = _folderPath;
            folderExists = _folderExists;
            folderCreationEnabled = _folderCreationEnabled;
            Children = new List<FolderStructureModel>();
        }

        #region Properties
        public string Name { get; private set; }
        public string folderPath { get; private set; }
        public bool folderCreationEnabled { get; private set; }
        public System.Windows.Media.Brush folderExists { get; private set; }
        public List<FolderStructureModel> Children { get; private set; }
        public bool IsInitiallySelected { get; private set; }
        bool? _isChecked = false;
        FolderStructureModel _parent;

        #region IsChecked
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetIsChecked(value, true, true); }
        }
        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked) return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue) Children.ForEach(c => c.SetIsChecked(_isChecked, true, false));

            if (updateParent && _parent != null) _parent.VerifyCheckedState();

            NotifyPropertyChanged("IsChecked");
        }
        void VerifyCheckedState()
        {
            bool? state = null;

            for (int i = 0; i < Children.Count; ++i)
            {
                bool? current = Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }

            SetIsChecked(state, false, true);
        }

        #endregion
        #endregion

        void Initialize()
        {
            foreach (FolderStructureModel child in Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        public static List<FolderStructureModel> SetTree(string topLevelName, string _folderPath, System.Windows.Media.Brush _folderExists, bool _folderCreationEnabled)
        {
            List<FolderStructureModel> treeView = new List<FolderStructureModel>();
            FolderStructureModel tv = new FolderStructureModel(topLevelName, _folderPath, _folderExists, _folderCreationEnabled);

            treeView.Add(tv);

            //add sub fodlers
            if (GV.selectedFodlerEntity.NumberOfChildren > 0)
            {
                foreach (VDF.Vault.Currency.Entities.Folder subFolder in GV.subFolders)
                {
                    string trimPath = subFolder.FolderPath.ToString().Remove(0, 2);
                    string path = GV.m_connection.WorkingFoldersManager.GetWorkingFolder("$") + trimPath + @"\" + subFolder.EntityName;
                    FolderStructureModel subFolder_Item;
                    string pathsubsubFinal = path.Replace(@"/", @"\");
                    if (Directory.Exists(pathsubsubFinal))
                    {
                        subFolder_Item = new FolderStructureModel(subFolder.EntityName, pathsubsubFinal, GV._brush_set_folder_exists, false);
                    }
                    else
                    {
                        subFolder_Item = new FolderStructureModel(subFolder.EntityName, pathsubsubFinal, GV._brush_set_folder_dontexists, true);
                    }

                    tv.Children.Add(subFolder_Item);

                    if (subFolder.FolderPath.ToString().Length > 3)
                    {

                        if (subFolder.NumberOfChildren > 0)
                        {
                            //get all the sub-sub folders.
                            //now add all sub-subs
                            IEnumerable<VDF.Vault.Currency.Entities.Folder> subFolders = GV.m_connection.FolderManager.GetChildFolders(subFolder, false, false);
                            foreach (VDF.Vault.Currency.Entities.Folder sub_subFolder in subFolders)
                            {
                                string trimPathsubsub = sub_subFolder.FolderPath.ToString().Remove(0, 2);
                                string pathsubsub = GV.m_connection.WorkingFoldersManager.GetWorkingFolder("$") + trimPathsubsub + @"\" + sub_subFolder.EntityName;
                                string pathsubsub_final = pathsubsub.Replace(@"/", @"\");
                                if (Directory.Exists(pathsubsub_final))
                                {
                                    subFolder_Item.Children.Add(new FolderStructureModel(sub_subFolder.EntityName.ToString(), pathsubsub_final, GV._brush_set_folder_exists, false));
                                }
                                else
                                {
                                    subFolder_Item.Children.Add(new FolderStructureModel(sub_subFolder.EntityName.ToString(), pathsubsub_final, GV._brush_set_folder_dontexists, true));
                                }

                            }
                        }
                        else
                        {

                        }
                    }
                }
            }
            //Perform recursive method to build treeview 

            #region Test Data
            //Doing this below for this example, you should do it dynamically 
            /***************************************************
            FolderStructureModel tvChild4 = new FolderStructureModel("Child4");

            tv.Children.Add(new FolderStructureModel("Child1"));
            tv.Children.Add(new FolderStructureModel("Child2"));
            tv.Children.Add(new FolderStructureModel("Child3"));
            tv.Children.Add(tvChild4);
            tv.Children.Add(new FolderStructureModel("Child5"));

            FolderStructureModel grtGrdChild2 = (new FolderStructureModel("GrandChild4-2"));

            tvChild4.Children.Add(new FolderStructureModel("GrandChild4-1"));
            tvChild4.Children.Add(grtGrdChild2);
            tvChild4.Children.Add(new FolderStructureModel("GrandChild4-3"));

            grtGrdChild2.Children.Add(new FolderStructureModel("GreatGrandChild4-2-1"));
            */
            #endregion

            tv.Initialize();

            return treeView;
        }

        public static void GetCheckedItems(FolderStructureModel node)
        {
            if (node.IsChecked == true && node.folderExists == GV._brush_set_folder_dontexists)
            {
                GV.folderList.Add(node);
            }
            foreach (var item in node.Children)
            {
                if (item.IsChecked == true && item.folderExists == GV._brush_set_folder_dontexists)
                {
                    GV.folderList.Add(item);
                    //MessageBox.Show(item.folderPath, "1");
                    //lView.Items.Add(item.Name + item.folderPath);
                }
                foreach (var item2 in item.Children)
                {
                    if (item2.IsChecked == true && item2.folderExists == GV._brush_set_folder_dontexists)
                    {
                        //MessageBox.Show(item2.folderPath, "2");
                        GV.folderList.Add(item2);
                        //lView.Items.Add(item2.Name + "|| " + item2.folderPath);
                    }
                    foreach (var item3 in item2.Children)
                    {
                        if (item3.IsChecked == true && item3.folderExists == GV._brush_set_folder_dontexists)
                        {
                            //MessageBox.Show(item3.folderPath, "3");
                            GV.folderList.Add(item3);
                        }
                    }
                }
            }

        }

        #region INotifyPropertyChanged Members

        void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }
}
