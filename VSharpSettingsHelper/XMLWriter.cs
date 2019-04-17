using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace VSharpSettingsHelper
{
    class XMLWriter
    {
        public static void writeXML(string XMLfile, string XElementName, ObservableCollection<Dictionary<string, string>> XMLRows)
        {
            try
            {
                var doc = new XmlDocument();

                XElement element = new XElement(XElementName);
                using (var writer = new System.IO.StreamWriter(XMLfile))
                {
                    foreach (Dictionary<string, string> RowElements in XMLRows)
                    {
                        var newelement = new XElement(RowElements.First().Key, RowElements["value"]);
                        foreach(var DictItem in RowElements)
                        {
                            if (DictItem.Key != RowElements.First().Key)
                            {
                                newelement.SetAttributeValue(DictItem.Key, DictItem.Value);
                            }
                        }
                        
                    }
                    element.Save(writer);
                }

            }
            catch(System.Exception ex) { string excep = ex.ToString(); }
        }

        
    }

    public class Nodes
    {
        public string Note { get; set; }

        [XmlElement(ElementName = "Sample")]

        public ObservableCollection<NodeValues> NoteValues { get; set; }
    }

    public class NodeValues
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Value { get; set; }
    }
}
