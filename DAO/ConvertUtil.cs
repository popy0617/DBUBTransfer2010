using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace MSDAO
{
    public class ConvertUtil
    {
        #region Math

        public static double Round(double val, int digits = 0)
        {
            return Math.Round(val, digits, MidpointRounding.AwayFromZero);
        }

        public static decimal Round(decimal val, int digits = 0)
        {
            return Math.Round(val, digits, MidpointRounding.AwayFromZero);
        }

        #endregion Math

        #region 基本型態轉換

        public static string ToString(object value)
        {
            string val = Convert.ToString(value);
            return string.IsNullOrEmpty(val) ? string.Empty : val;
        }

        public static string ToString(Control ctrl)
        {
            return ConvertUtil.ToString(CommonUtil.GetObjectValue(ctrl));
        }

        public static int ToInt(object value)
        {
            return string.IsNullOrEmpty(Convert.ToString(value)) ? 0 : Convert.ToInt32(Convert.ToDouble(value));
        }

        public static int? ToNInt(object value)
        {
            int i;
            return int.TryParse(ConvertUtil.ToString(value), out i) ? (int?)i : null;
        }

        public static int ToInt(Control ctrl)
        {
            try
            {
                return ConvertUtil.ToInt(CommonUtil.GetObjectValue(ctrl));
            }
            catch (Exception)
            {
                throw new Exception(Msg.IncorrectFormatMsgContent(CommonUtil.GetObjectLabel(ctrl)));
            }
        }

        public static long ToLong(object value)
        {
            return string.IsNullOrEmpty(Convert.ToString(value)) ? 0 : Convert.ToInt64(Convert.ToDouble(value));
        }

        public static long? ToNLong(object value)
        {
            long i;
            return long.TryParse(ConvertUtil.ToString(value), out i) ? (long?)i : null;
        }

        public static long ToLong(Control ctrl)
        {
            try
            {
                return ConvertUtil.ToLong(CommonUtil.GetObjectValue(ctrl));
            }
            catch (Exception)
            {
                throw new Exception(Msg.IncorrectFormatMsgContent(CommonUtil.GetObjectLabel(ctrl)));
            }
        }

        public static double ToDouble(object value)
        {
            return string.IsNullOrEmpty(Convert.ToString(value)) ? 0 : Convert.ToDouble(value);
        }

        public static double? ToNDouble(object value)
        {
            double i;
            return double.TryParse(ConvertUtil.ToString(value), out i) ? (double?)i : null;
        }

        public static double ToDouble(Control ctrl)
        {
            try
            {
                return ConvertUtil.ToDouble(CommonUtil.GetObjectValue(ctrl));
            }
            catch (Exception)
            {
                throw new Exception(Msg.IncorrectFormatMsgContent(CommonUtil.GetObjectLabel(ctrl)));
            }
        }

        public static decimal ToDecimal(object value)
        {
            return string.IsNullOrEmpty(Convert.ToString(value)) ? 0 : Convert.ToDecimal(value);
        }

        public static decimal? ToNDecimal(object value)
        {
            decimal i;
            return decimal.TryParse(ConvertUtil.ToString(value), out i) ? (decimal?)i : null;
        }

        public static decimal ToDecimal(Control ctrl)
        {
            try
            {
                return ConvertUtil.ToDecimal(CommonUtil.GetObjectValue(ctrl));
            }
            catch (Exception)
            {
                throw new Exception(Msg.IncorrectFormatMsgContent(CommonUtil.GetObjectLabel(ctrl)));
            }
        }

        public static DateTime? ToDateTime(TextBox objDate, string hh24 = "00", string mi = "00", string si = "00")
        {
            if (!objDate.Text.Equals(string.Empty))
            {//格式正確才IsEmpty = false
                if (ConvertUtil.ToInt(hh24) < 0 || ConvertUtil.ToInt(hh24) > 23)
                    throw new Exception("小時格式不正確");
                if (ConvertUtil.ToInt(mi) < 0 || ConvertUtil.ToInt(mi) > 59)
                    throw new Exception("分鐘格式不正確");
                if (ConvertUtil.ToInt(si) < 0 || ConvertUtil.ToInt(si) > 59)
                    throw new Exception("秒數格式不正確");

                return DateTime.Parse(Convert.ToDateTime(objDate.Text).ToString("yyyy/MM/dd" + " " + hh24 + ":" + mi + ":" + si));
            }
            else
            {
                ////IsEmpty = true 但RawText卻有值 >> 輸入的日期格式不正確
                //if (!string.IsNullOrEmpty(objDate.RawText))
                //    throw new Exception(Msg.IncorrectFormatMsgContent(CommonUtil.GetObjectLabel(objDate)));
                //else
                    return null;
            }
        }
        
        /// <summary>
        /// 西元字串轉日期物件
        /// </summary>
        /// <param name="dateString"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(string dateString)
        {
            try
            {
                if (string.IsNullOrEmpty(dateString))
                    return null;

                //yyyyMMdd特別處理
                string cs = Regex.Replace(dateString, "[^0-9]", "");
                if (dateString.Length == 8)
                    return DateTime.ParseExact(cs, "yyyyMMdd", CultureInfo.InvariantCulture);
                else
                    return DateTime.Parse(dateString);
            }
            catch (FormatException ex)
            {
                throw new Exception(Msg.IncorrectFormatMsgContent("日期") + ":" + dateString);
            }
        }

        #endregion 基本型態轉換

        #region Excel轉換

        #region Excel轉DataSet

        /// <summary>
        /// Excel轉DataSet
        /// </summary>
        /// <param name="filePath">Excel檔案位置</param>
        /// <param name="tabName">指定要進行轉換的Excel頁籤名稱，未指定全部頁籤進行轉換</param>
        /// <returns></returns>
        ///
        public static DataSet ExcelToDataSet(string filePath, string[] tabName = null)
        {
            string connString = string.Empty;

            //判斷Excel版本
            switch (Path.GetExtension(filePath).ToUpper())
            {
                case ".XLS":
                    connString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}; Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"", filePath);
                    break;

                case ".XLSX":
                    connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"", filePath);
                    break;
            }

            if (string.IsNullOrEmpty(connString))
            {
                throw new Exception("Excel格式不正確!!");
            }

            DataSet ds = new DataSet();
            foreach (string sheetName in GetExcelSheetNames(connString))
            {
                if (string.IsNullOrEmpty(sheetName))
                    continue;

                //判斷需要回Table名稱是否存在，如果不填則回傳全部
                if (tabName != null)
                {
                    string str = sheetName.Replace("$", "").Replace("'", "").Trim();
                    if (tabName.ToList().Intersect(new List<string>() { str }).Count() == 0)
                        continue;
                }

                //將資料載入DataSet
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    OleDbDataAdapter adapter = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}]", sheetName), conn);
                    DataTable dt = new DataTable(sheetName.Replace("$", "").Replace("'", "").Trim());
                    adapter.Fill(dt);
                    ds.Tables.Add(dt);
                }
            }

            return ds;
        }

        /// <summary>
        /// 取得Excel工作表名稱
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static string[] GetExcelSheetNames(string connectionString)
        {
            OleDbConnection conn = null;
            try
            {
                conn = new OleDbConnection(connectionString);
                conn.Open();

                //取字尾是$或$'的頁籤
                //開頭為數字時會在字串前後加單引號「'」，因此要多判斷字尾是「$'」
                var dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null)
                                   .AsEnumerable()
                                   .Where(row => (ConvertUtil.ToString(row.Field<string>("TABLE_NAME")).Substring(ConvertUtil.ToString(row.Field<string>("TABLE_NAME")).Length - 1, 1).Equals("$")
                                   || ConvertUtil.ToString(row.Field<string>("TABLE_NAME")).Substring(ConvertUtil.ToString(row.Field<string>("TABLE_NAME")).Length - 2, 2).Equals("$'")));

                DataTable temp = null;
                if (dt.Any())
                    temp = dt.CopyToDataTable();
                else
                    throw new Exception("無法取得Excel頁籤資訊");

                string[] excelSheetNames = new string[temp.Rows.Count];
                int i = 0;
                foreach (DataRow row in temp.Rows)
                {
                    excelSheetNames[i] = ConvertUtil.ToString(row["TABLE_NAME"]);
                    i++;
                }

                return excelSheetNames;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        #endregion Excel轉DataSet

        #region 根據Grid格式匯出Excel檔案

        //public static void GridToExcel(DataGrid grid, DataTable dt, string fileName, string sheetName = "sheet1")
        //{
        //    HSSFWorkbook WorkBook = new HSSFWorkbook();

        //    AppendSheet(WorkBook, dt, sheetName, grid);

        //    //回傳
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        WorkBook.Write(ms);

        //        fileName += ".xls";
        //        FileHelper.FlushToClient(ms.ToArray(), fileName);
        //    }
        //}

        //public static void AppendSheet(HSSFWorkbook workBook, DataTable dt, string sheetName, GridPanel grid)
        //{
        //    int SheetNameIndex = 1;
        //    string CurrentSheetName = sheetName;

        //    //sheet名稱不可重覆
        //    while (workBook.GetSheet(CurrentSheetName) != null)
        //    {
        //        CurrentSheetName = sheetName + SheetNameIndex;
        //        SheetNameIndex++;
        //    }

        //    ISheet Sheet = workBook.CreateSheet(CurrentSheetName);
        //    AppendDataTableByGrid(grid, workBook, Sheet, dt);
        //}

        //private static void AppendDataTableByGrid(GridPanel grid, HSSFWorkbook workBook, ISheet sheet, DataTable gridData)
        //{
        //    IRow curRow = null;
        //    ICell curCell = null;

        //    #region 取得整體性資料

        //    int hearderTotalRowCnt = 0;
        //    int headerTotalColCnt = 0;
        //    List<ColumnBase> dataIndexList = new List<ColumnBase>();
        //    for (int i = 0; i < grid.ColumnModel.Columns.Count; i++)
        //    {
        //        int colCurRow = 0;
        //        int colTotalRowCnt = 0;
        //        int colTotalColCnt = 0;

        //        GetColumnInfo(grid.ColumnModel.Columns[i], ref colTotalRowCnt, ref colCurRow, ref colTotalColCnt, dataIndexList);
        //        hearderTotalRowCnt = Math.Max(hearderTotalRowCnt, colTotalRowCnt);
        //        headerTotalColCnt += colTotalColCnt;
        //    }

        //    #endregion 取得整體性資料

        //    #region CELL STYLE 宣告

        //    ICellStyle HeaderStyle = SetCellStyle(workBook.CreateCellStyle(), workBook, EXCEL_STYLE.Header);
        //    ICellStyle AlignLeftCell = SetCellStyle(workBook.CreateCellStyle(), workBook, EXCEL_STYLE.General);
        //    ICellStyle AlignRightCell = SetCellStyle(workBook.CreateCellStyle(), workBook, EXCEL_STYLE.General);
        //    AlignRightCell.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;

        //    ICellStyle CellDateStyle = SetCellStyle(workBook.CreateCellStyle(), workBook, EXCEL_STYLE.DateData, "yyyy/mm/dd");
        //    ICellStyle CellNumberDec0Style = SetCellStyle(workBook.CreateCellStyle(), workBook, EXCEL_STYLE.NumberData, "#,##0");    //無小數點
        //    ICellStyle CellNumberDec1Style = SetCellStyle(workBook.CreateCellStyle(), workBook, EXCEL_STYLE.NumberData, "#,##0.0");    //小數點一位
        //    ICellStyle CellNumberDec2Style = SetCellStyle(workBook.CreateCellStyle(), workBook, EXCEL_STYLE.NumberData, "#,##0.00");    //小數點兩位
        //    ICellStyle CellNumberDec3Style = SetCellStyle(workBook.CreateCellStyle(), workBook, EXCEL_STYLE.NumberData, "#,##0.000");    //小數點三位
        //    ICellStyle CellNumberDec4Style = SetCellStyle(workBook.CreateCellStyle(), workBook, EXCEL_STYLE.NumberData, "#,##0.0000");    //小數點四位

        //    #endregion CELL STYLE 宣告

        //    #region 表頭

        //    #region 設定每個CELL的STYLE

        //    //功能名稱TITLE、GRID TITLE
        //    for (int r = 0; r < hearderTotalRowCnt + 1; r++)
        //    {
        //        curRow = sheet.CreateRow(r);
        //        for (int c = 0; c < headerTotalColCnt; c++)
        //        {
        //            curCell = curRow.CreateCell(c);
        //            curCell.CellStyle = HeaderStyle;
        //        }
        //    }

        //    #endregion 設定每個CELL的STYLE

        //    //功能名稱TITLE
        //    curRow = sheet.GetRow(0);
        //    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, headerTotalColCnt - 1));
        //    //curRow.GetCell(0).SetCellValue(UserProfile.getExecPrgName);

        //    #region 遞迴寫入每個CELL資料

        //    int curRowIdx = 1;  //0為功能名稱TITLE
        //    int curColIdx = 0;
        //    curRow = sheet.GetRow(curRowIdx);
        //    for (int i = 0; i < grid.ColumnModel.Columns.Count; i++)
        //    {
        //        ColumnBase mappingColumn = grid.ColumnModel.Columns[i];
        //        WriteHeaderByColumn(sheet, mappingColumn, hearderTotalRowCnt, ref curRowIdx, ref curColIdx);
        //    }

        //    #endregion 遞迴寫入每個CELL資料

        //    #endregion 表頭

        //    #region 資料

        //    curRowIdx = hearderTotalRowCnt + 1;
        //    for (int r = 0; r < gridData.Rows.Count; r++)
        //    {
        //        DataRow dataRow = gridData.Rows[r];
        //        curRow = sheet.CreateRow(curRowIdx + r);

        //        for (int c = 0; c < headerTotalColCnt; c++)
        //        {
        //            curCell = curRow.CreateCell(c);
        //            ColumnBase mappingColumn = dataIndexList[c];

        //            string dataIndex = mappingColumn.DataIndex;
        //            string mappingColumnType = ConvertUtil.ToString(mappingColumn.GetType());
        //            object curData = null;
        //            if (!string.IsNullOrEmpty(dataIndex) && !"null".Equals(dataIndex))
        //                curData = dataRow[dataIndex];
        //            string curDataStr = string.Empty;

        //            switch (mappingColumnType)
        //            {
        //                case "Ext.Net.DateColumn":
        //                    curCell.CellStyle = AlignRightCell;
        //                    if (curData != null && !string.IsNullOrEmpty(ConvertUtil.ToString(curData)))
        //                    {
        //                        if (!string.IsNullOrEmpty(((Ext.Net.DateColumn)mappingColumn).Format))
        //                            curDataStr = DateTime.Parse(ConvertUtil.ToString(curData)).ToString(((Ext.Net.DateColumn)mappingColumn).Format);
        //                        else
        //                            curDataStr = DateTime.Parse(ConvertUtil.ToString(curData)).ToString("yyyy/MM/dd");
        //                    }
        //                    curCell.SetCellValue(curDataStr);
        //                    break;

        //                case "Ext.Net.NumberColumn":

        //                    int decCnt = 0;
        //                    if (!string.IsNullOrEmpty(ConvertUtil.ToString(curData)))
        //                    {
        //                        string colFormat = ((Ext.Net.NumberColumn)mappingColumn).Format;
        //                        if (!string.IsNullOrEmpty(colFormat) && colFormat.Contains("."))
        //                            decCnt = colFormat.Split('.')[1].Length;    //小數點位數

        //                        switch (decCnt)
        //                        {
        //                            case 0:
        //                                curCell.CellStyle = CellNumberDec0Style;
        //                                break;

        //                            case 1:
        //                                curCell.CellStyle = CellNumberDec1Style;
        //                                break;

        //                            case 2:
        //                            default:
        //                                curCell.CellStyle = CellNumberDec2Style;
        //                                break;

        //                            case 3:
        //                                curCell.CellStyle = CellNumberDec3Style;
        //                                break;

        //                            case 4:
        //                                curCell.CellStyle = CellNumberDec4Style;
        //                                break;
        //                        }

        //                        curDataStr = ConvertUtil.ToString(curData);
        //                        curCell.SetCellValue(ConvertUtil.ToDouble(curData));
        //                    }
        //                    else
        //                    {
        //                        curCell.CellStyle = CellNumberDec0Style;
        //                        curCell.SetCellValue(string.Empty);
        //                    }

        //                    break;

        //                default:
        //                    curCell.CellStyle = AlignLeftCell;
        //                    curDataStr = ConvertUtil.ToString(curData);
        //                    curCell.SetCellValue(curDataStr);
        //                    break;
        //            }

        //            //借來存放最長字串
        //            if (Encoding.Default.GetBytes(mappingColumn.ToolTip).Length < Encoding.Default.GetBytes(curDataStr).Length)
        //                mappingColumn.ToolTip = curDataStr;
        //        }
        //    }

        //    #endregion 資料

        //    //調整欄寬
        //    for (int i = 0; i < headerTotalColCnt; i++)
        //        sheet.SetColumnWidth(i, (System.Text.Encoding.Default.GetBytes(dataIndexList[i].ToolTip).Length + 3) * 256);
        //}

        //private static void GetColumnInfo(ColumnBase gridColumn, ref int colTotalRowCnt, ref int colCurRow, ref int colTotalColCnt, List<ColumnBase> dataIndexList)
        //{
        //    //近來代表加一行
        //    colCurRow++;
        //    colTotalRowCnt = Math.Max(colTotalRowCnt, colCurRow);

        //    //遞迴繼續往下層Column找
        //    for (int i = 0; i < gridColumn.Columns.Count; i++)
        //        GetColumnInfo(gridColumn.Columns[i], ref colTotalRowCnt, ref colCurRow, ref colTotalColCnt, dataIndexList);

        //    //沒有子Column代表此Columns是最後一層
        //    if (gridColumn.Columns.Count == 0)
        //    {
        //        colTotalColCnt++;
        //        if (dataIndexList != null)
        //            dataIndexList.Add(gridColumn);
        //    }

        //    //離開前回上層則減一行
        //    colCurRow--;
        //}

        //private static void WriteHeaderByColumn(ISheet sheet, ColumnBase mappingColumn, int hearderTotalRowCnt, ref int curRowIdx, ref int curColIdx)
        //{
        //    int colCurRow = 0;
        //    int colTotalRowCnt = 0;
        //    int colTotalColCnt = 0;
        //    int oliColIdx = curColIdx;
        //    IRow curRow = null;
        //    ICell curCell = null;

        //    GetColumnInfo(mappingColumn, ref colTotalRowCnt, ref colCurRow, ref colTotalColCnt, null);

        //    curRow = sheet.GetRow(curRowIdx);
        //    curCell = curRow.GetCell(curColIdx);
        //    if (colTotalColCnt != 1)
        //    {//有多個子欄位
        //        //橫向合併儲存格
        //        sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, curRowIdx, curColIdx, curColIdx + colTotalColCnt - 1));
        //        curColIdx += colTotalColCnt;
        //    }
        //    else
        //    {//無子欄位或只有一個子欄位
        //        if (mappingColumn.Columns.Count == 0)   //無子欄位
        //        {
        //            if (curRowIdx < hearderTotalRowCnt)     //直向合併儲存格
        //                sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, hearderTotalRowCnt, curColIdx, curColIdx));
        //        }

        //        curColIdx++;
        //    }
        //    curCell.SetCellValue(ConvertUtil.ToString(mappingColumn.Text));

        //    //如為第一層則判斷是否需要隱藏Column (必須等合併完儲存格再處理)
        //    if (curRowIdx == 1)
        //    {
        //        if (mappingColumn.Visible == false || mappingColumn.Hidden == true)
        //        {
        //            for (int k = oliColIdx; k < curColIdx; k++)
        //                sheet.SetColumnHidden(k, true); ;
        //        }
        //    }

        //    //借來存放最長字串
        //    if (Encoding.Default.GetBytes(mappingColumn.ToolTip).Length < Encoding.Default.GetBytes(mappingColumn.Text).Length)
        //        mappingColumn.ToolTip = mappingColumn.Text;

        //    //遞迴處理子欄位
        //    if (mappingColumn.Columns.Count > 0)
        //        curRowIdx++;
        //    for (int c = 0; c < mappingColumn.Columns.Count; c++)
        //        WriteHeaderByColumn(sheet, mappingColumn.Columns[c], hearderTotalRowCnt, ref curRowIdx, ref oliColIdx);
        //    if (mappingColumn.Columns.Count > 0)
        //        curRowIdx--;
        //}

        //private static ColumnBase GetGridColumnByDataIndex(ColumnBase column, string CurrentColumnName)
        //{
        //    if (CurrentColumnName.Equals(column.DataIndex))
        //        return column;

        //    ColumnBase dataColumn = null;
        //    ItemsCollection<ColumnBase> innerClumns = column.Columns;
        //    foreach (ColumnBase innerClumn in innerClumns)
        //    {
        //        dataColumn = GetGridColumnByDataIndex(innerClumn, CurrentColumnName);
        //        if (dataColumn != null)
        //            return dataColumn;
        //    }

        //    return dataColumn;
        //}

        //private static ICellStyle SetCellStyle(ICellStyle currentCellStyle, HSSFWorkbook workBook, EXCEL_STYLE Style, string format = "General")
        //{
        //    switch (Style)
        //    {
        //        case EXCEL_STYLE.Header:
        //            currentCellStyle.Alignment = HorizontalAlignment.Center;
        //            currentCellStyle.VerticalAlignment = VerticalAlignment.Top;
        //            currentCellStyle.BorderTop = currentCellStyle.BorderBottom = currentCellStyle.BorderLeft = currentCellStyle.BorderRight = BorderStyle.Thin;
        //            break;

        //        default:
        //            currentCellStyle.DataFormat = workBook.CreateDataFormat().GetFormat(format);
        //            currentCellStyle.BorderTop = currentCellStyle.BorderBottom = currentCellStyle.BorderLeft = currentCellStyle.BorderRight = BorderStyle.Thin;
        //            break;
        //    }

        //    return currentCellStyle;
        //}

        //private enum EXCEL_STYLE
        //{
        //    Header,
        //    General,
        //    NumberData,
        //    DateData
        //}

        //private class HeaderColumn
        //{
        //    public string Text;
        //    public int SpanRow;
        //    public int SpanColumn;
        //    public int ColumnIndex;
        //    public string DataIndex;
        //    public List<HeaderColumn> ChildColumns = new List<HeaderColumn>();

        //    public HeaderColumn(string Text, int SpanRow, int SpanColumn, int ColumnIndex, string DataIndex)
        //    {
        //        this.Text = Text;
        //        this.SpanRow = SpanRow;
        //        this.SpanColumn = SpanColumn;
        //        this.ColumnIndex = ColumnIndex;
        //        this.DataIndex = DataIndex;
        //    }
        //}

        #endregion 根據Grid格式匯出Excel檔案

        #endregion Excel轉換
        
        /// <summary>
        /// 將傳入物件加上千分位符號
        /// </summary>
        /// <param name="s"></param>
        /// <param name="addPrefix"></param>
        /// <returns></returns>
        public static string FormatMoney(object s, bool addPrefix = false)
        {
            if (s == null || string.IsNullOrEmpty(s.ToString()))
                return string.Empty;

            decimal val = ConvertUtil.ToDecimal(s);
            bool isNegative = false;
            if (val < 0)
                isNegative = true;

            string[] tmp = ConvertUtil.ToString(Math.Abs(val)).Split('.');    //取絕對值,因為-0會被轉成0,所以負號另外處理
            string ret = ConvertUtil.ToDouble(tmp[0]).ToString("#,0");   //不知道小數點後有幾位,故以字串處理
            if (tmp.Length > 1 && !string.IsNullOrEmpty(tmp[1]))
                ret += "." + tmp[1];
            if (isNegative)
                ret = "-" + ret;

            if (addPrefix)
            {
                if (ConvertUtil.ToDouble(ret) > 0)
                    ret = "$" + ret;
                else
                    ret = "-$" + ret.Substring(1);
            }

            return ret;
        }
    }
}
