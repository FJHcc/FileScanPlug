using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileScan
{
    public class Realization
    {
        static void Main(string[] args)
        {
            //获取应用程序当前目录
            var path = System.AppDomain.CurrentDomain.BaseDirectory;

            //筛选文件
            var fileList = FileSearch.searchFile("D:\\MyGit\\Sparks\\src");
            //文件内容筛选
            var fileResult = FileFilter.Filter(fileList);
            if (fileResult.Count > 0)
            {
                var newTxtPath = "D:\\MyGit\\Sparks\\src\\出错文件及位置.txt";
                var write = new StreamWriter(newTxtPath, false, Encoding.Default);
                foreach (var files in fileResult)
                {
                    write.WriteLine(files.Key+"\n");
                    foreach (var fileDictionary in files.Value)
                    {
                        foreach (var singleFile in fileDictionary)
                        {
                            write.WriteLine(singleFile.Key);
                            foreach(var SqlAndPosition in singleFile.Value)
                            {
                                foreach (var singleSql in SqlAndPosition)
                                {
                                    write.Write("出错sql语句：");
                                    write.WriteLine(singleSql.Key);
                                    write.Write("出错行号：");
                                    singleSql.Value.ForEach(position => write.Write(position + " "));
                                    write.WriteLine();
                                    write.WriteLine();
                                } 
                            }
                        }            
                    }
                }
                write.Flush();
                write.Close();
                Console.Write("写入完成");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("文件正常");
                Console.ReadKey();
            }


        }
    }
}

