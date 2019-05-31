using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAO
{
    public class BaseVO
    {
        protected List<string> PropertyNames = new List<string>();

        protected string _OrderBy = string.Empty;

        protected string _UpdateString = string.Empty;

        protected string _ConditionString = string.Empty;

        public BaseVO()
        {
        }

        public List<string> GetPropertyNames
        {
            get
            {
                return PropertyNames;
            }
        }

        /// <summary>
        /// 排序條件
        /// EX：ORDER BY COL1, COL2 DESC
        /// </summary>
        public string OrderBy
        {
            get
            {
                return _OrderBy;
            }
            set
            {
                _OrderBy = value;
            }
        }

        /// <summary>
        /// 額外的查詢條件式
        /// EX：AND (COL3 IS NULL OR COL3 = 'C')
        /// </summary>
        public string ConditionString
        {
            get
            {
                return _ConditionString;
            }
            set
            {
                _ConditionString = value;
            }
        }

        /// <summary>
        /// 額外的更新欄位及其值
        /// EX：COL1 = NULL, COL2 = 'B', COl3 = 'C'
        /// </summary>
        public string UpdateString
        {
            get
            {
                return _UpdateString;
            }
            set
            {
                _UpdateString = value;
            }
        }
    }
}
