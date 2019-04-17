using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
    }
}
