using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GV = XMLParserApp.Global.variables;
using GH = XMLParserApp.Helper.GenHelper;
using ZSharpXMLHelper;
using System.Data.SqlClient;

namespace XMLParserApp.Global
{
    class config
    {
        public static string readDBString()
        {
            return GH.DecodeFrom64(xmlParser.getXMLValue(Global.variables.settingsFile, "Settings", "name", "dbstring"));
        }


        public static SqlConnection GetSQLConnection()
        {
            try
            {
                //string conn_string = "Data Source=./db.s3db; FailIfMissing=True; MultipleActiveResultSets=True";
                //MessageBox.Show(conn_string);
                SqlConnectionStringBuilder con_build = new SqlConnectionStringBuilder();
                string DBString = readDBString();

                con_build.DataSource = GH.Split_csv_get_specific(DBString, 0);
                con_build.InitialCatalog = GH.Split_csv_get_specific(DBString, 1);
                con_build.UserID = GH.Split_csv_get_specific(DBString, 2);
                con_build.Password = GH.Split_csv_get_specific(DBString, 3);
                //MessageBox.Show(con_build.ToString());


                SqlConnection conn = new SqlConnection(con_build.ToString());
                conn.Open();
                return conn;
            }
            catch (SystemException ex)
            {
                //MessageBox.Show(ex.Message);
                return null;

            }
        }
    }
}
