using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IndexedDictionary;

namespace ZSharpTextHelper.Global
{
    class ZTHvariable
    {
        public static IndexedDictionary<string, object> stringDrainages = new IndexedDictionary<string, object>();
        public static IndexedDictionary<string, object> stringDrainages_Collection = new IndexedDictionary<string, object>();
        //public static IndexedDictionary<string, stringDrainageData> stringDrainages = new IndexedDictionary<string, stringDrainageData>();
        public static IndexedDictionary<string, string> linenums_stringdriange = new IndexedDictionary<string, string>();
        public static IndexedDictionary<string, string> SDDPipe_properties = new IndexedDictionary<string, string>();
        public static List<IndexedDictionary<string, string[]>> pit_dataColl = new List<IndexedDictionary<string, string[]>>();
        public static List<IndexedDictionary<string, string[]>> pipe_dataColl = new List<IndexedDictionary<string, string[]>>();
        public static IndexedDictionary<string, string> sddate_sqlite = new IndexedDictionary<string, string>();
        public static IndexedDictionary<string, string> sddate_sqlite_insert = new IndexedDictionary<string, string>();
        public static string sdfFile;
        public static string SHPFolder;
        public static string TDFile;
        public static Int64 ExtractionMethod;
        //public static SDDCollection SDDColl = new SDDCollection();

        public static void initializeProps()
        {
            #region initializePipeProps
            Global.ZTHvariable.linenums_stringdriange.Clear();
            Global.ZTHvariable.stringDrainages_Collection.Clear();
            Global.ZTHvariable.stringDrainages.Clear();
            SDDPipe_properties.Clear();
            Global.ZTHvariable.SDDPipe_properties.Add("name", "name");
            Global.ZTHvariable.SDDPipe_properties.Add("type", "type");
            Global.ZTHvariable.SDDPipe_properties.Add("diameter", "diameter");
            Global.ZTHvariable.SDDPipe_properties.Add("us_level", "us_level");
            Global.ZTHvariable.SDDPipe_properties.Add("ds_level", "ds_level");
            Global.ZTHvariable.SDDPipe_properties.Add("pipe_size", "\"pipe size\"");
            Global.ZTHvariable.SDDPipe_properties.Add("length", "\"calculated pipe length\"");
            Global.ZTHvariable.SDDPipe_properties.Add("grade", "\"calculated pipe grade\"");
            Global.ZTHvariable.SDDPipe_properties.Add("toStructure", "\"ds pit index\"");
            Global.ZTHvariable.SDDPipe_properties.Add("fromStructure", "\"us pit index\"");

            /*
             * 
            foreach (var prop in Global.variable.SDDPipe_properties)
                    {
                        Debug.Write("Props: " + prop.Key + " | " + prop.Value);

                    }
            Global.SDDPit pitObj = new Global.SDDPit();

            foreach (var prop in pitObj.GetType().GetProperties())
            {
                if(true)
                {

                }
                else
                {
                    Global.variable.SDDPipe_properties.Add(prop.Name, prop.Name);
                }
            }
             */

            #endregion
        }
    }

    /*
    public class SDDCollection
    {
        //public string SDDName { get; set; }
        public string SDDID { get; set; }
        public string this_id { get; set; }
        public string name { get; set; }
        public stringDrainageData stringDrainageData { get; set; }
    }
    */

    public class stringDrainageData
    {
        //public string SDDName { get; set; }
        public string SDDID { get; set; }
        public string name { get; set; }
        public string this_id { get; set; }
        public string us_pit_index { get; set; }
        public string ds_pit_index { get; set; }
        public string number_of_us_trunks { get; set; }
        public string number_of_us_incoming { get; set; }
        public string us_incoming { get; set; }
        public string us_incoming_pit_index { get; set; }
        public string number_of_us_outgoing { get; set; }
        public string number_of_ds_trunks { get; set; }
        public string ds_trunk { get; set; }
        public string ds_trunk_pit_index { get; set; }
        public string number_of_ds_incoming { get; set; }
        public string ds_incoming { get; set; }
        public string ds_incoming_pit_index { get; set; }
        public string ds_incoming2 { get; set; }
        public string ds_incoming_pit_index2 { get; set; }
        public string number_of_ds_outgoing { get; set; }
    }

    public class SDDPitsCollection
    {
        public string SDDName { get; set; }
        public string SDDID { get; set; }
        public SDDPit pipeData { get; set; }
    }

    public class SDDPipesCollection
    {
        public string SDDName { get; set; }
        public string SDDID { get; set; }
        public SDDPipe pipeData { get; set; }
    }


    public class SDDPit
    {
        public string actualname { get; set; }
        public string type { get; set; }
        public string diameter { get; set; }
        public string width { get; set; }
        private string _id = string.Empty;
        public string ip
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;//(Convert.ToInt32(value) + 1).ToString();
            }
        }
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
        public string us_pit_index { get; set; }
        public string ds_pit_index { get; set; }
        public string number_of_us_trunks { get; set; }
        public string number_of_incoming { get; set; }
        public string incoming_uid { get; set; }
        public string incoming_pit_index { get; set; }
        public string number_of_outgoing { get; set; }
        public string us_pit_string_uid { get; set; }
        public string data_us_pit_string_uid { get; set; }
        public string data_us_pit_index { get; set; }
        public string ds_pit_string_uid { get; set; }
        public string data_ds_pit_string_uid { get; set; }
        public string data_ds_pit_index { get; set; }
        public string name { get; set; } //have this at the end since the ip needs to be save to class obj first.
    }
    public class SDDPipe
    {
        public string name { get; set; }
        public string type { get; set; }
        public string diameter { get; set; }
        public string us_level { get; set; }
        public string ds_level { get; set; }
        //public string us_pit_index { get; set; } to struct
        //public string ds_pit_index { get; set; } from struct
        //public string flow_velocity { get; set; }
        //public string flow_volume { get; set; }
        public string pipe_size { get; set; } //"pipe size"
        public string length { get; set; } //"calculated pipe length" 
        public string grade { get; set; } //"calculated pipe grade"   0.362525
        public string toStructure { get; set; } //"us pit index"
        public string fromStructure { get; set; } //"ds pit index"

        /*
        public SDDPipe(string _fromStructure, string _toStructure, string _grade, string _name, string _type, string _diameter, string _us_level, string _ds_level, string _us_hgl, string _ds_hgl, string _flow_velocity, string _flow_volume, string _invert_us, string _invert_ds, string _pipe_size, string _length)
        {
            name = _name;
            type = _type;
            diameter = _diameter;
            us_level = _us_level;
            ds_level = _ds_level;
            us_hgl = _us_hgl;
            ds_hgl = _ds_hgl;
            flow_volume = _flow_volume;
            flow_velocity = _flow_velocity;
            invert_us = _invert_us;
            invert_ds = _invert_ds;
            pipe_size = _pipe_size;
            length = _length;
            grade = _grade;
            toStructure = _toStructure;
            fromStructure = _fromStructure;
        }
         */
    }


}
