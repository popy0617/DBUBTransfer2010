using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSDAO;
using System.Data;

namespace DBUBTransfer
{
    /// <summary>
    /// 資料庫模組化架構
    /// </summary>
    public class DataModule
    {
        //private Dictionary<string, Dictionary<string, List<string>>> filteritem = new Dictionary<string, Dictionary<string, List<string>>>();
        private Dictionary<string, Dictionary<string, List<string>>> DBEntity = new Dictionary<string, Dictionary<string, List<string>>>();
        public DataModule()
        {
            try
            {
                GeneralDao dao = null;
                DataTable dtSource = new DataTable();
                DataTable dtDBNameTables = new DataTable();
                DataTable dtTablesName = new DataTable();
                DataTable dtFieldName = new DataTable();
                dtSource.Columns.Add("Selected", typeof(string));
                dtSource.Columns.Add("DBName", typeof(string));
                dtSource.Columns.Add("SourceTableName", typeof(string));
                dtSource.Columns.Add("DestinationTableName", typeof(string));

                dao = new GeneralDao("139.223.24.223", "master", "sa", "1qazxcvbnm,./");

                string strSql = "SELECT name FROM master.dbo.sysdatabases";
                dtDBNameTables = dao.QueryForDataTable(strSql);
                if (dtDBNameTables.Rows.Count > 0)
                {
                    foreach (DataRow dbrow in dtDBNameTables.Rows)
                    {
                        if (!DBEntity.ContainsKey(dbrow["name"].ToString()))
                        {
                            DBEntity.Add(dbrow["name"].ToString(), null);
                        }
                        switch (dbrow["name"].ToString())
                        {
                            case "jdcs":
                            case "g0":
                            case "jdcstest":
                            case "exdcs":
                            case "tdcs":
                            case "jdcsx3":
                                GeneralDao dbdao = new GeneralDao("139.223.24.223", dbrow["name"].ToString(), "sa", "1qazxcvbnm,./");
                                strSql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG = '" + dbrow["name"].ToString() + "'";
                                dtTablesName = dbdao.QueryForDataTable(strSql);
                                if (dtTablesName.Rows.Count > 0)
                                {
                                    foreach (DataRow tnrow in dtTablesName.Rows)
                                    {
                                        //if()
                                        strSql = "SELECT COLUMN_NAME,*FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tnrow["TABLE_NAME"].ToString() + "' AND TABLE_SCHEMA = 'dbo'";
                                        dtFieldName = dbdao.QueryForDataTable(strSql);
                                        foreach (DataRow fnrow in dtFieldName.Rows)
                                        {
                                            DBEntity.Add(dbrow["name"].ToString(), new Dictionary<string, List<string>>() { { tnrow["TABLE_NAME"].ToString(), new List<string>() { fnrow["COLUMN_NAME"].ToString() } } });
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 查詢是否已有欄位名稱相同物件
        /// </summary>
        /// <param name="dbname"></param>
        /// <param name="tablename"></param>
        /// <param name="fieldname"></param>
        /// <returns></returns>
        public bool FieldExists(string dbname, string tablename, string fieldname)
        {
            try
            {
                bool boolReturn = false;

                foreach (KeyValuePair<string, Dictionary<string, List<string>>> dbitem in DBEntity)
                {
                    if (dbitem.Key == dbname)
                    {
                        foreach (KeyValuePair<string, List<string>> tableitem in dbitem.Value)
                        {
                            if (tableitem.Key == tablename)
                            {
                                foreach (string field in tableitem.Value)
                                {
                                    if (field == fieldname)
                                    {
                                        boolReturn = true;
                                    }
                                }
                            }
                        }
                    }
                }

                return boolReturn;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 查詢是否已有資料表名稱相同物件
        /// </summary>
        /// <param name="dbname"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public bool TableExists(string dbname, string tablename)
        {
            try
            {
                bool boolReturn = false;

                foreach (KeyValuePair<string, Dictionary<string, List<string>>> dbitem in DBEntity)
                {
                    if (dbitem.Key == dbname)
                    {
                        foreach (KeyValuePair<string, List<string>> tableitem in dbitem.Value)
                        {
                            if (tableitem.Key == tablename)
                            {
                                boolReturn = true;
                            }
                        }
                    }
                }

                return boolReturn;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DataBasExists(string dbname)
        {
            try
            {
                bool boolReturn = false;

                foreach (KeyValuePair<string, Dictionary<string, List<string>>> dbitem in DBEntity)
                {
                    if (dbitem.Key == dbname)
                    {
                        boolReturn = true;
                    }
                }

                return boolReturn;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
