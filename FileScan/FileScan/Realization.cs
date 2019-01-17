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
        private static readonly string[] txtName = new string[] { "\\join语句问题文件.txt", "\\select头语句问题文件.txt" };
        static void Main(string[] args)
        {
            //获取应用程序当前目录
            var path = System.AppDomain.CurrentDomain.BaseDirectory;

            //筛选文件
            var fileList = FileSearch.SearchFile(path);
            //文件内容筛选
            var fileResult = FileFilter.Filter(fileList);
            if (fileResult.Count > 0)
            {
                var i = 0;
                foreach (var filePair in fileResult)
                {
                    WriteIntoTxt(path, txtName[i++], filePair);
                }
                Console.Write("写入完成");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("文件正常");
                Console.ReadKey();
            }

        }

        private static void WriteIntoTxt(string path, string txtName, KeyValuePair<string, List<Dictionary<string, List<string>>>> result)
        {
            var write = new StreamWriter(path + txtName, false, Encoding.Default);
            write.WriteLine(result.Key + "\n");
            foreach (var fileDictionary in result.Value)
            {
                foreach (var singleFile in fileDictionary)
                {
                    write.WriteLine(singleFile.Key);
                    write.Write("出错sql语句：");
                    singleFile.Value.ForEach(sql => write.WriteLine(sql));
                    write.WriteLine();
                    write.WriteLine();
                }
            }
            write.Flush();
            write.Close();
        }
    }
}

