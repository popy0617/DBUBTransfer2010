using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DBUBTransfer
{
    public static class GlobalParameters
    {
        /// <summary>
        /// 比對big5-cns-utf8後的資料
        /// </summary>
        public static XmlDocument SummaryTrans;

        public static string DBConStr_IP = string.Empty;
        public static string DBConStr_Port = string.Empty;
        public static string DBConStr_Account = string.Empty;
        public static string DBConStr_PW = string.Empty;
    }
}
