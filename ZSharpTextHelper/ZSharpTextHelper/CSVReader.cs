using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GV = ZSharpTextHelper.Global.variable;



namespace ZSharpTextHelper
{
    public class CSVReader : IDisposable
    {
        public const string NEWLINE = "\r\n";

        
        

        /// <summary>
        /// This reader will read all of the CSV data
        /// </summary>
        private BinaryReader reader;

        /// <summary>
        /// The number of rows to scan for types when building a DataTable (0 to scan the whole file)
        /// </summary>
        public int ScanRows = 0;

        #region Constructors

        /// <summary>
        /// Read CSV-formatted data from a file
        /// </summary>
        /// <param name="filename">Name of the CSV file</param>
        public CSVReader(FileInfo csvFileInfo)
        {
            if (csvFileInfo == null)
                throw new ArgumentNullException("Null FileInfo passed to CSVReader");

            this.reader = new BinaryReader(File.OpenRead(csvFileInfo.FullName));
        }

        /// <summary>
        /// Read CSV-formatted data from a string
        /// </summary>
        /// <param name="csvData">String containing CSV data</param>
        public CSVReader(string csvData)
        {
            if (csvData == null)
                throw new ArgumentNullException("Null string passed to CSVReader");


            this.reader = new BinaryReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvData)));
        }

        /// <summary>
        /// Read CSV-formatted data from a TextReader
        /// </summary>
        /// <param name="reader">TextReader that's reading CSV-formatted data</param>
        public CSVReader(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("Null TextReader passed to CSVReader");

            this.reader = new BinaryReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd())));
        }

        #endregion

        string currentLine = "";
        /// <summary>
        /// Read the next row from the CSV data
        /// </summary>
        /// <returns>A list of objects read from the row, or null if there is no next row</returns>
        ///
        private static List<object> objects;
        public List<object> ReadRow()
        {
            try
            {
                // ReadLine() will return null if there's no next line
                if (reader.BaseStream.Position >= reader.BaseStream.Length)
                    return null;

                StringBuilder builder = new StringBuilder();

                // Read the next line
                while ((reader.BaseStream.Position < reader.BaseStream.Length) && (!builder.ToString().EndsWith(NEWLINE)))
                {
                    char c = reader.ReadChar();                                       
                    builder.Append(c);
                    
                }
                

                currentLine = builder.ToString();
                if (currentLine.EndsWith(NEWLINE))
                    currentLine = currentLine.Remove(currentLine.IndexOf(NEWLINE), NEWLINE.Length);

                // Build the list of objects in the line
                objects = new List<object>();
                while (currentLine != "")
                    objects.Add(ReadNextObject());
                return objects; 
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("ArgException:" + ex.ToString());
                return null; 
            }
                       
            
        }

        /// <summary>
        /// Read the next object from the currentLine string
        /// </summary>
        /// <returns>The next object in the currentLine string</returns>
        private object ReadNextObject()
        {
            if (currentLine == null)
                return null;

            // Check to see if the next value is quoted
            bool quoted = false;
            if (currentLine.StartsWith("\""))
                quoted = true;

            // Find the end of the next value
            string nextObjectString = "";
            int i = 0;
            int len = currentLine.Length;
            bool foundEnd = false;
            while (!foundEnd && i <= len)
            {
                // Check if we've hit the end of the string
                if ((!quoted && i == len) // non-quoted strings end with a comma or end of line
                    || (!quoted && currentLine.Substring(i, 1) == ",")
                    // quoted strings end with a quote followed by a comma or end of line
                    || (quoted && i == len - 1 && currentLine.EndsWith("\""))
                    || (quoted && currentLine.Substring(i, 2) == "\","))
                    foundEnd = true;
                else
                    i++;
            }
            if (quoted)
            {
                if (i > len || !currentLine.Substring(i, 1).StartsWith("\""))
                    throw new FormatException("Invalid CSV format: " + currentLine.Substring(0, i));
                i++;
            }
            nextObjectString = currentLine.Substring(0, i).Replace("\"\"", "\"");

            if (i < len)
                currentLine = currentLine.Substring(i + 1);
            else
                currentLine = "";

            if (quoted)
            {
                if (nextObjectString.StartsWith("\""))
                    nextObjectString = nextObjectString.Substring(1);
                if (nextObjectString.EndsWith("\""))
                    nextObjectString = nextObjectString.Substring(0, nextObjectString.Length - 1);
                return nextObjectString;
            }
            else
            {
                object convertedValue;
                StringConverter.ConvertString(nextObjectString, out convertedValue);
                return convertedValue;
            }
        }

        /// <summary>
        /// Read the row data read using repeated ReadRow() calls and build a DataColumnCollection with types and column names
        /// </summary>
        /// <param name="headerRow">True if the first row contains headers</param>
        /// <returns>System.Data.DataTable object populated with the row data</returns>
        public DataTable CreateDataTable(bool headerRow)
        {
            // Read the CSV data into rows
            List<List<object>> rows = new List<List<object>>();
            List<object> readRow = null;
            while ((readRow = ReadRow()) != null)
                rows.Add(readRow);

            // The types and names (if headerRow is true) will be stored in these lists
            List<Type> columnTypes = new List<Type>();
            List<string> columnNames = new List<string>();

            // Read the column names from the header row (if there is one)
            if (headerRow)
                foreach (object name in rows[0])
                    columnNames.Add(name.ToString());

            // Read the column types from each row in the list of rows
            bool headerRead = false;
            foreach (List<object> row in rows)
                if (headerRead || !headerRow)
                    for (int i = 0; i < row.Count; i++)
                        // If we're adding a new column to the columnTypes list, use its type.
                        // Otherwise, find the common type between the one that's there and the new row.
                        if (columnTypes.Count < i + 1)
                            columnTypes.Add(row[i].GetType());
                        else
                            columnTypes[i] = StringConverter.FindCommonType(columnTypes[i], row[i].GetType());
                else
                    headerRead = true;

            // Create the table and add the columns
            DataTable table = new DataTable();            
            for (int i = 0; i < columnTypes.Count; i++)
            {                
                table.Columns.Add();
                table.Columns[i].DataType = columnTypes[i];
                if (i < columnNames.Count && columnNames[i] != "")
                {
                    Console.WriteLine("-------" + columnNames[i]);
                    table.Columns[i].ColumnName = columnNames[i];
                }
                                           
            }
            

            // Add the data from the rows
            headerRead = false;
            foreach (List<object> row in rows)
                if (headerRead || !headerRow)
                {
                    DataRow dataRow = table.NewRow();
                    for (int i = 0; i < row.Count; i++)
                        dataRow[i] = row[i];
                    table.Rows.Add(dataRow);
                }
                else
                    headerRead = true;

            return table;
        }

        /// <summary>
        /// Read a CSV file into a table
        /// </summary>
        /// <param name="filename">Filename of CSV file</param>
        /// <param name="headerRow">True if the first row contains column names</param>
        /// <param name="scanRows">The number of rows to scan for types when building a DataTable (0 to scan the whole file)</param>
        /// <returns>System.Data.DataTable object that contains the CSV data</returns>
        public static DataTable ReadCSVFile(string filename, bool headerRow, int scanRows)
        {
            using (CSVReader reader = new CSVReader(new FileInfo(filename)))
            {
                reader.ScanRows = scanRows;
                return reader.CreateDataTable(headerRow);
            }
        }

        /// <summary>
        /// Read a CSV file into a table
        /// </summary>
        /// <param name="filename">Filename of CSV file</param>
        /// <param name="headerRow">True if the first row contains column names</param>
        /// <returns>System.Data.DataTable object that contains the CSV data</returns>
        public static DataTable ReadCSVFile(string filename, bool headerRow)
        {
            using (CSVReader reader = new CSVReader(new FileInfo(filename)))
                return reader.CreateDataTable(headerRow);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (reader != null)
            {
                try
                {
                    // Can't call BinaryReader.Dispose due to its protection level
                    reader.Close();
                }
                catch { }
            }
        }

        #endregion
    }
}
