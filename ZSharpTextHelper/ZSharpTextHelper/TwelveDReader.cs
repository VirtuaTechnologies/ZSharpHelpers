
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using IndexedDictionary;
using SDFHelper = ZSharpFDOHelper.SDFHelper;
using SHPHelper = ZSharpFDOHelper.SHP.SHPHelper;
using SDFCRUD = ZSharpFDOHelper.SDFCRUD;
using FDOGenHelper = ZSharpFDOHelper.FDOGen.FDODataHelper;

namespace ZSharpTextHelper
{
    public class TwelveDReader
    {
        #region Variables
        private static List<Int64> linenums = new List<Int64>();
        private static bool collect = false;
        private static string linenum_values;
        private static string stringVal;
        private static IndexedDictionary<string, string> result = new IndexedDictionary<string, string>();
        private static IndexedDictionary<string, string> result_pit = new IndexedDictionary<string, string>();
        private static IndexedDictionary<string, string> pit_lineNums = new IndexedDictionary<string, string>();
        private static IndexedDictionary<string, string> pipe_lineNums = new IndexedDictionary<string, string>();
        private static IndexedDictionary<string, Global.stringDrainageData> SDDataDict = new IndexedDictionary<string, Global.stringDrainageData>();
        private static List<string> completed_props = new List<string>();
        private static Global.SDDPit fromStructure;
        private static Global.SDDPit toStructure;
        private static Global.stringDrainageData SDData;
        private static string res; 
        #endregion

        public static string extractTDFile(List<string> output_params)
        {
            SQL_Helper_Local.DBChecker();

            Debug.Write("\nFORMAT >>>: " + output_params[0] + "| " + output_params[1] + "| " + output_params[2] + " | " + output_params[3] + "\n");
            Global.ZTHvariable.TDFile = output_params[3];
            Global.ZTHvariable.ExtractionMethod = GenTextHelper.ExtractNumberfromString(output_params[4]);

            if (output_params[0].Equals("SDF"))
            {
                Global.ZTHvariable.sdfFile = output_params[2];

                Debug.Write("\nSDFFile2: " + Global.ZTHvariable.sdfFile);
                get12DaDrainage(output_params);
            }
            else if (output_params[0].Equals("SHP"))
            {
                Global.ZTHvariable.SHPFolder = output_params[2];
                get12DaDrainage(output_params);
                //get12DaDrainage(TDfile, output_params[0]);
            }
            
            //check the final dictionary and the class
            

            //Now return dict with objs
            return "Success";
        }

        public static IndexedDictionary<string, object> get12DaDrainage(List<string> output_params)
        {
            //get each string drainage and seperate string.
            try
            {
                #region Initial
                // initializePipeProps
                Global.ZTHvariable.initializeProps();
                pit_lineNums.Clear();
                pipe_lineNums.Clear();
                //first get all the start and end line numbers of all the string drainage instance
                startendLineNumbers("SD", Global.ZTHvariable.TDFile, "string drainage", Global.ZTHvariable.linenums_stringdriange);
                Global.ZTHvariable.pit_dataColl.Clear();
                result.Clear();
                //Global.ZTHvariable.linenums_stringdriange.Clear();
                //Global.ZTHvariable.stringDrainages.Clear();
                #endregion

                #region Get All Line number details
                //inside each drinage get number of pipes and pits
                //get ID values of from all string drainage
                   
                Int64 i = 1;
                foreach (var val in Global.ZTHvariable.linenums_stringdriange)
                {
                    //extract atribute block from each string drianage
                    Debug.Write("\nWWWWWWWWWWWWWWW");
                    string SDID = "SD-" + i;
                    getSSData(SDID, getStringBetweenLineNumbers(Global.ZTHvariable.TDFile, Convert.ToInt64(GenTextHelper.Split_csv_get_specific_dl(val.Value, 1, ',')), Convert.ToInt64(GenTextHelper.Split_csv_get_specific_dl(val.Value, 2, ','))));
                    Debug.Write("\nWWWWWWWWWWWWWWW");
                    //SDData.SDDName = val.Key;
                    //SDData.SDDID = 
                    Debug.Write("SSD: " + val.Key + "||" + val.Value + "\n");
                    Debug.Write("\nSD" + i + " : " + Convert.ToInt64(GenTextHelper.Split_csv_get_specific_dl(val.Value, 1, ',')) + " || " + Convert.ToInt64(GenTextHelper.Split_csv_get_specific_dl(val.Value, 2, ',')));

                    startendLineNumberswithLimts("SD-" + i, "pit", Global.ZTHvariable.TDFile, "pit {", Convert.ToInt64(GenTextHelper.Split_csv_get_specific_dl(val.Value, 1, ',')), Convert.ToInt64(GenTextHelper.Split_csv_get_specific_dl(val.Value, 2, ',')), pit_lineNums);
                    startendLineNumberswithLimts("SD-" + i, "pipe", Global.ZTHvariable.TDFile, "pipe {", Convert.ToInt64(GenTextHelper.Split_csv_get_specific_dl(val.Value, 1, ',')), Convert.ToInt64(GenTextHelper.Split_csv_get_specific_dl(val.Value, 2, ',')), pipe_lineNums);
                    i++;
                }

                Debug.Write("\nFile: " + Global.ZTHvariable.TDFile);

                Debug.Write("\n==================Final===================");
                #endregion

                //print all the line numbers to check
                #region PIT Extractor
                
                foreach (var item in pit_lineNums)
                {
                    Debug.Write("\n=========================================");
                    Debug.Write("\n==================PIT===================");
                    string networkName = ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 1, '-') + "-" + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 2, '-');
                    string pitName_raw = ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 1, '-') + "-" + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 2, '-') + "-";
                    Global.SDDPit pitObj = getPitObject(pitName_raw, getStringBetweenLineNumbers(Global.ZTHvariable.TDFile, Convert.ToInt64(GenTextHelper.Split_csv_get_specific_dl(item.Value, 1, ',')), Convert.ToInt64(GenTextHelper.Split_csv_get_specific_dl(item.Value, 2, ','))));

