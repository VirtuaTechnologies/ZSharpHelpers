using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZSharpGeneralHelper
{
    class FFManager
    {
        private static string txt_doc_string;
        private static List<string> AllFiles;

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
    }
}
