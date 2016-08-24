using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSGeo.FDO;
using OSGeo.FDO.ClientServices;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Commands.DataStore;
using osgeo_command = OSGeo.FDO.Commands;
using Metadata = ZSharpFDOHelper.SDF.Metadata;
using Schema_manager = ZSharpFDOHelper.SDF.SDFSchemaManager;
using System.IO;
using OSGeo.FDO.Commands.Schema;
using OSGeo.FDO.Commands.Feature;
using System.Collections;
using ZSharpQLogger;
using ZSharpFDOHelper.FDOGen;
using System.Diagnostics;
using OSGeo.FDO.Expression;
using IndexedDictionary;

namespace ZSharpFDOHelper
{
    public class SDFHelper
    {
        public static string result;

        public static string createSDFFile(string SDFFile)
        {
            try
            {
                //check if files exists - if so delete it
                if (File.Exists(SDFFile))
                {
                    File.Delete(SDFFile);
                }
                IConnection con = SDFConnection(SDFFile);
                ICreateDataStore datastore = con.CreateCommand(osgeo_command.CommandType.CommandType_CreateDataStore) as ICreateDataStore;

                datastore.DataStoreProperties.SetProperty("File", SDFFile);
                datastore.Execute();
                con.Close();
                if (File.Exists(SDFFile))
                {
                    result = "Successfully Created SDF File";
                }
                else
                {
                    result = "Failed Creating SDF File";
                }

            }
            catch (System.Runtime.InteropServices.SEHException sehex)
            {
                System.Diagnostics.Debug.WriteLine("Error SEHEX Helper>>" + sehex);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error SDF Helper>>" + ex);
            }
            return result;
        }

