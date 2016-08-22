using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSharpFDOHelper.GeneralHelper
{
    public class GenUtilities
    {
        public static string Split_csv_get_specific1(string csv_value, int part)
        {
            part -= 1;
            string result;
            try
            {
                result = csv_value.Split(',')[part];

            }
            catch (IndexOutOfRangeException ex)
            {
                result = "no";
            }

            return result;
        }

        public static string Split_csv_get_specific_dl(string csv_value, int part, char delimiter)
        {
            part -= 1;
            string result;
            try
            {
                result = csv_value.Split(delimiter)[part];

            }
            catch (IndexOutOfRangeException ex)
            {
                result = "no";
            }

            return result;
        }

    }
}
