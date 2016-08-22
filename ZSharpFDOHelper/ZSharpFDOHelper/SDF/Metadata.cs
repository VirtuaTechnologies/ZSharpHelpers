using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Connections;
using OSGeo.FDO.Schema;
using OSGeo.MapGuide;
using osgeo_command = OSGeo.FDO.Commands;

namespace ZSharpFDOHelper.SDF
{
    class Metadata
    {
        public static DataPropertyDefinition get_point_property(string i)
        {
            //add properties here
            DataPropertyDefinition point_property = new DataPropertyDefinition();

            switch (i)
            {
                case "number":
                    DataPropertyDefinition number_prop = new DataPropertyDefinition("Number", "Number property");
                    number_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_Int16;
                    number_prop.ReadOnly = false;
                    number_prop.Nullable = true;
                    point_property = number_prop;
                    break;
            }
            return point_property;
        }

        public static GeometricPropertyDefinition get_geo_property(string i)
        {
            //this will return a geometry property type
            //geometry property
            GeometricPropertyDefinition geom = new GeometricPropertyDefinition();
            switch (i)
            {
                //geometry property - point
                case "point":
                    GeometricPropertyDefinition geo_point = new GeometricPropertyDefinition("Geometry", "The geometry of the object");
                    geo_point.GeometryTypes = MgGeometryType.Point;
                    geom = geo_point;
                    break;

                //geometry property - curve
                case "curve":
                    GeometricPropertyDefinition geo_curve = new GeometricPropertyDefinition("Geometry", "The geometry of the object");
                    geo_curve.GeometryTypes = 2;
                    geom = geo_curve;
                    break;

                //geometry property - surface
                case "surface":
                    GeometricPropertyDefinition geo_surface = new GeometricPropertyDefinition("Geometry", "The geometry of the object");
                    geo_surface.GeometryTypes = 4;
                    geom = geo_surface;
                    break;
            }
            return geom;
        }

        public static void spatial_context(IConnection con)
        {
            string wkt = "GEOGCS[\"LL-ASTRLA66-Grid\",DATUM[\"ASTRLA66-Grid\",SPHEROID[\"AUSSIE\",6378160.000,298.25000000]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]]";

            osgeo_command.SpatialContext.ICreateSpatialContext cscontext = con.CreateCommand(CommandType.CommandType_CreateSpatialContext) as osgeo_command.SpatialContext.ICreateSpatialContext;
            cscontext.CoordinateSystemWkt = wkt;
            cscontext.Name = "CoOrdinate";
            cscontext.Extent = new byte[] { 0, 0, 0, 0 };
            cscontext.Description = "Description goes here";
            cscontext.XYTolerance = 0.0;
            cscontext.ZTolerance = 0.0;
            cscontext.Execute();

        }

        public static DataPropertyDefinition get_alignment_property(string i)
        {
            DataPropertyDefinition alignment_property = new DataPropertyDefinition();
            switch (i)
            {
                case "starting_station":
                    DataPropertyDefinition starting_station_prop = new DataPropertyDefinition("StartingStation", "StartingStation property");
                    starting_station_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_Double;
                    starting_station_prop.ReadOnly = false;
                    starting_station_prop.Nullable = true;
                    alignment_property = starting_station_prop;
                    break;

                case "ending_station":
                    DataPropertyDefinition ending_station_prop = new DataPropertyDefinition("EndingStation", "EndingStation property");
                    ending_station_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_Double;
                    ending_station_prop.ReadOnly = false;
                    ending_station_prop.Nullable = true;
                    alignment_property = ending_station_prop;
                    break;

                case "design_speed":
                    DataPropertyDefinition design_speed_prop = new DataPropertyDefinition("DesignSpeed", "DesignSpeed property");
                    design_speed_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_Double;
                    design_speed_prop.ReadOnly = false;
                    design_speed_prop.Nullable = true;
                    alignment_property = design_speed_prop;
                    break;
            }

            return alignment_property;
        }

