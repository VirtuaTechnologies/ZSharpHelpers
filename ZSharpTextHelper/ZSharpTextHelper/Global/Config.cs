using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace ZSharpTextHelper.Global
{
    class Config
    {
        public static SQLiteConnection GetConnection()
        {
            try
            {
                //string conn_string = "Data Source=./db.s3db; FailIfMissing=True; MultipleActiveResultSets=True";
                //MessageBox.Show(conn_string);
                SQLiteConnectionStringBuilder con_build = new SQLiteConnectionStringBuilder();
                //con_build.DataSource = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +  @"\Autodesk\ApplicationPlugins\GIS_SHP_Exporter.bundle\Contents\db.s3db");
                string path = GenTextHelper.AssemblyDirectory + @"\Data\db.s3db";
                con_build.DataSource = path;
                Debug.Write("DB Exists: " + File.Exists(path) + " | " + path);
                con_build.Version = 3;
                con_build.CacheSize = 4000;
                con_build.DefaultTimeout = 100;
                con_build.FailIfMissing = true;
                //con_build.JournalMode = SQLiteJournalModeEnum.Off;
                SQLiteConnection conn = new SQLiteConnection(con_build.ToString());
                return conn;
            }
            catch (SQLiteException ex)
            {
                Debug.Write(ex.Message);
                return null;

            }
        }
    }
}
