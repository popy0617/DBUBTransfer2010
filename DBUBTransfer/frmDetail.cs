using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MSDAO;

namespace DBUBTransfer
{
    public partial class frmDetail : Form
    {
        //private Dictionary<string, Dictionary<string, List<string>>> filteritem = new Dictionary<string, Dictionary<string, List<string>>>();
        string dbname = string.Empty;
        string tablename = string.Empty;
        public List<DataBaseModule> ComfirmItem = new List<DataBaseModule>();
        frmMain parentForm;
        //public Dictionary<string, Dictionary<string, List<string>>> GetData
        //{
        //    get { return filteritem; }
        //}
        public List<DataBaseModule> GetData
        {
            get { return ComfirmItem; }
        }

        public frmDetail()
        {
            InitializeComponent();
        }

        public frmDetail(string arg, List<DataBaseModule> DBM, frmMain pForm)
        {
            InitializeComponent();
            if (arg.Length > 0)
            {
                string[] tmp = arg.Split('|');
                if (tmp.Length == 2)
                {
                    dbname = tmp[0];
                    tablename = tmp[1];
                    ComfirmItem = DBM;
                    parentForm = pForm;
                    pForm.DetailDBName = dbname;
                    pForm.DetailTableName = tablename;
                }
                Text += "-" + tablename;
            }
        }

