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
using System.IO;
using OSGeo.FDO.Commands.Schema;

namespace ZSharpFDOHelper.SDF
{
    class SDFSchemaManager
    {
        public static void spatial_context(IConnection connect1)
        {
            //create spatial context
            //AcMapMap currentMap = AcMapMap.GetCurrentMap();
            string wkt = "GEOGCS[\"LL-ASTRLA66-Grid\",DATUM[\"ASTRLA66-Grid\",SPHEROID[\"AUSSIE\",6378160.000,298.25000000]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]]";

            osgeo_command.SpatialContext.ICreateSpatialContext cscontext = connect1.CreateCommand(CommandType.CommandType_CreateSpatialContext) as osgeo_command.SpatialContext.ICreateSpatialContext;
            cscontext.CoordinateSystemWkt = wkt;
            cscontext.Name = "";
            cscontext.Extent = new byte[] { 0, 0, 0, 0 };
            cscontext.Description = "Description goes here";
            cscontext.XYTolerance = 0.0;
            cscontext.ZTolerance = 0.0;
            cscontext.Execute();

            //apply schema
            //IApplySchema applysch = connect1.CreateCommand(CommandType.CommandType_ApplySchema) as IApplySchema;
            //applysch.FeatureSchema = Schema;
            //applysch.Execute();

        }

        //add property - this one is new and working...
        public static void add_property(FeatureClass fc, IConnection con)
        {
            //add property to point class
            #region Point
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
            #endregion

            #region Alignment
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
            #endregion
            //AlterationType schema for testing
            #region Parcels
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
            #endregion

            #region Pipes
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
            #endregion

            #region Structures
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

                //actualname property
                fc.Properties.Add(Metadata.get_general_property("actualname"));

                //network name
                fc.Properties.Add(Metadata.get_general_property("network_name"));

                //rim elevation property
                fc.Properties.Add(Metadata.get_structure_property("rim_elevation"));

                //part_size_name property
                fc.Properties.Add(Metadata.get_general_property("part_size_name"));

            } 
            #endregion
        }
        
        
    }
}
