using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DBUBTransfer
{
    public class ExcelHandler
    {
        Excel.Application _Excel = null;
        Excel.Workbook book = null;
        private string _filepath = "";
        /// <summary>
        /// 開啟WorkBook
        /// </summary>
        /// <returns></returns>
        public Excel.Workbook OpenBook()
        {
            try
            {
                //開啟舊檔案
                return _Excel.Workbooks.Open(_filepath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Dispose();
                return null;
            }
        }
        /// <summary>
        /// 開啟Sheet
        /// </summary>
        /// <param name="sheetNM"></param>
        /// <returns></returns>
        public Excel.Worksheet OpenSheet(int sheetNM)
        {
            try
            {
                //開啟舊檔案
                book = _Excel.Workbooks.Open(_filepath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                return (Excel.Worksheet)book.Sheets[sheetNM];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Dispose();
                return null;
            }
        }
        /// <summary>
        /// 初始化excel物件
        /// </summary>
        private void Initial()
        {
            try
            {
                //檢查pc有無excel在執行
                bool flag = false;
                foreach (var item in Process.GetProcesses())
                {
                    if (item.ProcessName == "EXCEL")
                    {
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    _Excel = new Excel.Application();
                }
                else
                {
                    object obj = null;
                    try
                    {
                        obj = Marshal.GetActiveObject("Excel.Application");//引用已在執行的Excel

                    }
                    catch (Exception)
                    {
                        obj = new Excel.Application();
                    }
                    _Excel = obj as Excel.Application;
                }

                this._Excel.Visible = false;//設false效能會比較好
                _Excel.DisplayAlerts = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;
            }
        }

        private void Dispose()
        {
            try
            {
                book.Close(Type.Missing, Type.Missing, Type.Missing);
                book = null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ExcelHandler()
        {
            Initial();
        }
        public ExcelHandler(string filepath)
        {
            Initial();
            _filepath = filepath;
        }
    }
}
