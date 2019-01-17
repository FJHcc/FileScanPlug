using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileScan
{
    public  class FileSearch
    {
        public static List<string> fileList = new List<string>();
        public static List<string> searchFile(string folder)
        {
            //传进来如果只是一个文件
            if (File.Exists(folder))
            {
                FileInfo file = new FileInfo(folder);
                if (file.Name.Contains("Repository"))
                {
                    fileList.Add(folder + @"\" + file.ToString());
                }
                return fileList;
            }
            DirectoryInfo dir = new DirectoryInfo(folder);
            FileInfo[] files = dir.GetFiles("*Repository.cs", SearchOption.AllDirectories);
            foreach(var file in files)
            {
                fileList.Add(file.FullName);
            }
            return fileList;
        }
    }
}
