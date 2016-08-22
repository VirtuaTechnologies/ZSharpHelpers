using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultAPI.Data
{
    public class vObjs
    {

    }

    public class fileInfo
    {
        public string Cat { get; set; }
        public bool CheckedOut { get; set; }
        public string CkInDate { get; set; }
        public string CkOutUserId { get; set; }
        public bool ControlledByChangeOrder { get; set; }
        public string CreateDate { get; set; }
        public string CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public string DesignVisAttmtStatus { get; set; }
        public string FileClass { get; set; }
        public string FileLfCyc { get; set; }
        public string FileRev { get; set; }
        public string FileSize { get; set; }
        public string FileStatus { get; set; }
        public string FolderId { get; set; }
        public bool Hidden { get; set; }
        public string Id { get; set; }
        public string Locked { get; set; }
        public string MasterId { get; set; }
        public string MaxCkInVerNum { get; set; }
        public string ModDate { get; set; }
        public string Name { get; set; }
        public string VerNum { get; set; }
        public string VerName { get; set; }

    }

    public class folderInfo
    {
        public string EntityName { get; set; }
        public string FolderPath { get; set; }
        public string FullName { get; set; }
        public int NumberOfChildren { get; set; }
        public string Category { get; set; }
        public long catID { get; set; }
        public string CreateDate { get; set; }
        public long CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public string EntityClass { get; set; }
        public long EntityIterationId { get; set; }
        public long EntityMasterId { get; set; }
        public long Id { get; set; }
        public string FullUncName { get; set; }
        public bool IsLibraryFolder { get; set; }
        public bool IsVaultRoot { get; set; }
        public bool Locked { get; set; }
        public long ParentId { get; set; }
        public long fileCount { get; set; }
        public string Color { get; set; }
        public List<folderInfo> childFolders { get; set; }
    }
}
