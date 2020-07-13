using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Xml;
using System.Diagnostics;

/*
 * <App>
  <STYLE name="IRR">1,5</STYLE>
  <STYLE name="W">1,4</STYLE>
  <STYLE name="PV">1</STYLE>
  <STYLE name="GR">2</STYLE>
  <STYLE name="RW">3,4</STYLE>
  <STYLE name="SWR">5</STYLE>
  <STYLE name="STM">6</STYLE>
  <STYLE name="STR">2,3</STYLE>
</App>

    XML Key - App and then STYLE
    ATT - name
    result.Add(att, reader.Value.Trim());

    This would add (IRR, 1,5) to the collection.

 */

namespace VSharpXMLHelper
{
    public class xmlParser
    {
        private static string result;
        private static Dictionary<string, string[]> xmlVauleColl = new Dictionary<string, string[]>();
        public static string getXMLValue(string xmlFile, string XMLKey, string attribute, string attVal)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(xmlFile))
                {
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == XMLKey)
                            {
                                string att = reader[attribute];
                                Debug.Write("\n" + result);
                                if (reader.Read() && att == attVal)
                                {
                                    result = reader.Value.Trim();
                                    Debug.Write("\n  Text node: " + reader.Value.Trim() + "\n");
                                }
                            }
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return result;
        }

        public static string getXMLDict(string xmlFile, string XMLKey, string attribute, string attVal)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(xmlFile))
                {
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == XMLKey)
                            {
                                string att = reader[attribute];
                                Debug.Write("\n" + result);
                                if (reader.Read() && att == attVal)
                                {
                                    result = reader.Value.Trim();
                                    Debug.Write("\n  Text node: " + reader.Value.Trim() + "\n");
                                }
                            }
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return result;
        }

        public static int getXMLKeyCount(string xmlFile, string XMLKey, string attribute, string attVal)
        {
            int result = 0;
            try
            {
                using (XmlReader reader = XmlReader.Create(xmlFile))
                {
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == XMLKey)
                            {
                                string att = reader[attribute];
                                Debug.Write("\n" + result);
                                if (reader.Read() && att == attVal)
                                {
                                    result += 1;
                                    Debug.Write("\n  Text node: " + reader.Value.Trim() + "\n");
                                }
                            }
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return result;
        }


        public static Dictionary<string, string> getXMLKeyswihtspace(string xmlFile, string rootKey, string XMLspace)
        {
            Dictionary<string, string> xmlkeywithspace = new Dictionary<string, string>();
            try
            {
                using (XmlReader reader = XmlReader.Create(xmlFile))
                {
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            if (reader.Name != rootKey)
                            {

                                //check withe keynote spacer

                                if (reader.Name.Contains(XMLspace))
                                {
                                    val = reader.Name.Replace(XMLspace, " ");
                                    if (!xmlkeywithspace.Keys.Contains(val))
                                    {

                                        xmlkeywithspace.Add(reader.Name, val);
                                    }
                                }
                                
                                
                            }
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return xmlkeywithspace;
        }

        private static string val;
        public static List<string> getXMLKeys(string xmlFile, string rootKey, string XMLspace)
        {
            List<string> result = new List<string>();
            try
            {
                using (XmlReader reader = XmlReader.Create(xmlFile))
                {
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            if (reader.Name != rootKey)
                            {

                                //check withe keynote spacer

                                if (reader.Name.Contains(XMLspace))
                                {
                                    val = reader.Name.Replace(XMLspace, " ");
                                }
                                else
                                {
                                    val = reader.Name;
                                }
                                if (!result.Contains(val))
                                {
                                    
                                    result.Add(val);
                                }
                            }
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return result;
        }
        
        public static Dictionary<int, string> getXMLVaules(string xmlFile, string rootKey, string attribute)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            try
            {
                using (XmlReader reader = XmlReader.Create(xmlFile))
                {
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            if (reader.Name != rootKey)
                            {
                                int att = Convert.ToInt32(reader[attribute]);
                                //Debug.Write("\n" + result);
                                if (reader.Read())
                                {
                                    result.Add(att, reader.Value.Trim());
                                    //result += 1;
                                    //Debug.Write("\n  Text node: " + reader.Value.Trim() + "\n");
                                }
                            }
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return result;
        }

        public static Dictionary<int, string> getXMLVaulesSpec(string xmlFile, string XMLKey, string attribute, Dictionary<string, string> xmlkeywithspace)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            try
            {
                using (XmlReader reader = XmlReader.Create(xmlFile))
                {
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            if(xmlkeywithspace.ContainsKey(reader.Name))
                            {
                                XMLKey = reader.Name;
                            }
                            if (reader.Name == XMLKey)
                            {
                                int att = Convert.ToInt32(reader[attribute]);
                                //Debug.Write("\n" + result);
                                if (reader.Read())
                                {
                                    result.Add(att, reader.Value.Trim());
                                    //result += 1;
                                    //Debug.Write("\n  Text node: " + reader.Value.Trim() + "\n");
                                }
                            }
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return result;
        }

        public static Dictionary<string, string> getXMLVaulesStrings(string xmlFile, string XMLKey, string attribute)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                using (XmlReader reader = XmlReader.Create(xmlFile))
                {
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == XMLKey)
                            {
                                string att = reader[attribute];
                                //Debug.Write("\n" + result);
                                if (reader.Read())
                                {
                                    if (!result.ContainsKey(att))
                                    {
                                        result.Add(att, reader.Value.Trim());
                                    }
                                    //result += 1;
                                    //Debug.Write("\n  Text node: " + reader.Value.Trim() + "\n");
                                }
                            }
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return result;
        }

        public static Dictionary<int, string> getXMLVaulesDict(string xmlFile, string XMLKey, string attribute, string attVal)
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            try
            {
                using (XmlReader reader = XmlReader.Create(xmlFile))
                {
                    int index = 1;
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == XMLKey)
                            {
                                string att = reader[attribute];
                                //Debug.Write("\n" + result);
                                if (reader.Read() && att == attVal)
                                {
                                    dict.Add(index, reader.Value.Trim());
                                    index += 1;
                                    //Debug.Write("\n  Text node: " + reader.Value.Trim() + "\n");
                                }
                            }
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return dict;
        }

        public static List<Dictionary<string, string>> getXMLChildVaules(string xmlFile, string XMLKey, string attribute, string attVal)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            Dictionary<string, string> keyVals = new Dictionary<string, string>();
            try
            {
                using (XmlReader reader = XmlReader.Create(xmlFile))
                {
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == XMLKey)
                            {
                                string att = reader[attribute];
                                //Debug.Write("\n" + result);
                                if (reader.Read() && att == attVal)
                                {
                                    //result.Add(reader.Value.Trim());
                                    //result += 1;
                                    //Debug.Write("\n  Text node: " + reader.Value.Trim() + "\n");
                                }
                            }
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return result;
        }
        private static List<string> columnName;
        public static List<string> gettingColumnName(string xmlFile, string XMLNode, string attribute)
        {
            columnName = new List<string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes(XMLNode);
            foreach (XmlNode node in nodeList)
            {
                columnName.Add(node.SelectSingleNode(attribute).InnerText);
                
            }
            return columnName;
        }

        private static List<string> columnWidth;
        public static List<string> gettingColumnWidth(string xmlFile, string XMLNode, string attribute)
        {
            columnWidth = new List<string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes(XMLNode);
            foreach (XmlNode node in nodeList)
            {
                columnWidth.Add(node.SelectSingleNode(attribute).InnerText);

            }
            return columnWidth;
        }

        public static List<string> getXMLValues2(string xmlFile, string XMLNode, string attribute)
        {
            List<string> result = new List<string>();
            try
            {
                
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFile);
                XmlNodeList nodes = doc.DocumentElement.SelectNodes(XMLNode);
                foreach (XmlNode node in nodes)
                {
                    result.Add(node.SelectSingleNode(attribute).InnerText);
                    Debug.Write("\nNode :" + node.Value);
                    Debug.Write("\nVaultPropName :" + node.SelectSingleNode(attribute).InnerText);
                    //Debug.Write("\nPDFField :" + node.SelectSingleNode("title").InnerText);
                }

            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return result;
        }

        public static int getXMLKeyCount2(string xmlFile, string XMLNode)
        {
            int result = 0;
            try
            {

                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFile);
                XmlNodeList nodes = doc.DocumentElement.SelectNodes(XMLNode);
                Debug.Write("Count: " + nodes.Count);
                result = nodes.Count;

            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            //Debug.Write("Count Final: " + result);
            return result;
        }

        
    }
}
