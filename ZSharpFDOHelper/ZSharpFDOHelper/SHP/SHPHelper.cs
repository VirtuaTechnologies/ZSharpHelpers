using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

using OSGeo.FDO.Connections;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Commands.Feature;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Expression;
using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite;
using NetTopologySuite.IO;
using DotSpatial.Data;
using DotSpatial.Topology;
using System.Data;
using IndexedDictionary;

namespace ZSharpFDOHelper.SHP
{
    public class SHPHelper
    {
        public static void createSHP()
        {
            // See comments below this code for an updated version.

            // define the feature type for this file
            FeatureSet fs = new FeatureSet(FeatureType.Polygon);


            // Add Some Columns
            fs.DataTable.Columns.Add(new DataColumn("ID", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("Text", typeof(string)));


            // create a geometry (square polygon)
            List<DotSpatial.Topology.Coordinate> vertices = new List<DotSpatial.Topology.Coordinate>();

            vertices.Add(new DotSpatial.Topology.Coordinate(0, 0));
            vertices.Add(new DotSpatial.Topology.Coordinate(0, 100));
            vertices.Add(new DotSpatial.Topology.Coordinate(100, 100));
            vertices.Add(new DotSpatial.Topology.Coordinate(100, 0));
            Polygon geom = new Polygon(vertices);

            // add the geometry to the featureset. 
            DotSpatial.Data.IFeature feature = fs.AddFeature(geom);

            // now the resulting features knows what columns it has
            // add values for the columns
            feature.DataRow.BeginEdit();
            feature.DataRow["ID"] = 1;
            feature.DataRow["Text"] = "Hello World";
            feature.DataRow.EndEdit();


            // save the feature set
            fs.SaveAs(@"H:\temp\12Converter\SHPTEST\test.shp", true);

        }

       

        public static void createSHPPointBulk(string fileName, string tableName, List<IndexedDictionary<string, string[]>> dataColl)
        {
            try
            {
                FeatureSet fs = new FeatureSet(FeatureType.Point);

                fs = SHPSchemaManager.createSchema(fs, dataColl[0]);


                foreach (var data in dataColl)
                {
                    fs = SHPCRUD.SHPPointDataHandler(fs, data);
                    
                }

                // define the feature type for this file
                

                // create a geometry and add data
               


                // save the feature set
                Debug.Write("\nSHP FILE::: " + fileName + "\n");

                fs.SaveAs(fileName, true);
            }
            catch (SystemException ex)
            {
                Debug.Write("\n" + ex.ToString());
            }

        }

        public static void createSHPLineBulk(string fileName, string tableName, List<IndexedDictionary<string, string[]>> dataColl)
        {
            try
            {
                FeatureSet fs = new FeatureSet(FeatureType.Line);

                fs = SHPSchemaManager.createSchema(fs, dataColl[0]);


                foreach (var data in dataColl)
                {
                    fs = SHPCRUD.SHPLineDataHandler(fs, data);

                }

                // save the feature set
                Debug.Write("\nSHP FILE::: " + fileName + "\n");

                fs.SaveAs(fileName, true);
            }
            catch (SystemException ex)
            {
                Debug.Write("\n" + ex.ToString());
            }

        }
    
    }

    public class SHPCRUD
    {
        public static FeatureSet SHPLineDataHandler(FeatureSet fs, IndexedDictionary<string, string[]> data)
        {
            try
            {
                Debug.Write("\nCoor: " + Convert.ToDouble(data["coordinates"][1]) + "| " + Convert.ToDouble(data["coordinates"][2]) + "| " + Convert.ToDouble(data["coordinates"][3]) + "| " + Convert.ToDouble(data["coordinates"][4]));
                DotSpatial.Topology.Coordinate ptcoor1 = new DotSpatial.Topology.Coordinate(Convert.ToDouble(data["coordinates"][1]), Convert.ToDouble(data["coordinates"][2]));
                DotSpatial.Topology.Coordinate ptcoor2 = new DotSpatial.Topology.Coordinate(Convert.ToDouble(data["coordinates"][3]), Convert.ToDouble(data["coordinates"][4]));
                List<DotSpatial.Topology.Coordinate> lineCoor = new List<DotSpatial.Topology.Coordinate>();
                lineCoor.Add(ptcoor1);
                lineCoor.Add(ptcoor2);
                LineString line = new LineString(lineCoor);
                DotSpatial.Data.IFeature feature = fs.AddFeature(line);

                //remove geometry
                data.Remove("Geometry");
                //now fill in rest of the columns
                foreach (var item in data)
                {
                    string dataType = data[item.Key][0];
                    string value = data[item.Key][1];
                    Debug.Write("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~SHP null AND dOUBLE DEBUG~~~~~~~~~~~~~~~~ VALUE : " + value + " =" + (string.IsNullOrEmpty(value)));
                    //check if value is null - double dont accept string null values. need to fix it before sending.
                    if (value.Equals("null") && dataType.Equals("Double"))
                    {
                        //for double
                        double d;
                        if(double.TryParse(value, out d))
                        {
                          // valid number
                            Debug.Write("\n~~~~VALID");
                        }
                        else
                        {
                          // not a valid number 
                            Debug.Write("\n~~~~VALID Assigning 0");
                          value = "0";
                        }

                    }


                    if (!item.Key.Equals("Geometry") || !item.Key.Equals("coordinates"))
                    {
                        Debug.Write("\n~~SHP WRITE Property: " + item.Key);
                        Debug.Write("\n~~SHP WRITE dataType: " + dataType);
                        Debug.Write("\n~~SHP WRITE value: " + value);

                        feature.DataRow.BeginEdit();
                        feature.DataRow[item.Key] = value;
                        feature.DataRow.EndEdit();
                    }
                    Debug.Write("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~SHP null AND dOUBLE DEBUG~~~~~~~~~~~~~~~~\n");
                }
                
            }
            catch (SystemException ex)
            {
                Debug.Write("\n" + ex.ToString());
            }
            return fs;
        }

