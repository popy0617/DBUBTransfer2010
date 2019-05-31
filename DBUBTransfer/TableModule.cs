using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSDAO;
using System.Data;

namespace DBUBTransfer
{
    public class TableModule
    {
        private bool _Selected;
        private string _TableName;
        private List<FieldModule> _Fields;
        public TableModule(string dbname, string tablename)
        {
            _Selected = false;
            try
            {
                _TableName = tablename;
                _Fields = new List<FieldModule>();
                //GeneralDao dao = new GeneralDao("139.223.24.227", dbname, "sa", "1qazxcvbnm,./");
                GeneralDao dao = new GeneralDao(GlobalParameters.DBConStr_IP, dbname, GlobalParameters.DBConStr_Account, GlobalParameters.DBConStr_PW);
                string strSql = "SELECT COLUMN_NAME,* FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tablename + "' AND TABLE_SCHEMA = 'dbo'";
                DataTable dtField = dao.QueryForDataTable(strSql);
                if (dtField.Rows.Count > 0)
                {
                    foreach (DataRow row in dtField.Rows)
                    {
                        FieldModule newItem = new FieldModule(false, row["COLUMN_NAME"].ToString(), row["DATA_TYPE"].ToString(), row["CHARACTER_MAXIMUM_LENGTH"].ToString());
                        if (!_Fields.Contains(newItem))
                        {
                            _Fields.Add(newItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; }
        }

        public string TableName
        {
            get { return _TableName; }
        }
        public List<FieldModule> Fields
        {
            get { return _Fields; }
        }

    }
}
