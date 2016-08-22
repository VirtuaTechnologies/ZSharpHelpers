using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace ZSharpSQLITEHelper
{
    public class SQLITEGenHelper
    {
        
        public static string cmdd_string = null;
        public static int row_count;
        public static Int64 intResult = 0;

        public static Int64 Get_int_result(SQLiteConnection con, string command)
        {
            try
            {
                con.Open();

                SQLiteCommand cmd = new SQLiteCommand(con);
                SQLiteDataReader dr;

                cmd.CommandText = command;

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    intResult = dr.GetInt32(0);
                }

                dr.Close();
                con.Close();
                
            }
            catch (System.InvalidOperationException ex)
            {
                //Helper.Logger.LOG_IT(ex.ToString());
            }
            return intResult;
        }

        public static void get_distinct_vals(SQLiteConnection con, string table_name, string col, ObservableCollection<string> coll)
        {


            //coll.Add(dr[col].ToString());
            //dict.Add(i, dr[col].ToString());
            //i++;
            cmdd_string = string.Format("SELECT DISTINCT \"{0}\" FROM \"{1}\"", col, table_name);
            //Helper.Logger.LOG_IT("\n" + cmdd_string + "\n");

            try
            {
                con.Open();
                using (var cmd1 = new SQLiteCommand(cmdd_string, con))
                {
                    using (SQLiteDataReader dr = cmd1.ExecuteReader())
                    {
                        //dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            //Helper.Logger.LOG_IT("\nDistinct Vals Get DVals: " + dr[col].ToString());
                            coll.Add(dr[col].ToString());
                        }
                        dr.Close();
                    }

                    cmd1.Dispose();
                }
                con.Close();
                GC.Collect();
            }
            catch (System.InvalidOperationException ex)
            {
                //Helper.Logger.LOG_IT(ex.ToString());
            }

        }

        public static void get_vals_group_by(SQLiteConnection con, string table_name, string col, string group_by_coll, ObservableCollection<string> coll)
        {
            cmdd_string = string.Format("SELECT \"{0}\" FROM \"{1}\" group by \"{2}\"", col, table_name, group_by_coll);
            //Helper.Logger.LOG_IT("\n" + cmdd_string + "\n");

            try
            {
                con.Open();
                using (var cmd1 = new SQLiteCommand(cmdd_string, con))
                {
                    using (SQLiteDataReader dr = cmd1.ExecuteReader())
                    {
                        //dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            //Helper.Logger.LOG_IT("\nVals by Group: " + dr[col].ToString());
                            if (!coll.Contains(dr[col].ToString()))
                                coll.Add(dr[col].ToString());
                        }
                        dr.Close();
                    }

                    cmd1.Dispose();
                }
                con.Close();
                GC.Collect();
            }
            catch (System.InvalidOperationException ex)
            {
                //Helper.Logger.LOG_IT(ex.ToString());
            }
        }

        public static bool db_status(SQLiteConnection con)
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

            return result;
        }

        public static string Get_sql_result(SQLiteConnection con, string command)
        {

            string result = null;
            /*
            try
            {
                con.Open();
                using (var cmd = new SQLiteCommand(cmdd_string, con))
                {
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            result = dr.GetString(0);
                        }
                        dr.Close();
                    }
                    cmd.Dispose();
                }
                con.Close();
                GC.Collect();
            }
            catch (System.Exception ex)
            {
                Helper.Logger.LOG_IT("Error opening window" + ex.ToString());
            }
            */

            con.Open();

            SQLiteCommand cmd = new SQLiteCommand(con);
            SQLiteDataReader dr;

            cmd.CommandText = command;

            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                result = dr.GetString(0);
            }

            dr.Close();
            con.Close();

            return result;
        }

        public static int get_row_count(SQLiteConnection con, string table_name)
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

        public static void reset_auto_increment(SQLiteConnection con, string table_name)
        {
            con.Open();
            SQLiteCommand cmd1 = new SQLiteCommand(con);
            cmd1.CommandText = string.Format("update SQLITE_SEQUENCE set seq = 0 where name = '{0}'", table_name);
            cmd1.ExecuteNonQuery();
            cmd1.Dispose();
            con.Close();

        }

        public static void clear_table(SQLiteConnection con, string table_name)
        {
            del_all_row(con, table_name);
            reset_auto_increment(con, table_name);
        }

        public static void del_all_row(SQLiteConnection con, string table_name)
        {
            if (get_row_count(con, table_name) > 0)
            {
                con.Open();
                SQLiteCommand cmd = new SQLiteCommand(con);
                cmd.CommandText = "DELETE FROM " + table_name;

                cmd.ExecuteNonQuery();
                cmd.Dispose();
                con.Close();
            }
        }

        public static void copy_table(SQLiteConnection con, string from_table, string to_table)
        {
            del_all_row(con, to_table);
            con.Open();
            SQLiteCommand cmd1 = new SQLiteCommand(con);
            cmd1.CommandText = string.Format("INSERT INTO {0} SELECT * FROM {1}", to_table, from_table);
            //Helper.Logger.LOG_IT("\nSQL>> " + cmd1.CommandText.ToString());
            cmd1.ExecuteNonQuery();
            cmd1.Dispose();
            con.Close();
        }

        public static int get_row_count_where(SQLiteConnection con, string table_name, string where_column, string condition)
        {
            Int32 result = 0;
            try
            {
                using (con)
                {
                    con.Open();
                    //begin sqlite transaction
                    SQLiteTransaction sqlite_tran = con.BeginTransaction();
                    SQLiteCommand cmd1 = new SQLiteCommand(con);
                    cmd1.CommandText = string.Format("SELECT COUNT(ID) FROM {0} WHERE {1} =\"{2}\"", table_name, where_column, condition);
                    SQLiteDataReader dr;
                    //Helper.Logger.LOG_IT("\nSQLH DB SQL>> " + cmd1.CommandText);
                    using (cmd1 = new SQLiteCommand(cmd1.CommandText, con, sqlite_tran))
                    {
                        dr = cmd1.ExecuteReader();
                        while (dr.Read())
                        {
                            result = dr.GetInt32(0);
                        }
                        cmd1.Dispose();
                    }
                    con.Close();
                    GC.Collect();
                }
            }
            catch (System.InvalidOperationException ex)
            {
                //Helper.Logger.LOG_IT(ex.ToString());
            }
            return result;
        }

        public static void get_column_result(SQLiteConnection con, string column_name, string table_name, string where_column, string condition, ObservableCollection<string> collection)
        {
            try
            {
                using (con)
                {
                    con.Open();
                    //begin sqlite transaction
                    SQLiteTransaction sqlite_tran = con.BeginTransaction();
                    SQLiteCommand cmd1 = new SQLiteCommand(con);
                    cmd1.CommandText = string.Format("SELECT {0} FROM {1} WHERE {2} =\"{3}\"", column_name, table_name, where_column, condition);
                    SQLiteDataReader dr;
                    //Helper.Logger.LOG_IT("\nSQLH DB SQL>> " + cmd1.CommandText);
                    using (cmd1 = new SQLiteCommand(cmd1.CommandText, con, sqlite_tran))
                    {
                        dr = cmd1.ExecuteReader();
                        while (dr.Read())
                        {
                            if (!collection.Contains(dr[column_name].ToString()))
                            {
                                collection.Add(dr[column_name].ToString());
                                //Helper.Logger.LOG_IT("\nSQLH FROM DB PPLayer " + dr[column_name].ToString());
                            }

                        }
                        cmd1.Dispose();
                    }
                    con.Close();
                    GC.Collect();
                }
            }
            catch (System.InvalidOperationException ex)
            {
                //Helper.Logger.LOG_IT(ex.ToString());
            }
        }

    }
}
