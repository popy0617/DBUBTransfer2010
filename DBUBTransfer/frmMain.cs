using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MSDAO;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Reflection;
using System.Xml;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ClosedXML.Excel;

namespace DBUBTransfer
{
    public partial class frmMain : Form
    {
        public List<DataBaseModule> ComfirmItem = new List<DataBaseModule>();
        //Dictionary<string, Dictionary<string, List<string>>> filteritem = new Dictionary<string, Dictionary<string, List<string>>>();
        private string _DetailDBName = string.Empty;
        private string _DetailTableName = string.Empty;
        private List<string> strSelectedFields;
        private List<string> strSelectedMSFields;

        private void StartTransfer(int iActFlag)
        {
            try
            {
                GeneralDao dao = null; ;
                string strSql = string.Empty;

                Stopwatch sw = new Stopwatch();
                string rowid = string.Empty;
                double TotalBackupRows = 0;
                double TotalTransRows = 0;
                //開始轉碼
                //備份各欄位資料
                #region 備份資料
                if (iActFlag == 0 || iActFlag == 2)//全部||只備份不轉碼
                {
                    foreach (DataBaseModule dbm in ComfirmItem)
                    {
                        //dao = new GeneralDao("139.223.24.227", dbm.DBName, "sa", "1qazxcvbnm,./");
                        dao = new GeneralDao(GlobalParameters.DBConStr_IP, dbm.DBName, GlobalParameters.DBConStr_Account, GlobalParameters.DBConStr_PW);
                        ////重建資料型態TVP_Transfer
                        //strSql = "IF type_id('[dbo].[TVP_Transfer]') IS NOT NULL DROP TYPE[dbo].[TVP_Transfer]";
                        //dao.Execute(strSql);
                        ////strSql = "CREATE TYPE [dbo].[TVP_Transfer] AS TABLE([rowid][int] NOT NULL,[tablename] [varchar] (20) NOT NULL,[fieldname] [varchar] (20) NOT NULL,[value] [nvarchar] (512) NOT NULL)";
                        //strSql = "CREATE TYPE [dbo].[TVP_Transfer] AS TABLE([rowid][int] NOT NULL,[value] [nvarchar] (512) NOT NULL)";
                        //dao.Execute(strSql);

                        foreach (TableModule tbm in dbm.Tables)
                        {
                            DataTable tvp = new DataTable();
                            //測試2 使用TVP
                            using (SqlConnection cn = new SqlConnection(dao.ConnectionString))
                            {
                                cn.Open();
                                sw.Reset();
                                sw.Start();
                                int fieldCount = 0;
                                string UpdateField = string.Empty;
                                SqlCommand cmd = cn.CreateCommand();
                                cmd.CommandTimeout = 600;
                                cmd.Parameters.Clear();
                                SqlParameter pTVP = cmd.Parameters.Add("@UpdateTable", SqlDbType.Structured);
                                pTVP.Value = tvp; //SqlParameter選用SqlDbType.Structured並指定TypeName
                                pTVP.TypeName = "TVP_Transfer";
                                //cmd.Parameters.AddWithValue("@tablename", tbm.TableName);
                                tvp.Columns.Add("rowid", typeof(SqlInt32));
                                //tvp.Columns.Add("tablename", typeof(SqlString));
                                //tvp.Columns.Add("fieldname", typeof(SqlString));
                                //tvp.Columns.Add("value", typeof(SqlString));
                                strSelectedFields = new List<string>();
                                strSelectedMSFields = new List<string>();

                                foreach (FieldModule fdm in tbm.Fields)
                                {
                                    if (fdm.Selected)
                                    {
                                        fieldCount++;
                                        #region 欄位處理
                                        try
                                        {
                                            //查詢備份欄位
                                            strSql = "select Name from sys.columns where Name = '" + fdm.MS950_FieldName + "' and Object_ID = Object_ID('" + tbm.TableName + "')";
                                            DataTable dt = dao.QueryForDataTable(strSql);

                                            if (dt.Rows.Count == 0) //查無備份欄位
                                            {
                                                //新增備位欄位
                                                strSql = "ALTER TABLE " + tbm.TableName + " ADD  " + fdm.MS950_FieldName + " [varchar](" + (Convert.ToInt16(Convert.ToInt16(fdm.DataLength) * 1.5)) + ") NOT NULL DEFAULT ''";
                                                dao.Execute(strSql);

                                            }
                                            strSelectedFields.Add(fdm.FieldName);
                                            strSelectedMSFields.Add(fdm.MS950_FieldName);
                                            tvp.Columns.Add(fdm.FieldName, typeof(SqlString));
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ex;
                                        }
                                        #endregion
                                    }
                                }
                                if (strSelectedFields.Count > 0)
                                {
                                    //重建資料型態TVP_Transfer
                                    strSql = "IF type_id('[dbo].[TVP_Transfer]') IS NOT NULL DROP TYPE[dbo].[TVP_Transfer]";
                                    dao.Execute(strSql);
                                    //strSql = "CREATE TYPE [dbo].[TVP_Transfer] AS TABLE([rowid][int] NOT NULL,[tablename] [varchar] (20) NOT NULL,[fieldname] [varchar] (20) NOT NULL,[value] [nvarchar] (512) NOT NULL)";
                                    strSql = "CREATE TYPE [dbo].[TVP_Transfer] AS TABLE([rowid][int]";// NOT NULL,[value] [nvarchar] (512) NOT NULL)";
                                    foreach (string ss in strSelectedFields)
                                    {
                                        strSql += ",[" + ss + "][nvarchar](512) NOT NULL DEFAULT ''";
                                    }
                                    strSql += ")";
                                    dao.Execute(strSql);

                                    //將原有資料匯入ms950欄位
                                    //取出原所有需備份資料//新增欄位值   
                                    int fieldcount = 0;
                                    strSql = "SELECT rowid" + (strSelectedFields.Count > 0 ? "," + string.Join(",", strSelectedFields) : "") + " FROM " + tbm.TableName;
                                    DataTable Backdt = dao.QueryForDataTable(strSql);
                                    if (Backdt.Rows.Count > 0) //有資料
                                    {
                                        for (int i = 0; i < Backdt.Rows.Count; i++)
                                        {
                                            TotalBackupRows++;
                                            DataRow tvpNew = tvp.NewRow();
                                            tvpNew["rowid"] = Backdt.Rows[i]["rowid"];
                                            foreach (string field in strSelectedFields)
                                            {
                                                tvpNew[field] = Backdt.Rows[i][field] == DBNull.Value ? "" : Backdt.Rows[i][field];
                                            }
                                            //tvpNew["tablename"] = tbm.TableName;
                                            //tvpNew["fieldname"] = fdm.FieldName;
                                            //tvpNew["aaa"] = Backdt.Rows[i][fdm.FieldName] == DBNull.Value ? "" : Backdt.Rows[i][fdm.FieldName];
                                            tvp.Rows.Add(tvpNew);
                                            //滿10000更新一次資料
                                            if (tvp.Rows.Count > 999)
                                            {
                                                cmd.CommandText = "UPDATE " + tbm.TableName + " SET ";
                                                for (int ii = 0; ii < strSelectedMSFields.Count; ii++)
                                                {
                                                    if (ii > 0) { cmd.CommandText += ","; }
                                                    cmd.CommandText += (strSelectedMSFields[ii] + "=ut." + strSelectedFields[ii]);
                                                }
                                                //cmd.CommandText += fdm.MS950_FieldName + "=ut.value";// + (((fdm.FieldName == "DJ_WORD") && (dbm.DBName == "jdcs") && (tbm.TableName == "DCS0_DOC_JONL")) ? " ,flag=1" : "");
                                                cmd.CommandText += " FROM " + tbm.TableName + " bt Join @UpdateTable ut On bt.rowid=ut.rowid";

                                                try
                                                {
                                                    fieldcount = cmd.ExecuteNonQuery();                                                    
                                                }
                                                catch (Exception ex)
                                                {
                                                    MessageBox.Show(ex.Message + "\r\n" + dbm.DBName + "：" + tbm.TableName);
                                                }

                                                tvp.Rows.Clear();//清除已更新資料庫後的資料集

                                                Console.WriteLine("資料表{0}欄位，{2:N0}ms {1}筆紀錄備份", tbm.TableName, fieldcount, sw.ElapsedMilliseconds);
                                                txtMsg.AppendText(Environment.NewLine + string.Format("資料表{0}欄位，{2:N0}ms {1}筆紀錄備份", tbm.TableName, fieldcount, sw.ElapsedMilliseconds));
                                                txtMsg.SelectionStart = txtMsg.TextLength;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                            }
                                        }
                                        //已更新批次萬筆資料後，剩餘資料更新
                                        if (tvp.Rows.Count > 0)
                                        {
                                            cmd.CommandText = "UPDATE " + tbm.TableName + " SET ";
                                            for (int ii = 0; ii < strSelectedMSFields.Count; ii++)
                                            {
                                                if (ii > 0) { cmd.CommandText += ","; }
                                                cmd.CommandText += (strSelectedMSFields[ii] + "=ut." + strSelectedFields[ii]);
                                            }
                                            //cmd.CommandText += fdm.MS950_FieldName + "=ut.value";
                                            cmd.CommandText += " FROM " + tbm.TableName + " bt Join @UpdateTable ut On bt.rowid=ut.rowid";

                                            try
                                            {
                                                fieldcount = cmd.ExecuteNonQuery();                                                
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show(ex.Message + "\r\n" + dbm.DBName + "：" + tbm.TableName);
                                            }

                                            tvp.Rows.Clear();// 清除已更新資料庫後的資料集

                                            Console.WriteLine("資料表{0}欄位，{2:N0}ms {1}筆紀錄備份", tbm.TableName, fieldcount, sw.ElapsedMilliseconds);
                                            txtMsg.AppendText(Environment.NewLine + string.Format("資料表{0}欄位，{2:N0}ms {1}筆紀錄備份", tbm.TableName, fieldcount, sw.ElapsedMilliseconds));
                                            txtMsg.SelectionStart = txtMsg.TextLength;
                                            txtMsg.ScrollToCaret();
                                            txtMsg.Refresh();
                                        }
                                    }

                                }
                                sw.Stop();
                            }
                        }
                    }
                    Console.WriteLine(DateTime.Now + "備份結束");
                    txtMsg.AppendText(Environment.NewLine + string.Format(DateTime.Now + "備份結束"));
                    txtMsg.SelectionStart = txtMsg.TextLength;
                    txtMsg.ScrollToCaret();
                    txtMsg.Refresh();
                }
                #endregion
                #region 資料轉碼
                if (iActFlag == 0 || iActFlag == 1)//全部||只轉碼不備份
                {
                    DialogResult result = MessageBox.Show("所有欄位已備份，請問是否繼續？", "請選擇", MessageBoxButtons.YesNoCancel);
                    if (DialogResult.Yes == result)
                    {
                        sw.Reset();
                        sw.Start();
                        Console.WriteLine(DateTime.Now + "轉碼開始");
                        txtMsg.AppendText(Environment.NewLine + string.Format(DateTime.Now + "轉碼開始"));
                        txtMsg.SelectionStart = txtMsg.TextLength;
                        txtMsg.ScrollToCaret();
                        txtMsg.Refresh();
                        //轉碼並更新各欄位
                        foreach (DataBaseModule dbm in ComfirmItem)
                        {
                            //dao = new GeneralDao("139.223.24.227", dbm.DBName, "sa", "1qazxcvbnm,./");
                            dao = new GeneralDao(GlobalParameters.DBConStr_IP, dbm.DBName, GlobalParameters.DBConStr_Account, GlobalParameters.DBConStr_PW);
                            foreach (TableModule tbm in dbm.Tables)
                            {
                                DataTable tvp = new DataTable();

                                //測試2 使用TVP
                                using (SqlConnection cn = new SqlConnection(dao.ConnectionString))
                                {
                                    cn.Open();
                                    int fieldCount = 0;
                                    SqlCommand cmd = cn.CreateCommand();
                                    cmd.ResetCommandTimeout();
                                    cmd.CommandTimeout = 120;
                                    cmd.Parameters.Clear();
                                    SqlParameter pTVP = cmd.Parameters.Add("@UpdateTable", SqlDbType.Structured);
                                    pTVP.Value = tvp; //SqlParameter選用SqlDbType.Structured並指定TypeName
                                    pTVP.TypeName = "TVP_Transfer";
                                    tvp.Columns.Add("rowid");
                                    strSelectedFields = new List<string>();
                                    strSelectedMSFields = new List<string>();

                                    foreach (FieldModule fdm in tbm.Fields)
                                    {
                                        if (fdm.Selected)
                                        {
                                            fieldCount++;
                                            #region 欄位處理
                                            try
                                            {
                                                DataTable dt;
                                                strSql = "select Name from sys.columns where Name = '" + fdm.FieldName + "' and Object_ID = Object_ID('" + tbm.TableName + "')";
                                                dt = dao.QueryForDataTable(strSql);
                                                if (dt.Rows.Count == 1)
                                                {
                                                    //strSql = "IF NOT EXISTS (SELECT * FROM sys.columns WHERE  object_id = OBJECT_ID(N'" + tbm.TableName + "') AND name = '" + fdm.MS950_FieldName + "' ) ALTER TABLE " + tbm.TableName + " ADD  " + fdm.MS950_FieldName + " [varchar](" + fdm.DataLength + ") NOT NULL DEFAULT ''";
                                                    strSql = "ALTER TABLE " + tbm.TableName + " ALTER COLUMN  " + fdm.FieldName + " [nvarchar](" + (fdm.DataLength) + ")";
                                                    dao.Execute(strSql);
                                                }
                                                strSelectedFields.Add(fdm.FieldName);
                                                strSelectedMSFields.Add(fdm.MS950_FieldName);
                                                tvp.Columns.Add(fdm.FieldName, typeof(SqlString));
                                            }
                                            catch (Exception ex)
                                            {

                                                throw ex;
                                            }
                                            #endregion
                                        }
                                    }
                                    if (strSelectedFields.Count > 0)
                                    {
                                        //重建資料型態TVP_Transfer
                                        strSql = "IF type_id('[dbo].[TVP_Transfer]') IS NOT NULL DROP TYPE[dbo].[TVP_Transfer]";
                                        dao.Execute(strSql);
                                        //strSql = "CREATE TYPE [dbo].[TVP_Transfer] AS TABLE([rowid][int] NOT NULL,[tablename] [varchar] (20) NOT NULL,[fieldname] [varchar] (20) NOT NULL,[value] [nvarchar] (512) NOT NULL)";
                                        strSql = "CREATE TYPE [dbo].[TVP_Transfer] AS TABLE([rowid][int]";// NOT NULL,[value] [nvarchar] (512) NOT NULL)";
                                        foreach (string ss in strSelectedFields)
                                        {
                                            strSql += ",[" + ss + "][nvarchar](512) NOT NULL DEFAULT ''";
                                        }
                                        strSql += ")";
                                        dao.Execute(strSql);

                                        //確認是否資料表存在
                                        strSql = "IF OBJECT_ID('dbo.EncodingTransform', 'U') IS NULL CREATE TABLE [dbo].[EncodingTransform]([rowid][int] IDENTITY(1, 1) NOT NULL,[RefRowid] [int] NOT NULL,[Position] [int] NOT NULL,[Original] [varchar] (1000) NOT NULL,[TableName] [varchar] (200) NOT NULL,[FieldName] [varchar] (200) NOT NULL,[TransWord] [nvarchar] (200) NULL,[ENCType] [INT] NULL,[CreateDate] [datetime] NULL,[ModifyDate] [datetime] NULL)";
                                        dao.Execute(strSql);
                                        strSql = "IF OBJECT_ID('dbo.EncodingTransform', 'U') IS NULL ALTER TABLE [dbo].[EncodingTransform] ADD  CONSTRAINT [DF_EncodingTransform_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]";
                                        dao.Execute(strSql);

                                        strSql = "SELECT rowid" + (strSelectedMSFields.Count > 0 ? "," + string.Join(",", strSelectedMSFields) : "") + " FROM " + tbm.TableName;// + " WHERE rowid='3153358'";
                                        DataTable dtResult = dao.QueryForDataTable(strSql);
                                        if (dtResult.Rows.Count > 0)
                                        {
                                            for (int i = 0; i < dtResult.Rows.Count; i++)
                                            {
                                                TotalTransRows++;

                                                rowid = dtResult.Rows[i]["rowid"].ToString();
                                                EncodingTransfer ET = new EncodingTransfer();
                                                DataRow tvpNew = tvp.NewRow();
                                                tvpNew["rowid"] = rowid;
                                                for (int iii = 0; iii < strSelectedMSFields.Count; iii++)
                                                {
                                                    string strUTF8Result = ET.filterOutUTF8ExtraChar(dtResult.Rows[i][strSelectedMSFields[iii]].ToString());
                                                    //寫入已轉好的文字進資料庫
                                                    if (ET.UnknowWords.Count > 0)
                                                    {
                                                        //SendtfResultList.Add(string.Format(@"檔名：{0}，轉碼錯誤：{1}", fn, memo));
                                                        //回寫異常轉碼錯誤說明至資料庫
                                                        foreach (Abnormal item in ET.UnknowWords)
                                                        {
                                                            strSql = "INSERT INTO EncodingTransform (RefRowid,Position,Original,TableName,FieldName,TransWord,ENCType) VALUES (";
                                                            strSql += "'" + rowid + "'";
                                                            strSql += ",'" + item.WordCount + "'";
                                                            strSql += ",N'" + item.OriginWord + "'";
                                                            strSql += ",'" + tbm.TableName + "'";
                                                            strSql += ",'" + strSelectedMSFields[iii] + "'";
                                                            strSql += ",'" + item.TransWord + "'";
                                                            strSql += ",0";
                                                            strSql += ")";
                                                            dao.Execute(strSql);
                                                        }
                                                        ET.UnknowWords.Clear();
                                                    }
                                                    else if (ET.UnknowWords.Count == 0)
                                                    {

                                                    }
                                                    //回寫已轉碼文字至資料庫
                                                    if (ET.TransferedWords.Count > 0)
                                                    {
                                                        foreach (Abnormal item in ET.TransferedWords)
                                                        {
                                                            strSql = "INSERT INTO EncodingTransform (RefRowid,Position,Original,TableName,FieldName,TransWord,ENCType) VALUES (";
                                                            strSql += "'" + rowid + "'";
                                                            strSql += ",'" + item.WordCount + "'";
                                                            strSql += ",N'" + item.OriginWord + "'";
                                                            strSql += ",'" + tbm.TableName + "'";
                                                            strSql += ",'" + strSelectedMSFields[iii] + "'";
                                                            strSql += ",N'" + item.TransWord + "'";
                                                            strSql += ",1";
                                                            strSql += ")";
                                                            dao.Execute(strSql);
                                                        }
                                                        ET.TransferedWords.Clear();
                                                    }
                                                    tvpNew[strSelectedFields[iii]] = strUTF8Result;//dtResult.Rows[i][field] == DBNull.Value ? "" : dtResult.Rows[i][field];                                                                                                
                                                }
                                                tvp.Rows.Add(tvpNew);
                                                if ((tvp.Rows.Count > 999))
                                                {
                                                    cmd.CommandText = "UPDATE " + tbm.TableName + " SET ";
                                                    for (int ii = 0; ii < strSelectedFields.Count; ii++)
                                                    {
                                                        if (ii > 0) { cmd.CommandText += ","; }
                                                        cmd.CommandText += (strSelectedFields[ii] + "=ut." + strSelectedFields[ii]);
                                                    }
                                                    cmd.CommandText += " FROM " + tbm.TableName + " bt Join @UpdateTable ut On bt.rowid=ut.rowid";

                                                    int ECount = 0;
                                                    try
                                                    {
                                                        ECount = cmd.ExecuteNonQuery();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        MessageBox.Show(ex.Message + "\r\n" + dbm.DBName + "：" + tbm.TableName);
                                                    }
                                                    Console.WriteLine("TVP匯入資料庫{3}：資料表 {1}  {0:N0}ms，共{2}筆資料被更新", sw.ElapsedMilliseconds, tbm.TableName, ECount, dbm.DBName);
                                                    txtMsg.AppendText(Environment.NewLine + string.Format("TVP匯入資料庫{3}：資料表 {1}  {0:N0}ms，共{2}筆資料被更新", sw.ElapsedMilliseconds, tbm.TableName, ECount, dbm.DBName));
                                                    txtMsg.SelectionStart = txtMsg.TextLength;
                                                    txtMsg.ScrollToCaret();
                                                    txtMsg.Refresh();
                                                    tvp.Rows.Clear();
                                                }
                                            }
                                            if (tvp.Rows.Count > 0)
                                            {
                                                cmd.CommandText = "UPDATE " + tbm.TableName + " SET ";
                                                for (int ii = 0; ii < strSelectedFields.Count; ii++)
                                                {
                                                    if (ii > 0) { cmd.CommandText += ","; }
                                                    cmd.CommandText += (strSelectedFields[ii] + "=ut." + strSelectedFields[ii]);
                                                }
                                                cmd.CommandText += " FROM " + tbm.TableName + " bt Join @UpdateTable ut On bt.rowid=ut.rowid";

                                                int ECount = 0;
                                                try
                                                {
                                                    ECount = cmd.ExecuteNonQuery();
                                                }
                                                catch (Exception ex)
                                                {
                                                    MessageBox.Show(ex.Message + "\r\n" + dbm.DBName + "：" + tbm.TableName);
                                                }
                                                Console.WriteLine("TVP匯入資料庫{3}：資料表 {1}  {0:N0}ms，共{2}筆資料被更新", sw.ElapsedMilliseconds, tbm.TableName, ECount, dbm.DBName);
                                                txtMsg.AppendText(Environment.NewLine + string.Format("TVP匯入資料庫{3}：資料表 {1}  {0:N0}ms，共{2}筆資料被更新", sw.ElapsedMilliseconds, tbm.TableName, ECount, dbm.DBName));
                                                txtMsg.SelectionStart = txtMsg.TextLength;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                tvp.Rows.Clear();
                                            }
                                        }
                                    }
                                    cn.Close();
                                }
                            }
                        }
                        Console.WriteLine(DateTime.Now + "轉碼結束");
                        Console.WriteLine("共備份" + TotalBackupRows + "筆資料");
                        Console.WriteLine("共轉碼" + TotalTransRows + "筆資料");
                        txtMsg.AppendText(Environment.NewLine + string.Format(DateTime.Now + "轉碼結束"));
                        txtMsg.AppendText(Environment.NewLine + string.Format("共備份" + TotalBackupRows + "筆資料"));
                        txtMsg.AppendText(Environment.NewLine + string.Format("共轉碼" + TotalTransRows + "筆資料"));
                        txtMsg.SelectionStart = txtMsg.TextLength;
                        txtMsg.ScrollToCaret();
                        txtMsg.Refresh();
                        sw.Stop();
                    }
                    else if (DialogResult.No == result || DialogResult.Cancel == result)
                    {
                        MessageBox.Show("取消轉碼作業");
                        return;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public frmMain()
        {
            InitializeComponent();
        }
        public string DetailDBName
        {
            get { return _DetailDBName; }
            set { _DetailDBName = value; }
        }
        public string DetailTableName
        {
            get { return _DetailTableName; }
            set { _DetailTableName = value; }
        }

        private void RefreshData(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgvData.Rows)
                {
                    foreach (DataBaseModule dbm in ComfirmItem)
                    {
                        if (dbm.DBName == row.Cells["DBName"].Value.ToString())
                        {
                            foreach (TableModule tbm in dbm.Tables)
                            {
                                if (tbm.TableName == row.Cells["SourceTableName"].Value.ToString())
                                {
                                    row.Cells["Selected"].Value = tbm.Selected;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void dgvData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int ColumnIdx = 3;
            if (e.ColumnIndex == ColumnIdx)
            {
                //MessageBox.Show(dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString());
                frmDetail detail = new frmDetail(dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString(), ComfirmItem, this);
                detail.FormClosed += new FormClosedEventHandler(RefreshData);
                detail.Show();
                if (detail.IsDisposed)
                {
                    ComfirmItem = detail.GetData;
                    //RefreshData(dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString());
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("確定開始轉碼嗎？", "確認", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    Console.WriteLine(DateTime.Now + "開始");
                    txtMsg.AppendText(Environment.NewLine + string.Format(DateTime.Now + "開始"));
                    txtMsg.SelectionStart = txtMsg.TextLength;
                    txtMsg.ScrollToCaret();
                    txtMsg.Refresh();
                    StartTransfer(0);
                    Console.WriteLine(DateTime.Now + "結束");
                    txtMsg.AppendText(Environment.NewLine + string.Format(DateTime.Now + "結束"));
                    txtMsg.SelectionStart = txtMsg.TextLength;
                    txtMsg.ScrollToCaret();
                    txtMsg.Refresh();

                    MessageBox.Show("轉碼完成");

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                PropertyInfo info = this.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                info.SetValue(this, true, null);

                XmlDocument setting = new XmlDocument();
                setting.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\setting.xml");
                GlobalParameters.DBConStr_IP = setting.SelectSingleNode("/settings/serverip") == null ? "" : setting.SelectSingleNode("/settings/serverip").InnerText;
                GlobalParameters.DBConStr_Port = setting.SelectSingleNode("/settings/serverport") == null ? "" : setting.SelectSingleNode("/settings/serverport").InnerText;
                GlobalParameters.DBConStr_Account = setting.SelectSingleNode("/settings/userid") == null ? "" : setting.SelectSingleNode("/settings/userid").InnerText;
                GlobalParameters.DBConStr_PW = setting.SelectSingleNode("/settings/password") == null ? "" : setting.SelectSingleNode("/settings/password").InnerText;
                if (GlobalParameters.DBConStr_IP.Length == 0 || GlobalParameters.DBConStr_Port.Length == 0 || GlobalParameters.DBConStr_Account.Length == 0 || GlobalParameters.DBConStr_PW.Length == 0)
                {
                    MessageBox.Show("請先設定連線資料");
                    Application.Exit();
                }

                Util tool = new Util();
                GlobalParameters.SummaryTrans = tool.RestoreCNSCode();
                #region 讀取應轉碼欄位
                DataTable dtExcel = new DataTable();
                using (var workbook = new XLWorkbook(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\tablefield.xlsx"))
                {
                    dtExcel.Columns.Add("DBName", typeof(string));
                    dtExcel.Columns.Add("SourceTableName", typeof(string));
                    dtExcel.Columns.Add("SourceFieldName", typeof(string));
                    dtExcel.Columns.Add("DestinationFieldName", typeof(string));
                    //ExcelHandler xls = new ExcelHandler(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\tablefield.xls");
                    //Excel.Worksheet sheet = xls.OpenSheet(6);                    
                    //var sheet = workbook.Worksheet("nvarchar全欄位");
                    var sheet = workbook.Worksheet(6);
                    //var sheetData = sheet.GetFirstChild<SheetData>();

                    int iStartRow = 1;
                    int iStartCol = 1;

                    if (sheet != null)
                    {
                        bool isFinish = false;
                        while (!isFinish)
                        {
                            string strDBName = (sheet.Cell(iStartRow, iStartCol)).Value.ToString() ?? "";
                            string strDBTable = (sheet.Cell(iStartRow, iStartCol + 1)).Value.ToString() ?? "";
                            string strOriFieldName = (sheet.Cell(iStartRow, iStartCol + 2)).Value.ToString() ?? "";
                            string strNewFieldName = "MS950_" + (sheet.Cell(iStartRow, iStartCol + 2)).Value.ToString() ?? "";
                            strDBName = strDBName.Trim();
                            strDBTable = strDBTable.Trim();
                            strOriFieldName = strOriFieldName.Trim();
                            strNewFieldName = strNewFieldName.Trim();
                            if (strDBName.Length == 0 || strDBTable.Length == 0 || strOriFieldName.Length == 0 || strNewFieldName.Length == 0)
                            {
                                isFinish = true;
                            }
                            if (!isFinish)
                            {
                                DataRow row = dtExcel.NewRow();
                                row["DBName"] = strDBName;
                                row["SourceTableName"] = strDBTable;
                                row["SourceFieldName"] = strOriFieldName;
                                row["DestinationFieldName"] = strNewFieldName;
                                dtExcel.Rows.Add(row);
                            }
                            iStartRow++;
                        }
                    }
                }
                #endregion

                DataBaseModule dbitem = null;
                foreach (DataRow row in dtExcel.Rows)
                {
                    List<DataBaseModule> CheckExist = ComfirmItem.Where(a => a.DBName == row["DBName"].ToString()).ToList();
                    if (CheckExist.Count == 0)
                    {
                        dbitem = new DataBaseModule(row["DBName"].ToString());
                    }
                    if (dbitem != null)
                    {
                        dbitem.FieldExists(row["DBName"].ToString(), row["SourceTableName"].ToString(), row["SourceFieldName"].ToString());
                    }
                    if (CheckExist.Count == 0)
                    {
                        ComfirmItem.Add(dbitem);
                    }
                }
                GeneralDao dao = null;
                DataTable dtTables;
                DataTable dtSource = new DataTable();
                dtSource.Columns.Add("Selected", typeof(string));
                dtSource.Columns.Add("DBName", typeof(string));
                dtSource.Columns.Add("SourceTableName", typeof(string));
                dtSource.Columns.Add("DestinationTableName", typeof(string));
                dtSource.Columns.Add("Control", typeof(Button));
                dgvData.AutoGenerateColumns = false;
                foreach (DataBaseModule dbm in ComfirmItem)
                {
                    foreach (TableModule tbm in dbm.Tables)
                    {
                        DataRow dtrow = dtSource.NewRow();
                        dtrow["Selected"] = tbm.Selected;
                        dtrow["DBName"] = dbm.DBName;
                        dtrow["SourceTableName"] = tbm.TableName;
                        dtSource.Rows.Add(dtrow);
                    }
                }
                //foreach (KeyValuePair<string, Dictionary<string, List<string>>> dbitem in filteritem)
                //{
                //    dao = new GeneralDao("139.223.24.227", dbitem.Key, "sa", "1qazxcvbnm,./");

                //    string strSql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG = '" + dbitem.Key + "'";
                //    dtTables = dao.QueryForDataTable(strSql);
                //    if (dtTables.Rows.Count > 0)
                //    {
                //        foreach (DataRow row in dtTables.Rows)
                //        {
                //            DataRow dtrow = dtSource.NewRow();
                //            dtrow["DBName"] = dbitem.Key;
                //            dtrow["Selected"] = dbitem.Value.Keys.Contains(row["TABLE_NAME"].ToString()) ? true : false;
                //            dtrow["SourceTableName"] = row["TABLE_NAME"].ToString();
                //            //dtrow["DestinationTableName"] = "MS950_" + row["TABLE_NAME"].ToString();

                //            //DataGridViewButtonColumn btnControl = new DataGridViewButtonColumn();
                //            //btnControl.Text = "...";
                //            ////btnControl.Click += delegate (object send, EventArgs ea) { Command_Click(sender, e, row["TABLE_NAME"].ToString()); };
                //            //btnControl.UseColumnTextForButtonValue = true;
                //            //dtrow["Control"] = btnControl;                            
                //            dtSource.Rows.Add(dtrow);
                //            ////foreach (string fielditem in tableitem.Value)
                //            ////{

                //            ////}
                //            ////tableData.Add(new Tables() { selected = dbitem.Value.Keys.Contains(row["TABLE_NAME"].ToString()), SourceTableName = row["TABLE_NAME"].ToString(), DestiTableName = row["TABLE_NAME"].ToString(), DBName = dbitem.Key });
                //            //Dictionary<string, Tables> item = new Dictionary<string, Tables>();
                //            //item.Add(row["TABLE_NAME"].ToString(), new Tables(row["TABLE_NAME"].ToString(), dbitem.Value, dao));
                //            //ConverTables.Add(item);
                //        }
                //    }
                //}
                //dgvData.DataSource = dtSource;
                //insert datagridview data
                DataGridViewButtonColumn cmdbtn = new DataGridViewButtonColumn();

                dgvData.Columns.Add(cmdbtn);

                cmdbtn.HeaderText = "命令";
                cmdbtn.Text = "…";
                cmdbtn.Name = "Button";
                cmdbtn.UseColumnTextForButtonValue = true;

                int ColumnIdx = 3;


                foreach (DataRow row in dtSource.Rows)
                {
                    dgvData.Rows.Add(row["Selected"], row["DBName"], row["SourceTableName"], row["DestinationTableName"]);
                    cmdbtn.DataGridView.Rows[dgvData.Rows.Count - 1].Cells[ColumnIdx].Value = "…";
                    cmdbtn.DataGridView.Rows[dgvData.Rows.Count - 1].Cells[ColumnIdx].Tag = row["DBName"] + "|" + row["SourceTableName"];
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("確定開始轉碼嗎？", "確認", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    Console.WriteLine(DateTime.Now + "開始");
                    txtMsg.AppendText(Environment.NewLine + string.Format(DateTime.Now + "開始"));
                    txtMsg.SelectionStart = txtMsg.TextLength;
                    txtMsg.ScrollToCaret();
                    txtMsg.Refresh();
                    StartTransfer(1);
                    Console.WriteLine(DateTime.Now + "結束");
                    txtMsg.AppendText(Environment.NewLine + string.Format(DateTime.Now + "結束"));
                    txtMsg.SelectionStart = txtMsg.TextLength;
                    txtMsg.ScrollToCaret();
                    txtMsg.Refresh();

                    MessageBox.Show("轉碼完成");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("確定開始備份嗎？", "確認", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    Console.WriteLine(DateTime.Now + "開始");
                    txtMsg.AppendText(Environment.NewLine + string.Format(DateTime.Now + "開始"));
                    txtMsg.SelectionStart = txtMsg.TextLength;
                    txtMsg.ScrollToCaret();
                    txtMsg.Refresh();
                    StartTransfer(2);
                    Console.WriteLine(DateTime.Now + "結束");
                    txtMsg.AppendText(Environment.NewLine + string.Format(DateTime.Now + "結束"));
                    txtMsg.SelectionStart = txtMsg.TextLength;
                    txtMsg.ScrollToCaret();
                    txtMsg.Refresh();

                    MessageBox.Show("備份完成");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
