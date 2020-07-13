using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace VSharpXMLHelper
{
    public class xmlWriter
    {
        public static void editXML(string xmlFile, string xPath, string attributeName, string attVal, string newVal)
        {
            try
            {
                // instantiate XmlDocument and load XML from file
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFile);

                // get a list of nodes - in this case, I'm selecting all <AID> nodes under
                // the <GroupAIDs> node - change to suit your needs
                XmlNodeList aNodes = doc.SelectNodes(xPath);
                // loop through all AID nodes
                foreach (XmlNode aNode in aNodes)
                {
                    string child = aNode.FirstChild.InnerText.ToString();

                    // grab the "id" attribute
                    XmlAttribute idAttribute = aNode.Attributes[attributeName];

                    if (idAttribute.Value == attVal)
                    {
                        aNode.FirstChild.InnerText = newVal;
                        break;
                    }

                }

                // save the XmlDocument back to disk
                doc.Save(xmlFile);
            }
            catch (System.Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public static void cleanWrite(string xmlFile, string mainKey, string subKey, string attName, Dictionary<string, string> keyvalue)
        {
            try
            {
                // instantiate XmlDocument and load XML from file - XDocument d = new XDocument(new XComment("VSHARP XML Writer Library"), new XProcessingInstruction("xml-stylesheet", "href='mystyle.css' title='Compact' type='text/css'"));
                XDocument d = new XDocument(new XComment("VSHARP XML Writer Library"), new XProcessingInstruction("xml-stylesheet", "href='mystyle.css' title='Compact' type='text/css'"));
                d.Declaration = new XDeclaration("1.0", "utf-8", "true");

                XElement mainKey_Element = new XElement(mainKey);

                foreach (var item in keyvalue)
                {
                    XElement subKey_Element = new XElement(subKey, new XAttribute(attName, item.Key), item.Value);
                    mainKey_Element.Add(subKey_Element);
                }
                d.Add(mainKey_Element);

                if (System.IO.File.Exists(xmlFile))
                {
                    System.IO.File.Delete(xmlFile);
                    d.Save(xmlFile);
                }
                else
                {
                    d.Save(xmlFile);
                }

            }
            catch (System.Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        public static void cleanWritevariableKey(string xmlFile, string mainKey, string attName, Dictionary<string, Dictionary<string, string>> keyvalue)
        {
            try
            {
                // instantiate XmlDocument and load XML from file - XDocument d = new XDocument(new XComment("VSHARP XML Writer Library"), new XProcessingInstruction("xml-stylesheet", "href='mystyle.css' title='Compact' type='text/css'"));
                XDocument d = new XDocument(new XComment("VSHARP XML Writer Library"), new XProcessingInstruction("xml-stylesheet", "href='mystyle.css' title='Compact' type='text/css'"));
                d.Declaration = new XDeclaration("1.0", "utf-8", "true");

                XElement mainKey_Element = new XElement(mainKey);

                foreach (var item in keyvalue)
                {
                    foreach (var type in item.Value)
                    {
                        XElement subKey_Element = new XElement(item.Key, new XAttribute(attName, type.Key), type.Value);
                        mainKey_Element.Add(subKey_Element);
                    }
                }
                d.Add(mainKey_Element);

                if (System.IO.File.Exists(xmlFile))
                {
                    System.IO.File.Delete(xmlFile);
                    d.Save(xmlFile);
                }
                else
                {
                    d.Save(xmlFile);
                }

            }
            catch (System.Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }
    }
}
