using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZSharpGeneralHelper
{
    public class FFManager
    {
        private static string txt_doc_string;
        private static List<string> AllFiles;
        private static byte[] fileBytes;

        public static List<string> getFilesfromDir(string[] extensionList, string[] dirList, bool includeSubDir)
        {
            //AllDirs.Clear();
            AllFiles = new List<string>();
            List<string> allFiles_updated = new List<string>();
            try
            {
                if(dirList != null)
                {
                    
                    foreach(string dir in dirList)
                    {
                        //AllDirs.Add(dir);
                        System.Diagnostics.Debug.WriteLine("dir >>" + dir + " ||" + GetSubdirectoriesContainingOnlyFiles(dir).Count<string>());
                        getFileswithExt(dir, extensionList);
                        //only if there are more than one sub directory
                        if(includeSubDir)//need to get all sub directories
                        {
                            foreach (DirectoryInfo dinfo in getSubDirectories(dir))
                            {
                                System.Diagnostics.Debug.WriteLine("sdir info>>" + dinfo.FullName);
                                getFileswithExt(dinfo.FullName, extensionList);
                                
                            }
                        }
                    }

                    //list all the files
                    foreach (string file in AllFiles)
                    {
                        System.Diagnostics.Debug.WriteLine("file info>>" + file);

                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No directories");
                }
                

            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error>>" + ex);
            }
            return AllFiles;// allFiles_updated;
        }
        //get only the file names wihtout the path
        public static List<string> getFileNames(List<string> files)
        {
            List<string> allFiles = new List<string>();
            foreach (String file in files)
            {
                System.Diagnostics.Debug.WriteLine("File wihtout Path>>" + file);
                allFiles.Add(Path.GetFileName(file));
            }
            return allFiles;
        }

        public static DirectoryInfo[] getSubDirectories(string path)
        {
            DirectoryInfo dInfo = new DirectoryInfo(path);
            DirectoryInfo[] subdirs = dInfo.GetDirectories();
            return subdirs;
        }

        public static IEnumerable<string> GetSubdirectoriesContainingOnlyFiles(string path)
        {
            return from subdirectory in Directory.GetDirectories(path, "*", SearchOption.AllDirectories)
                   where Directory.GetDirectories(subdirectory).Length == 0
                   select subdirectory;
        }

        public static List<string> getFileswithExt(string path, string[] extensions)
        {
            try
            {
                foreach (string fileExtension in extensions)
                {
                    System.Diagnostics.Debug.WriteLine("EXT info>>" + fileExtension);
                    System.Diagnostics.Debug.WriteLine("fileCount>>" + Directory.GetFiles(path, fileExtension, SearchOption.TopDirectoryOnly).ToList<string>().Count());
                    foreach (string file in Directory.GetFiles(path, fileExtension, SearchOption.TopDirectoryOnly).ToList<string>())
                    {
                        if (!AllFiles.Contains(file))
                            AllFiles.Add(file);
                    }
                    //AllFiles.AddRange(Directory.GetFiles(path, fileExtension, SearchOption.TopDirectoryOnly).ToList<string>());
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error info>>" + ex);
            }
            return AllFiles;
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        public static void Txt_writer(string file, string data)
        {
            using (System.IO.StreamWriter StreamWriter1 = new System.IO.StreamWriter(file, true))
            {
                StreamWriter1.WriteLine(data);
            }
        }
        
        public static List<string> stringReader(string file)
        {
            List<string> list = new List<string>();
            using (StreamReader reader = new StreamReader(file))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    list.Add(line); // Add to list.
                }
            }
            return list;
        }

        public static byte[] getFileBytes(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    fileBytes = File.ReadAllBytes(file);
                }
                else
                {
                    fileBytes = null;
                }
            }
           
            catch (System.Exception ex)
            {
                //GH.writeAdminLog(ex.ToString());
            }
            return fileBytes;
        }

        public static bool IsFileLocked(string fileString)
        {
            FileInfo file = new FileInfo(fileString);
            FileStream stream = null;

            try
            {
                if (File.Exists(fileString))
                {
                    stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                }
                else
                {
                    //file is not found
                    return false;
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static void deleteFolderContent(string path)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        public static List<int> getNumericFolder(string path)
        {
            List<int> folderNums = new List<int>();
            try
            {
                
                foreach (var folder in System.IO.Directory.GetDirectories(path, "*", System.IO.SearchOption.AllDirectories).ToList())
                {

                    int number;
                    bool result = Int32.TryParse(folder.Replace(path, ""), out number);
                    if (result)
                    {
                        folderNums.Add(number);
                    }
                }
            }

            catch (System.Exception ex)
            {
                //GH.writeAdminLog(ex.ToString());
            }
            return folderNums;
        }

        public static int getNextNumericFolder(string path)
        {
            List<int> folderNums = new List<int>();
            folderNums = getNumericFolder(path);
            
            return folderNums.Max() + 1;
        }

        public static byte[] streamtoByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static void createFolder(string path)
        {
            try
            {
                //check app path is created if not create it
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (System.Exception ex) { }
        }

        
    }
}
