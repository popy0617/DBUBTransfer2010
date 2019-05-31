using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAO
{
    public class TaSqlCommand
    {
        private SqlCommand _cmd;

        # region 分頁資訊

        // 每頁筆數
        public int PageSize { get; set; }

        // 第幾頁
        public int PageIdx { get; set; }

        // 是否需要分頁，如為0則不分頁
        public bool IsPaging()
        {
            return (PageIdx != 0);
        }

        #endregion

        public TaSqlCommand()
        {
            _cmd = new SqlCommand();
            _cmd.Parameters.Clear();
            PageSize = 5;
            PageIdx = 0;
        }

        public SqlCommand DbCmd
        {
            get
            {
                return _cmd;
            }
        }
    }
}
