using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using DB = DataSourceBuilder;
using CodeEngine.Framework.QueryBuilder;
using IndexedDictionary;


namespace ZSharpTextHelper
{
    class SQL_Helper_Local
    {
        public static SQLiteConnection con = Global.Config.GetConnection();
        public static string cmdd_string = null;
        public static SQLiteCommand cmd = new SQLiteCommand(con);

        public static void DBChecker()
        {
            try
            {
                bool result;
                con.Open();
                if (con.State.ToString() == "Open")
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
                con.Close();
                Debug.Write("DB STATUS: " + result);
            }
            catch (SQLiteException ex)
            {
                Debug.Write("\n" + ex.Message);

            }
            catch (System.Exception ex)
            {
                Debug.Write("\n" + ex.Message);

            }
        }

        public static void insert_data_to_db(string table_name, IndexedDictionary<string, string> field_value, int dict_count)
        {
            try
            {
                //Helper.Logger.LOG_IT("\nDB begin");
                con.Open();
                //SQLiteCommand cmd = new SQLiteCommand(con);
                //begin sqlite transaction
                SQLiteTransaction sqlite_tran = con.BeginTransaction();
                InsertQueryBuilder insert_query = new InsertQueryBuilder();
                insert_query.Table = table_name;

                for (int i = 0; i < field_value.Count; i++)
                {
                    Debug.Write("\nDB -" + "==" + i + field_value.GetKeyByIndex(i) + field_value[field_value.GetKeyByIndex(i)]);
                    insert_query.SetField(field_value.GetKeyByIndex(i), field_value[field_value.GetKeyByIndex(i)]);
                }

                cmdd_string = insert_query.BuildQuery();
                cmd.CommandText = cmdd_string;
                Debug.Write("\nSQL insert_data_to_db: " + cmd.CommandText + "\n");
                cmd.ExecuteNonQuery();
                sqlite_tran.Commit();
                cmd.Dispose();
                con.Close();
            }
            catch (SQLiteException ex)
            {
                Debug.Write("\n" + ex.Message);

            }
            catch (System.Exception ex)
            {
                Debug.Write("\n" + ex.Message);

            }

        }

        public static void insert_to_db(string table_name, string field_name, string field_value)
        {
            try
            {
                //Helper.Logger.LOG_IT("\nDB begin");
                con.Open();
                SQLiteCommand cmd = new SQLiteCommand(con);
                //begin sqlite transaction
                SQLiteTransaction sqlite_tran = con.BeginTransaction();
                InsertQueryBuilder insert_query = new InsertQueryBuilder();
                insert_query.Table = table_name;
                insert_query.SetField(field_name, field_value);
                cmdd_string = insert_query.BuildQuery();
                cmd.CommandText = cmdd_string;
                Debug.Write("\nSQL insert_to_db: " + cmd.CommandText);
                //Helper.Logger.LOG_IT("\n" + cmd.CommandText + "\n");
                cmd.ExecuteNonQuery();
                sqlite_tran.Commit();
                cmd.Dispose();
                con.Close();
            }
            catch (SQLiteException ex)
            {
                Debug.Write("\n" + ex.Message);

            }
            catch (System.Exception ex)
            {
                Debug.Write("\n" + ex.Message);

            }

        }

        public static void del_all_row(string table_name)
        {
            if (get_row_count(table_name) > 0)
            {
                con.Open();
                SQLiteCommand cmd = new SQLiteCommand(con);
                cmd.CommandText = "DELETE FROM " + table_name;

                cmd.ExecuteNonQuery();
                cmd.Dispose();
                con.Close();
            }
        }

        public static void clear_table(string table_name)
        {
            del_all_row(table_name);
            reset_auto_increment(table_name);
        }

        public static void reset_auto_increment(string table_name)
        {
            con.Open();
            SQLiteCommand cmd1 = new SQLiteCommand(con);
            cmd1.CommandText = string.Format("update SQLITE_SEQUENCE set seq = 0 where name = '{0}'", table_name);
            cmd1.ExecuteNonQuery();
            cmd1.Dispose();
            con.Close();

        }

        public static int get_row_count(string table_name)
        {
            con.Open();

            SQLiteCommand cmd = new SQLiteCommand(con);
            SQLiteDataReader dr;

            Int32 result = 0;

            cmd.CommandText = string.Format("SELECT COUNT(ID) FROM {0}", table_name);

            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                result = dr.GetInt32(0);
            }

            dr.Close();
            con.Close();
            return result;
        }

        
    }

    public class sql12dHelper
    {
        public static SQLiteConnection con = Global.Config.GetConnection();
        public static string cmdd_string = null;
        public static SQLiteCommand cmd = new SQLiteCommand(con);

        public static void getSDData(int index)
        {
            try
            {
                Global.ZTHvariable.sddate_sqlite.Clear();
                con.Open();
                SQLiteDataReader dr;
                cmd.CommandText = "select varName, actualname from sddinfo WHERE ID = " + index + " ;";
                Debug.Write("\nSQL getSDData: " + cmd.CommandText);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Global.ZTHvariable.sddate_sqlite.Add(dr["varName"].ToString(), dr["actualname"].ToString());
                }

                dr.Close();
                con.Close();
            }
            catch (System.Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }
    }
}
