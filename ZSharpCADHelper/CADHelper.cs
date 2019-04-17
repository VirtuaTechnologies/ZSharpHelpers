//This is an Intelectual Property of Zcodia Technologies and Raghulan Gowthaman.
//www.zcodiatechnologies.com.au

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSharpCADHelper
{
    public class CADHelper
    {
        private static string result;
        public static string convertObjname(string name, string convertTo)
        {
            
            switch (name)
            {
               case "structurelabel":
               case "AeccDbStructureLabel":
                    if (convertTo == "ObjectClass.Name")
                        result = "AeccDbStructureLabel";
                    if (convertTo == "DxfName")
                        result = "AECC_STRUCTURE_LABEL";
                    if (convertTo == "normal")
                        result = "structurelabel";
                    break;

                case "pipelabel":
                case "AeccDbPipeLabel":
                    if (convertTo == "ObjectClass.Name")
                        result = "AeccDbPipeLabel";
                    if (convertTo == "DxfName")
                        result = "AECC_PIPE_LABEL";
                    if (convertTo == "normal")
                        result = "pipelabel";
                    break;

                case "generalnotelabel":
                case "AeccDbNoteLabel":
                    if (convertTo == "ObjectClass.Name")
                        result = "AeccDbNoteLabel";
                    if (convertTo == "DxfName")
                        result = "AECC_GENERAL_NOTE_LABEL";
                    if (convertTo == "normal")
                        result = "generalnotelabel";
                    break;

                case "generalsegmentlabel":
                case "AeccDbGeneralLabel":
                    if (convertTo == "ObjectClass.Name")
                        result = "AeccDbGeneralLabel";
                    if (convertTo == "DxfName")
                        result = "AECC_GENERAL_SEGMENT_LABEL";
                    if (convertTo == "normal")
                        result = "generalsegmentlabel";
                    break;

                case "stationoffsetlabel":
                case "AeccDbStaOffsetLabel":
                    if (convertTo == "ObjectClass.Name")
                        result = "AeccDbStaOffsetLabel";
                    if (convertTo == "DxfName")
                        result = "AECC_STATION_OFFSET_LABEL";
                    if (convertTo == "normal")
                        result = "stationoffsetlabel";
                    break;
            }

            return result;
        }

        
    }
}
