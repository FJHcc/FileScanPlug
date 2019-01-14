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
        private static Dictionary<string,List<Dictionary<string, List<Dictionary<string,List<int>>>>>> result = new Dictionary<string, List<Dictionary<string, List<Dictionary<string, List<int>>>>>>();
        private const string rex = "(?s)(.*?)(?<=\")(\\s*select|\\s*join).*?((?<=where)|(?<=\";\r\n))";
        private const string matchSelect = "(?s)(?<=\")(\\s*select|\\s*join).*?((?=where)|(?<=\";))";
        //分解每条join的正则
        private const string JoinSelect = "join\\s{1,}.*(\\s*on).*";
        //select*的正则
        private const string select = "(?s)(.*?)(?<=\")\\s*select.*?\r\n";
        private const string errorSelect = "\\s*select\\s*\\*.*?((?<=\",)|(?<=\";))";
        private const string sqlOrg = "(=\\s*)[a-z]*(.OrgId)";
        private const string lineRex = "\\\n";
        //错误类型
        private static readonly string[] errorMes = new [] { "join语句缺少orgId的文件及位置：", "存在select *的的文件及位置：" }  ;

        /// <summary>
        /// 实现方法
        /// </summary>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public static Dictionary<string, List<Dictionary<string, List<Dictionary<string, List<int>>>>>> Filter(List<string> fileList)
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
                        //join语句错误
                        var joinError = FindSqlError(line,rex, JoinSelect,path,ErrorEnum.JoinScreen);
                        //select *错误
                        var selectError = FindSqlError(line, select, errorSelect, path, ErrorEnum.SelectScreen);
                        DictionaryAdd(result, errorMes[0], joinError);
                        DictionaryAdd(result, errorMes[1], selectError);
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
        
        private static Dictionary<string,List<Dictionary<string,List<int>>>> FindSqlError(string line, string sqlRex, string errorSql,string path, ErrorEnum errorType)
        {
            var dictionary = new Dictionary<string, List<Dictionary<string, List<int>>>>();
            var matchCollection = Regex.Matches(line, sqlRex, RegexOptions.IgnoreCase);
            int lineCount = 0;
            foreach(var match in matchCollection)
            {
                lineCount += CalculationCount(match.ToString(), lineRex);
                var matchCollectionError = Regex.Matches(match.ToString(), errorSql, RegexOptions.IgnoreCase);
                MatchError(dictionary,match.ToString(),matchCollectionError,path,lineCount,errorType);
            }
            return dictionary;
        }
    //    /// <summary>
    //    /// 找出Join语句问题方法
    //    /// </summary>
    //    /// <param name="line"></param>
    //    /// <param name="path"></param>
    //    private static void FindJoinError(string line,string path)
    //{
    //    var joinDictionary = new Dictionary<string, List<int>>();
    //    var matchCollection = Regex.Matches(line, rex, RegexOptions.IgnoreCase);
    //    int lineCount = 0;
    //    foreach (var match in matchCollection)
    //    {
    //        //计算行号
    //        lineCount += CalculationCount(match.ToString(), lineRex);
    //        //分解join语句
    //        var matchCollectionJoin = Regex.Matches(match.ToString(), sqlJoin, RegexOptions.IgnoreCase);
    //        //有join语句才进行计算，没有直接下一次
    //        MatchError(joinDictionary, matchCollectionJoin, path, lineCount);
    //    }   
    //        DictionaryAdd(result, errorMes[0], joinDictionary);
    //}

    //    /// <summary>
    //    /// 找出select*问题方法
    //    /// </summary>
    //    /// <param name="line"></param>
    //    /// <param name="path"></param>
    //    private static void FindSelectError(string line, string path)
    //{
    //    var selectDictionary = new Dictionary<string, List<int>>();
    //    int lineCount = 0;
    //    var matchCollection = Regex.Matches(line, select, RegexOptions.IgnoreCase);
    //    foreach(var match in matchCollection)
    //        {
    //            //计算行号
    //            lineCount += CalculationCount(match.ToString(), lineRex);
    //            var matchCollectionSelect = Regex.Matches(match.ToString(), errorSelect, RegexOptions.IgnoreCase);
    //            foreach (var matchSelect in matchCollectionSelect)
    //            {
    //                DictionaryAdd(selectDictionary, path, lineCount);
    //            }

    //        }
    //        DictionaryAdd(result,errorMes[1],selectDictionary);
    //    }

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
            dictionary[key] = dictionary[key].Distinct().ToList();
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

        /// <summary>
        /// 删除多余空格方法
        /// </summary>
        /// <param name="old"></param>
        /// <returns></returns>
        private static string DeleteRedundantSpace(string old)
        {
            old = old.Replace("\r", "").Replace("\n", " ");
            var newArray = old.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var newString = string.Join(" ", newArray);
            return newString;
        }

        /// <summary>
        /// 匹配sql错误方法
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="matchCollection"></param>
        /// <param name="path"></param>
        /// <param name="lineCount"></param>
        private static void MatchError(Dictionary<string, List<Dictionary<string, List<int>>>> dictionary, string match, MatchCollection matchCollection, string path, int lineCount, ErrorEnum errorType)
        {
            var sqlDictionary = new Dictionary<string, List<int>>();
            switch ((int)errorType)
            {
                //join筛选
                case 1:
                    switch (matchCollection.Count)
                    {
                        case 0:
                            break;
                        case 1:
                            if (!Regex.IsMatch(matchCollection[0].ToString(), sqlOrg))
                            {
                                var matchSql = Regex.Match(match, matchSelect, RegexOptions.IgnoreCase);
                                DictionaryAdd(sqlDictionary, DeleteRedundantSpace(matchSql.ToString()), lineCount);
                                DictionaryAdd(dictionary, path, sqlDictionary);
                            }
                            break;
                        default:
                            lineCount = lineCount - matchCollection.Count + 1;
                            var nexLineCount = lineCount;
                            foreach (var matchJoin in matchCollection)
                            {
                                if (!Regex.IsMatch(matchJoin.ToString(), sqlOrg))
                                {
                                    var matchSql = Regex.Match(match, matchSelect, RegexOptions.IgnoreCase);
                                    DictionaryAdd(sqlDictionary, DeleteRedundantSpace(matchSql.ToString()), lineCount++);
                                    DictionaryAdd(dictionary, path, sqlDictionary);
                                }
                            }
                            lineCount = nexLineCount - 1;
                            break;
                    }
                    break;
                //select *筛选
                case 2:
                    if (matchCollection.Count > 0)
                    {
                        DictionaryAdd(sqlDictionary, matchCollection[0].ToString().Replace("\r", "").Replace("\n", ""), lineCount);
                        DictionaryAdd(dictionary, path, sqlDictionary);
                    }
                    break;
            }

        }
    }
    #endregion
}