        public static DataPropertyDefinition get_general_property(string i)
        {
            //this is will have general properties which can be used in all classes

            DataPropertyDefinition general_property = new DataPropertyDefinition();

            switch (i)
            {
                //auto increment id
                case "id":
                    DataPropertyDefinition auto_id = new DataPropertyDefinition("Autogenerated_SDF_ID", "Autogenerated identity property");
                    auto_id.DataType = OSGeo.FDO.Schema.DataType.DataType_Int32;
                    auto_id.IsAutoGenerated = true;
                    auto_id.ReadOnly = true;
                    auto_id.Nullable = false;
                    general_property = auto_id;
                    break;

                //Name property
                case "name":
                    DataPropertyDefinition name_prop = new DataPropertyDefinition("Name", "Name Property");
                    name_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    name_prop.ReadOnly = false;
                    name_prop.Nullable = true;
                    name_prop.Length = 255;
                    general_property = name_prop;
                    break;

                //actual property
                case "actualname":
                    DataPropertyDefinition actualname_prop = new DataPropertyDefinition("ActualName", "Actual Name Property");
                    actualname_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    actualname_prop.ReadOnly = false;
                    actualname_prop.Nullable = true;
                    actualname_prop.Length = 255;
                    general_property = actualname_prop;
                    break;

                //Lattitude  property
                case "latitude":
                    DataPropertyDefinition lattitude_prop = new DataPropertyDefinition("Latitude", "Latitude Property");
                    lattitude_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    lattitude_prop.ReadOnly = false;
                    lattitude_prop.Nullable = true;
                    general_property = lattitude_prop;
                    break;

                //Longitude  property
                case "longitude":
                    DataPropertyDefinition longitude_prop = new DataPropertyDefinition("Longitude", "Longitude Property");
                    longitude_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    longitude_prop.ReadOnly = false;
                    longitude_prop.Nullable = true;
                    general_property = longitude_prop;
                    break;

                //length property
                case "length":
                    DataPropertyDefinition length_prop = new DataPropertyDefinition("Length", "Length property");
                    length_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_Double;
                    length_prop.ReadOnly = false;
                    length_prop.Nullable = true;
                    general_property = length_prop;
                    break;

                case "elevation":
                    DataPropertyDefinition elevation_prop = new DataPropertyDefinition("Elevation", "Elevation property");
                    elevation_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_Double;
                    elevation_prop.ReadOnly = false;
                    elevation_prop.Nullable = true;
                    general_property = elevation_prop;
                    break;

                //Description property
                case "description":
                    DataPropertyDefinition desc_prop = new DataPropertyDefinition("Description", "");
                    desc_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    desc_prop.ReadOnly = false;
                    desc_prop.Nullable = true;
                    desc_prop.Length = 255;
                    general_property = desc_prop;
                    break;

                //raw Description property
                case "raw_description":
                    DataPropertyDefinition rw_desc_prop = new DataPropertyDefinition("RawDescription", "");
                    rw_desc_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    rw_desc_prop.ReadOnly = false;
                    rw_desc_prop.Nullable = true;
                    rw_desc_prop.Length = 255;
                    general_property = rw_desc_prop;
                    break;

                //changing datatype from double to string to cehck insert and updte
                case "area":
                    DataPropertyDefinition area_prop = new DataPropertyDefinition("Area", "Area property");
                    area_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    area_prop.ReadOnly = false;
                    area_prop.Nullable = true;
                    area_prop.Length = 255;
                    general_property = area_prop;
                    break;
                //changing datatype from double to string to cehck insert and updte
                case "perimeter":
                    DataPropertyDefinition perimeter_prop = new DataPropertyDefinition("Perimeter", "Perimeter property");
                    perimeter_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    perimeter_prop.ReadOnly = false;
                    perimeter_prop.Nullable = true;
                    perimeter_prop.Length = 255;
                    general_property = perimeter_prop;
                    break;

                case "network_name":
                    DataPropertyDefinition network_name_prop = new DataPropertyDefinition("NetworkName", "NetworkName property");
                    network_name_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    network_name_prop.ReadOnly = false;
                    network_name_prop.Nullable = true;
                    general_property = network_name_prop;
                    break;

                case "slope":
                    DataPropertyDefinition slope_prop = new DataPropertyDefinition("Slope", "Slope property");
                    slope_prop.DataType = OSGeo.FDO.Schema.DataType.DataType_Double;
                    slope_prop.ReadOnly = false;
                    slope_prop.Nullable = true;
                    general_property = slope_prop;
                    break;

                case "part_size_name":
                    DataPropertyDefinition part_size_name = new DataPropertyDefinition("PartSizeName", "PartSizeName of the pipe");
                    part_size_name.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    part_size_name.Nullable = true;
                    part_size_name.ReadOnly = false;
                    general_property = part_size_name;
                    break;

            }
            return general_property;

        }

