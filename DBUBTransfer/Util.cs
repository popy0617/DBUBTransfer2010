using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using System.Globalization;

namespace DBUBTransfer
{
    public class Util
    {
        
        /// <summary>
        /// 字串轉布林值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StrToBool(string value)
        {
            try
            {
                switch (value.ToLower())
                {
                    case "true":
                        return true;
                    case "false":
                        return false;
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static string Unicode2UTF8(string strUnicode)
        {
            //unicode: 100111101100000                  4F60
            //断开后 ：100 , 111101,100000
            //补位：    *1110* 0100 , *10* 111101,*10* 100000 * *内为补填的
            //utf - 8:    11100100,10111101,10100000       E4BDA0
            string strReturn = string.Empty;
            try
            {
                string binarystring = String.Join(String.Empty,
  strUnicode.Select(
    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
  )
);
                //if (binarystring.Length != 15)
                //{
                //    return strUnicode;
                //}
                string FirstString = string.Empty;
                string SecondString = string.Empty;
                string ThirdString = string.Empty;
                FirstString = binarystring.Substring(0, 4);
                SecondString = binarystring.Substring(4, 6);
                ThirdString = binarystring.Substring(10, 6);

                strReturn = BinaryStringToHexString("1110" + FirstString + "10" + SecondString + "10" + ThirdString);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return strReturn;
        }
        public static string BinaryStringToHexString(string binary)
        {
            StringBuilder result = new StringBuilder(binary.Length / 8 + 1);

            // TODO: check all 1's or 0's... Will throw otherwise

            int mod4Len = binary.Length % 8;
            if (mod4Len != 0)
            {
                // pad to length multiple of 8
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
            }

            for (int i = 0; i < binary.Length; i += 8)
            {
                string eightBits = binary.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }

            return result.ToString();
        }

        public XmlDocument RestoreCNSCode()
        {
            try
            {
                //XmlHandler xmlRoot = new XmlHandler(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())) + @"\CodeFiles\CodeSummary.xml");
                XmlHandler xmlRoot = new XmlHandler(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\CodeSummary1.xml");
                //XmlNodeList Codes = xmlRoot.GetRootXmlNodes();
                return xmlRoot.LoadRoot();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static char HexToUnicode(string hex)
        {
            byte[] bytes = HexToBytes(hex);
            char c;
            if (bytes.Length == 1)
            {
                c = (char)bytes[0];
            }
            else if (bytes.Length == 2)
            {
                c = (char)((bytes[0] << 8) + bytes[1]);
            }
            else
            {
                throw new Exception(hex);
            }

            return c;
        }
        public static byte[] HexToBytes(string hex)
        {
            hex = hex.Trim();

            byte[] bytes = new byte[hex.Length / 2];

            for (int index = 0; index < bytes.Length; index++)
            {
                bytes[index] = byte.Parse(hex.Substring(index * 2, 2), NumberStyles.HexNumber);
            }

            return bytes;
        }

        public void LoadCNCode()
        {
            try
            {
                Big5Info big5Info = new Big5Info();
                UTF8Info utf8Info = new UTF8Info();

                DataTable dtBig5 = big5Info.LoadFile();
                DataTable dtUtf8 = utf8Info.LoadFile();

                DataView dvBig5 = new DataView(dtBig5);
                DataView dvUtf8 = new DataView(dtUtf8);


                //BIG5-UNICODE-CNS對照表
                DataTable dtBIG5UNICODE = new DataTable("BIG5UNICODE");
                dtBIG5UNICODE.Columns.Add("CNSPage", typeof(String));
                dtBIG5UNICODE.Columns.Add("CNSCode", typeof(String));
                dtBIG5UNICODE.Columns.Add("UCSCode", typeof(String));
                dtBIG5UNICODE.Columns.Add("BIG5Code", typeof(String));
                dtBIG5UNICODE.Columns.Add("UnicodeCode", typeof(String));
                dtBIG5UNICODE.Columns.Add("UTF8Code", typeof(String));

                DataRow addRow;
                foreach (DataRow row in dtBig5.Rows)
                {
                    addRow = dtBIG5UNICODE.NewRow();
                    addRow["CNSPage"] = row["CNSPage"];
                    addRow["CNSCode"] = row["CNSCode"];
                    addRow["BIG5Code"] = row["BIG5Code"];
                    dvUtf8.RowFilter = "CNSPage='" + row["CNSPage"] + "' and CNSCode='" + row["CNSCode"] + "'";
                    if (dvUtf8.Count == 1)
                    {
                        addRow["UnicodeCode"] = dvUtf8[0]["UnicodeCode"];
                        addRow["UTF8Code"] = Unicode2UTF8(dvUtf8[0]["UnicodeCode"].ToString());
                    }
                    dtBIG5UNICODE.Rows.Add(addRow);
                }
                //GlobalParameters.SummaryTrans = dtBIG5UNICODE;


                XmlDocument doc = new XmlDocument();
                //建立根節點
                XmlElement company = doc.CreateElement("root");

                //建立子節點
                int i = 0;
                foreach (DataRow row in dtBIG5UNICODE.Rows)
                {
                    XmlElement info = doc.CreateElement("Encoding");
                    info.SetAttribute("index", i.ToString());
                    XmlElement detail = doc.CreateElement("CNSPage");
                    detail.InnerText = row["CNSPage"].ToString();
                    info.AppendChild(detail);
                    detail = doc.CreateElement("CNSCode");
                    detail.InnerText = row["CNSCode"].ToString();
                    info.AppendChild(detail);
                    detail = doc.CreateElement("BIG5Code");
                    detail.InnerText = row["BIG5Code"].ToString();
                    info.AppendChild(detail);
                    detail = doc.CreateElement("UnicodeCode");
                    detail.InnerText = row["UnicodeCode"].ToString();
                    info.AppendChild(detail);
                    detail = doc.CreateElement("UTF8Code");
                    detail.InnerText = row["UTF8Code"].ToString();
                    info.AppendChild(detail);
                    company.AppendChild(info);
                    i++;
                }
                doc.AppendChild(company);
                doc.Save("CodeSummary.xml");

            }
            catch (Exception ex)
            {
                Console.WriteLine("111");
            }
        }
    }

    public abstract class EncodingForTrans
    {
        public EncodingForTrans()
        {
            Initializer();
        }
        private void Initializer()
        {

        }        
        public abstract DataTable LoadFile();
    }
    public class Big5Info : EncodingForTrans
    {
        public List<FileInfo> CodeFiles;

        public Big5Info()
        {
            try
            {
                
                DirectoryInfo Folder = new DirectoryInfo(Path.GetDirectoryName(Directory.GetCurrentDirectory()) + @"\CodeFiles\");
                FileInfo[] Files = Folder.GetFiles("*.xlsx", SearchOption.TopDirectoryOnly);
                IEnumerable<string> FilesFullNames = Files.Select(file => file.FullName);
                foreach (string item in FilesFullNames)
                {
                    FileInfo fi = new FileInfo(item);
                    CodeFiles.Add(fi);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public override DataTable LoadFile()
        {
            try
            {
                ////BIG5對照表
                //DataTable dtCNS2BIG5 = new DataTable("BIG5Reflection");
                //dtCNS2BIG5.Columns.Add("UCSCode", typeof(String));
                //dtCNS2BIG5.Columns.Add("BIG5Code", typeof(String));
                //dtCNS2BIG5.Columns.Add("UnicodeCode", typeof(String));
                //dtCNS2BIG5.Columns.Add("CNSCode", typeof(String));

                //// use text reader to open tab delimited text file

                //    #region 讀取應轉碼欄位
                //    DataTable dtExcel = new DataTable();
                //    dtExcel.Columns.Add("DBName", typeof(string));
                //    dtExcel.Columns.Add("SourceTableName", typeof(string));
                //    dtExcel.Columns.Add("SourceFieldName", typeof(string));
                //    dtExcel.Columns.Add("DestinationFieldName", typeof(string));
                //    ExcelHandler xls = new ExcelHandler(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\tablefield.xls");
                //    Microsoft.Office.Interop.Excel.Worksheet sheet = xls.OpenSheet(2);
                //    int iStartRow = 1;
                //    int iStartCol = 1;

                //    //Dictionary<string, Dictionary<string, List<string>>> ConfirmItem = new Dictionary<string, Dictionary<string, List<string>>>();
                //    if (sheet != null)
                //    {

                //        bool isFinish = false;
                //        while (!isFinish)
                //        {
                //            string strDBName = (sheet.Cells[iStartRow, iStartCol] as Excel.Range).Value ?? "";
                //            string strDBTable = (sheet.Cells[iStartRow, iStartCol + 1] as Excel.Range).Value ?? "";
                //            string strOriFieldName = (sheet.Cells[iStartRow, iStartCol + 2] as Excel.Range).Value ?? "";
                //            string strNewFieldName = "MS950_" + (sheet.Cells[iStartRow, iStartCol + 2] as Excel.Range).Value ?? "";
                //            strDBName = strDBName.Trim();
                //            strDBTable = strDBTable.Trim();
                //            strOriFieldName = strOriFieldName.Trim();
                //            strNewFieldName = strNewFieldName.Trim();
                //            if (strDBName.Length == 0 || strDBTable.Length == 0 || strOriFieldName.Length == 0 || strNewFieldName.Length == 0)
                //            {
                //                isFinish = true;
                //            }
                //            if (!isFinish)
                //            {
                //                DataRow row = dtExcel.NewRow();
                //                row["DBName"] = strDBName;
                //                row["SourceTableName"] = strDBTable;
                //                row["SourceFieldName"] = strOriFieldName;
                //                row["DestinationFieldName"] = strNewFieldName;
                //                dtExcel.Rows.Add(row);
                //                ////Dictionary<string, Tables> fiterItem = new Dictionary<string, Tables>();
                //                //if (!filteritem.ContainsKey(strDBName))
                //                //{
                //                //    filteritem.Add(strDBName, new Dictionary<string, List<string>>() { { strDBTable, new List<string>() { strOriFieldName } } });
                //                //}
                //                //else
                //                //{
                //                //    foreach (Dictionary<string, List<string>> tables in filteritem.Values)
                //                //    {
                //                //        if (!tables.ContainsKey(strDBTable))
                //                //        {
                //                //            tables.Add(strDBTable, new List<string>() { strOriFieldName });
                //                //        }
                //                //        else
                //                //        {
                //                //            foreach (KeyValuePair<string, List<string>> fields in tables)
                //                //            {
                //                //                if (fields.Key == strDBTable && !fields.Value.Contains(strOriFieldName))
                //                //                {
                //                //                    fields.Value.Add(strOriFieldName);
                //                //                }
                //                //            }
                //                //        }
                //                //    }
                //                //}
                //                ////foreach (Dictionary<string, List<string>> tables in filteritem.Values)
                //                ////{
                //                ////    if (!tables.Keys.Contains(strDBName))
                //                ////    {

                //                ////    }
                //                ////}
                //            }
                //            iStartRow++;
                //        }
                //        //FilterData = new Tables();
                //        //FilterData.SourceTableName = strDBTable;
                //        //FilterData.DestiTableName = strDBTable;
                //        //FilterData.Fields = 
                //    }
                //    #endregion
                //    //using (TextReader tr = File.OpenText(fi.FullName))
                //    //{
                //    //    string line;
                //    //    DataRow row;
                //    //    while ((line = tr.ReadLine()) != null)
                //    //    {
                //    //        // split the line of text into a collection
                //    //        string[] items = line.Split('\t');
                //    //        row = dtCNS2BIG5.NewRow();
                //    //        row["CNSPage"] = items[0];
                //    //        row["CNSCode"] = items[1];
                //    //        row["BIG5Code"] = items[2];
                //    //        dtCNS2BIG5.Rows.Add(row);
                //    //    }
                //    //}

                //return dtCNS2BIG5;
                return new DataTable();
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }
    }

    public class UTF8Info : EncodingForTrans
    {
        public List<FileInfo> CodeFiles;

        public UTF8Info()
        {
            try
            {
                CodeFiles = new List<FileInfo>();
                DirectoryInfo Folder = new DirectoryInfo(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())) + @"\CodeFiles\Unicode");
                FileInfo[] Files = Folder.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
                IEnumerable<string> FilesFullNames = Files.Select(file => file.FullName);
                foreach (string item in FilesFullNames)
                {
                    FileInfo fi = new FileInfo(item);
                    CodeFiles.Add(fi);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public override DataTable LoadFile()
        {
            try
            {
                ///UNICODE對照CNS
                DataTable dtCNS2UNICODE = new DataTable("CNS2UNICODE");
                dtCNS2UNICODE.Columns.Add("CNSPage", typeof(String));
                dtCNS2UNICODE.Columns.Add("CNSCode", typeof(String));
                dtCNS2UNICODE.Columns.Add("UnicodeCode", typeof(String));
                // use text reader to open tab delimited text file
                foreach (FileInfo fi in CodeFiles)
                {
                    using (TextReader tr = File.OpenText(fi.FullName))
                    {
                        string line;
                        DataRow row;
                        while ((line = tr.ReadLine()) != null)
                        {
                            // split the line of text into a collection
                            string[] items = line.Split('\t');
                            row = dtCNS2UNICODE.NewRow();
                            row["CNSPage"] = items[0];
                            row["CNSCode"] = items[1];
                            row["UnicodeCode"] = items[2];
                            dtCNS2UNICODE.Rows.Add(row);
                        }
                    }
                }
                return dtCNS2UNICODE;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }
    }
    /// <summary>
    /// 轉碼文字
    /// </summary>
    public class Abnormal
    {
        /// <summary>
        /// 文字所在位置
        /// </summary>
        public string WordCount { get; set; }        
        /// <summary>
        /// 原始文字
        /// </summary>
        public string OriginWord { get; set; }
        /// <summary>
        /// 轉碼文字
        /// </summary>
        public string TransWord { get; set; }

        public Abnormal()
        {
            WordCount = string.Empty;            
            OriginWord = string.Empty;
            TransWord = string.Empty;
        }
        public override string ToString()
        {
            string strReturn = string.Empty;
            try
            {
                strReturn = (string.Format(@"第{0}個字，來源(BIG5)內容為{1}",  WordCount, OriginWord));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strReturn;
        }
    }
}
