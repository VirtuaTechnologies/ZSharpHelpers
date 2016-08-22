using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Data;
using IndexedDictionary;

namespace ZSharpFDOHelper.SHP
{
    class SHPSchemaManager
    {
        public static FeatureSet createSchema(FeatureSet fs, IndexedDictionary<string, string[]> data)
        {
            try
            {
                foreach (var item in data)
                {
                    if (!item.Key.Equals("Geometry") || !item.Key.Equals("coordinates"))
                    {
                        string dataType = data[item.Key][0];
                        string value = data[item.Key][1];
                        Debug.Write("\nProperty: " + item.Key);
                        Debug.Write("\ndataType: " + dataType);
                        Debug.Write("\nvalue: " + value);
                        if (getDataType(dataType) != null)
                            fs.DataTable.Columns.Add(new DataColumn(item.Key, getDataType(dataType)));
                    }
                }
            }
            catch (SystemException ex)
            {
                Debug.Write("\n" + ex.ToString());
            }
            return fs;
        }

        private static Type t;
        public static Type getDataType(string dataType)
        {
            try
            {
                Debug.Write("\nIncoming DATA TYPE ::: " + dataType);
                if (dataType.Equals("String"))
                    t = typeof(string);
                else if (dataType.Equals("Double"))
                    t = typeof(double);
                else if (dataType.Equals("int"))
                    t = typeof(Int32);
                else
                    t = null;
            }
            catch (SystemException ex)
            {
                Debug.Write("\n" + ex.ToString());
            }

            return t;
        }
    }
}