        public static DataPropertyDefinition get_pipe_property(string i)
        {
            //add properties here.
            DataPropertyDefinition pipe_property = new DataPropertyDefinition();

            switch (i)
            {
                case "inside_dia":
                    DataPropertyDefinition inside_dia = new DataPropertyDefinition("InsideDiameter", "InsideDiameter of the pipe");
                    inside_dia.DataType = OSGeo.FDO.Schema.DataType.DataType_Double;
                    inside_dia.Nullable = true;
                    inside_dia.ReadOnly = false;
                    pipe_property = inside_dia;
                    break;

                case "outside_dia":
                    DataPropertyDefinition outside_dia = new DataPropertyDefinition("OutsideDiameter", "OutsideDiameter of the pipe");
                    outside_dia.DataType = OSGeo.FDO.Schema.DataType.DataType_Double;
                    outside_dia.Nullable = true;
                    outside_dia.ReadOnly = false;
                    pipe_property = outside_dia;
                    break;

                case "start_invert":
                    DataPropertyDefinition start_invert = new DataPropertyDefinition("StartInvert", "StartInvert of the pipe");
                    start_invert.DataType = OSGeo.FDO.Schema.DataType.DataType_Double;
                    start_invert.Nullable = true;
                    start_invert.ReadOnly = false;
                    pipe_property = start_invert;
                    break;

                case "end_invert":
                    DataPropertyDefinition end_invert = new DataPropertyDefinition("EndInvert", "EndInvert of the pipe");
                    end_invert.DataType = OSGeo.FDO.Schema.DataType.DataType_Double;
                    end_invert.Nullable = true;
                    end_invert.ReadOnly = false;
                    pipe_property = end_invert;
                    break;

                case "structure_start":
                    DataPropertyDefinition structure_start = new DataPropertyDefinition("StructureStart", "StructureStart of the pipe");
                    structure_start.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    structure_start.Nullable = true;
                    structure_start.ReadOnly = false;
                    pipe_property = structure_start;
                    break;

                case "structure_end":
                    DataPropertyDefinition structure_end = new DataPropertyDefinition("StructureEnd", "StructureEnd of the pipe");
                    structure_end.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    structure_end.Nullable = true;
                    structure_end.ReadOnly = false;
                    pipe_property = structure_end;
                    break;

                case "pipe_radius":
                    DataPropertyDefinition pipe_radius = new DataPropertyDefinition("PipeRadius", "Radius of the pipe");
                    pipe_radius.DataType = OSGeo.FDO.Schema.DataType.DataType_Double;
                    pipe_radius.Nullable = true;
                    pipe_radius.ReadOnly = false;
                    pipe_property = pipe_radius;
                    break;

                case "pipe_material":
                    DataPropertyDefinition pipe_material = new DataPropertyDefinition("PipeMaterial", "Material of the pipe");
                    pipe_material.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    pipe_material.Nullable = true;
                    pipe_material.ReadOnly = false;
                    pipe_property = pipe_material;
                    break;

                case "pipe_alignment":
                    DataPropertyDefinition pipe_Al = new DataPropertyDefinition("PipeAlignment", "Alignment associated with the pipe");
                    pipe_Al.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    pipe_Al.Nullable = true;
                    pipe_Al.ReadOnly = false;
                    pipe_property = pipe_Al;
                    break;

                case "pipe_style":
                    DataPropertyDefinition pipe_style = new DataPropertyDefinition("PipeStyle", "Pipe Style");
                    pipe_style.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    pipe_style.Nullable = true;
                    pipe_style.ReadOnly = false;
                    pipe_property = pipe_style;
                    break;

                case "connected_structure":
                    DataPropertyDefinition Connected_strcuture = new DataPropertyDefinition("ConnectedStrcutureCount", "Number of structure the pipe is connected");
                    Connected_strcuture.DataType = OSGeo.FDO.Schema.DataType.DataType_Int16;
                    Connected_strcuture.Nullable = true;
                    Connected_strcuture.ReadOnly = false;
                    pipe_property = Connected_strcuture;
                    break;

                case "pipe_description":
                    DataPropertyDefinition Pipe_description = new DataPropertyDefinition("PipeDescription", "Description of the pipe");
                    Pipe_description.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    Pipe_description.Nullable = true;
                    Pipe_description.ReadOnly = false;
                    pipe_property = Pipe_description;
                    break;

                case "pipe_type":
                    DataPropertyDefinition Pipe_type = new DataPropertyDefinition("PipeType", "type of pipe");
                    Pipe_type.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    Pipe_type.Nullable = true;
                    Pipe_type.ReadOnly = false;
                    pipe_property = Pipe_type;
                    break;

                case "pipe_flow_direction":
                    DataPropertyDefinition Pipe_flow_direction = new DataPropertyDefinition("PipeFlowDirection", "type of pipe");
                    Pipe_flow_direction.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    Pipe_flow_direction.Nullable = true;
                    Pipe_flow_direction.ReadOnly = false;
                    pipe_property = Pipe_flow_direction;
                    break;

                case "surface":
                    DataPropertyDefinition Pipe_Surface = new DataPropertyDefinition("Associatedsurface", "Surface associated with the pipe");
                    Pipe_Surface.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    Pipe_Surface.Nullable = true;
                    Pipe_Surface.ReadOnly = false;
                    pipe_property = Pipe_Surface;
                    break;

                default:
                    break;
            }

            return pipe_property;
        }

