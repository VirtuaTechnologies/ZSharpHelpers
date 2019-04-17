//This is an Intelectual Property of Zcodia Technologies and Raghulan Gowthaman.
//www.zcodiatechnologies.com.au


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Xml.Serialization;
using ZSharpXMLHelper;

namespace XMLParserApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_readXML_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //XmlSerializer serializer = new XmlSerializer(typeof(msg));
                //StringReader rdr = new StringReader(inputString);
                //msg resultingMessage = (msg)serializer.Deserialize(rdr);
                //StringReader rdr = new StringReader(File.ReadAllText(tBox_xmlFile.Text));
                Dictionary<int, string> dictfromXML = new Dictionary<int, string>(); ;
                dictfromXML = xmlParser.getXMLVaulesDict(tBox_xmlFile.Text, "SEWERNOTES", "name", "note");



            }
            catch(System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static string SerializeObject<T>(T dataObject)
        {
            if (dataObject == null)
            {
                return string.Empty;
            }
            try
            {
                using (StringWriter stringWriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stringWriter, dataObject);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static T DeserializeObject<T>(string xml)
             where T : new()
        {
            if (string.IsNullOrEmpty(xml))
            {
                return new T();
            }
            try
            {
                using (var stringReader = new StringReader(xml))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stringReader);
                }
            }
            catch (Exception ex)
            {
                return new T();
            }
        }

        private void tBox_xmlFile_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btn_keys_Click(object sender, RoutedEventArgs e)
        {
            List<string> result = xmlParser.getXMLKeys(tBox_xmlFile.Text, "KeyNotes");
            var itme = result;
        }

        private void btn_getValswithoutAtt_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<int, string> result = xmlParser.getXMLVaulesSpec(tBox_xmlFile.Text, tBox_xmlKey.Text, tBox_att.Text);
            var itme = result;
        }

        private void tBox_rootKey_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
