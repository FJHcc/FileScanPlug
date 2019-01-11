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
            DirectoryInfo[] dirs = dir.GetDirectories();
            //如果文件夹内没有子目录了
            if (dirs.Length <= 0)
            {
                foreach (FileInfo file in dir.GetFiles("*Repository.cs", SearchOption.TopDirectoryOnly))
                {
                    fileList.Add(dir + @"\" + file.ToString());
                }
            }
            int t = 1;
            foreach (DirectoryInfo dirInfo in dirs)
            {
                searchFile(dir + @"\" + dirInfo.ToString());
                if (t == 1)
                {
                    foreach (FileInfo file in dir.GetFiles("*Repository.cs", SearchOption.TopDirectoryOnly))
                    {
                        fileList.Add(dir + @"\" + file.ToString());
                    }
                    t++;
                }
            }
            return fileList;
        }
    }
}
