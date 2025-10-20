using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SyncDirectory
{
    public class FileExistComparer : IEqualityComparer<FileRecord>
    {
        public bool Equals(FileRecord x, FileRecord y)
        {
            return x.FilePath == y.FilePath;
        }

        public int GetHashCode(FileRecord obj)
        {
            return obj.FilePath.GetHashCode();
        }
    }

    public class FileNeedUpdateComparer : IEqualityComparer<FileRecord>
    {
        private bool useMD5;
        public FileNeedUpdateComparer(bool useMD5 = false)
        {
            this.useMD5 = useMD5;
        }

        public bool Equals(FileRecord x, FileRecord y)
        {
            bool result = true;
            result = x.FilePath == y.FilePath;
            if (result)
            {
                if (!useMD5 || (x.SizeGb > Constants.BigSizeGb))
                {
                    result = ((y.Info.Length == x.Info.Length) && (y.Info.LastWriteTimeUtc == x.Info.LastWriteTimeUtc));
                }
                else result = y.Md5String == x.Md5String;
            }
            return result;
        }

        public int GetHashCode(FileRecord obj)
        {
            string result = obj.FilePath;
            if (!useMD5 || (obj.SizeGb > Constants.BigSizeGb))
            {
                result += obj.Info.Length.ToString()+" " + obj.Info.LastWriteTimeUtc.ToString("dd.MM.yyyy HH:mm:ss.fff");// (y.Info.Length == x.Info.Length) && (y.Info.LastWriteTimeUtc == x.Info.LastWriteTimeUtc));
            }
            else result += obj.Md5String;

            return result.GetHashCode();
        }
    }

    public class FileRecord
    {
        public string FilePath { get; set; }
        public string FileFullPath { get; set; }
        public double SizeGb { get; set; }
        public FileInfo Info { get; set; }
        public byte[] Md5Hash { get; set; }
        public string Md5String
        {
            get
            {
                string s = "";
                if (Md5Hash != null)
                    foreach (byte b in Md5Hash)
                        s += b.ToString("x2");
                return s;
            }
        }

        public FileRecord()
        {
            FilePath = "";
            FileFullPath = "";
            SizeGb = 0.0;
            Info = null;
            Md5Hash = null;
        }

        public override bool Equals(object obj)
        {
            if (obj is FileRecord) return FilePath == (obj as FileRecord).FilePath;
            else return false;
        }

        public override int GetHashCode()
        {
            return FileFullPath.GetHashCode();
        }

        public override string ToString()
        {
            return FileFullPath;
        }
    }

    public class FileList : List<FileRecord>
    {
        public FileList(int count)
            : base(count)
        { }
    }
}
