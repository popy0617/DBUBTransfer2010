using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MSDAO
{
    /// <summary>
    /// SqlUtil 的摘要描述
    /// 負責拆解物件組成SQL語法
    /// </summary>
    public class SqlHelper
    {
        /// <summary>
        /// 由vo中取得刪除字串
        /// </summary>
        /// <param name="ObjVO"></param>
        /// <returns></returns>
        public static string GetDeleteString(BaseVO ObjVO)
        {
            Type objType = ObjVO.GetType();

            StringBuilder QueryString = new StringBuilder();

            QueryString.AppendFormat("DELETE FROM {0} ", objType.InvokeMember("TargetTable", BindingFlags.GetProperty, null, ObjVO, new object[] { }).ToString());

            QueryString.Append(GetQueryString(ObjVO));

            return QueryString.ToString();
        }

        /// <summary>
        /// 由vo中取得insert字串
        /// </summary>
        /// <param name="ObjVO"></param>
        /// <returns>string</returns>
        public static string GetInsertString(BaseVO ObjVO)
        {
            Type objType = ObjVO.GetType();

            StringBuilder SqlColDesc = new StringBuilder();
            StringBuilder SqlValuelDesc = new StringBuilder();
            StringBuilder SqlDesc = new StringBuilder();

            List<string> ColNames = ObjVO.GetPropertyNames;

            foreach (string ColName in ColNames)
            {
                SqlColDesc.AppendFormat(",\"{0}\"", ColName);
                SqlValuelDesc.AppendFormat(",:{0}", ColName);
            }

            SqlDesc.AppendFormat("INSERT INTO {0} (", objType.InvokeMember("TargetTable", BindingFlags.GetProperty, null, ObjVO, new object[] { }).ToString());
            SqlDesc.AppendFormat("{0}) VALUES ({1})", SqlColDesc.ToString().Substring(1), SqlValuelDesc.ToString().Substring(1));

            return SqlDesc.ToString();
        }

        /// <summary>
        /// 傳入VO，自動產生where條件
        /// </summary>
        /// <param name="ObjVO"></param>
        /// <returns>string</returns>
        public static string GetQueryString(BaseVO ObjVO)
        {
            Type objType = ObjVO.GetType();

            StringBuilder QueryString = new StringBuilder(" WHERE 1 = 1");

            List<string> ColNames = ObjVO.GetPropertyNames;

            foreach (string ColName in ColNames)
            {
                QueryString.Append(" AND \"" + ColName + "\" =: " + ColName);
            }

            if (!string.IsNullOrEmpty(ObjVO.ConditionString.Trim()))
                QueryString.Append("  " + ObjVO.ConditionString.Trim() + " ");

            if (!string.IsNullOrEmpty(ObjVO.OrderBy.Trim()))
                QueryString.Append("  " + ObjVO.OrderBy.Trim() + " ");

            return QueryString.ToString();
        }

        /// <summary>
        /// 由vo中取得查詢字串
        /// </summary>
        /// <param name="ObjVO"></param>
        /// <returns></returns>
        public static string GetSelectString(BaseVO ObjVO)
        {
            Type objType = ObjVO.GetType();

            StringBuilder QueryString = new StringBuilder();

            QueryString.AppendFormat("SELECT * FROM {0} ", objType.InvokeMember("TargetTable", BindingFlags.GetProperty, null, ObjVO, new object[] { }).ToString());

            QueryString.Append(GetQueryString(ObjVO));

            return QueryString.ToString();
        }

        /// <summary>
        /// 由vo中取得更新字串
        /// </summary>
        /// <param name="myVO"></param>
        /// <returns>string</returns>
        public static string GetUpdateString(BaseVO ObjVO)
        {
            Type objType = ObjVO.GetType();

            string QueryString = string.Empty;

            List<string> UpColNames = ObjVO.GetPropertyNames;

            foreach (string ColName in UpColNames)
            {
                QueryString = QueryString + " , \"" + ColName + "\" =: " + ColName;
            }

            if (!string.IsNullOrEmpty(ObjVO.UpdateString.Trim()))
            {
                string tmpColName = ObjVO.UpdateString.Trim();
                QueryString = QueryString + ", " + tmpColName;
            }
            if (QueryString.Length != 0)
            {
                QueryString = "SET " + QueryString.Substring(2);
            }

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" Update {0} ", objType.InvokeMember("TargetTable", BindingFlags.GetProperty, null, ObjVO, new object[] { }).ToString());

            sql.Append(QueryString);

            return sql.ToString();
        }

        /// <summary>
        /// 由vo中取得更新字串
        /// </summary>
        /// <param name="ObjVO"></param>
        /// <param name="ObjVObyWhere"></param>
        /// <returns></returns>
        public static string GetUpdateString(BaseVO ObjVO, BaseVO ObjVObyWhere)
        {
            //更新欄位
            string QueryString = string.Empty;
            Type objType = ObjVO.GetType();
            List<string> UpColNames = ObjVO.GetPropertyNames;
            foreach (string ColName in UpColNames)
            {
                QueryString += " , \"" + ColName + "\" =: " + ColName;
            }
            if (!string.IsNullOrEmpty(ObjVO.UpdateString))
            {
                string tmpColName = ObjVO.UpdateString.Trim();
                QueryString += ", " + tmpColName;
            }
            if (QueryString.Length != 0)
            {
                QueryString = "SET " + QueryString.Substring(2);
            }

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" UPDATE {0} ", objType.InvokeMember("TargetTable", BindingFlags.GetProperty, null, ObjVO, new object[] { }).ToString());

            //條件式
            QueryString += " WHERE 1 = 1";
            objType = ObjVObyWhere.GetType();
            List<string> ColNames = ObjVObyWhere.GetPropertyNames;
            foreach (string ColName in ColNames)
            {
                QueryString += " AND \"" + ColName + "\"= :F#_" + ColName;
            }
            if (!string.IsNullOrEmpty(ObjVObyWhere.ConditionString))
                QueryString += " " + ObjVObyWhere.ConditionString.Trim() + " ";

            //排序
            if (!string.IsNullOrEmpty(ObjVObyWhere.OrderBy.Trim()))
                QueryString += " " + ObjVObyWhere.OrderBy.Trim() + " ";

            sql.Append(QueryString);
            return sql.ToString();
        }

        /// <summary>
        /// 傳入VO與OracleCommand，自動產生OracleCommand的Parameter參數
        /// </summary>
        /// <param name="ObjVO"></param>
        /// <param name="objCommand"></param>
        public static void SetParametersByVO(BaseVO ObjVO, SqlCommand objCommand)
        {
            Type objType = ObjVO.GetType();
            List<string> ColNames = ObjVO.GetPropertyNames;
            foreach (string ColName in ColNames)
            {
                objCommand.Parameters.Add(":" + ColName, objType.InvokeMember("GetPropertyValue", BindingFlags.InvokeMethod, null, ObjVO, new object[] { ColName }) ?? DBNull.Value);
            }
        }

        /// <summary>
        /// 傳入VO、條件與OracleCommand，自動產生OracleCommand的Parameter參數
        /// </summary>
        /// <param name="ObjVO"></param>
        /// <param name="ObjVObyWhere"></param>
        /// <param name="objCommand"></param>
        public static void SetParametersByVO(BaseVO ObjVO, BaseVO ObjVObyWhere, SqlCommand objCommand)
        {
            Type objType = ObjVO.GetType();
            List<string> ColNames = ObjVO.GetPropertyNames;
            foreach (string ColName in ColNames)
            {
                objCommand.Parameters.AddWithValue (":" + ColName, objType.InvokeMember("GetPropertyValue", BindingFlags.InvokeMethod, null, ObjVO, new object[] { ColName }) ?? DBNull.Value);
            }

            if (ObjVObyWhere != null)
            {
                objType = ObjVObyWhere.GetType();
                ColNames = ObjVObyWhere.GetPropertyNames;
                foreach (string ColName in ColNames)
                {
                    objCommand.Parameters.AddWithValue (":F#_" + ColName, objType.InvokeMember("GetPropertyValue", BindingFlags.InvokeMethod, null, ObjVObyWhere, new object[] { ColName }) ?? DBNull.Value);
                }
            }
        }

        /// <summary>
        /// 將傳入的DataTable轉換成SQL的 IN 條件式
        /// EX：('A', 'B', 'C')
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ConditionIn(DataTable dt, int colIdx = 0)
        {
            if (dt == null || dt.Rows.Count == 0)
                return string.Empty;

            //取得資料型態
            Type dataType = dt.Columns[colIdx].DataType;

            string res = "(";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i != 0)
                    res += ",";

                if (dataType == typeof(string))
                    res += "'";

                res += dt.Rows[i][colIdx];

                if (dataType == typeof(string))
                    res += "'";
            }
            res += ")";

            return res;
        }

        public static string ConditionLike(string para)
        {
            return "%" + para + "%";
        }

        public static string ConditionLikeLeft(string para)
        {
            return "%" + para;
        }

        public static string ConditionLikeRight(string para)
        {
            return para + "%";
        }
    }
}