        public static DataPropertyDefinition get_structure_property(string i)
        {
            //add properties here.
            DataPropertyDefinition structure_property = new DataPropertyDefinition();

            switch (i)
            {
                case "rim_elevation":
                    DataPropertyDefinition rim_elevation = new DataPropertyDefinition("RimElevation", "RimElevation of the Structure");
                    rim_elevation.DataType = OSGeo.FDO.Schema.DataType.DataType_Double;
                    rim_elevation.Nullable = true;
                    rim_elevation.ReadOnly = false;
                    structure_property = rim_elevation;
                    break;

                case "structure_description":
                    DataPropertyDefinition structure_description = new DataPropertyDefinition("StructureDescription", "Description of the Structure");
                    structure_description.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    structure_description.Nullable = true;
                    structure_description.ReadOnly = false;
                    structure_property = structure_description;
                    break;

                case "structure_domain":
                    DataPropertyDefinition structure_domain = new DataPropertyDefinition("StructureDomain", "Structure Domain");
                    structure_domain.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    structure_domain.Nullable = true;
                    structure_domain.ReadOnly = false;
                    structure_property = structure_domain;
                    break;

                case "structure_material":
                    DataPropertyDefinition structure_material = new DataPropertyDefinition("StructureMaterial", "Structure Material");
                    structure_material.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    structure_material.Nullable = true;
                    structure_material.ReadOnly = false;
                    structure_property = structure_material;
                    break;

                case "structure_type":
                    DataPropertyDefinition structure_type = new DataPropertyDefinition("StructureType", "Structure Type");
                    structure_type.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    structure_type.Nullable = true;
                    structure_type.ReadOnly = false;
                    structure_property = structure_type;
                    break;

                case "structure_Alignment":
                    DataPropertyDefinition structure_Alignment = new DataPropertyDefinition("StructureAlignment", "Alignment associated with the strcuture");
                    structure_Alignment.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    structure_Alignment.Nullable = true;
                    structure_Alignment.ReadOnly = false;
                    structure_property = structure_Alignment;
                    break;

                case "structure_surface":
                    DataPropertyDefinition structure_surface = new DataPropertyDefinition("Associatedsurface", "Surface associated with the strcuture");
                    structure_surface.DataType = OSGeo.FDO.Schema.DataType.DataType_String;
                    structure_surface.Nullable = true;
                    structure_surface.ReadOnly = false;
                    structure_property = structure_surface;
                    break;

                default:
                    break;
            }

            return structure_property;
        }
    }
}