        public static string addSchemtoSDF(string SDFFile, string schemaName, string schemaDesc)
        {
            try
            {
                FeatureSchema schema1 = new FeatureSchema(schemaName, schemaDesc);
                IConnectionManager conmanager = FeatureAccessManager.GetConnectionManager();

                //sdf fdo connection
                using (IConnection connect = SDFConnection(SDFFile))
                {
                    //get connection dict
                    IConnectionPropertyDictionary conprop = connect.ConnectionInfo.ConnectionProperties;
                    
                    connect.Open();

                    //check connection state
                    OSGeo.FDO.Connections.ConnectionState connstate = connect.ConnectionState;
                    if (connstate != OSGeo.FDO.Connections.ConnectionState.ConnectionState_Open)
                    {
                        result = "\nCannot read SDF File \n" + SDFFile;
                    }
                    else
                    {
                        //add spatial context to the schema
                        SDF.SDFSchemaManager.spatial_context(connect);

                        result = "Read SDF File \n" + SDFFile;

                        //create command to alter schema
                        IApplySchema applyschema = (IApplySchema)connect.CreateCommand(CommandType.CommandType_ApplySchema) as IApplySchema;

                        //FeatureClassCollection fcoll;
                        ClassCollection fcoll = schema1.Classes;

                        //create all feature class
                        FeatureClass class_points = new FeatureClass("Points", "Point information");
                        FeatureClass class_alignment = new FeatureClass("Alignments", "Alignment information");
                        FeatureClass class_parcels = new FeatureClass("Parcels", "Parcels information");
                        FeatureClass class_pipes = new FeatureClass("Pipes", "Pipes information");
                        FeatureClass class_structures = new FeatureClass("Structures", "Structures information");

                        //add properties here
                        Schema_manager.add_property(class_points, connect);
                        Schema_manager.add_property(class_alignment, connect);
                        Schema_manager.add_property(class_parcels, connect);
                        Schema_manager.add_property(class_pipes, connect);
                        Schema_manager.add_property(class_structures, connect);

                        schema1.Classes.Add(class_points);
                        schema1.Classes.Add(class_alignment);
                        schema1.Classes.Add(class_parcels);
                        schema1.Classes.Add(class_pipes);
                        schema1.Classes.Add(class_structures);

                        //create property definition for each feature class

                        applyschema.FeatureSchema = schema1;
                        applyschema.Execute();
                        result = "Successfully Created SDF file with blank Schema \n" + SDFFile;
                    }
                    connect.Close();
                }

                
            }
            catch (SystemException ex)
            {

            }
            return result;
        }
        /*

        public static void add_property(FeatureClass fc, IConnection con)
        {
            //add property to point class
            if (fc.Name == "Points")
            {
                //add spatial context
                Metadata.spatial_context(con);

                //add geometry property
                fc.Properties.Add(Metadata.get_geo_property("point"));
                fc.GeometryProperty = Metadata.get_geo_property("point");

                //adding autoincrement property
                fc.Properties.Add(Metadata.get_general_property("id"));
                fc.IdentityProperties.Add(Metadata.get_general_property("id"));

                //name property
                fc.Properties.Add(Metadata.get_general_property("name"));

                //actualname property
                fc.Properties.Add(Metadata.get_general_property("actualname"));

                //description property
                fc.Properties.Add(Metadata.get_general_property("description"));

                //raw_description property
                fc.Properties.Add(Metadata.get_general_property("raw_description"));

                //number property
                fc.Properties.Add(Metadata.get_point_property("number"));

                //Elevation property
                fc.Properties.Add(Metadata.get_general_property("elevation"));

                //Lattitude property
                fc.Properties.Add(Metadata.get_general_property("latitude"));

                //longitude property
                fc.Properties.Add(Metadata.get_general_property("longitude"));

            }

            if (fc.Name == "Alignments")
            {
                //add spatial context
                Metadata.spatial_context(con);

                //add geometry property
                fc.Properties.Add(Metadata.get_geo_property("curve"));
                fc.GeometryProperty = Metadata.get_geo_property("curve");

                //adding autoincrement property
                fc.Properties.Add(Metadata.get_general_property("id"));
                fc.IdentityProperties.Add(Metadata.get_general_property("id"));

                //name property
                fc.Properties.Add(Metadata.get_general_property("name"));

                //length property
                fc.Properties.Add(Metadata.get_general_property("length"));

                //starting station property
                fc.Properties.Add(Metadata.get_alignment_property("starting_station"));

                //ending station property
                fc.Properties.Add(Metadata.get_alignment_property("ending_station"));

                //design speed property
                fc.Properties.Add(Metadata.get_alignment_property("design_speed"));

            }
            //AlterationType schema for testing
            if (fc.Name == "Parcels")
            {
                //add spatial context
                //Metadata.spatial_context(con);

                //add geometry property
                //fc.Properties.Add(Metadata.get_geo_property("surface"));
                //fc.GeometryProperty = Metadata.get_geo_property("surface");

                //adding autoincrement property
                fc.Properties.Add(Metadata.get_general_property("id"));
                fc.IdentityProperties.Add(Metadata.get_general_property("id"));

                //name property
                fc.Properties.Add(Metadata.get_general_property("name"));

                //area property
                fc.Properties.Add(Metadata.get_general_property("area"));

                //perimeter property
                fc.Properties.Add(Metadata.get_general_property("perimeter"));

            }

            if (fc.Name == "Pipes")
            {

                //add spatial context
                Metadata.spatial_context(con);


                //add geometry property
                fc.Properties.Add(Metadata.get_geo_property("curve"));
                fc.GeometryProperty = Metadata.get_geo_property("curve");

                //adding autoincrement property
                fc.Properties.Add(Metadata.get_general_property("id"));
                fc.IdentityProperties.Add(Metadata.get_general_property("id"));

                //name property
                fc.Properties.Add(Metadata.get_general_property("name"));

                //network name
                fc.Properties.Add(Metadata.get_general_property("network_name"));

                //inside dia
                fc.Properties.Add(Metadata.get_pipe_property("inside_dia"));

                //outside dia
                fc.Properties.Add(Metadata.get_pipe_property("outside_dia"));

                //length property
                fc.Properties.Add(Metadata.get_general_property("length"));

                //slope property
                fc.Properties.Add(Metadata.get_general_property("slope"));

                //start invert dia
                fc.Properties.Add(Metadata.get_pipe_property("start_invert"));

                //end invert dia
                fc.Properties.Add(Metadata.get_pipe_property("end_invert"));

                //start invert dia
                fc.Properties.Add(Metadata.get_pipe_property("structure_start"));

                //start invert dia
                fc.Properties.Add(Metadata.get_pipe_property("structure_end"));

                //part_size_name property
                fc.Properties.Add(Metadata.get_general_property("part_size_name"));

            }

            if (fc.Name == "Structures")
            {
                //add spatial context
                Metadata.spatial_context(con);

                //add geometry property
                fc.Properties.Add(Metadata.get_geo_property("point"));
                fc.GeometryProperty = Metadata.get_geo_property("point");

                //adding autoincrement property
                fc.Properties.Add(Metadata.get_general_property("id"));
                fc.IdentityProperties.Add(Metadata.get_general_property("id"));

                //name property
                fc.Properties.Add(Metadata.get_general_property("name"));

                //network name
                fc.Properties.Add(Metadata.get_general_property("network_name"));

                //rim elevation property
                fc.Properties.Add(Metadata.get_structure_property("rim_elevation"));

                //part_size_name property
                fc.Properties.Add(Metadata.get_general_property("part_size_name"));

            }
        }
        */
        private static IConnection con;
        public static IConnection SDFConnection(string SDFFile)
        {
            
            try
            {
                Debug.Write("\nSDF File to connect: >> " + SDFFile + "\n");
                //first get an instance of the connection manager
                IConnectionManager connman = FeatureAccessManager.GetConnectionManager();
                con = connman.CreateConnection("OSGeo.SDF");


                //check connection state

                //get connection dictionary
                IConnectionPropertyDictionary props = con.ConnectionInfo.ConnectionProperties;

                props.SetProperty("File", SDFFile);
                props.SetProperty("ReadOnly", "FALSE");

                if (con.ConnectionState == OSGeo.FDO.Connections.ConnectionState.ConnectionState_Open)
                {
                    con.Close();
                }
            }
            catch (OSGeo.FDO.Common.Exception ge)
            {

                bool ok = ge.Message.Contains("read-only");
                Debug.Write(ge.ToString());

            }
            catch (SystemException ex)
            {

                bool ok = ex.Message.Contains("read-only");
                Debug.Write(ex.ToString());

            }

            return con;
        }
    }
    public class SDFCRUD
    {
        public static void SDFRead(string SDFFile, string schemaName, string filter_value)
        {
            //this is to read value of a property
            //ZSharpLogger.DebugHelper.printVSConsole("SDFH", "Read Data module running...", true);
            //ed.WriteMessage("\nRead Data module running...");
            try
            {
                IConnection con = SDFHelper.SDFConnection(SDFFile);
                con.Open();
                using (con)
                {
                    using (osgeo_command.Feature.ISelect get_data = con.CreateCommand(osgeo_command.CommandType.CommandType_Select) as osgeo_command.Feature.ISelect)
                    {
                        //set target schema
                        get_data.SetFeatureClassName("Civil_schema:Pipes");
                        var filter = "(Name='R-1P1')";
                        //var filter = "(" + filter_value + ")";
                        get_data.SetFilter(filter);
                        //get_data.Filter.

                        //count the number of rowms
                        int count = 0;

                        //execute the select command
                        using (IFeatureReader reader = get_data.Execute())
                        {
                            //get class definition
                            ClassDefinition cdef = reader.GetClassDefinition();
                            //ZSharpLogger.DebugHelper.printVSConsole("SDFH", "Class definition: " + cdef.Name, true);
                            //ed.WriteMessage("\nClass definition: " + cdef.Name);

                            //string array to print each row
                            ArrayList row = new ArrayList();

                            //print the column name
                            foreach (PropertyDefinition def in cdef.Properties)
                            {
                                if (def is DataPropertyDefinition)
                                {
                                    var p = (DataPropertyDefinition)def;
                                    row.Add(def.Name);
                                    //ZSharpLogger.DebugHelper.printVSConsole("SDFH", "Property Header Name: " + def.Name, true);
                                    //ed.WriteMessage("\nProperty Header Name: " + def.Name);
                                }
                                // Print the header row (property names)
                                //ed.WriteMessage(string.Join("\t", row.ToArray()));

                            }
                            while (reader.ReadNext())
                            {
                                row.Clear();
                                try
                                {
                                    //for each property print value
                                    foreach (PropertyDefinition def in cdef.Properties)
                                    {
                                        try
                                        {

                                            //only data properties
                                            if (!(def is DataPropertyDefinition)) continue;
                                            var p = (DataPropertyDefinition)def;
                                            string value = "";
                                            value = FDODataHelper.expressionToString(FDODataHelper.ParseByDataType(reader.ToString(), p.DataType));
                                            //ZSharpLogger.DebugHelper.printVSConsole("SDFH", "Value: " + value, true);
                                            //depending upon the data type, use the approriate "GET"
                                            //method on the reader
                                            switch (p.DataType)
                                            {
                                                case OSGeo.FDO.Schema.DataType.DataType_Boolean:
                                                    value = reader.IsNull(p.Name) ? "" : reader.GetBoolean(p.Name).ToString();
                                                    break;

                                                case OSGeo.FDO.Schema.DataType.DataType_String:
                                                    value = reader.IsNull(p.Name) ? "" : reader.GetString(p.Name).ToString();
                                                    break;
                                            }
                                            row.Add(value);
                                            //ZSharpLogger.DebugHelper.printVSConsole("SDFH", "Value: " + value, true);
                                            //ed.WriteMessage("\nvalue == " + value);
                                        }
                                        catch (System.Exception ex)
                                        {
                                            //ZSharpLogger.DebugHelper.printVSConsole("SDFH", "Exception: " + ex, true);
                                            //System.Windows.MessageBox.Show(ex.Message);
                                        }
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    //ZSharpLogger.DebugHelper.printVSConsole("SDFH", "Exception: " + ex, true);
                                    //System.Windows.MessageBox.Show(ex.Message);
                                }

                                //ed.WriteMessage(string.Join("\t", row.ToArray()));
                                count++;

                            }
                            reader.Close();
                        }
                        //ed.WriteMessage("{0} rows.", count);
                    }
                    con.Close();
                }

            }
            catch (OSGeo.FDO.Common.Exception ex)
            {
                //ZSharpLogger.DebugHelper.printVSConsole("SDFH", "Exception: " + ex, true);
                //ed.WriteMessage("\nException:" + ge.Message);
                //bool ok = ge.Message.Contains("read-only");

                //ed.WriteMessage("no read-only for the schema", ok);
            }


        }

        public static void SDFUpdate(string SDFFile, string schemaName)
        {
            Debug.Write("SDFH", "Update Data module running...");
            try
            {
                IConnection con = SDFHelper.SDFConnection(SDFFile);
                con.Open();
                using (con)
                {
                    using (osgeo_command.Feature.IUpdate update_data = con.CreateCommand(osgeo_command.CommandType.CommandType_Update) as osgeo_command.Feature.IUpdate)
                    {
                        //set the target schema
                        update_data.SetFeatureClassName("Civil_schema:Pipes");
                        //var filter = "('[Name]'='R-1P1')";
                        var filter = "(Name='R-1P1')";
                        update_data.SetFilter(filter);
                        Debug.Write("SDFH", "Updating...");
                        PropertyValueCollection pcoll = update_data.PropertyValues;

                        osgeo_command.PropertyValue propvalue = null;
                        propvalue = new osgeo_command.PropertyValue();

                        //ValueExpression expression = (ValueExpression)Expression.Parse("Iron");
                        ValueExpression expression = new StringValue("Iron");

                        //propvalue.SetName(prop.Name);
                        propvalue = new PropertyValue("PartSizeName", expression);
                        //propvalue.SetName("PipeType");

                        pcoll.Add(propvalue);

                        if (1 != update_data.Execute())
                        {
                            
                            Debug.Fail("test debug fail message");
                        }
                    }

                }

            }
            catch (OSGeo.FDO.Common.Exception ge)
            {
                
                bool ok = ge.Message.Contains("read-only");
                Debug.Write(ge.ToString());
                
            }
            catch (SystemException ex)
            {

                bool ok = ex.Message.Contains("read-only");
                Debug.Write(ex.ToString());

            }

        }

        //still work needs to be done
        public static void SDFReadSpecific(string SDFFile, string col)
        {
            try
            {
                //this is to read a specific value
                IConnection con = SDFHelper.SDFConnection(SDFFile);
                con.Open();
                using (con)
                {
                    using (osgeo_command.Feature.ISelect get_data = con.CreateCommand(osgeo_command.CommandType.CommandType_Select) as osgeo_command.Feature.ISelect)
                    {

                        //set target schema
                        get_data.SetFeatureClassName("Civil_schema:Pipes");

                        //filter data
                        var filter = "(Name='R-1P1')";
                        get_data.SetFilter(filter);

                        //execute the select command
                        using (IFeatureReader reader = get_data.Execute())
                        {
                            //get class defninition
                            ClassDefinition cdef = reader.GetClassDefinition();

                            //print the column name
                            foreach (PropertyDefinition def in cdef.Properties)
                            {
                                if (def is DataPropertyDefinition)
                                {
                                    if (def.Name == col)
                                    {
                                        //get only data properties
                                        var p = (DataPropertyDefinition)def;
                                        string value = "";
                                        //value = reader.GetString(p.Name).ToString();
                                        //ed.WriteMessage("\nValue: " + value);
                                        /*depending upon the datatype use appropriate get method.
                                        switch (p.DataType)
                                        {
                                            case OSGeo.FDO.Schema.DataType.DataType_String:
                                                value = reader.IsNull(p.Name) ? "" : reader.GetString(p.Name).ToString();
                                                break;
                                        }
                                        ed.WriteMessage(value);
                                         */
                                    }
                                    //ed.WriteMessage("\nColumn name : " + def.Name);
                                }

                            }
                            //get the value
                            while (reader.ReadNext())
                            {
                                //for each property print value
                                foreach (PropertyDefinition def in cdef.Properties)
                                {
                                    //only data properties
                                    if (!(def is DataPropertyDefinition)) continue;
                                    var p = (DataPropertyDefinition)def;
                                    string value = "";

                                    //depending upon the data type, use the approriate "GET"
                                    //method on the reader
                                    switch (p.DataType)
                                    {
                                        case OSGeo.FDO.Schema.DataType.DataType_Boolean:
                                            value = reader.IsNull(p.Name) ? "" : reader.GetBoolean(p.Name).ToString();
                                            break;

                                        case OSGeo.FDO.Schema.DataType.DataType_String:
                                            value = reader.IsNull(p.Name) ? "" : reader.GetString(p.Name).ToString();
                                            break;
                                    }
                                    Debug.Write("\nvalue == " + value);

                                }


                            }
                            reader.Close();
                        }

                    }
                }
            }
            catch (OSGeo.FDO.Common.Exception ge)
            {

                bool ok = ge.Message.Contains("read-only");
                Debug.Write(ge.ToString());

            }
            catch (SystemException ex)
            {

                bool ok = ex.Message.Contains("read-only");
                Debug.Write(ex.ToString());

            }
        }

        public static void SDFInsert(string SDFFile, string propertyName, string value, string tableName)
        {
            try
            {
                IConnection con = SDFHelper.SDFConnection(SDFFile);
                con.Open();
                //ITransaction fdotrans = null;
                //if (concap.SupportsTransactions()) fdotrans = con.BeginTransaction();
                //ed.WriteMessage("\nFDO Transaction");

                //implement changes
                //IApplySchema applyschema = (IApplySchema)con.CreateCommand(osgeo_command.CommandType.CommandType_ApplySchema);
                //applyschema.Execute();
                //applyschema.Dispose();

                //creating insert
                using (con)
                {
                    using (osgeo_command.Feature.IInsert insert_comm = con.CreateCommand(osgeo_command.CommandType.CommandType_Insert) as osgeo_command.Feature.IInsert)
                    {

                        //set feature calss
                        insert_comm.SetFeatureClassName("Civil_schema:" + tableName);
                        //string test = insert_comm.FeatureClassName.Name;

                        //ed.WriteMessage("Set class name: " + test);

                        //inserting value
                        PropertyValueCollection valcoll = insert_comm.PropertyValues;

                        PropertyValue pvalue1 = new PropertyValue();



                        insert_comm.PropertyValues.Add(new PropertyValue(propertyName, new StringValue(value)));

                        IReader reader = insert_comm.Execute();
                        if (reader.ReadNext())
                        {

                        }
                        reader.Close();

                    }
                    
                        
                }
                //con.Dispose();
                //con.Close();
                /*ed.WriteMessage("\nInser command created: " + insert_comm.ParameterValues.Count);

                OSGeo.FDO.Commands.PropertyValue pvalue = null;
                pvalue = new osgeo_command.PropertyValue();

                pvalue.SetName("PipeStyle");
                pvalue.Value = new OSGeo.FDO.Expression.StringValue("test string value");

                insert_comm.PropertyValues.Add(pvalue);

                try
                {
                    using(osgeo_command.Feature.IFeatureReader reader = insert_comm.Execute());
                    
                }
                catch(Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    ed.WriteMessage("\nException: " + ex.Message);

                }
            }
                 */
                //if (concap.SupportsTransactions())fdotrans.Commit();
            }
            catch (OSGeo.FDO.Common.Exception ge)
            {

                bool ok = ge.Message.Contains("read-only");
                Debug.Write(ge.ToString());

            }
            catch (SystemException ex)
            {

                bool ok = ex.Message.Contains("read-only");
                Debug.Write(ex.ToString());

            }
        }

        public static void SDFInsert_bulk(string SDFFile, string tableName, IndexedDictionary<string, string[]> data)
        {
            try
            {
                IConnection mConnection = SDFHelper.SDFConnection(SDFFile);
                mConnection.Open();
                IInsert insertCmd = (IInsert)mConnection.CreateCommand(CommandType.CommandType_Insert);

                insertCmd.SetFeatureClassName("Civil_schema:" + tableName);
                
                PropertyValueCollection propValmain = insertCmd.PropertyValues;
                propValmain.Clear();
                //remove coordinates - which is not needed for sdf
                data.Remove("coordinates");
                foreach (var item in data)
                {
                    PropertyValue propVal = new PropertyValue();
                    propVal.Name = new Identifier(item.Key);
                    Expression expValue;
                    string dataType = data[item.Key][0];
                    string value = data[item.Key][1];

                    Debug.Write("\nProperty: " + item.Key);
                    Debug.Write("\ndataType: " + dataType);
                    Debug.Write("\nvalue: " + value);

                    if (data[item.Key][0].Equals("geometry"))
                    {
                        expValue = Expression.Parse(value);

                    }
                    else if (data[item.Key][0].Equals("String"))
                    {
                        Debug.Write("\nInside String");
                        DataType dt = FDODataHelper.getDataType(dataType);
                        expValue = FDODataHelper.ParseByDataType("'" + value + "'", dt);
                    }
                    else
                    {
                        Debug.Write("\nInside Else");
                        DataType dt = FDODataHelper.getDataType(dataType);
                        expValue = FDODataHelper.ParseByDataType(value, dt);

                    }
                    propValmain.Add(propVal);
                    propVal.Value = (ValueExpression)expValue;
                }
                #region MyRegion
                /*
                 * PropertyValueCollection propVals = insertCmd.PropertyValues;
                PropertyValue propVal1 = new PropertyValue();
                propVal1.Name = new Identifier("Name");
                PropertyValue propVal2 = new PropertyValue();
                propVal2.Name = new Identifier("NetworkName");
                PropertyValue propVal3 = new PropertyValue();
                propVal3.Name = new Identifier("PartSizeName");
                //PropertyValue propVal4 = new PropertyValue();
                //propVal4.Name = new Identifier("RimElevation");
                PropertyValue propVal5 = new PropertyValue();
                propVal5.Name = new Identifier("Geometry");

                Expression expr1 = Expression.Parse("'AB'");
                Expression expr2 = Expression.Parse("'Poor'");
                Expression expr4 = Expression.Parse("'Down'");
                Expression expr5 = Expression.Parse("GEOMFROMTEXT('LINESTRING XY (100000.0 100000.0, 200000.0 200000.0, 100000.0 300000.0)')");
                //Int32Value intVal = new Int32Value(0);

                propVals.Clear();
                propVals.Add(propVal1);
                propVals.Add(propVal2);
                propVals.Add(propVal3);
                //propVals.Add(propVal4);
                propVals.Add(propVal5);

                propVal1.Value = (ValueExpression)expr1;
                propVal2.Value = (ValueExpression)expr2;
                //propVal3.Value = (ValueExpression)intVal;
                //propVal4.Value = (ValueExpression)expr4;
                propVal5.Value = (ValueExpression)expr5;
                */
                
                #endregion

                IFeatureReader reader;
                //reader.Dispose();
                reader = insertCmd.Execute();
                reader.Close();

                mConnection.Close();
                Debug.Write("Test_LargeDataVolumeInsert runs successfully !");
            }
            catch (OSGeo.FDO.Common.Exception ge)
            {

                bool ok = ge.Message.Contains("read-only");
                Debug.Write(ge.ToString());

            }
            catch (SystemException ex)
            {

                bool ok = ex.Message.Contains("read-only");
                Debug.Write(ex.ToString());

            }
        }

        public static void SFDInsertC3DPit(string SDFFile, string tableName, IndexedDictionary<string, string[]> data)
        {
            try
            {
                IConnection mConnection = SDFHelper.SDFConnection(SDFFile);
                mConnection.Open();
                IInsert insertCmd = (IInsert)mConnection.CreateCommand(CommandType.CommandType_Insert);

                insertCmd.SetFeatureClassName("Civil_schema:" + tableName);
                PropertyValueCollection propVals = insertCmd.PropertyValues;

                PropertyValue propVal1 = new PropertyValue();
                propVal1.Name = new Identifier("Name");
                PropertyValue propVal2 = new PropertyValue();
                propVal2.Name = new Identifier("NetworkName");
                PropertyValue propVal3 = new PropertyValue();
                propVal3.Name = new Identifier("PartSizeName");
                //PropertyValue propVal4 = new PropertyValue();
                //propVal4.Name = new Identifier("RimElevation");
                PropertyValue propVal5 = new PropertyValue();
                propVal5.Name = new Identifier("Geometry");
                

                Expression expr1 = Expression.Parse("'"+ data["Name"][1] + "'");
                Expression expr2 = Expression.Parse("'" + data["NetworkName"][1] + "'");
                Expression expr3 = Expression.Parse("'" + data["PartSizeName"][1] + "'");
                Expression expr5 = Expression.Parse("GEOMFROMTEXT('" + data["Geometry"][1] + "')");
                //Int32Value intVal = new Int32Value(0);

                propVals.Clear();
                propVals.Add(propVal1);
                propVals.Add(propVal2);
                propVals.Add(propVal3);
                //propVals.Add(propVal4);
                propVals.Add(propVal5);

                propVal1.Value = (ValueExpression)expr1;
                propVal2.Value = (ValueExpression)expr2;
                //propVal3.Value = (ValueExpression)intVal;
                //propVal4.Value = (ValueExpression)expr4;
                propVal5.Value = (ValueExpression)expr5;

                IFeatureReader reader;
                //reader.Dispose();

                for (Int32 counter = 0; counter < 1000; counter++)
                {
                    //intVal.Int32 = counter;
                    reader = insertCmd.Execute();
                    reader.Close();
                }

                mConnection.Close();
                Debug.Write("Test_LargeDataVolumeInsert runs successfully !");
            }
            catch (OSGeo.FDO.Common.Exception ge)
            {

                bool ok = ge.Message.Contains("read-only");
                Debug.Write(ge.ToString());

            }
            catch (SystemException ex)
            {

                bool ok = ex.Message.Contains("read-only");
                Debug.Write(ex.ToString());

            }
        }

        public static void Test_LargeDataVolumeInsert2(string SDFFile, string tableName, IndexedDictionary<string, string[]> data)
        {
            try
            {
                IConnection mConnection = SDFHelper.SDFConnection(SDFFile);
                mConnection.Open();
                IInsert insertCmd = (IInsert)mConnection.CreateCommand(CommandType.CommandType_Insert);

                insertCmd.SetFeatureClassName("Civil_schema:" + tableName);
                PropertyValueCollection propVals = insertCmd.PropertyValues;

                Debug.Write("\n================CHECK INSERT TO SDF ================= " + data.Count());
                Debug.Write("\n==========Dict===");
                foreach (var it in data)
                {
                    Debug.Write("\n== Key: " + "'" + it.Key + "'");
                    Debug.Write("\n== val: " + "'" + it.Value[0] + "'");
                    Debug.Write("\n== val: " + "'" + it.Value[1] + "'");
                }
                Debug.Write("\n==========Dict===");
               
                //Debug.Write("\n== name: " + "'" + data["Name"][1] + "'");
                //Debug.Write("\n== NetworkName: " + "'" + data["NetworkName"][1] + "'");
                //Debug.Write("\n== PartSizeName: " + "'" + data["PartSizeName"][1] + "'");
                //Debug.Write("\n== Geometry: " + "GEOMFROMTEXT('" + data["Geometry"][1] + "')");

                //Expression expr1 = Expression.Parse("'" + data["Name"][1] + "'");
                //Expression expr2 = Expression.Parse("'" + data["NetworkName"][1] + "'");
                //Expression expr3 = Expression.Parse("'" + data["PartSizeName"][1] + "'");
                //Expression expr5 = Expression.Parse("GEOMFROMTEXT('" + data["Geometry"][1] + "')");

                Debug.Write("\n================CHECK INSERT TO SDF =================");


                PropertyValue propVal1 = new PropertyValue();
                propVal1.Name = new Identifier("Name");
                PropertyValue propVal2 = new PropertyValue();
                propVal2.Name = new Identifier("NetworkName");
                PropertyValue propVal3 = new PropertyValue();
                propVal3.Name = new Identifier("PartSizeName");
                //PropertyValue propVal4 = new PropertyValue();
                //propVal4.Name = new Identifier("RimElevation");
                PropertyValue propVal5 = new PropertyValue();
                propVal5.Name = new Identifier("Geometry");

                Expression expr1 = Expression.Parse("'AB'");
                Expression expr2 = Expression.Parse("'Poor'");
                Expression expr4 = Expression.Parse("'Down'");
                Expression expr5 = Expression.Parse("GEOMFROMTEXT('LINESTRING XY (100000.0 100000.0, 200000.0 200000.0, 100000.0 300000.0)')");
                //Int32Value intVal = new Int32Value(0);

                propVals.Clear();
                propVals.Add(propVal1);
                propVals.Add(propVal2);
                propVals.Add(propVal3);
                //propVals.Add(propVal4);
                propVals.Add(propVal5);

                propVal1.Value = (ValueExpression)expr1;
                propVal2.Value = (ValueExpression)expr2;
                //propVal3.Value = (ValueExpression)intVal;
                //propVal4.Value = (ValueExpression)expr4;
                propVal5.Value = (ValueExpression)expr5;

                IFeatureReader reader;
                //reader.Dispose();
                reader = insertCmd.Execute();
                reader.Close();
                

                mConnection.Close();
                Debug.Write("Test_LargeDataVolumeInsert runs successfully !");
            }
            catch (OSGeo.FDO.Common.Exception ge)
            {

                bool ok = ge.Message.Contains("read-only");
                Debug.Write(ge.ToString());

            }
            catch (SystemException ex)
            {

                bool ok = ex.Message.Contains("read-only");
                Debug.Write(ex.ToString());

            }
        }

        public static void Test_LargeDataVolumeInsert(string SDFFile, string tableName)
        {
            try
            {
                IConnection mConnection = SDFHelper.SDFConnection(SDFFile);
                mConnection.Open();
                IInsert insertCmd = (IInsert)mConnection.CreateCommand(CommandType.CommandType_Insert);

                insertCmd.SetFeatureClassName("Civil_schema:" + tableName);
                PropertyValueCollection propVals = insertCmd.PropertyValues;

                PropertyValue propVal1 = new PropertyValue();
                propVal1.Name = new Identifier("Name");
                PropertyValue propVal2 = new PropertyValue();
                propVal2.Name = new Identifier("NetworkName");
                PropertyValue propVal3 = new PropertyValue();
                propVal3.Name = new Identifier("PartSizeName");
                //PropertyValue propVal4 = new PropertyValue();
                //propVal4.Name = new Identifier("RimElevation");
                PropertyValue propVal5 = new PropertyValue();
                propVal5.Name = new Identifier("Geometry");

                Expression expr1 = Expression.Parse("'AB'");
                Expression expr2 = Expression.Parse("'Poor'");
                Expression expr4 = Expression.Parse("'Down'");
                Expression expr5 = Expression.Parse("GEOMFROMTEXT('LINESTRING XY (100000.0 100000.0, 200000.0 200000.0, 100000.0 300000.0)')");
                //Int32Value intVal = new Int32Value(0);

                propVals.Clear();
                propVals.Add(propVal1);
                propVals.Add(propVal2);
                propVals.Add(propVal3);
                //propVals.Add(propVal4);
                propVals.Add(propVal5);

                propVal1.Value = (ValueExpression)expr1;
                propVal2.Value = (ValueExpression)expr2;
                //propVal3.Value = (ValueExpression)intVal;
                //propVal4.Value = (ValueExpression)expr4;
                propVal5.Value = (ValueExpression)expr5;

                IFeatureReader reader;
                //reader.Dispose();

                for (Int32 counter = 0; counter < 1000; counter++)
                {
                    //intVal.Int32 = counter;
                    reader = insertCmd.Execute();
                    reader.Close();
                }
                
                mConnection.Close();
                Debug.Write("Test_LargeDataVolumeInsert runs successfully !");
            }
            catch (OSGeo.FDO.Common.Exception ge)
            {

                bool ok = ge.Message.Contains("read-only");
                Debug.Write(ge.ToString());

            }
            catch (SystemException ex)
            {

                bool ok = ex.Message.Contains("read-only");
                Debug.Write(ex.ToString());

            }
        }

    }
}