        public static FeatureSet SHPPointDataHandler(FeatureSet fs, IndexedDictionary<string, string[]> data)
        {
            try
            {

                DotSpatial.Topology.Coordinate ptcoor = new DotSpatial.Topology.Coordinate(Convert.ToDouble(data["Northing"][1]), Convert.ToDouble(data["Easting"][1]));
                Point pt = new Point(ptcoor);
                DotSpatial.Data.IFeature feature = fs.AddFeature(pt);

                //remove geometry
                data.Remove("Geometry");

                //now fill in rest of the columns
                foreach (var item in data)
                {
                    string dataType = data[item.Key][0];
                    string value = data[item.Key][1];


                    if (!item.Key.Equals("Northing") || !item.Key.Equals("Easting") || !item.Key.Equals("Geometry"))
                    {
                        Debug.Write("\nProperty: " + item.Key);
                        Debug.Write("\ndataType: " + dataType);
                        Debug.Write("\nvalue: " + value);

                        feature.DataRow.BeginEdit();
                        feature.DataRow[item.Key] = value;
                        feature.DataRow.EndEdit();
                    }

                }

            }
            catch (SystemException ex)
            {
                Debug.Write("\n" + ex.ToString());
            }
            return fs;
        }
    }
}


/* TEST CODE
       public static void createSHPPolygonTEST()
       {
           // See comments below this code for an updated version.

           // define the feature type for this file
           FeatureSet fs = new FeatureSet(FeatureType.Polygon);

           // Add Some Columns
            
           IndexedDictionary<string, Type> schemaDetails = new IndexedDictionary<string, Type>();
           schemaDetails.Add("Col1", typeof(int));
           schemaDetails.Add("ID", typeof(int));
           schemaDetails.Add("Text", typeof(string));
           fs = SHPSchemaManager.createSchema(fs, schemaDetails);


           // create a geometry (square polygon)
           List<DotSpatial.Topology.Coordinate> vertices = new List<DotSpatial.Topology.Coordinate>();

           vertices.Add(new DotSpatial.Topology.Coordinate(0, 0));
           vertices.Add(new DotSpatial.Topology.Coordinate(0, 100));
           vertices.Add(new DotSpatial.Topology.Coordinate(100, 100));
           vertices.Add(new DotSpatial.Topology.Coordinate(100, 0));
           Polygon geom = new Polygon(vertices);

           // add the geometry to the featureset. 
           DotSpatial.Data.IFeature feature = fs.AddFeature(geom);
           //DotSpatial.Data.Feature f = new DotSpatial.Data.Feature();
           //FeatureSet fs1 = new FeatureSet(f.FeatureType);
           //fs1.Features.Add(ptcoor);
           // now the resulting features knows what columns it has
           // add values for the columns
           feature.DataRow.BeginEdit();
           feature.DataRow["ID"] = 1;
           feature.DataRow["Text"] = "Hello World";
           feature.DataRow.EndEdit();
           // save the feature set
           fs.SaveAs(@"H:\temp\12Converter\SHPTEST\test.shp", true);
       }

       public static void createSHPPointTEST()
       {
           // See comments below this code for an updated version.

           // define the feature type for this file
           FeatureSet fs = new FeatureSet(FeatureType.Polygon);

           // Add Some Columns

           IndexedDictionary<string, Type> schemaDetails = new IndexedDictionary<string, Type>();
           schemaDetails.Add("Col1", typeof(int));
           schemaDetails.Add("ID", typeof(int));
           schemaDetails.Add("Text", typeof(string));
           fs = SHPSchemaManager.createSchema(fs, schemaDetails);
           //fs.DataTable.Columns.Add(new DataColumn("ID", typeof(int)));
           //fs.DataTable.Columns.Add(new DataColumn("Text", typeof(string)));


           // create a geometry (square polygon)
           DotSpatial.Topology.Coordinate ptcoor = new DotSpatial.Topology.Coordinate(110, 100);
           Point pt = new Point(ptcoor);

           // add the geometry to the featureset. 
           DotSpatial.Data.IFeature feature = fs.AddFeature(pt);
           //DotSpatial.Data.Feature f = new DotSpatial.Data.Feature();
           //FeatureSet fs1 = new FeatureSet(f.FeatureType);
           //fs1.Features.Add(ptcoor);
           // now the resulting features knows what columns it has
           // add values for the columns
           feature.DataRow.BeginEdit();
           feature.DataRow["ID"] = 1;
           feature.DataRow["Text"] = "Hello World";
           feature.DataRow.EndEdit();

           // save the feature set
           fs.SaveAs(@"H:\temp\12Converter\SHPTEST\test.shp", true);

       }

       public static void createSHPLineTEST()
       {
           // See comments below this code for an updated version.

           // define the feature type for this file
           FeatureSet fs = new FeatureSet(FeatureType.Line);

           // Add Some Columns

           IndexedDictionary<string, Type> schemaDetails = new IndexedDictionary<string, Type>();
           schemaDetails.Add("Col1", typeof(int));
           schemaDetails.Add("ID", typeof(int));
           schemaDetails.Add("Text", typeof(string));
           fs = SHPSchemaManager.createSchema(fs, schemaDetails);
           //fs.DataTable.Columns.Add(new DataColumn("ID", typeof(int)));
           //fs.DataTable.Columns.Add(new DataColumn("Text", typeof(string)));


           // create a geometry (square polygon)
           DotSpatial.Topology.Coordinate ptcoor1 = new DotSpatial.Topology.Coordinate(110, 100);
           DotSpatial.Topology.Coordinate ptcoor2 = new DotSpatial.Topology.Coordinate(120, 150);
           List<DotSpatial.Topology.Coordinate> lineCoor = new List<DotSpatial.Topology.Coordinate>();
           lineCoor.Add(ptcoor1);
           lineCoor.Add(ptcoor2);
           LineString line = new LineString(lineCoor);

           // add the geometry to the featureset. 
           DotSpatial.Data.IFeature feature = fs.AddFeature(line);
           //DotSpatial.Data.Feature f = new DotSpatial.Data.Feature();
           //FeatureSet fs1 = new FeatureSet(f.FeatureType);
           //fs1.Features.Add(ptcoor);
           // now the resulting features knows what columns it has
           // add values for the columns
           feature.DataRow.BeginEdit();
           feature.DataRow["ID"] = 1;
           feature.DataRow["Text"] = "Hello World";
           feature.DataRow.EndEdit();

           // save the feature set
           fs.SaveAs(@"H:\temp\12Converter\SHPTEST\test.shp", true);

       }

       public static void createSHPLineBulk(string SHPFolder, string tableName, IndexedDictionary<string, string[]> data)
       {
           // See comments below this code for an updated version.

           // define the feature type for this file
           FeatureSet fs = new FeatureSet(FeatureType.Line);

           // Add Some Columns

           IndexedDictionary<string, Type> schemaDetails = new IndexedDictionary<string, Type>();
           schemaDetails.Add("Col1", typeof(int));
           schemaDetails.Add("ID", typeof(int));
           schemaDetails.Add("Text", typeof(string));
           fs = SHPSchemaManager.createSchema(fs, schemaDetails);
           //fs.DataTable.Columns.Add(new DataColumn("ID", typeof(int)));
           //fs.DataTable.Columns.Add(new DataColumn("Text", typeof(string)));


           // create a geometry (square polygon)
           DotSpatial.Topology.Coordinate ptcoor1 = new DotSpatial.Topology.Coordinate(110, 100);
           DotSpatial.Topology.Coordinate ptcoor2 = new DotSpatial.Topology.Coordinate(120, 150);
           List<DotSpatial.Topology.Coordinate> lineCoor = new List<DotSpatial.Topology.Coordinate>();
           lineCoor.Add(ptcoor1);
           lineCoor.Add(ptcoor2);
           LineString line = new LineString(lineCoor);

           // add the geometry to the featureset. 
           DotSpatial.Data.IFeature feature = fs.AddFeature(line);
           //DotSpatial.Data.Feature f = new DotSpatial.Data.Feature();
           //FeatureSet fs1 = new FeatureSet(f.FeatureType);
           //fs1.Features.Add(ptcoor);
           // now the resulting features knows what columns it has
           // add values for the columns
           feature.DataRow.BeginEdit();
           feature.DataRow["ID"] = 1;
           feature.DataRow["Text"] = "Hello World";
           feature.DataRow.EndEdit();

           // save the feature set
           fs.SaveAs(@"H:\temp\12Converter\SHPTEST\test.shp", true);

       }
       */