                    //add the class object ot dictionary
                    Global.ZTHvariable.stringDrainages.Add(item.Key, pitObj);
                    //If Method 1 - add ip and pit obj
                    //if Method 2 - add actual name and pit obj
                    if (Global.ZTHvariable.ExtractionMethod == 1)
                    {
                        Debug.Write("\n Dict Addition KEY => " + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 1, '-') + "-" + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 2, '-') + "-" + pitObj.ip);
                        Debug.Write("\n - PIT " + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 1, '-') + "-" + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 2, '-') + "-" + pitObj.ip + "\n");
                        if (!Global.ZTHvariable.stringDrainages_Collection.Contains(ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 1, '-') + "-" + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 2, '-') + "-" + pitObj.ip))
                            Global.ZTHvariable.stringDrainages_Collection.Add(ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 1, '-') + "-" + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 2, '-') + "-" + pitObj.ip, pitObj);
                    }
                    else
                    {
                        Debug.Write("\n Dict Addition KEY => " + pitObj.actualname);
                        Debug.Write("\n - PIT " + pitObj.name + " || " + pitObj.ip + "\n");
                        if (!Global.ZTHvariable.stringDrainages_Collection.Contains(pitObj.actualname))
                            Global.ZTHvariable.stringDrainages_Collection.Add(networkName+ "-" + pitObj.actualname, pitObj);
                    }
                    
                    Debug.Write("\n COORD: " + pitObj.x + " | " + pitObj.y + "\n");
                    Debug.Write("\n Pit IP=> " + pitObj.ip + " || " + pitObj.name);
                    //store pit object with respect to their ip so it can extracted and used for distance calculation.

                    #region load data and output file
                    //load to sdf
                    string geom = string.Format("GEOMFROMTEXT('POINT ({0} {1})')", pitObj.x, pitObj.y);
                    Int64 ip = Convert.ToInt64(pitObj.ip);
                    IndexedDictionary<string, string[]> dataVal = new IndexedDictionary<string, string[]>();
                    dataVal.Add("Geometry", new string[] { "geometry", geom });
                    dataVal.Add("Name", new string[] { "String", ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 1, '-') + "-" + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 2, '-') + "-" + ip.ToString() }); //.Replace(" ", string.Empty) <---if spaced needs to be rmoved use this
                    dataVal.Add("NetworkName", new string[] { "String", ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 1, '-') + "-" + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 2, '-') });
                    dataVal.Add("PartSizeName", new string[] { "String", pitObj.type });
                    dataVal.Add("ActualName", new string[] { "String", networkName + "-" + pitObj.actualname });
                    dataVal.Add("RimElevation", new string[] { "Double", pitObj.z });
                    dataVal.Add("Northing", new string[] { "Double", pitObj.x });
                    dataVal.Add("Easting", new string[] { "Double", pitObj.y });

                    //create respective file formats
                    if (output_params[0].Equals("SDF"))
                    {
                        SDFCRUD.SDFInsert_bulk(Global.ZTHvariable.sdfFile, "Structures", dataVal);
                        Debug.Write("\nSDFFile2: " + Global.ZTHvariable.sdfFile);
                    }
                    else if (output_params[0].Equals("SHP"))
                    {
                        Global.ZTHvariable.pit_dataColl.Add(dataVal);
                       
                        //get12DaDrainage(TDfile, output_params[0]);
                    }
                    
                    
                    #region Debug Code
                     //Debug code
                     Debug.Write("\nWriting to OUTPUTFILE .."); //@"E:\GoogleDrive\Zcodia Labs\Virtua\ZLABPRJ02- Virtua Map\Source\12DaTool\TestFiles\21.sdf"
                     Debug.Write("\n==========Dict==========");
                    foreach (var it in dataVal)
                    {
                        Debug.Write("\n== Key: " + "'" + it.Key + "'");
                        Debug.Write("\n== DataType: " + "'" + it.Value[0] + "'");
                        Debug.Write("\n== val: " + "'" + it.Value[1] + "'");
                    }
                    Debug.Write("\n==========Dict==========" + Global.ZTHvariable.sdfFile);
                    Debug.Write("\n=========================================");
                    Debug.Write("\n------PIT-CLASS---------------");
                    //TEST THE CLASS
                    foreach (var prop in pitObj.GetType().GetProperties())
                    {
                        Debug.Write("\n---->Property: " + prop.Name);
                        Debug.Write("\n---->Value : " + prop.GetValue(pitObj, null));
                    }
                    
                    Debug.Write("\n=========================================");
                    Debug.Write("\nWriting to SDF");
                    /**/

                    #endregion

                    #endregion
                }

                if (output_params[0].Equals("SHP"))
                {
                    //send the complete data collection to shp load
                    SHPHelper.createSHPPointBulk(Global.ZTHvariable.SHPFolder + @"\Structure.shp", "Strcutures", Global.ZTHvariable.pit_dataColl);
                }
                #endregion

                #region Pipe Extractor
                foreach (var item in pipe_lineNums)
                {
                    Debug.Write("\n=========================================");
                    Debug.Write("\n==================PIPE===================");
                    Debug.Write("\n - PIPE " + item.Key + " || " + item.Value);
                    Global.SDDPipe pipeObj = getPipeObject(getStringBetweenLineNumbers(Global.ZTHvariable.TDFile, Convert.ToInt64(GenTextHelper.Split_csv_get_specific_dl(item.Value, 1, ',')), Convert.ToInt64(GenTextHelper.Split_csv_get_specific_dl(item.Value, 2, ','))));
                    string networkName = ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 1, '-') + "-" + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 2, '-');
                    Debug.Write("\n == NetworkName: >> " + networkName);
                    //add the class object ot dictionary
                    Global.ZTHvariable.stringDrainages.Add(item.Key, pipeObj);
                    
                    #region load to sdf
                    //Debub Code - get all Global.ZTHvariable.stringDrainages_Collection values and keys
                    //Extraction Method 1 - IP, Extraction Method 2 - Name (Actual name)
                    
                    foreach (var valte in output_params)
                    {
                        Debug.Write("\n----------------------Params---------------- " + valte.IndexOf(valte) + " | " + valte);
                    }
                    
                    
                    if (Global.ZTHvariable.ExtractionMethod == 1)
                    {
                        Debug.Write("\n----------------------Extraction Method---------------- " + output_params[4] + " | " + Global.ZTHvariable.ExtractionMethod);
                        Debug.Write("\n ==> From Structure: " + pipeObj.fromStructure);
                        Debug.Write("\n ==> To Structure: " + pipeObj.toStructure);
                        Debug.Write("\n == Found from Structure: >> " + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 1, '-') + "-" + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 2, '-') + "-" + pipeObj.fromStructure + "\n");
                        Debug.Write("\n == Found to Structure: >> " + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 1, '-') + "-" + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 2, '-') + "-" + pipeObj.toStructure + "\n");
                        fromStructure = (Global.SDDPit)Global.ZTHvariable.stringDrainages_Collection[ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 1, '-') + "-" + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 2, '-') + "-" + pipeObj.fromStructure];
                        toStructure = (Global.SDDPit)Global.ZTHvariable.stringDrainages_Collection[ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 1, '-') + "-" + ZSharpTextHelper.GenTextHelper.Split_csv_get_specific_dl(item.Key, 2, '-') + "-" + pipeObj.toStructure];
                    }
                    else
                    {
                        Debug.Write("\n----------------------Extraction Method---------------- " + output_params[4] + " | " + Global.ZTHvariable.ExtractionMethod);
                        Debug.Write("\n ==> PipeName: " + pipeObj.name);
                        Debug.Write("\n ==> From Structure: " + networkName + "-" + GenTextHelper.Split_string_before_After(pipeObj.name, 1, " to"));
                        Debug.Write("\n ==> To Structure: " + networkName + "-" + GenTextHelper.Split_string_before_After(pipeObj.name, 2, "to "));
                        fromStructure = (Global.SDDPit)Global.ZTHvariable.stringDrainages_Collection[networkName + "-" + GenTextHelper.Split_string_before_After(pipeObj.name, 1, " to")];
                        toStructure = (Global.SDDPit)Global.ZTHvariable.stringDrainages_Collection[networkName + "-" + GenTextHelper.Split_string_before_After(pipeObj.name, 2, "to ")];
                    }
                    foreach (var sditem in Global.ZTHvariable.stringDrainages_Collection)
                    {
                        Debug.Write("\n ==> SDColl Key: " + sditem.Key);
                        Global.SDDPit Structure = (Global.SDDPit)sditem.Value;

                        Debug.Write("\n ==> SDColl Value: " + Structure.ip + " || " + Structure.name + " || " + pipeObj.fromStructure + " || " + pipeObj.toStructure);
                    }
                    Debug.Write("\n--------------------------------------------------");

                    
                    Debug.Write("\n == Final to Structure: " + toStructure.ip + " | " + toStructure.actualname + " | " + toStructure.name);
                    Debug.Write("\n == Final from Structure: " + fromStructure.ip + " | " + fromStructure.actualname + " | " + fromStructure.name);
                    Debug.Write("\n COORD: " + toStructure.x + " | " + toStructure.y + "\n");

                    string length = ZSharpFDOHelper.Geometry.GeometryHelper.GetDistanceBetweenPoints(Convert.ToDouble(fromStructure.x), Convert.ToDouble(fromStructure.y), Convert.ToDouble(toStructure.x), Convert.ToDouble(toStructure.y)).ToString();
                    Debug.Write("\n length: " + length + "\n");
                    string geom = string.Format("GEOMFROMTEXT('LINESTRING ({0} {1}, {2} {3})')", fromStructure.x, fromStructure.y, toStructure.x, toStructure.y);
                    IndexedDictionary<string, string[]> dataVal = new IndexedDictionary<string, string[]>();
                    dataVal.Add("Geometry", new string[] { "geometry", geom });
                    dataVal.Add("Name", new string[] { "String", pipeObj.name }); //.Replace(" ", string.Empty) <---if spaced needs to be rmoved use this
                    dataVal.Add("NetworkName", new string[] { "String", networkName });
                    dataVal.Add("PartSizeName", new string[] { "String", pipeObj.type });
                    dataVal.Add("InsideDiameter", new string[] { "Double", pipeObj.diameter });
                    dataVal.Add("OutsideDiameter", new string[] { "Double", pipeObj.diameter });
                    dataVal.Add("Slope", new string[] { "Double", pipeObj.grade });
                    dataVal.Add("StartInvert", new string[] { "Double", pipeObj.us_level });
                    dataVal.Add("EndInvert", new string[] { "Double", pipeObj.ds_level });
                    dataVal.Add("StructureEnd", new string[] { "String", networkName + "-" + toStructure.ip });//pipeObj.toStructure });
                    dataVal.Add("StructureStart", new string[] { "String", networkName + "-" + fromStructure.ip });// pipeObj.fromStructure });
                    dataVal.Add("Length", new string[] { "Double", length });
                    dataVal.Add("coordinates", new string[] { "Double", fromStructure.x, fromStructure.y, toStructure.x, toStructure.y });

                    //create respect file formats
                    if (output_params[0].Equals("SDF"))
                    {
                        SDFCRUD.SDFInsert_bulk(Global.ZTHvariable.sdfFile, "Pipes", dataVal);
                        Debug.Write("\nSDFFile2: " + Global.ZTHvariable.sdfFile);
                    }
                    else if (output_params[0].Equals("SHP"))
                    {
                        Global.ZTHvariable.pipe_dataColl.Add(dataVal);
                        //SHPHelper.createSHPPointBulk(Global.ZTHvariable.SHPFolder + @"\Structure.shp", "Strcutures", dataVal);
                        //get12DaDrainage(TDfile, output_params[0]);
                    }

                    #region Debug Code
                    Debug.Write("\nWriting to OUTPUTFILE.."); //@"E:\GoogleDrive\Zcodia Labs\Virtua\ZLABPRJ02- Virtua Map\Source\12DaTool\TestFiles\21.sdf"
                    Debug.Write("\n==========Dict===");
                    foreach (var it in dataVal)
                    {
                        Debug.Write("\n== Key: " + "'" + it.Key + "'");
                        Debug.Write("\n== val: " + "'" + it.Value[0] + "'");
                        Debug.Write("\n== val: " + "'" + it.Value[1] + "'");
                    }
                    Debug.Write("\n==========Dict===" + Global.ZTHvariable.sdfFile);
                    Debug.Write("\n=========================================");
                    Debug.Write("\n------PIPE-CLASS---------------");
                    //TEST THE CLASS
                    foreach (var prop in pipeObj.GetType().GetProperties())
                    {

                        Debug.Write("\n---->Property: " + prop.Name);
                        Debug.Write("\n---->Value : " + prop.GetValue(pipeObj, null));
                    }
                    Debug.Write("\n=========================================");
                    Debug.Write("\nWriting to SDF");
                     /**/

                    #endregion

                    #endregion
                }
                if (output_params[0].Equals("SHP"))
                {
                    //send the complete data collection to shp load
                    SHPHelper.createSHPLineBulk(Global.ZTHvariable.SHPFolder + @"\Pipes.shp", "Pipes", Global.ZTHvariable.pipe_dataColl);

                }

                Debug.Write("\n==================Final===================");
                #endregion
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return Global.ZTHvariable.stringDrainages;
        }

        public static void startendLineNumbers(string key, string file, string targetString, IndexedDictionary<string, string> dict_key_linnums)
        {
            //first get all the start and end line numbers of all the string drainage instance
            using (StreamReader sr = File.OpenText(file))
            {
                Int64 lineNum = 1;
                Int64 diid = 1;
                string s = string.Empty;
                Int64 internalopenbrackets = 0;
                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Contains(targetString) && internalopenbrackets == 0)
                    {
                        linenums.Add(lineNum);
                        linenum_values = lineNum + ",";
                        collect = true;
                    }
                    if (collect == true && s.Contains("{") && !linenums.Contains(lineNum))
                    {
                        //Console.WriteLine("intenral open bracket" + lineNum + " - " + s);
                        internalopenbrackets += 1;
                    }
                    if (collect == true && s.Contains("}") && !linenums.Contains(lineNum))
                    {
                        if (internalopenbrackets > 0)
                        {
                            internalopenbrackets -= 1;
                        }
                        else
                        {

                            linenums.Add(lineNum);
                            linenum_values += lineNum;
                            if (!dict_key_linnums.Contains(key))
                                dict_key_linnums.Add(key + diid, linenum_values);
                            linenum_values = null;
                            collect = false;
                            diid++;
                        }
                    }
                    lineNum++;
                }
            }
        }

        public static IndexedDictionary<string, string> startendLineNumbersStream(string key, Stream streamstring, string targetString)
        {
            result.Clear();
            //first get all the start and end line numbers of all the string drainage instance
            using (StreamReader sr = new StreamReader(streamstring))
            {
                Int64 lineNum = 1;
                Int64 diid = 1;
                string s = string.Empty;
                Int64 internalopenbrackets = 0;
                while ((s = sr.ReadLine()) != null)
                {
                    Debug.Write(lineNum + " " + s + "\n");
                    if (s.Contains(targetString) && internalopenbrackets == 0)
                    {
                        linenums.Add(lineNum);
                        linenum_values = lineNum + ",";
                        Debug.Write(lineNum + " " + ">>>" + s + "\n");
                        collect = true;
                    }
                    if (collect == true && s.Contains("{") && !linenums.Contains(lineNum))
                    {
                        //Console.WriteLine("intenral open bracket" + lineNum + " - " + s);
                        internalopenbrackets += 1;
                    }
                    if (collect == true && s.Contains("}") && !linenums.Contains(lineNum))
                    {
                        if (internalopenbrackets > 0)
                        {
                            internalopenbrackets -= 1;
                        }
                        else
                        {

                            linenums.Add(lineNum);
                            linenum_values += lineNum;
                            result.Add(key + diid, linenum_values);
                            Debug.Write(lineNum + " " + ">>>" + s + "\n");
                            linenum_values = null;
                            collect = false;
                            diid++;
                        }
                    }
                    lineNum++;
                }
            }
            return Global.ZTHvariable.linenums_stringdriange;
        }

        public static IndexedDictionary<string, string> startendLineNumberswithLimts(string stoageKey, string key, string file, string targetString, Int64 startlineNum, Int64 endlineNum, IndexedDictionary<string, string> linenumberStorage)
        {
            try
            {
                //first get all the start and end line numbers of all the string drainage instance
                #region getlineNumbersLimits
                using (StreamReader sr = File.OpenText(file))
                {
                    result_pit.Clear();
                    Int64 lineNum = 1;
                    Int64 diid = 1;
                    string s = string.Empty;
                    Int64 internalopenbrackets = 0;
                    while ((s = sr.ReadLine()) != null)
                    {
                        
                        if (lineNum >= startlineNum && lineNum <= endlineNum)
                        {
                            //Debug.Write(lineNum + s + "\n");
                            if (s.Contains(targetString) && internalopenbrackets == 0)
                            {
                                linenums.Add(lineNum);
                                linenum_values = lineNum + ",";
                                collect = true;
                            }
                            if (collect == true && s.Contains("{") && !linenums.Contains(lineNum))
                            {
                                //Console.WriteLine("intenral open bracket" + lineNum + " - " + s);
                                internalopenbrackets += 1;
                            }
                            if (collect == true && s.Contains("}") && !linenums.Contains(lineNum))
                            {
                                if (internalopenbrackets > 0)
                                {
                                    internalopenbrackets -= 1;
                                }
                                else
                                {
                                    linenums.Add(lineNum);
                                    linenum_values += lineNum;

                                    linenumberStorage.Add(stoageKey + "-" + key + "-" + diid, linenum_values); // << --- HERE storing the line numbers

                                    Debug.Write("\n===" + stoageKey + "-" + key + diid + "--" + linenum_values + "\n");
                                    linenum_values = null;
                                    collect = false;
                                    diid++;
                                }
                            }
                            
                        }
                        lineNum++;
                    }
                }
                #endregion
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return result_pit;
        }

        public static string getStringBetweenLineNumbers(string file, Int64 startlineNum, Int64 endlineNum)
        {
            try
            {
                //Debug.Write(startlineNum +"||" + endlineNum + "\n");
                 using (StreamReader sr = File.OpenText(file))
                {
                    stringVal = null;

                    Int64 lineNum = 1;
                    string s = string.Empty;
                    while ((s = sr.ReadLine()) != null)
                    {
                        
                        if (lineNum >= startlineNum && lineNum <= endlineNum)
                        {
                            //Debug.Write(lineNum + s + "\n");
                            stringVal += s + "\n";

                        }
                        lineNum++;
                    }
                 }
            }
            catch (SystemException ex)
            {
                Debug.Write(ex.ToString());
            }
            return stringVal;
        }

        public static Global.SDDPit getPitObject(string pitNameRaw, string inputString)
        {
            Global.SDDPit pitObj = new Global.SDDPit();
            completed_props.Clear();
            Debug.Write("\nInput String: " + inputString);
            foreach (var prop in pitObj.GetType().GetProperties())
            {
                string result; 
                
                Debug.Write("\nProperty: " + getSpecificLine(inputString, prop.Name) + "\n");
                result = getValuefromLine(getSpecificLine(inputString, prop.Name));
                completed_props.Add(prop.Name);
                //add values to class
                PropertyInfo propertyInfo = pitObj.GetType().GetProperty(prop.Name);
                if (propertyInfo != null)
                {
                    if (prop.Name.Equals("actualname"))
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "name"));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("name"))
                    {
                        propertyInfo.SetValue(pitObj, pitNameRaw + pitObj.ip, null);
                    }
                    else if (prop.Name.Equals("us_pit_index"))
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"us pit index\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("ds_pit_index"))
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"ds pit index\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("number_of_us_trunks"))
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"number of trunks\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("number_of_incoming"))//
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"number of incoming\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("incoming_uid"))//uid     "incoming"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"incoming\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("incoming_pit_index"))//integer "incoming pit index"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"incoming pit index\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("number_of_outgoing"))//integer "number of outgoing"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"number of outgoing\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("us_pit_string_uid"))//
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"us pit string\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("data_us_pit_string_uid"))//uid     "data us pit string"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"data us pit string\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("data_us_pit_index"))//integer "data us pit index"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"data us pit index\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("ds_pit_string_uid"))//uid     "ds pit string"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"ds pit string\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("data_ds_pit_string_uid"))//uid     "data ds pit string"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"data ds pit string\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("data_ds_pit_index"))//integer "data ds pit index"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"data ds pit index\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else
                    {
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    
                }

                //SetObjectProperty(prop.Name, result, pitObj.GetType().GetProperties(prop.Name));
                Debug.Write("\n-----------------------------------");
                Debug.Write("\n---->Property: " + prop.Name);
                Debug.Write("\n---->Value : " + result);
                Debug.Write("\n==================================");
            }

            return pitObj;
        }

        public static Global.SDDPit getPitObject_db(string pitNameRaw, string inputString)
        {
            Global.SDDPit pitObj = new Global.SDDPit();
            completed_props.Clear();
            Debug.Write("\nInput String: " + inputString);
            foreach (var prop in pitObj.GetType().GetProperties())
            {
                string result;

                Debug.Write("\nProperty: " + getSpecificLine(inputString, prop.Name) + "\n");
                result = getValuefromLine(getSpecificLine(inputString, prop.Name));
                completed_props.Add(prop.Name);
                //add values to class
                PropertyInfo propertyInfo = pitObj.GetType().GetProperty(prop.Name);
                if (propertyInfo != null)
                {
                    if (prop.Name.Equals("actualname"))
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "name"));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("name"))
                    {
                        propertyInfo.SetValue(pitObj, pitNameRaw + pitObj.ip, null);
                    }
                    else if (prop.Name.Equals("us_pit_index"))
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"us pit index\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("ds_pit_index"))
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"ds pit index\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("number_of_us_trunks"))
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"number of trunks\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("number_of_incoming"))//
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"number of incoming\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("incoming_uid"))//uid     "incoming"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"incoming\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("incoming_pit_index"))//integer "incoming pit index"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"incoming pit index\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("number_of_outgoing"))//integer "number of outgoing"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"number of outgoing\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("us_pit_string_uid"))//
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"us pit string\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("data_us_pit_string_uid"))//uid     "data us pit string"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"data us pit string\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("data_us_pit_index"))//integer "data us pit index"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"data us pit index\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("ds_pit_string_uid"))//uid     "ds pit string"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"ds pit string\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("data_ds_pit_string_uid"))//uid     "data ds pit string"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"data ds pit string\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else if (prop.Name.Equals("data_ds_pit_index"))//integer "data ds pit index"   
                    {
                        result = getValuefromLine(getSpecificLine(inputString, "\"data ds pit index\""));
                        propertyInfo.SetValue(pitObj, result, null);
                    }
                    else
                    {
                        propertyInfo.SetValue(pitObj, result, null);
                    }

                }

                //SetObjectProperty(prop.Name, result, pitObj.GetType().GetProperties(prop.Name));
                Debug.Write("\n-----------------------------------");
                Debug.Write("\n---->Property: " + prop.Name);
                Debug.Write("\n---->Value : " + result);
                Debug.Write("\n==================================");
            }

            return pitObj;
        }

        public static Global.SDDPipe getPipeObject(string inputString)
        {
            completed_props.Clear();
            Global.SDDPipe pipeObj = new Global.SDDPipe();
            foreach (var prop in pipeObj.GetType().GetProperties())
            {
                
                string result = result = getValuefromLine(getSpecificLine(inputString, Global.ZTHvariable.SDDPipe_properties[prop.Name]));
                completed_props.Add(prop.Name);
                Debug.Write("\n-----------------getPipeObject------------------");
                //add values to class
                PropertyInfo propertyInfo = pipeObj.GetType().GetProperty(prop.Name);
                if (propertyInfo != null)
                {
                    if (prop.Name.Equals("toStructure") || prop.Name.Equals("fromStructure"))
                    {
                        propertyInfo.SetValue(pipeObj, (Convert.ToInt32(result) -1).ToString(), null);
                        Debug.Write("\n---++-->Property: " + prop.Name + " | " + Global.ZTHvariable.SDDPipe_properties[prop.Name]);
                        Debug.Write("\n---++-->Value : " + (Convert.ToInt32(result) - 1).ToString());
                    }
                    else
                    {
                        propertyInfo.SetValue(pipeObj, result, null);
                        Debug.Write("\n---++->Property: " + prop.Name + " | " + Global.ZTHvariable.SDDPipe_properties[prop.Name]);
                        Debug.Write("\n---++->Value : " + result);
                    }
                }

                
                Debug.Write("\n---->Property: " + prop.Name + " | " + Global.ZTHvariable.SDDPipe_properties[prop.Name]);
                Debug.Write("\n---->Value : " + result);
                Debug.Write("\n==================================");

            }

            return pipeObj;
        }

        //Use this method to get all String drianage data into class
        public static void getSSData(string SDDID, string inputString) //extract String drainage data
        {
            //now we would get each string drianage strings we need to extract all relavent information.

            Global.ZTHvariable.sddate_sqlite_insert.Clear();
            Global.ZTHvariable.sddate_sqlite_insert.Add("SDDID", SDDID);
            int SDDInfo_row_count = SQL_Helper_Local.get_row_count("sddinfo");
            string field;
            string actualname;
            string val;

            for (int i = 1; i <= SDDInfo_row_count; i++)
            {
                sql12dHelper.getSDData(i);
                field = Global.ZTHvariable.sddate_sqlite.Keys.ElementAt(0);
                actualname = Global.ZTHvariable.sddate_sqlite[0];
                if(field != "name")
                    val =  getValuefromLine(getSpecificLine(inputString, '"' + actualname + '"'));
                else
                    val = getValuefromLine(getSpecificLine(inputString, actualname));
                
                Debug.Write("\n varName: " + field);
                Debug.Write("\n actualname: " + actualname);
                Debug.Write("\n Val: " +val);
                Global.ZTHvariable.sddate_sqlite_insert.Add(field, val);
            }
            SQL_Helper_Local.insert_data_to_db("SDData", Global.ZTHvariable.sddate_sqlite_insert, SDDInfo_row_count);
                
        }

        public static string getSpecificLine(string inputString, string targetString)
        {
            
            using (StringReader sr = new StringReader(inputString))
            {
                string s = string.Empty;
                Int64 lineNum = 1;
                string targets = @"(^|\s)" + targetString + @"(\s|$)";

                while ((s = sr.ReadLine()) != null)
                {
                    stringVal = null;
                    if (Regex.IsMatch(s, targets))
                    {
                        if (!completed_props.Contains(targets))
                        {
                            stringVal = s;
                            Debug.Write("\nLine Check: " + s);
                            return stringVal;
                        }
                        Debug.Write("\n ==>> " + lineNum + " - " + targetString + "| ++ | " + s);
                    }

                    lineNum++;
                }

            }
            return stringVal;
        }

        public static string getValuefromLine(string line)
        {
            
            if (string.IsNullOrEmpty(line))
            {
                res = "null";
                //Debug.Write("\n ==>> null  " + line);
            }
            else if (line.Contains("\"") && line.Contains("text"))
            {
                string[] separators = new string[] { "-", "," };
                //Debug.Write("\n ==>> " + "==> " + line);
                var reg = new Regex("\".*?\"");
                var matches = reg.Matches(line);
                foreach (var item in matches)
                {
                    
                    //Debug.Write("\nSPL ==>> " + item.ToString() + "==> " );
                    foreach (string replacestring in separators)
                    {
                        res = item.ToString().Replace(replacestring, " ");
                    }
                    res = item.ToString().Replace("\"", "");
                        
                }
            }
            else if (line.Substring(line.Length - 1, 1).Equals("\"") && line.Contains("name"))
            {
                Debug.Write("\n ==>> " + "==> Double Q" + line);
                string[] separators = new string[] { "-", "," };
                var reg = new Regex("\".*?\"");
                var matches = reg.Matches(line);
                foreach (var item in matches)
                {

                    //Debug.Write("\nSPL ==>> " + item.ToString() + "==> " );
                    foreach (string replacestring in separators)
                    {
                        res = item.ToString().Replace(replacestring, " ");
                    }
                    res = item.ToString().Replace("\"", "");

                }

            }
            else if (line.Substring(line.Length - 1, 1).Equals("\"") && line.Contains("type"))
            {
                Debug.Write("\n ==>> " + "==> Double Q" + line);
                string[] separators = new string[] { "-", "," };
                var reg = new Regex("\".*?\"");
                var matches = reg.Matches(line);
                foreach (var item in matches)
                {

                    //Debug.Write("\nSPL ==>> " + item.ToString() + "==> " );
                    foreach (string replacestring in separators)
                    {
                        res = item.ToString().Replace(replacestring, " ");
                    }
                    res = item.ToString().Replace("\"", "");

                }

            }
            else // this is if string doesnt contain double quotes
            {
                string[] delimiters = new string[] { ",", "\t", " ", "\"", " \"" };
                string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                Int64 i = 0;
                Debug.Write("\n ==>> " + parts.Length + "==> " + line);
                foreach (var item in parts)
                {
                    Debug.Write("\n ==>> " + i + " | " + item);
                    i++;
                }

                if (parts.Length > 1)
                {
                    res = parts[parts.Length - 1];
                }
                else
                    res = "null";
                /*
                else if (parts.Length > 2)
                {
                    res = parts[2];
                }
                
                 */
            }
            return res;
        }

        private void SetObjectProperty(string propertyName, string value, object obj)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);
            // make sure object has the property we are after
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value, null);
            }
        }

        public static void tester() //storing data in class and sub class
        {
            IndexedDictionary<string, Global.stringDrainageData> lst = new IndexedDictionary<string, Global.stringDrainageData>();
            for (Int64 ivar = 1; ivar < 3; ivar++)
            {
                Global.SDDPipe pipe = new Global.SDDPipe();
                pipe.diameter = "dia" + ivar;
                //pipe.ds_hgl = "ds_hgl" + ivar;
                pipe.ds_level = "ds_level" + ivar;
                //pipe.flow_velocity = "flow_velocity" + ivar;
                pipe.fromStructure = "fromStructure" + ivar;
                pipe.length = "length" + ivar;
                pipe.name = "name" + ivar;

                pipe.diameter = "dia" + ivar;
                pipe.diameter = "dia" + ivar;
                Global.stringDrainageData sd = new Global.stringDrainageData();
                //sd.SDDName = "name" + ivar;
                //sd.SDDUID = ivar.ToString();
                //sd.pipeData = pipe;
                lst.Add(ivar.ToString(), sd);
            }

            //now try to get values
            //Global.stringDrainageData sd1 = new Global.stringDrainageData();
            foreach (var item in lst)
            {
                Debug.Write(" \n" + item.Key);
                var valuu = item.Value;
                //var pipe = valuu.pipeData;
                //Debug.Write(" \n" + valuu.SDDName + "||" + valuu.SDDID + "||" + pipe.name + "||" +  pipe.length + "||" + pipe.diameter);
            }
        }
    }
}
