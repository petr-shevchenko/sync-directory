using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace SyncDirectory
{
    public class HashTabRec
    {
        public string md5 = "";

        public string fileFullPath { get; set; }
        public string md5string
        {
            get
            {
                string s = "";
                if (MD5hash != null)
                    foreach (byte b in MD5hash)
                        s += b.ToString("x2");
                return s;
            }
        }
        public byte[] MD5hash { get; set; }

        public HashTabRec()
        {
            md5 = "";
            
            fileFullPath = "";
            MD5hash = null;
        }
    }

    public class HashTab : List<HashTabRec>
    {
        public HashTab(int count)
            : base(count)
        { }

        public HashTab()
            : base()
        { }

      
        public void UpdateHash(string fileFullPath, byte[] MD5hash)
        {
            
            int k = base.FindIndex(ss => ss.fileFullPath == fileFullPath);
            if (k >= 0)
            {
                if (base[k].MD5hash == null) base[k].MD5hash = new byte[MD5hash.Length];
                MD5hash.CopyTo(base[k].MD5hash, 0);
            }
        }
    }

    public static class HashTabMethods
    {
        public static HashTabRec SolveFileMD5(FileRecord OneFile)
        {
            MD5 md5hash = MD5.Create();
            OneFile.Md5Hash = (OneFile.SizeGb < Constants.BigSizeGb) ? md5hash.ComputeHash(File.ReadAllBytes(OneFile.FileFullPath)) : null;
            HashTabRec hashTabRec = new HashTabRec() { fileFullPath = OneFile.FileFullPath, MD5hash = OneFile.Md5Hash };
            return hashTabRec;
        }

        public static string GetMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        } 
    
    
    }


}
