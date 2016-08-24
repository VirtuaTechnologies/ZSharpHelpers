using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq;
using System.Text;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using VDF = Autodesk.DataManagement.Client.Framework;
using VHPH = ZSharpVault16lib.PropertyHelper;
using VHGH = ZSharpVault16lib.GenVaultHelper;
using VHFH = ZSharpVault16lib.FileHelper;
using TH = ZSharpTextHelper.GenTextHelper;
using GV = Vault16Tester.Global.variables;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.Web.Services.Protocols;
using Autodesk.Connectivity.WebServices;
using VDFVF = Autodesk.DataManagement.Client.Framework.Vault.Forms;
using VDFVCP = Autodesk.DataManagement.Client.Framework.Vault.Currency.Properties;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using System.IO;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using Autodesk.Connectivity.Explorer.ExtensibilityTools;
using VHPFH = ZSharpVault16lib.FilePropertiesHelper;
using ZSharpVault16lib;
using VHFLDH = ZSharpVault16lib.FolderHelper;
using VHLH = ZSharpVault16lib.lifeCycleHelper;
using System.ComponentModel;
using VaultAPI.Data;

namespace Vault16Tester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public static VHPH vaultPropHelper = new VHPH();
        public static VHGH genVaultHelper = new VHGH();

        public MainWindow()
        {
            InitializeComponent();
            GV.objType.Add("File");
            GV.objType.Add("Item");
            GV.objType.Add("Change Order");
            GV.objType.Add("All");
            cBox_obj_Type.ItemsSource = GV.objType;

        }

        #region General
        private async void btn_login_Click(object sender, RoutedEventArgs e)
        {
            //loginBOX(null, null);
            var controller = await this.ShowProgressAsync("TESTER", "Login into Vault...");

            controller.SetIndeterminate();

            GV.server = tBox_SERVER.Text;
            GV.vaultStore = tBox_vaultDS.Text;
            GV.username = tBox_uName.Text;
            GV.pass = tBox_password.Text;
            VHPH vaultPropHelper = new VHPH();

            GV.connection = VDF.Vault.Library.ConnectionManager.LogIn(GV.server, GV.vaultStore, GV.username, GV.pass, VDF.Vault.Currency.Connections.AuthenticationFlags.Standard, null).Connection;//VHPH.getVaultConnection(ip, vaultInstance, userName, pass);
            //GV.connection = vaultPropHelper.getpsVaultConn(GV.server, GV.vaultStore, GV.username, GV.pass);
            if (GV.connection.IsConnected)
            {
                btn_Login.Content = "ONLINE";
            }
            else
            {
                btn_Login.Content = "OFFLINE";
            }

            await controller.CloseAsync();
            await this.ShowMessageAsync("Success!", "Logged into Vault successfully.");
            /**/
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_clearlogs_Click(object sender, RoutedEventArgs e)
        {
            lView_Log.Items.Clear();
        }

        public static T FindChild<T>(DependencyObject parent, string childName)
            where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        private static string copy;
        private void btn_Copy_Click(object sender, RoutedEventArgs e)
        {

            foreach (var item in lView_Log.SelectedItems)
            {
                Clipboard.SetText(item.ToString());
                copy += item.ToString() + "\n";
                Debug.Write(item.ToString() + "\n");
            }


        }

        public async void pBAR(string title, string message)
        {
            var controller = await this.ShowProgressAsync(title, message);
        }

        public async void loginBOX(object sender, RoutedEventArgs e)
        {

            this.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            var dialog = (BaseMetroDialog)this.Resources["CustomDialogTest"];

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Hi",
                AnimateShow = true,
                NegativeButtonText = "Go away!",
                FirstAuxiliaryButtonText = "Cancel",
                ColorScheme = MetroDialogColorScheme.Accented
            };

            await this.ShowMetroDialogAsync(dialog, mySettings);
            // await this.HideMetroDialogAsync(dialog,mySettings); --> this for close the dialog

        }

        private async void ShowLoginDialog(object sender, RoutedEventArgs e)
        {
            var metroDialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "OK",
                NegativeButtonText = "CANCEL",
                AnimateHide = true,
                AnimateShow = true,
                ColorScheme = MetroDialogColorScheme.Accented,
            };

            LoginDialogData result = await this.ShowLoginAsync("Authentication", "Enter your credentials", new LoginDialogSettings { ColorScheme = this.MetroDialogOptions.ColorScheme, InitialUsername = "MahApps", });
            if (result == null)
            {
                //User pressed cancel
            }
            else
            {
                MessageDialogResult messageResult = await this.ShowMessageAsync("Authentication Information", String.Format("Username: {0}\nPassword: {1}", result.Username, result.Password));
            }
        }
        #endregion

        #region Custom Objects
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {


            GV.custEntities = vaultPropHelper.getCustomentitiesbyPropName(GV.connection, "CUSTENT", "Name", "3", "OBJNAME11");
            GV.dataTable = new DataTable();
            GV.dataTable.Clear();
            List<string> properties = new System.Collections.Generic.List<string>();
            //properties.Add("Name");
            //properties.Add("Client");
            //properties.Add("Create Date");
            //properties.Add("Created By");
            //properties.Add("Current Owner");
            GV.dataTable = vaultPropHelper.getCENTCollPropertyValinDataTable(GV.connection, "CUSTENT", GV.custEntities, properties);
            dGrid_CustEntities.ItemsSource = GV.dataTable.DefaultView;

            dGrid_CustEntities.AutoGenerateColumns = true;
        }

        private void btn_fetch_multi_Click(object sender, RoutedEventArgs e)
        {
            VHPH vaultPropHelper = new VHPH();
            GV.connection = vaultPropHelper.getpsVaultConn(GV.server, GV.vaultStore, GV.username, GV.pass);

            var propDef = VHPH.getPropertyDefDetails(GV.connection, "CUSTENT", "Client");
            SrchCond srchCond1 = new SrchCond()
            {
                PropDefId = propDef.Id,
                PropTyp = PropertySearchType.AllProperties,
                SrchOper = vaultPropHelper.SrchOperator("IsExactly"), // is equal
                SrchRule = SearchRuleType.Must,
                SrchTxt = "abc46"
            };

            propDef = VHPH.getPropertyDefDetails(GV.connection, "CUSTENT", "Name");
            SrchCond srchCond2 = new SrchCond()
            {
                PropDefId = propDef.Id,
                PropTyp = PropertySearchType.AllProperties,
                SrchOper = vaultPropHelper.SrchOperator("IsExactly"), // is equal
                SrchRule = SearchRuleType.Must,
                SrchTxt = "OBJNAME11"
            };

            SrchCond[] srchConditions = { srchCond1, srchCond2 };

            GV.custEntities = vaultPropHelper.getCustomentitiesbyPropNameMulti(GV.connection, srchConditions);
            GV.dataTable = new DataTable();
            GV.dataTable.Clear();
            List<string> properties = new System.Collections.Generic.List<string>();
            //properties.Add("Name");
            //properties.Add("Client");
            //properties.Add("Create Date");
            //properties.Add("Created By");
            //properties.Add("Current Owner");
            GV.dataTable = vaultPropHelper.getCENTCollPropertyValinDataTable(GV.connection, "CUSTENT", GV.custEntities, properties);
            dGrid_CustEntities.ItemsSource = GV.dataTable.DefaultView;

            dGrid_CustEntities.AutoGenerateColumns = true;
        }
        #endregion

        #region Num Schemes
        private static NumSchm[] numS;
        private void btn_get_NumSchemes_Click(object sender, RoutedEventArgs e)
        {
            if (cBox_obj_Type.SelectedItem.Equals("Item"))
            {
                ItemService iServ = GV.connection.WebServiceManager.ItemService;

                numS = iServ.GetNumberingSchemesByType(NumSchmType.Activated);
                foreach (var ns in numS)
                {
                    cBox_NumberingSchemes.Items.Add(ns.Name);
                }
            }
            else if (cBox_obj_Type.SelectedItem.Equals("File"))
            {
                WebServiceManager wServ = GV.connection.WebServiceManager;
                numS = wServ.DocumentService.GetNumberingSchemesByType(NumSchmType.Activated);
                foreach (var ns in numS)
                {
                    cBox_NumberingSchemes.Items.Add(ns.Name);
                }
            }
            else if (cBox_obj_Type.SelectedItem.Equals("Change Order"))
            {

            }

            /**/
        }
        private static NumSchm NS;
        private static Dictionary<string, string> UIControlsColl = new System.Collections.Generic.Dictionary<string, string>();
        private static int UINameInc = 0;
        private void cBox_NumberingSchemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var ns in numS)
            {
                if (ns.Name.Equals(cBox_NumberingSchemes.SelectedItem.ToString()))
                {
                    lbl_sDetails.Content = ns.IsAct.ToString() + " | " + ns.IsDflt.ToString() + " | " + ns.IsInUse + " | " + ns.IsSys + " | " + ns.SchmID + " | " + ns.FieldArray.ToString();
                    foreach (var item in ns.FieldArray)
                    {

                        lView_Log.Items.Add(item.FieldTyp + " | " + item.Name + "| " + item.GetHashCode() + "| " + item.FieldTyp.GetTypeCode() + "| ");
                        System.Windows.Controls.Label lbl = new System.Windows.Controls.Label();
                        lbl.Name = "lbl_" + item.FieldTyp.ToString().Replace(" ", string.Empty);
                        lbl.Content = item.FieldTyp;
                        lbl.Foreground = Brushes.Black;
                        stackPanel_NumSchemes.Children.Add(lbl);
                        stackPanel_NumSchemes.CanVerticallyScroll = true;

                        if (item.FieldTyp.ToString().Equals("PredefinedList"))
                        {
                            PredefListField pdlf = (PredefListField)item;
                            System.Windows.Controls.ComboBox cBOX = new System.Windows.Controls.ComboBox();
                            if (UIControlsColl.ContainsKey("cBox_" + UINameInc))
                            {
                                UINameInc += 1;
                                cBOX.Name = "cBox_" + UINameInc;
                            }
                            else
                            {
                                cBOX.Name = "cBox_" + UINameInc;
                            }


                            foreach (var ca in pdlf.CodeArray)
                            {
                                cBOX.Items.Add(ca.Code);
                            }
                            cBOX.SelectedIndex = 0;
                            stackPanel_NumSchemes.Children.Add(cBOX);

                            Binding mybnd = new Binding();
                            mybnd.Source = cBOX.SelectedItem;
                            BindingOperations.SetBinding(tBlock_num, TextBlock.TextProperty, mybnd);
                            UIControlsColl.Add(cBOX.Name, "ComboBox," + "PredefinedList");
                        }
                        else if (item.FieldTyp.ToString().Equals("FreeText"))
                        {
                            FreeTxtField ftf = (FreeTxtField)item;
                            System.Windows.Controls.TextBox tBOX = new System.Windows.Controls.TextBox();
                            if (UIControlsColl.ContainsKey("tBox_" + UINameInc))
                            {
                                UINameInc += 1;
                                tBOX.Name = "tBox_" + UINameInc;
                            }
                            else
                            {
                                tBOX.Name = "tBox_" + UINameInc;
                            }
                            tBOX.Text = ftf.DfltVal;
                            stackPanel_NumSchemes.Children.Add(tBOX);
                            UIControlsColl.Add(tBOX.Name, "TextBox," + "FreeText");
                        }
                        else if (item.FieldTyp.ToString().Equals("Delimiter"))
                        {
                            DelimField dlf = (DelimField)item;
                            System.Windows.Controls.TextBlock tBlock = new System.Windows.Controls.TextBlock();
                            if (UIControlsColl.ContainsKey("tBlock_" + UINameInc))
                            {
                                UINameInc += 1;
                                tBlock.Name = "tBlock_" + UINameInc;
                            }
                            else
                            {
                                tBlock.Name = "tBlock_" + UINameInc;
                            }
                            tBlock.Foreground = Brushes.Black;
                            tBlock.Text = dlf.DelimVal.ToString();
                            //tBlock.IsReadOnly = true;
                            stackPanel_NumSchemes.Children.Add(tBlock);
                            UIControlsColl.Add(tBlock.Name, "TextBlock," + "Delimiter");
                        }
                        else if (item.FieldTyp.ToString().Equals("FixedText"))
                        {
                            FixedTxtField ftf = (FixedTxtField)item;
                            System.Windows.Controls.TextBlock tBlock = new System.Windows.Controls.TextBlock();
                            if (UIControlsColl.ContainsKey("tBlock_" + UINameInc))
                            {
                                UINameInc += 1;
                                tBlock.Name = "tBlock_" + UINameInc;
                            }
                            else
                            {
                                tBlock.Name = "tBlock_" + UINameInc;
                            }
                            tBlock.Foreground = Brushes.Black;
                            tBlock.Text = ftf.FixedTxtVal.ToString();
                            //tBlock.IsReadOnly = true;
                            stackPanel_NumSchemes.Children.Add(tBlock);
                            UIControlsColl.Add(tBlock.Name, "TextBlock," + "FixedText");
                        }
                        else if (item.FieldTyp.ToString().Equals("WorkgroupLabel"))
                        {
                            WkgrpLabelField wgl = (WkgrpLabelField)item;
                            System.Windows.Controls.TextBlock tBlock = new System.Windows.Controls.TextBlock();
                            if (UIControlsColl.ContainsKey("tBlock_" + UINameInc))
                            {
                                UINameInc += 1;
                                tBlock.Name = "tBlock_" + UINameInc;
                            }
                            else
                            {
                                tBlock.Name = "tBlock_" + UINameInc;
                            }
                            tBlock.Foreground = Brushes.Black;
                            tBlock.Text = wgl.Val.ToString();
                            //tBlock.IsReadOnly = true;
                            stackPanel_NumSchemes.Children.Add(tBlock);
                            UIControlsColl.Add(tBlock.Name, "TextBlock," + "WorkgroupLabel");
                        }
                        else if (item.FieldTyp.ToString().Equals("Autogenerated"))
                        {
                            ItemService iServ = GV.connection.WebServiceManager.ItemService;

                            AutogenField agf = (AutogenField)item;
                            System.Windows.Controls.TextBlock tBlock = new System.Windows.Controls.TextBlock();
                            if (UIControlsColl.ContainsKey("tBlock_" + UINameInc))
                            {
                                UINameInc += 1;
                                tBlock.Name = "tBlock_" + UINameInc;
                            }
                            else
                            {
                                tBlock.Name = "tBlock_" + UINameInc;
                            }
                            tBlock.Foreground = Brushes.Black;
                            KnowledgeVaultService kvs = GV.connection.WebServiceManager.KnowledgeVaultService;
                            //kvs.GetVaultOption(
                            //tBOX.Text = iServ.//agf..To.ToString();
                            //tBOX.IsReadOnly = true;

                            stackPanel_NumSchemes.Children.Add(tBlock);
                            UIControlsColl.Add(tBlock.Name, "TextBlock," + "Autogenerated");
                        }
                        else
                        {
                            lView_Log.Items.Add(item.FieldTyp + " --");
                        }
                    }
                    NS = ns;
                }
            }
        }

        private static TextBlock TextBlock_obj;
        private static TextBox TextBox_obj;
        private static ComboBox ComboBox_obj;
        private static List<string> fldinputs = new System.Collections.Generic.List<string>();
        private void btn_getNum_Click(object sender, RoutedEventArgs e)
        {
            lView_Log.Items.Add("NS Details: " + NS.SchmID + " | " + NS.Name + " | ");
            string childname = "no name";
            string UIObjName = "no name";
            fldinputs.Clear();
            foreach (object child in stackPanel_NumSchemes.Children)//.GetChildObjects(false))
            {
                if (child is FrameworkElement)
                {
                    childname = (child as FrameworkElement).Name;

                    //lView_Log.Items.Add("== " + childname + " | " );
                    UIObjName = TH.Split_csv_get_specific_dl(childname, 1, '_');
                    switch (UIObjName)//item.DependencyObjectType.Name.ToString()
                    {
                        case "tBlock":
                            {
                                TextBlock_obj = FindChild<TextBlock>(Application.Current.MainWindow, childname);
                                lView_Log.Items.Add("UIObj: " + childname + "Val == >> " + TextBlock_obj.Text);
                                //fldinputs.Add(TextBlock_obj.Text);
                                break;
                            }
                        case "cBox":
                            {
                                ComboBox_obj = FindChild<ComboBox>(Application.Current.MainWindow, childname);
                                lView_Log.Items.Add("UIObj: " + childname + "Val == >> " + ComboBox_obj.SelectedItem);
                                fldinputs.Add(ComboBox_obj.SelectedItem.ToString());
                                break;
                            }
                        case "tBox":
                            {
                                TextBox_obj = FindChild<TextBox>(Application.Current.MainWindow, childname);
                                lView_Log.Items.Add("UIObj: " + childname + "Val == >> " + TextBox_obj.Text);
                                fldinputs.Add(TextBox_obj.Text);
                                break;
                            }

                    }
                }

                if (true)//!item.DependencyObjectType.Name.ToString().Equals("Label"))
                {

                    //lView_Log.Items.Add("== " + item.DependencyObjectType.BaseType.ToString() + " | " + item.DependencyObjectType.Name.ToString() + " | " + " | " item.DependencyObjectType.Id.ToString()  );
                }
            }
            foreach (string item in fldinputs.ToArray())
            {
                lView_Log.Items.Add("----FLD : " + item);
            }
            //string[] fInp = new string[] {"FREETXT", "dwg", "" };
            WebServiceManager wServ = GV.connection.WebServiceManager;
            string NUMVAL = wServ.DocumentService.GenerateFileNumber(NS.SchmID, fldinputs.ToArray());
            lView_Log.Items.Add("GENNUM >>> " + NUMVAL);
        }

        #endregion

        #region Item Master
        private void btn_getItemMasterCategories_Click(object sender, RoutedEventArgs e)
        {
            /**/
            Style style = new Style(typeof(ComboBox));

            //set up the stack panel
            FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(StackPanel));
            spFactory.Name = "myComboFactory";
            spFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            var d = new DataTemplate();

            FrameworkElementFactory blankSpace = new FrameworkElementFactory(typeof(Rectangle));
            blankSpace.SetValue(Rectangle.WidthProperty, (double)15);
            spFactory.AppendChild(blankSpace);

            FrameworkElementFactory rec = new FrameworkElementFactory(typeof(Rectangle));
            rec.SetValue(Rectangle.WidthProperty, (double)15);
            rec.SetValue(Rectangle.HeightProperty, (double)15);
            //SolidColorBrush trnsRedBrush = new SolidColorBrush(Colors.Blue);
            //rec.SetValue(Rectangle.FillProperty, trnsRedBrush);
            rec.SetBinding(Rectangle.FillProperty, new Binding("ColorCode"));
            spFactory.AppendChild(rec);

            MultiBinding mb = new MultiBinding();
            mb.StringFormat = "   {1}";//"{0} | {1}";
            mb.Bindings.Add(new Binding("CatID"));
            mb.Bindings.Add(new Binding("CATName"));
            FrameworkElementFactory textElement = new FrameworkElementFactory(typeof(TextBlock));
            textElement.SetBinding(TextBlock.TextProperty, mb);
            spFactory.AppendChild(textElement);


            d.VisualTree = spFactory;
            style.Setters.Add(new Setter(ComboBox.ItemTemplateProperty, d));
            this.Resources.Add("cBox_ItemMasterCategoryList", style);

            cBox_ItemMasterCategoryList.Style = style;

            List<Global.CategoryData> CDLIst = new System.Collections.Generic.List<Global.CategoryData>();


            CatCfg[] categories = GV.connection.WebServiceManager.CategoryService.GetCategoryConfigurationsByBehaviorNames(VDF.Vault.Currency.Entities.EntityClassIds.Items, false, new[] { "Category" });
            foreach (var cat in categories)
            {
                //ComboBoxItem cbi = new ComboBoxItem();
                Global.CategoryData CD = new Global.CategoryData(cat.Cat.Name, cat.Cat.Descr, cat.Cat.SysName, cat.Cat.Color, cat.Cat.Id);
                CDLIst.Add(CD);
                //cbi.
                //cBox_ItemMasterCategoryList.Items.Add(cat.Cat.Name.ToString());
            }

            cBox_ItemMasterCategoryList.ItemsSource = CDLIst;

        }

        private void btn_GetItemNumSchemes_Click(object sender, RoutedEventArgs e)
        {
            Style NSstyle = new Style(typeof(ComboBox));

            //set up the stack panel
            FrameworkElementFactory NSspFactory = new FrameworkElementFactory(typeof(StackPanel));
            NSspFactory.Name = "NSComboFactory";
            NSspFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            var NSd = new DataTemplate();

            FrameworkElementFactory NSblankSpace = new FrameworkElementFactory(typeof(Rectangle));
            NSblankSpace.SetValue(Rectangle.WidthProperty, (double)15);
            NSspFactory.AppendChild(NSblankSpace);

            MultiBinding NSmb = new MultiBinding();
            NSmb.StringFormat = "{0} | {1}";// "{0}";//
            NSmb.Bindings.Add(new Binding("Name"));
            NSmb.Bindings.Add(new Binding("SchemeID"));
            FrameworkElementFactory textElement = new FrameworkElementFactory(typeof(TextBlock));
            textElement.SetBinding(TextBlock.TextProperty, NSmb);
            NSspFactory.AppendChild(textElement);

            NSd.VisualTree = NSspFactory;
            NSstyle.Setters.Add(new Setter(ComboBox.ItemTemplateProperty, NSd));
            this.Resources.Clear();
            this.Resources.Add("cBox_ItemNumScheme", NSstyle);

            cBox_ItemNumScheme.Style = NSstyle;
            ItemService iServ = GV.connection.WebServiceManager.ItemService;
            GV.NSLIst.Clear();
            numS = iServ.GetNumberingSchemesByType(NumSchmType.Activated);
            foreach (var ns in numS)
            {
                lView_Log.Items.Add(ns.Name);
                Global.NumSchemes NS = new Global.NumSchemes(ns.FieldArray, ns.IsAct, ns.IsDflt, ns.IsInUse, ns.IsSys, ns.Name, ns.SysName, ns.SchmID);
                GV.NSLIst.Add(ns.Name, NS);
            }
            cBox_ItemNumScheme.ItemsSource = GV.NSLIst.Values;
        }

        public void AddItem(WebServiceManager serviceManager)
        {
            Cat[] categories = serviceManager.CategoryService.GetCategoriesByEntityClassId("ITEM", true);
            long catId = -1;
            foreach (Cat category in categories)
            {
                if (category.SysName == "Document")
                    catId = category.Id;
            }

            Item item = serviceManager.ItemService.AddItemRevision(catId);

            // set the needed information
            item.Title = "Test Item";
            item.Detail = "Test Item";

            // save the item revision
            Item[] itemarr = { item };
            serviceManager.ItemService.UpdateAndCommitItems(itemarr);
        }


        public static string GetErrorCodeString(Exception e)
        {
            SoapException se = e as SoapException;
            if (se != null)
            {
                try
                {
                    return se.Detail["sl:sldetail"]["sl:errorcode"].InnerText.Trim();
                }
                catch
                { }
            }
            return null;
        }

        private static StringArray[] ItemNSFieldInputs;
        private static string NUMVAL;
        private string getItemNum()
        {
            try
            {
                //lView_Log.Items.Add("NS Details: " + NS.SchmID + " | " + NS.Name + " | ");
                string childname = "no name";
                string UIObjName = "no name";
                int index = 0;

                #region get array size
                foreach (object child in sPanel_IM_NScheme.Children)//.GetChildObjects(false))
                {

                    if (child is FrameworkElement)
                    {
                        childname = (child as FrameworkElement).Name;

                        lView_Log.Items.Add("== " + childname + " | ");
                        UIObjName = TH.Split_csv_get_specific_dl(childname, 1, '_');
                        switch (UIObjName)//item.DependencyObjectType.Name.ToString()
                        {
                            case "tBlock":
                                {
                                    //index++;
                                    break;
                                }
                            case "cBox":
                                {
                                    //index++;
                                    break;
                                }
                            case "tBox":
                                {
                                    //index++;
                                    break;
                                }
                            case "tBlockAutogenerated":
                                {
                                    index++;
                                    break;
                                }

                        }
                    }

                    if (true)//!item.DependencyObjectType.Name.ToString().Equals("Label"))
                    {

                        // lView_Log.Items.Add("== " + item.DependencyObjectType.BaseType.ToString() + " | " + item.DependencyObjectType.Name.ToString() + " | " + " | " item.DependencyObjectType.Id.ToString()  );
                    }
                }
                #endregion

                ItemNSFieldInputs = new StringArray[index];
                index = 0;
                StringArray tempArr;
                #region NUM GEN
                foreach (object child in sPanel_IM_NScheme.Children)//.GetChildObjects(false))
                {
                    tempArr = null;
                    string[] newItemNum;
                    if (child is FrameworkElement)
                    {
                        childname = (child as FrameworkElement).Name;

                        lView_Log.Items.Add("== " + childname + " | ");
                        UIObjName = TH.Split_csv_get_specific_dl(childname, 1, '_');
                        switch (UIObjName)//item.DependencyObjectType.Name.ToString()
                        {
                            case "tBlock":
                                {
                                    TextBlock_obj = FindChild<TextBlock>(Application.Current.MainWindow, childname);
                                    //MessageBox.Show(TextBlock_obj.Text + " | " + UIControlsColl[childname]);
                                    lView_Log.Items.Add("UIObj: " + childname + "Val == >> " + TextBlock_obj.Text + " | " + UIControlsColl[childname]);
                                    //lView_Log.Items.Add("UIObj: " + childname + "Val == >> " + TextBlock_obj.Text);
                                    //fldinputs.Add(TextBlock_obj.Text);
                                    /*
                                    newItemNum = new string[] { TextBlock_obj.Text };
                                    tempArr = new StringArray();
                                    tempArr.Items = newItemNum;
                                    ItemNSFieldInputs[index] = tempArr;
                                    index++;
                                     * */
                                    break;
                                }
                            case "cBox":
                                {
                                    ComboBox_obj = FindChild<ComboBox>(Application.Current.MainWindow, childname);
                                    lView_Log.Items.Add("UIObj: " + childname + "Val == >> " + ComboBox_obj.SelectedItem + " | " + UIControlsColl[childname]);
                                    //fldinputs.Add(ComboBox_obj.SelectedItem.ToString());
                                    /*
                                    newItemNum = new string[] { ComboBox_obj.SelectedItem.ToString() };
                                    tempArr = new StringArray();
                                    tempArr.Items = newItemNum;
                                    ItemNSFieldInputs[index] = tempArr;
                                    index++;
                                    */
                                    break;
                                }
                            case "tBox":
                                {
                                    TextBox_obj = FindChild<TextBox>(Application.Current.MainWindow, childname);
                                    lView_Log.Items.Add("UIObj: " + childname + "Val == >> " + TextBox_obj.Text + " | " + UIControlsColl[childname]);

                                    //fldinputs.Add();
                                    /*
                                    newItemNum = new string[] { TextBox_obj.Text };
                                    tempArr = new StringArray();
                                    tempArr.Items = newItemNum;
                                    ItemNSFieldInputs[index] = tempArr;
                                    index++;
                                     */
                                    break;
                                }
                            case "tBlockAutogenerated":
                                {
                                    TextBlock_obj = FindChild<TextBlock>(Application.Current.MainWindow, childname);
                                    lView_Log.Items.Add("UIObj: " + childname + "Val == >> " + TextBlock_obj.Text);
                                    //fldinputs.Add("");

                                    newItemNum = new string[] { "" };
                                    tempArr = new StringArray();
                                    tempArr.Items = newItemNum;
                                    ItemNSFieldInputs[index] = tempArr;
                                    index++;
                                    /* */
                                    break;
                                }

                        }
                    }

                    if (true)//!item.DependencyObjectType.Name.ToString().Equals("Label"))
                    {

                        // lView_Log.Items.Add("== " + item.DependencyObjectType.BaseType.ToString() + " | " + item.DependencyObjectType.Name.ToString() + " | " + " | " item.DependencyObjectType.Id.ToString()  );
                    }
                }
                #endregion

                //string[] fInp = new string[] {"FREETXT", "dwg", "" };
                //GV.wServ = GV.connection.WebServiceManager;
                //NUMVAL = GV.wServ.DocumentService.GenerateFileNumber(NS.SchmID, fldinputs.ToArray());
                //lView_Log.Items.Add("----NVAL : " + NUMVAL);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return NUMVAL;
        }

        private static Dictionary<long, List<string>> propertyItems = new System.Collections.Generic.Dictionary<long, System.Collections.Generic.List<string>>();
        private void btn_GetProperties_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sPanel_Properties.Children.Clear();
                using (GV.wServ = GV.connection.WebServiceManager)
                {
                    //lView_Log.Items.Clear();
                    var cd = (Global.CategoryData)cBox_ItemMasterCategoryList.SelectedItem;
                    ItemService IS = GV.wServ.ItemService;
                    var item = IS.AddItemRevision(cd.CatID);
                    mID = new[] { item.MasterId };
                    Dictionary<long, PropDef> props = VHPH.getPropertiesbyClass(GV.connection, "ITEM");
                    AssocPropDef[] ps = GV.wServ.PropertyService.GetAssociationPropertyDefinitionsByType(AssocPropTyp.ChangeOrderItem);//GetAllAssociationPropertyDefinitionInfos(AssocPropTyp.ItemBOMAssoc);
                    List<long> defIDS = new System.Collections.Generic.List<long>();
                    foreach (var psitem in props)
                    {
                        //lView_Log.Items.Add("ITEM Props: " + psitem.Value.DispName + " | " + psitem.Value.Id + " | " + psitem.Value.IsSys);
                        if (psitem.Value.IsSys == false)
                        {
                            if (psitem.Value.DispName.Equals("GPC DWGNO") || psitem.Value.DispName.Equals("GPC Title 4") || psitem.Value.DispName.Equals("GPC Title 5"))
                            {
                                lView_Log.Items.Add("ITEM Props: " + psitem.Value.DispName + " | " + psitem.Value.Id + " | " + psitem.Value.IsSys);
                                //defIDS.Add(psitem.Value.Id);
                                System.Windows.Controls.Label lbl = new System.Windows.Controls.Label();
                                lbl.Name = "lbl_" + psitem.Value.Id;
                                lbl.Width = 200;
                                lbl.Content = psitem.Value.DispName;

                                System.Windows.Controls.TextBox tBOX = new System.Windows.Controls.TextBox();
                                tBOX.Name = "tBox_" + psitem.Value.Id;
                                tBOX.Width = 300;
                                System.Windows.Controls.StackPanel sPANEL = new StackPanel { Orientation = Orientation.Horizontal };
                                sPANEL.Name = "sPanel_" + psitem.Value.Id;
                                sPANEL.Children.Add(lbl);
                                sPANEL.Children.Add(tBOX);
                                List<string> uiITEMS = new System.Collections.Generic.List<string>();
                                uiITEMS.Add("sPanel_" + psitem.Value.Id);
                                uiITEMS.Add("tBox_" + psitem.Value.Id);
                                propertyItems.Add(psitem.Value.Id, uiITEMS);
                                sPanel_Properties.Children.Add(sPANEL);
                            }
                        }
                    }


                    //MessageBox.Show(item.MasterId.ToString());
                    /*
                     long[] mid = {8825 };
                    PropInst[] ITEMprops = GV.connection.WebServiceManager.PropertyService.GetProperties("ITEM", mid, defIDS.ToArray());
                    foreach (var proITem in ITEMprops)
                    {
                        lView_Log.Items.Add("ITEM Props: " + proITem.EntityId + " | " + proITem.PropDefId.ToString() + " | " + proITem.Val + " | " + proITem.ValTyp);
                    }
                    */
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void cBox_ItemNumScheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UIControlsColl.Clear();
            Global.NumSchemes NS = (Global.NumSchemes)cBox_ItemNumScheme.SelectedItem;
            lView_Log.Items.Add(NS.Name + NS.SchemeID + NS.ISSys);
            sPanel_IM_NScheme.Children.Clear();
            foreach (var item in NS.FieldArray)
            {
                lView_Log.Items.Add(item.FieldTyp + " | " + item.Name + "| " + item.GetHashCode() + "| " + item.FieldTyp.GetTypeCode() + "| ");
                System.Windows.Controls.Label lbl = new System.Windows.Controls.Label();
                lbl.Name = "lbl_" + item.FieldTyp.ToString().Replace(" ", string.Empty);
                lbl.Content = item.FieldTyp;
                lbl.Foreground = Brushes.Black;
                sPanel_IM_NScheme.Children.Add(lbl);
                sPanel_IM_NScheme.CanVerticallyScroll = true;

                #region PredefinedList
                if (item.FieldTyp.ToString().Equals("PredefinedList"))
                {
                    PredefListField pdlf = (PredefListField)item;
                    System.Windows.Controls.ComboBox cBOX = new System.Windows.Controls.ComboBox();
                    if (UIControlsColl.ContainsKey("cBox_" + UINameInc))
                    {
                        UINameInc += 1;
                        cBOX.Name = "cBox_" + UINameInc;
                    }
                    else
                    {
                        cBOX.Name = "cBox_" + UINameInc;
                    }


                    foreach (var ca in pdlf.CodeArray)
                    {
                        cBOX.Items.Add(ca.Code);
                    }
                    cBOX.SelectedIndex = 0;
                    sPanel_IM_NScheme.Children.Add(cBOX);

                    Binding mybnd = new Binding();
                    mybnd.Source = cBOX.SelectedItem;
                    //BindingOperations.SetBinding(tBlock_num, TextBlock.TextProperty, mybnd);
                    UIControlsColl.Add(cBOX.Name, "ComboBox," + "PredefinedList");
                }
                #endregion

                #region FreeText
                else if (item.FieldTyp.ToString().Equals("FreeText"))
                {
                    FreeTxtField ftf = (FreeTxtField)item;
                    System.Windows.Controls.TextBox tBOX = new System.Windows.Controls.TextBox();
                    if (UIControlsColl.ContainsKey("tBox_" + UINameInc))
                    {
                        UINameInc += 1;
                        tBOX.Name = "tBox_" + UINameInc;
                    }
                    else
                    {
                        tBOX.Name = "tBox_" + UINameInc;
                    }
                    tBOX.Text = ftf.DfltVal;
                    sPanel_IM_NScheme.Children.Add(tBOX);
                    UIControlsColl.Add(tBOX.Name, "TextBox," + "FreeText");
                }
                #endregion

                #region Delimiter
                else if (item.FieldTyp.ToString().Equals("Delimiter"))
                {
                    DelimField dlf = (DelimField)item;
                    System.Windows.Controls.TextBlock tBlock = new System.Windows.Controls.TextBlock();
                    if (UIControlsColl.ContainsKey("tBlock_" + UINameInc))
                    {
                        UINameInc += 1;
                        tBlock.Name = "tBlock_" + UINameInc;
                    }
                    else
                    {
                        tBlock.Name = "tBlock_" + UINameInc;
                    }
                    tBlock.Foreground = Brushes.Black;
                    tBlock.Text = dlf.DelimVal.ToString();
                    //tBlock.IsReadOnly = true;
                    sPanel_IM_NScheme.Children.Add(tBlock);
                    UIControlsColl.Add(tBlock.Name, "TextBlock," + "Delimiter");
                }
                #endregion

                #region FixedText
                else if (item.FieldTyp.ToString().Equals("FixedText"))
                {
                    FixedTxtField ftf = (FixedTxtField)item;
                    System.Windows.Controls.TextBlock tBlock = new System.Windows.Controls.TextBlock();
                    if (UIControlsColl.ContainsKey("tBlock_" + UINameInc))
                    {
                        UINameInc += 1;
                        tBlock.Name = "tBlock_" + UINameInc;
                    }
                    else
                    {
                        tBlock.Name = "tBlock_" + UINameInc;
                    }
                    tBlock.Foreground = Brushes.Black;
                    tBlock.Text = ftf.FixedTxtVal.ToString();
                    //tBlock.IsReadOnly = true;
                    sPanel_IM_NScheme.Children.Add(tBlock);
                    UIControlsColl.Add(tBlock.Name, "TextBlock," + "FixedText");
                }
                #endregion

                #region WorkgroupLabel
                else if (item.FieldTyp.ToString().Equals("WorkgroupLabel"))
                {
                    WkgrpLabelField wgl = (WkgrpLabelField)item;
                    System.Windows.Controls.TextBlock tBlock = new System.Windows.Controls.TextBlock();
                    if (UIControlsColl.ContainsKey("tBlock_" + UINameInc))
                    {
                        UINameInc += 1;
                        tBlock.Name = "tBlock_" + UINameInc;
                    }
                    else
                    {
                        tBlock.Name = "tBlock_" + UINameInc;
                    }
                    tBlock.Foreground = Brushes.Black;
                    tBlock.Text = wgl.Val.ToString();
                    //tBlock.IsReadOnly = true;
                    sPanel_IM_NScheme.Children.Add(tBlock);
                    UIControlsColl.Add(tBlock.Name, "TextBlock," + "WorkgroupLabel");
                }
                #endregion

                #region Autogenerated
                else if (item.FieldTyp.ToString().Equals("Autogenerated"))
                {
                    ItemService iServ = GV.connection.WebServiceManager.ItemService;

                    AutogenField agf = (AutogenField)item;
                    System.Windows.Controls.TextBlock tBlock = new System.Windows.Controls.TextBlock();
                    if (UIControlsColl.ContainsKey("tBlockAutogenerated_" + UINameInc))
                    {
                        UINameInc += 1;
                        tBlock.Name = "tBlockAutogenerated_" + UINameInc;
                    }
                    else
                    {
                        tBlock.Name = "tBlockAutogenerated_" + UINameInc;
                    }
                    tBlock.Foreground = Brushes.Black;
                    KnowledgeVaultService kvs = GV.connection.WebServiceManager.KnowledgeVaultService;
                    //kvs.GetVaultOption(
                    //tBOX.Text = iServ.//agf..To.ToString();
                    //tBOX.IsReadOnly = true;


                    sPanel_IM_NScheme.Children.Add(tBlock);
                    UIControlsColl.Add(tBlock.Name, "TextBlock," + "Autogenerated");
                }
                #endregion

                else
                {
                    lView_Log.Items.Add(item.FieldTyp + " --");
                }
            }
        }
        private static long[] mID;
        private static long[] sID;
        private static long[] rID;
        private void btn_CreateItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (GV.wServ = GV.connection.WebServiceManager)
                {
                    var cd = (Global.CategoryData)cBox_ItemMasterCategoryList.SelectedItem;
                    var ns = (Global.NumSchemes)cBox_ItemNumScheme.SelectedItem;

                    lView_Log.Items.Add(cd.CATName + " | " + cd.CatID);
                    ItemService IS = GV.wServ.ItemService;
                    var item = IS.AddItemRevision(cd.CatID);
                    ProductRestric[] pres; //, out pres
                    mID = new[] { item.MasterId };
                    sID = new[] { ns.SchemeID };
                    rID = new[] { item.RevId };
                    getItemNum();

                    #region TEST CODE
                    /* TEST STATIC CODE
                    string[] newItemNum;
                    newItemNum = new string[] { "blabla" };
                    StringArray[] fieldInputs = new StringArray[1];
                    StringArray tempArr = new StringArray();
                    tempArr.Items = newItemNum;
                    fieldInputs[0] = tempArr;
                    //ItemNum[] newNumbers = IS.AddItemNumbers(mID, sID, fieldInputs, out pres);
                    */
                    //fInp.Items 
                    #endregion


                    ItemNum[] newNumbers = IS.AddItemNumbers(mID, sID, ItemNSFieldInputs, out pres);
                    item.ItemNum = newNumbers[0].ItemNum1;
                    string[] itemNums = { item.ItemNum };
                    item.Detail = "Details";
                    item.Title = "TitleVal";
                    foreach (var prop in propertyItems)
                    {
                        //StackPanel sPanel_obj = FindChild<StackPanel>(Application.Current.MainWindow, prop.Value[0]);
                        TextBox TextBlock_obj = FindChild<TextBox>(Application.Current.MainWindow, prop.Value[1]);
                        MessageBox.Show(prop.Key + prop.Value[0] + prop.Value[1] + " | " + TextBlock_obj.Text);
                        PropInstParam PIP = new PropInstParam() { PropDefId = prop.Key, Val = TextBlock_obj.Text };
                        IS.UpdateItemProperties(rID, new PropInstParamArray[] { new PropInstParamArray() { Items = new PropInstParam[] { PIP } } });
                    }
                    //IS.UpdateItemProperties(rID,
                    IS.CommitItemNumbers(mID, itemNums);
                    IS.UpdateAndCommitItems(new Item[] { item });

                    lView_Log.Items.Add(item.ItemNum);

                }
            }
            catch (SoapException se)
            {
                MessageBox.Show(se.ToString());
            }
            catch (System.Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }
        #endregion

        #region File
        private void btn_browseFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // create browse dialog settings object & configure
                var settings = new VDFVF.Settings.SelectEntitySettings();
                settings.PersistenceKey = "MyCompany.MyApp.MyDialog";
                settings.MultipleSelect = false;
                settings.ActionableEntityClassIds.Add(VDF.Vault.Currency.Entities.EntityClassIds.Files);

                // run the dialog
                var results = VDFVF.Library.SelectEntity(GV.connection, settings);

                // fill the textbox with the selected file path
                var selected = results.SelectedEntities.SingleOrDefault();
                if (selected == null)
                {
                    MessageBox.Show("null");
                    return;

                }
                GV.selectedFile = selected as VDF.Vault.Currency.Entities.FileIteration;
                if (GV.selectedFile == null)
                {
                    MessageBox.Show("null");
                    return;
                }
               
                //Folder fld = GV.connection.WebServiceManager.DocumentService.GetFolderById(selectedFile.FolderId);
                //GV.connection.FileManager.LoadParentFolders(selectedFile.);
                tBox_selectedFile.Text = VHFH.getPathbyFileID(GV.connection, GV.selectedFile.EntityMasterId);// fld.FullName + "/" + selectedFile.Name;// selectedFile.Parent.FullName + "/" + selectedFile.EntityName;
                //MessageBox.Show(selectedFile.Parent.FullName + "/" + selectedFile.EntityName);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_fileCheckOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GV.vaultfilePropsString["GPC Title 1"] = "---Her we go! 34";
                GV.vaultfilePropsString["GPC Title 22"] = "---Her we go! 34--";
                GV.vaultfilePropsString["GPC Title 3"] = "---Her we go!---56-";
                VHPFH.updateFilePropertyBulkbyFileId(GV.connection, GV.selectedFile.EntityMasterId, GV.vaultfilePropsString);
                //Autodesk.Connectivity.WebServices.File[] selFile = GV.connection.WebServiceManager.DocumentService.GetFilesByMasterId(GV.selectedFile.EntityMasterId);
                //updateProp(selFile[0]);
                //GV.vaultfilePropsUPd.Clear();
                //GV.vaultfilePropsUPd.Add(VHPH.getPropertyDefDetails(GV.connection, "FILE", "GPC Title 1"), ">>>Her we go!");
                //GV.vaultfilePropsUPd.Add(VHPH.getPropertyDefDetails(GV.connection, "FILE", "GPC Title 2"), ">>>Her we go again!");
                //GV.vaultfilePropsUPd.Add(VHPH.getPropertyDefDetails(GV.connection, "FILE", "GPC Title 3"), ">>>Her we go yet again!");
                //VHPFH.updateFilePropertiesBulk(GV.connection, selFile[0], GV.vaultfilePropsUPd);
                //Checkout(GV.selectedFile);

                //GV.connection.WebServiceManager.DocumentService.CheckoutFile(GV.selectedFile.Id, Auw2w2w2todesk.Connectivity.WebServices.CheckoutFileOptions.Master, System.Environment.MachineName, downloadpath, "", out xx);

            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static Autodesk.Connectivity.WebServices.File Checkout(FileIteration file)
        {
            //Folder[] folders = GV.connection.WebServiceManager.DocumentService.GetFoldersByFileMasterId(file.MasterId);
            string filePath = @"C:\Temp";

            if (System.IO.File.Exists(filePath) == true)
            {
                System.IO.FileAttributes attr = System.IO.File.GetAttributes(filePath);
                // The local file can't be read only.

                if ((attr & System.IO.FileAttributes.ReadOnly) != 0)
                {
                    System.IO.File.SetAttributes(filePath, FileAttributes.Normal);
                }
            }
            // check out the file
            //GV.connection.FileManager.LoadParentFolders(file.ToSingleArray());
            ByteArray downloadTicket;
            Autodesk.Connectivity.WebServices.File checkedOutFile = GV.connection.WebServiceManager.DocumentService.CheckoutFile(file.EntityIterationId, CheckoutFileOptions.Master, Environment.MachineName, @"c:\temp",
                "test create new version", out downloadTicket);
            // Get the latest data of the file from Vault                  
            //System.IO.File.WriteAllBytes(filePath, xx);

            return checkedOutFile;


        }

        public void updateProp(Autodesk.Connectivity.WebServices.File file)
        {
            try
            {
                GV.vaultfileProps.Clear();
                GV.vaultfilePropsUPd.Clear();
                var ticket = GV.connection.WebServiceManager.DocumentService.SecurityHeader.Ticket;
                var userID = GV.connection.UserID;
                var EXPUtil = ExplorerLoader.LoadExplorerUtil(GV.connection.Server, GV.connection.Vault, GV.connection.UserID, ticket);// ticket);
                GV.vaultfileProps = VHPFH.getFilePropertiesDefs(Global.variables.connection, file);
                foreach (var it in GV.vaultfileProps)
                {
                    lView_Log.Items.Add("VaultPropVal : " + it.Key.DispName + " | " + it.Value);
                    //logit.logger("VaultPropVal : " + it.Key + " | " + it.Value);
                }
                GV.vaultfilePropsUPd.Add(VHPH.getPropertyDefDetails(GV.connection, "FILE", "GPC Title 1"), "Her we go!");
                GV.vaultfilePropsUPd.Add(VHPH.getPropertyDefDetails(GV.connection, "FILE", "GPC Title 2"), "Her we go again!");
                GV.vaultfilePropsUPd.Add(VHPH.getPropertyDefDetails(GV.connection, "FILE", "GPC Title 3"), "Her we go yet again!");
                //GV.connection.WebServiceManager.PropertyService.GetPropertyDefinitionInfosByEntityClassId(
                EXPUtil.UpdateFileProperties(file, GV.vaultfilePropsUPd);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_getFilesfromFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Autodesk.Connectivity.WebServices.File[] files = VHFH.getFilesfromVaultFolderID(GV.connection, tBox_FolderID_Copy.Text);
                MessageBox.Show("Files in Fodler Count: " + VHFH.getFilesinFolderCountID(GV.connection, tBox_FolderID_Copy.Text).ToString());
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        lView_FileLog.Items.Add("--" + file.Name + " | " + file.Id + " | " + file.CreateUserId + " | " + file.MasterId);
                    }
                }
                else
                {
                    MessageBox.Show("No Files Found");
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_fileProps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict = VHFH.PrintProperties(GV.connection, VHFH.geFilebyID(GV.connection, Convert.ToInt64(tBox_fileID.Text)));
                foreach(var item in dict)
                {
                    lView_FileLog.Items.Add(item.Key + " | " + item.Value);
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        #region Folders
        private void btn_fetchFolders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, folderInfo> fodlerDict = new Dictionary<string, folderInfo>();
                fodlerDict = VHFLDH.getAllVaultFolders(GV.connection);
                foreach (var fld in fodlerDict)
                {
                    //MessageBox.Show(fld.Value.GetType().FullName);
                    folderInfo fldi = new folderInfo();
                    fldi = fld.Value as folderInfo;
                    lView.Items.Add("--" + fldi.EntityClass + " | " + fldi.Id + " | " + fldi.FullName + " | " + fldi.NumberOfChildren);

                }
                //get folder list from vault
                //foreach(var folder in GV.connection.FolderManager.GetChildFolders(GV.connection.FolderManager.RootFolder, false, false))
                //{
                //    lView.Items.Add(folder.FolderPath + " | " + folder.EntityName + " | " + folder.FullName + " | " + folder.);
                //    if (folder.NumberOfChildren > 0)
                //    {
                //        foreach (var sfolder1 in GV.connection.FolderManager.GetChildFolders(folder, false, false))
                //        {
                //            lView.Items.Add("--" + sfolder1.FolderPath + " | " + sfolder1.EntityName + " | " + sfolder1.FullName + " | " + sfolder1.NumberOfChildren);

                //            if (folder.NumberOfChildren > 0)
                //            {
                //                foreach (var sfolder2 in GV.connection.FolderManager.GetChildFolders(sfolder1, false, false))
                //                {
                //                    lView.Items.Add("----" + sfolder2.FolderPath + " | " + sfolder2.EntityName + " | " + sfolder2.FullName + " | " + sfolder2.NumberOfChildren);
                //                }
                //            }
                //        }
                //    }
                //}
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

            BindingOperations.ClearAllBindings(lView);
            GV.folderList.Clear();
            FolderStructureModel node = tView_folders.Items[0] as FolderStructureModel;

            FolderStructureModel.GetCheckedItems(node);
            lView.ItemsSource = GV.folderList;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            BindingOperations.ClearAllBindings(lView);
            GV.folderList.Clear();
            FolderStructureModel node = tView_folders.Items[0] as FolderStructureModel;
            FolderStructureModel.GetCheckedItems(node);
            lView.ItemsSource = GV.folderList;
        }

        private void lView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FolderStructureModel fld = lView.SelectedItem as FolderStructureModel;
            if (Directory.Exists(fld.folderPath))
            {
                Process.Start(fld.folderPath);
            }
            else
            {
                MessageBox.Show("Folder " + fld.folderPath + " Dont Exist!", "");
            }
        }

        private void btn_getFolderDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, folderInfo> fodlerDict = new Dictionary<string, folderInfo>();
                fodlerDict = VHFLDH.getAllVaultSubFolderFolders(GV.connection, tBox_FolderID.Text);
                foreach (var fld in fodlerDict)
                {
                    MessageBox.Show(fld.Value.GetType().FullName);
                    folderInfo fldi = new folderInfo();
                    fldi = fld.Value as folderInfo;
                    lView.Items.Add("--" + fldi.EntityClass + " | " + fldi.EntityName + " | " + fldi.FullName + " | " + fldi.NumberOfChildren);
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        


        #endregion

        #region Life Cycles
        private void btn_getlifeCycles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VHFH.gefilebyfilePath(GV.connection, tBox_FilePath.Text);
                FileLfCyc currentlifeCycle = VHLH.getFileCurrentlifeCycle(GV.connection, tBox_FilePath.Text);

                Dictionary<string, LfCycState> lifeCycleDict = new Dictionary<string, LfCycState>();

                lifeCycleDict = VHLH.getFileLifeCycleStates(GV.connection, tBox_FilePath.Text);
                cBox_LifeCycle.ItemsSource = lifeCycleDict.Values;

                

            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_updateLifeCycle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VHFH.gefilebyfilePath(GV.connection, tBox_FilePath.Text);
                LfCycState ls = cBox_LifeCycle.SelectedItem as LfCycState;
                VHLH.updateFileLifeCycle(GV.connection, tBox_FilePath.Text, ls.Id, "NEO EDITED");
                
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_getFolder_Click(object sender, RoutedEventArgs e)//folder lifecycle
        {
            try
            {
                Dictionary<string, LfCycState> lifeCycleDict = new Dictionary<string, LfCycState>();
                VHFH.gefilebyfilePath(GV.connection, tBox_FilePath.Text);
                lifeCycleDict = VHLH.getFolderLifeCycleStates(GV.connection, tBox_fldPath.Text);
                cBox_FolderLifeCycle.ItemsSource = lifeCycleDict.Values;
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        #endregion

        private void btn_FindFile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(VHFH.checkifFileExists(GV.connection, tBox_FilePath.Text).ToString());
        }

        private void btn_updateFolderLifecycle_Click(object sender, RoutedEventArgs e)
        {
            LfCycState LS = cBox_FolderLifeCycle.SelectedItem as LfCycState;
            VHLH.updateFolderLifeCycle(GV.connection, tBox_fldPath.Text, LS.Id, "NEO");
        }
    }

    public static class ObjectToDictionaryHelper
    {
        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();
        }

        public static IDictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null)
                ThrowExceptionWhenSourceArgumentIsNull();

            var dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToDictionary<T>(property, source, dictionary);
            return dictionary;
        }

        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value))
                dictionary.Add(property.Name, (T)value);
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
        }
    }
}
