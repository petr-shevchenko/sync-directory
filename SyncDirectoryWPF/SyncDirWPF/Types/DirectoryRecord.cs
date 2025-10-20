using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncDirectory
{
    public class DirRec
    {
        public string Path = "";
        public string FullPath = "";
        public DirectoryInfo Info = null;
        public override string ToString()
        {
            return FullPath;
        }
    }


    public class DirComparer : IComparer<DirRec>
    {
        public int Compare(DirRec x, DirRec y)
        {
            return x.FullPath.CompareTo(y.FullPath);
        }
    }

    public class DirExistComparer : IEqualityComparer<DirRec>
    {
        public bool Equals(DirRec x, DirRec y)
        {
            return x.Path == y.Path;
        }

        public int GetHashCode(DirRec obj)
        {
            return obj.Path.GetHashCode();
        }
    }

    public class DirectoryList : List<DirRec>
    {
        public DirectoryList(int count)
            : base(count)
        { }

        public DirectoryList()
            : base()
        { }

        public void SortByFullPath()
        {
            base.Sort(new DirComparer());
        }
    }




}
