using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileScan
{
    public static class FileFilter
    {
        private static Dictionary<string,List<Dictionary<string, List<int>>>> result = new Dictionary<string,List<Dictionary<string, List<int>>>>();
        //存储找出的sql和对应位置
        private const string rex = "(?s)(.*?)(?<=\")(\\s*select|\\s*join).*?((?<=where)|(?<=\";\r\n))";
        //分解每条join的正则
        private const string sqlJoin = "join\\s{1,}.*(\\s*on).*";
        //select*的正则
        private const string select = "(?s)(.*?)(?<=\")\\s*select.*?(?<=from)";
        private const string errorSelect = "\\s*select\\s*\\*";
        private const string sqlOrg = "(=\\s*)[a-z]*(.OrgId)";
        private const string lineRex = "\\\n";
        //错误类型
        private static readonly string[] errorMes = new [] { "join语句缺少orgId的文件及位置：", "存在select *的的文件及位置：" }  ;

        /// <summary>
        /// 实现方法
        /// </summary>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public static Dictionary<string, List<Dictionary<string, List<int>>>> Filter(List<string> fileList)
        {
            foreach (string path in fileList)
            {
                string line;
                var file = new FileInfo(path);
                try
                {
                    if (file.Length > 0)
                    {
                        //一次性读完文本内容
                        line = file.OpenText().ReadToEnd();
                        FindJoinError(line,path);
                        FindSelectError(line, path);
                        file.OpenText().Close();
                        continue;
                    }
                    else
                        continue;
                }
                catch (Exception e)
                {

                }
            }
            return result;
        }

        #region 私有方法
        /// <summary>
        /// 找出Join语句问题方法
        /// </summary>
        /// <param name="line"></param>
        /// <param name="path"></param>
        private static void FindJoinError(string line,string path)
    {
        var joinDictionary = new Dictionary<string, List<int>>();
        var matchCollection = Regex.Matches(line, rex, RegexOptions.IgnoreCase);
        int lineCount = 0;
        foreach (var match in matchCollection)
        {
            //计算行号
            lineCount += CalculationCount(match.ToString(), lineRex);
            //分解join语句
            var matchCollectionJoin = Regex.Matches(match.ToString(), sqlJoin, RegexOptions.IgnoreCase);
            //有join语句才进行计算，没有直接下一次
            switch (matchCollectionJoin.Count)
            {
                case 0:
                    break;
                case 1:
                    foreach (var joinInSql in matchCollectionJoin)
                    {
                        if(!Regex.IsMatch(joinInSql.ToString(), sqlOrg))
                            {
                                DictionaryAdd(joinDictionary, path, lineCount);
                            }
                    }
                    break;
                default:
                    lineCount = lineCount - matchCollectionJoin.Count + 1;
                    var nexLineCount = lineCount;
                    foreach (var matchJoin in matchCollectionJoin)
                    {
                        if(!Regex.IsMatch(matchJoin.ToString(), sqlOrg))
                        {
                              DictionaryAdd(joinDictionary, path, nexLineCount++);
                        }
                        continue;
                    }
                    lineCount = nexLineCount - 1;
                    break;
            }                
        }
            DictionaryAdd(result, errorMes[0], joinDictionary);
    }

        /// <summary>
        /// 找出select*问题方法
        /// </summary>
        /// <param name="line"></param>
        /// <param name="path"></param>
        private static void FindSelectError(string line, string path)
    {
        var selectDictionary = new Dictionary<string, List<int>>();
        int lineCount = 0;
        var matchCollection = Regex.Matches(line, select, RegexOptions.IgnoreCase);
        foreach(var match in matchCollection)
            {
                //计算行号
                lineCount += CalculationCount(match.ToString(), lineRex);
                var matchCollectionSelect = Regex.Matches(match.ToString(), errorSelect, RegexOptions.IgnoreCase);
                foreach (var matchSelect in matchCollectionSelect)
                {
                    DictionaryAdd(selectDictionary, path, lineCount + 1);
                }

            }
            DictionaryAdd(result,errorMes[1],selectDictionary);
        }

        /// <summary>
        /// 字典Add去重方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="addParameter"></param>
        private static void DictionaryAdd<T>(Dictionary<string,List<T>> dictionary, string key, T addParameter)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key].Add(addParameter);
            }
            else
            {
                dictionary.Add(key, new List<T> { addParameter });
            }
        }

        /// <summary>
        /// 私有计数方法
        /// </summary>
        /// <param name="fatherSql"></param>
        /// <param name="sonSql"></param>
        /// <returns></returns>
        private static int CalculationCount(string fatherSql, string sonSql)
        {
            int count = 0;
            var matchCollection = Regex.Matches(fatherSql, sonSql);
            count = matchCollection.Count;
            return count;
        }
    }
    #endregion
}