        private void frmDetail_Load(object sender, EventArgs e)
        {
            try
            {
                GeneralDao dao = null;
                DataTable dtTables;
                DataTable dtSource = new DataTable();
                dtSource.Columns.Add("Selected", typeof(string));
                dtSource.Columns.Add("SourceFieldName", typeof(string));
                dtSource.Columns.Add("DestinationFieldName", typeof(string));

                dgvData.AutoGenerateColumns = false;
                foreach (DataBaseModule dbm in ComfirmItem)
                {
                    foreach (TableModule tbm in dbm.Tables)
                    {
                        if (tbm.TableName == tablename)
                        {
                            foreach (FieldModule fdm in tbm.Fields)
                            {
                                DataRow dtrow = dtSource.NewRow();
                                dtrow["Selected"] = fdm.Selected;
                                dtrow["SourceFieldName"] = fdm.FieldName;
                                dtrow["DestinationFieldName"] = fdm.MS950_FieldName;
                                dtSource.Rows.Add(dtrow);
                            }
                        }
                    }
                }
                //dao = new GeneralDao("139.223.24.227", dbname, "sa", "1qazxcvbnm,./");
                //string strSql = "SELECT COLUMN_NAME,* FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tablename + "' AND TABLE_SCHEMA = 'dbo'";
                //dtTables = dao.QueryForDataTable(strSql);
                //if (dtTables.Rows.Count > 0)
                //{
                //    foreach (DataRow row in dtTables.Rows)
                //    {
                //        foreach (KeyValuePair<string, Dictionary<string, List<string>>> dbitem in filteritem)
                //        {
                //            if (dbitem.Key != dbname)
                //            {
                //                continue;
                //            }
                //            if (dbitem.Value.ContainsKey(tablename))
                //            {
                //                foreach (KeyValuePair<string, List<string>> tableitem in dbitem.Value)
                //                {
                //                    if (tableitem.Key == tablename)
                //                    {
                //                        DataRow dtrow = dtSource.NewRow();
                //                        dtrow["Selected"] = tableitem.Value.Contains(row["COLUMN_NAME"].ToString().Trim()) ? true : false;
                //                        dtrow["SourceFieldName"] = row["COLUMN_NAME"].ToString().Trim();
                //                        dtrow["DestinationFieldName"] = "MS950_" + row["COLUMN_NAME"].ToString().Trim();

                //                        //DataGridViewButtonColumn btnControl = new DataGridViewButtonColumn();
                //                        //btnControl.Text = "...";
                //                        ////btnControl.Click += delegate (object send, EventArgs ea) { Command_Click(sender, e, row["TABLE_NAME"].ToString()); };
                //                        //btnControl.UseColumnTextForButtonValue = true;
                //                        //dtrow["Control"] = btnControl;
                //                        dtSource.Rows.Add(dtrow);
                //                        ////foreach (string fielditem in tableitem.Value)
                //                        ////{

                //                        ////}
                //                        ////tableData.Add(new Tables() { selected = dbitem.Value.Keys.Contains(row["TABLE_NAME"].ToString()), SourceTableName = row["TABLE_NAME"].ToString(), DestiTableName = row["TABLE_NAME"].ToString(), DBName = dbitem.Key });
                //                        //Dictionary<string, Tables> item = new Dictionary<string, Tables>();
                //                        //item.Add(row["TABLE_NAME"].ToString(), new Tables(row["TABLE_NAME"].ToString(), dbitem.Value, dao));
                //                        //ConverTables.Add(item);                                    
                //                    }
                //                }
                //            }
                //            else
                //            {
                //                DataRow dtrow = dtSource.NewRow();
                //                dtrow["Selected"] = false;
                //                dtrow["SourceFieldName"] = row["COLUMN_NAME"].ToString().Trim();
                //                dtrow["DestinationFieldName"] = "MS950_" + row["COLUMN_NAME"].ToString().Trim();
                //                dtSource.Rows.Add(dtrow);
                //            }
                //        }
                //    }

                //}
                ////foreach (KeyValuePair<string, Dictionary<string, List<string>>> dbitem in filteritem)
                ////{
                ////    if (dbitem.Key != dbname)
                ////    {
                ////        continue;
                ////    }
                ////    foreach (KeyValuePair<string, List<string>> tableitem in dbitem.Value)
                ////    {
                ////        dao = new GeneralDao("139.223.24.227", dbitem.Key, "sa", "1qazxcvbnm,./");
                ////        if (tableitem.Key == tablename)
                ////        {
                ////            string strSql = "SELECT COLUMN_NAME,* FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableitem.Key + "' AND TABLE_SCHEMA = 'dbo'";
                ////            dtTables = dao.QueryForDataTable(strSql);
                ////            if (dtTables.Rows.Count > 0)
                ////            {
                ////                foreach (DataRow row in dtTables.Rows)
                ////                {
                ////                    DataRow dtrow = dtSource.NewRow();
                ////                    dtrow["Selected"] = tableitem.Value.Contains(row["COLUMN_NAME"].ToString().Trim()) ? true : false;
                ////                    dtrow["SourceFieldName"] = row["COLUMN_NAME"].ToString().Trim();
                ////                    dtrow["DestinationFieldName"] = "MS950_" + row["COLUMN_NAME"].ToString().Trim();

                ////                    //DataGridViewButtonColumn btnControl = new DataGridViewButtonColumn();
                ////                    //btnControl.Text = "...";
                ////                    ////btnControl.Click += delegate (object send, EventArgs ea) { Command_Click(sender, e, row["TABLE_NAME"].ToString()); };
                ////                    //btnControl.UseColumnTextForButtonValue = true;
                ////                    //dtrow["Control"] = btnControl;
                ////                    dtSource.Rows.Add(dtrow);
                ////                    ////foreach (string fielditem in tableitem.Value)
                ////                    ////{

                ////                    ////}
                ////                    ////tableData.Add(new Tables() { selected = dbitem.Value.Keys.Contains(row["TABLE_NAME"].ToString()), SourceTableName = row["TABLE_NAME"].ToString(), DestiTableName = row["TABLE_NAME"].ToString(), DBName = dbitem.Key });
                ////                    //Dictionary<string, Tables> item = new Dictionary<string, Tables>();
                ////                    //item.Add(row["TABLE_NAME"].ToString(), new Tables(row["TABLE_NAME"].ToString(), dbitem.Value, dao));
                ////                    //ConverTables.Add(item);
                ////                }
                ////            }
                ////        }
                ////    }
                ////}
                foreach (DataRow row in dtSource.Rows)
                {
                    dgvData.Rows.Add(row["Selected"], row["SourceFieldName"], row["DestinationFieldName"]);
                }

                ////foreach (Dictionary<string,List<string>> dictb in filteritem)
                ////{
                ////foreach (Tables tb in dictb.Values)
                ////{
                ////    foreach (TableFields tf in tb.Fields)
                ////    {
                ////        GeneralDao dao = new GeneralDao();
                ////        strSql = "SELECT COLUMN_NAME,* FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + strTableName + "' AND TABLE_SCHEMA = 'dbo'";
                ////        dtTable = dao.QueryForDataTable(strSql);
                ////        if (dtTable.Rows.Count > 0)
                ////        {
                ////            foreach (DataRow row in dtTable.Rows)
                ////            {
                ////                tableData.Add(new TableFields() { selected = true, SourceColumnName = row["COLUMN_NAME"].ToString(), DestiColumnName = row["COLUMN_NAME"].ToString() });
                ////            }
                ////        }
                ////    }
                ////}
                ////}                
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                Util tool = new Util();
                bool HasSelected = false; //全不選
                foreach (DataGridViewRow row in dgvData.Rows)
                {
                    if (tool.StrToBool(row.Cells["Selected"].Value.ToString()))
                    {
                        HasSelected = true;
                        //資料集內無資料勾選要新增
                        foreach (DataBaseModule dbm in ComfirmItem)
                        {
                            dbm.CheckFieldValue(dbm.DBName, tablename, row.Cells["SourceFieldName"].Value.ToString(), true);
                        }
                    }
                    else
                    {
                        //資料集內有資料勾選要去除
                        foreach (DataBaseModule dbm in ComfirmItem)
                        {
                            dbm.CheckFieldValue(dbm.DBName, tablename, row.Cells["SourceFieldName"].Value.ToString(), false);
                        }
                    }
                }
                if (!HasSelected)
                {
                    //資料集內有資料勾選要去除
                    foreach (DataBaseModule dbm in ComfirmItem)
                    {
                        dbm.CheckTableValue(dbm.DBName, tablename, false);
                    }
                }
                //filteritem;
                //foreach(DataGridViewRow row in dgvData.Rows)
                //{
                //    if (tool.StrToBool(row.Cells["Selected"].Value.ToString()))
                //    {
                //        AllDisSelected = true;
                //        //資料集內無資料要新增
                //        if (!filteritem.ContainsKey(dbname))
                //        {
                //            filteritem.Add(dbname, new Dictionary<string, List<string>>() { { tablename, new List<string>() { row.Cells["SourceFieldName"].Value.ToString() } } });
                //        }
                //        else
                //        {

                //        }
                //    }
                //    else
                //    {
                //        AllDisSelected = false;
                //        break;
                //    }                    
                //}
                //if (AllDisSelected)
                //{
                //    foreach (DataGridViewRow row in dgvData.Rows)
                //    {
                //        foreach (KeyValuePair<string, Dictionary<string, List<string>>> dbitem in filteritem)
                //        {
                //            if (dbitem.Key == dbname)
                //            {
                //                foreach (KeyValuePair<string, List<string>> tableitem in dbitem.Value.ToArray())
                //                {
                //                    if (tableitem.Key == tablename)
                //                    {

                //                        if (tool.StrToBool(row.Cells["Selected"].Value.ToString()) == true)
                //                        {
                //                            if (!tableitem.Value.Contains(row.Cells["SourceFieldName"].Value))
                //                            {
                //                                List<string> lst = tableitem.Value;
                //                                lst.Add(row.Cells["SourceFieldName"].Value.ToString());
                //                                Dictionary<string, List<string>> dic = new Dictionary<string, List<string>> { { tableitem.Key, lst } };
                //                                dbitem.Value[tableitem.Key] = lst;
                //                            }
                //                        }
                //                        else
                //                        {
                //                            if (tableitem.Value.Contains(row.Cells["SourceFieldName"].Value))
                //                            {
                //                                dbitem.Value.Remove(tableitem.Key);
                //                            }
                //                        }

                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                //else
                //{

                //}
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                btnRefresh_Click(sender, e);
                Close();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
