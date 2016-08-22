using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Xml;
using System.Diagnostics;

namespace ZSharpXMLHelper
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

        public static List<string> getXMLVaules(string xmlFile, string XMLKey, string attribute, string attVal)
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
                            if (reader.Name == XMLKey)
                            {
                                string att = reader[attribute];
                                //Debug.Write("\n" + result);
                                if (reader.Read() && att == attVal)
                                {
                                    result.Add(reader.Value.Trim());
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
