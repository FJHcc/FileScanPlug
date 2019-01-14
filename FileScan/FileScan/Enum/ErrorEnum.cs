using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileScan
{
    public enum ErrorEnum
    {
        /// <summary>
        /// 筛选join
        /// </summary>
        [Description("筛选join")]
        JoinScreen = 1,

        /// <summary>
        /// 筛选select *
        /// </summary>
        [Description("筛选select *")]
        SelectScreen = 2

    }
}
