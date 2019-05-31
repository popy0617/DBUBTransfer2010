using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Globalization;

namespace DBUBTransfer
{
    public class EncodingTransfer
    {
        public static byte[] ALTERATE_BIG5_CHAR = { (byte)161, (byte)188 }; //"□". 必須是 2 byte 字元, 才能無縫取代原 Big5 資料的外字(假設也是 2 byte 字元)

        #region Code Range
        #region BIG5 Code Range
        //定義BIG5編碼對應Unicode編碼造字範圍
        int Section_01_S = Convert.ToInt32(0x8140);
        int Section_01_E = Convert.ToInt32(0xA0FE);

        int Section_02_S = Convert.ToInt32(0xA3C0);
        int Section_02_E = Convert.ToInt32(0xA3FE);

        int Section_03_S = Convert.ToInt32(0xC6A1);
        int Section_03_E = Convert.ToInt32(0xC8FE);

        int Section_04_S = Convert.ToInt32(0xF9FE);
        int Section_04_E = Convert.ToInt32(0xFEFE);

        //標點符號區
        int Section_05_S = Convert.ToInt32(0xA140);
        int Section_05_E = Convert.ToInt32(0xA3BF);
        //常用漢字區
        int Section_06_S = Convert.ToInt32(0xA440);
        int Section_06_E = Convert.ToInt32(0xC67E);
        //次常用漢字區
        int Section_07_S = Convert.ToInt32(0xC940);
        int Section_07_E = Convert.ToInt32(0xF9D5);
        //CP950使用內容
        int Section_08_S = Convert.ToInt32(0xF9D6);
        int Section_08_E = Convert.ToInt32(0xF9FE);
        #endregion
        #region UTF8 Code Range
        //index=StartPoint,value=EndPoint
        Dictionary<int, string[]> dicUTF8Range = new Dictionary<int, string[]>{
        //1）標準CJK文字
        //http://www.unicode.org/Public/UNIDATA/Unihan.html
        //Code point range Block name Release
        //U + 3400..U + 4DB5 CJK Unified Ideographs Extension A 3.0
        //U + 4E00..U + 9FA5 CJK Unified Ideographs 1.1
        //U + 9FA6..U + 9FBB CJK Unified Ideographs 4.1
        //U + F900..U + FA2D CJK Compatibility Ideographs 1.1
        //U + FA30..U + FA6A CJK Compatibility Ideographs 3.2
        //U + FA70..U + FAD9 CJK Compatibility Ideographs 4.1
        //U + 20000..U + 2A6D6 CJK Unified Ideographs Extension B 3.1
        //U + 2F800..U + 2FA1D CJK Compatibility Supplement 3.1
        //中文漢字範圍
        { 1, new string[] { "E4B880", "E9BEA5" } },
        //2）全形ASCII、全形中英文標點、半寬片假名、半寬平假名、半寬韓文字母：FF00 - FFEF
        //http://www.unicode.org/charts/PDF/UFF00.pdf
        {2, new string[] { Util.Unicode2UTF8("FF00"), Util.Unicode2UTF8("FFEF") } },
        //3）CJK部首補充：2E80 - 2EFF
        //http://www.unicode.org/charts/PDF/U2E80.pdf
        {3, new string[] { Util.Unicode2UTF8("2E80"), Util.Unicode2UTF8("2EFF") } },
        //4）CJK標點符號：3000 - 303F
        //http://www.unicode.org/charts/PDF/U3000.pdf
        { 4, new string[] { Util.Unicode2UTF8("3000"), Util.Unicode2UTF8("303F") } },
        //5）CJK筆劃：31C0 - 31EF
        //http://www.unicode.org/charts/PDF/U31C0.pdf
        { 5, new string[] { Util.Unicode2UTF8("31C0"), Util.Unicode2UTF8("31EF") } },
        //6）康熙部首：2F00 - 2FDF
        //http://www.unicode.org/charts/PDF/U2F00.pdf
        {6, new string[] { Util.Unicode2UTF8("2F00"), Util.Unicode2UTF8("2FDF") } },
        //7）漢字結構描述字元：2FF0 - 2FFF
        //http://www.unicode.org/charts/PDF/U2FF0.pdf
        {7, new string[] { Util.Unicode2UTF8("2FF0"), Util.Unicode2UTF8("2FFF") } },
        //8）注音符號：3100 - 312F
        //http://www.unicode.org/charts/PDF/U3100.pdf
        {8, new string[] { Util.Unicode2UTF8("3100"), Util.Unicode2UTF8("312F") } },
        //9）注音符號（閩南語、客家語擴展）：31A0 - 31BF
        //http://www.unicode.org/charts/PDF/U31A0.pdf
        {9, new string[] { Util.Unicode2UTF8("31A0"), Util.Unicode2UTF8("31BF") } },
        //10）日文平假名：3040 - 309F
        //http://www.unicode.org/charts/PDF/U3040.pdf
        {10, new string[] { Util.Unicode2UTF8("3040"), Util.Unicode2UTF8("309F") } },
        //11）日文片假名：30A0 - 30FF
        //http://www.unicode.org/charts/PDF/U30A0.pdf
        {11, new string[] { Util.Unicode2UTF8("30A0"), Util.Unicode2UTF8("30FF") } },
        //12）日文片假名拼音擴展：31F0 - 31FF
        //http://www.unicode.org/charts/PDF/U31F0.pdf
        {12, new string[] { Util.Unicode2UTF8("31F0"), Util.Unicode2UTF8("31FF") } },
        //13）韓文拼音：AC00 - D7AF
        //http://www.unicode.org/charts/PDF/UAC00.pdf
        {13, new string[] { Util.Unicode2UTF8("AC00"), Util.Unicode2UTF8("D7AF") } },
        //14）韓文字母：1100 - 11FF
        //http://www.unicode.org/charts/PDF/U1100.pdf
        { 14, new string[] { Util.Unicode2UTF8("1100"), Util.Unicode2UTF8("11FF") } },
        //15）韓文相容字母：3130 - 318F
        //http://www.unicode.org/charts/PDF/U3130.pdf
        { 15, new string[] { Util.Unicode2UTF8("3130"), Util.Unicode2UTF8("318F") } },
        //16）太玄經符號：1D300 - 1D35F
        //http://www.unicode.org/charts/PDF/U1D300.pdf
        {16, new string[] { Util.Unicode2UTF8("1D300"), Util.Unicode2UTF8("1D35F") } },
        //17）易經六十四卦象：4DC0 - 4DFF
        //http://www.unicode.org/charts/PDF/U4DC0.pdf
        { 17, new string[] { Util.Unicode2UTF8("4DC0"), Util.Unicode2UTF8("4DFF") } },
        //18）彝文音節：A000 - A48F
        //http://www.unicode.org/charts/PDF/UA000.pdf
        {18, new string[] { Util.Unicode2UTF8("A000"), Util.Unicode2UTF8("A48F") } },
        //19）彝文部首：A490 - A4CF
        //http://www.unicode.org/charts/PDF/UA490.pdf
        { 19, new string[] { Util.Unicode2UTF8("A490"), Util.Unicode2UTF8("A4CF") }},
        //20）盲文符號：2800 - 28FF
        //http://www.unicode.org/charts/PDF/U2800.pdf
        {20, new string[] { Util.Unicode2UTF8("2800"), Util.Unicode2UTF8("28FF") }},
        //21）CJK字母及月份：3200 - 32FF
        //http://www.unicode.org/charts/PDF/U3200.pdf
        { 21, new string[] { Util.Unicode2UTF8("3200"), Util.Unicode2UTF8("32FF") }},
        //22）CJK特殊符號（日期合併）：3300 - 33FF
        //http://www.unicode.org/charts/PDF/U3300.pdf
        { 22, new string[] { Util.Unicode2UTF8("3300"), Util.Unicode2UTF8("33FF") }},
        //23）裝飾符號（非CJK專用）：2700 - 27BF
        //http://www.unicode.org/charts/PDF/U2700.pdf
        { 23, new string[] { Util.Unicode2UTF8("2700"), Util.Unicode2UTF8("27BF") }},
        //24）雜項符號（非CJK專用）：2600 - 26FF
        //http://www.unicode.org/charts/PDF/U2600.pdf
        { 24, new string[] { Util.Unicode2UTF8("2600"), Util.Unicode2UTF8("26FF") }},
        //25）中文豎排標點：FE10 - FE1F
        //http://www.unicode.org/charts/PDF/UFE10.pdf
        { 25, new string[] { Util.Unicode2UTF8("FE10"), Util.Unicode2UTF8("FE1F") }},
        //26）CJK相容符號（豎排變體、底線、頓號）：FE30 - FE4F
        //http://www.unicode.org/charts/PDF/UFE30.pdf
        { 26, new string[] { Util.Unicode2UTF8("FE30"), Util.Unicode2UTF8("FE4F") }},
            //27）幾何圖形	Geometric Shapes ：25A0	- 25FF(○符號在此區)
            {27,new string[]{Util.Unicode2UTF8("25A0"),Util.Unicode2UTF8("25FF") } },
            //27）幾何圖形	Geometric Shapes ：2100	- 214F(℃符號在此區)
            {28,new string[]{Util.Unicode2UTF8("2100"),Util.Unicode2UTF8("214F") } }
        };
        #endregion
        #endregion
        /// <summary>
        /// 無法轉碼的文字
        /// </summary>
        public List<Abnormal> UnknowWords { get; set; }
        /// <summary>
        /// 已轉碼自造字文字
        /// </summary>
        public List<Abnormal> TransferedWords { get; set; }

        public EncodingTransfer()
        {
            UnknowWords = new List<Abnormal>();
            TransferedWords = new List<Abnormal>();
        }

        /// <summary>
        /// BIG5轉UTF8文字
        /// 不適用軍網轉民網，軍網有許多造字編碼會與全字庫編碼相衝突
        /// </summary>
        /// <param name="path"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        public string ReplaceOutUt8(string content)
        {
            try
            {
                XmlDocument xmlSummary = GlobalParameters.SummaryTrans;
                Encoding big5 = Encoding.GetEncoding("big5");
                Encoding utf8 = Encoding.UTF8;
                StringBuilder strResult = new StringBuilder();
                StringBuilder strUnknowMsg = new StringBuilder();
                string strFillUnknowWord = UnKnowWord(big5); //無法轉碼呈現文字

                char[] arr = content.ToCharArray();
                for (int i = 0; i < arr.Length; i++)
                {
                    byte[] ByteArrayUtf8 = big5.GetBytes(arr[i].ToString().ToCharArray());
                    string bigCode = BitConverter.ToString(ByteArrayUtf8);
                    //string bigCode = big5.GetString(ByteArrayUtf8);
                    XmlNode specificNode = xmlSummary.SelectSingleNode("//Encoding/BIG5Code[text()='" + bigCode.Replace("-", "") + "']");
                    if (specificNode == null)
                    {
                        strResult.Append(utf8.GetString(ByteArrayUtf8));
                    }
                    else
                    {
                        XmlNode UTF8Node = specificNode.ParentNode.SelectSingleNode("UTF8Code");
                        if (UTF8Node.InnerText.Length > 0)
                        {
                            //運算後的位元組長度:16進位數字字串長/2
                            byte[] byteOUT = new byte[UTF8Node.InnerText.Length / 2];
                            for (int k = 0; k < UTF8Node.InnerText.Length; k = k + 2)
                            {
                                //每2位16進位數字轉換為一個10進位整數
                                byteOUT[k / 2] = Convert.ToByte(UTF8Node.InnerText.Substring(k, 2), 16);
                            }
                            strResult.Append(utf8.GetString(byteOUT));
                        }
                        else
                        {
                            Abnormal ukw = new Abnormal
                            {
                                WordCount = (i + 1).ToString(),
                                OriginWord = GetUnknowString(content, i, 10)
                            };
                            UnknowWords.Add(ukw);
                            strUnknowMsg.AppendLine(ukw.ToString());
                            strResult.Append(strFillUnknowWord);
                        }
                    }
                }
                strResult.AppendLine();
                return strResult.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// BIG5轉UTF8文字
        /// 適用軍網轉民網轉碼
        /// </summary>
        /// <param name="path"></param>        
        /// <returns></returns>
        public string filterOutUTF8ExtraChar(string content)
        {
            try
            {
                Encoding big5 = Encoding.GetEncoding("big5");
                Encoding utf8 = new UTF8Encoding(false);

                StringBuilder strResult = new StringBuilder();
                StringBuilder strUnknowMsg = new StringBuilder();
                string strFillUnknowWord = UnKnowWord(big5); //無法轉碼呈現文字
                //if (output == null) output = new File.Create(@"c:\temp\BIG5.TXT");                                
                XmlDocument xmlSummary = GlobalParameters.SummaryTrans;

                char[] arr = content.ToCharArray();
                for (int i = 0; i < arr.Length; i++)
                {
                    byte[] ByteArrayUtf8 = big5.GetBytes(arr[i].ToString().ToCharArray());
                    if (ByteArrayUtf8.Length == 2)
                    {
                        int code = ByteArrayUtf8[0] * 256 + ByteArrayUtf8[1];
                        //int value = Hex(String.Format("{0:X}", code));
                        //Debug.Print(arr[i].ToString());
                        if ((code >= 0x8140 && code <= 0xA0FE) || (code >= 0xC6A1 && code <= 0xC8FE))
                        {
                            XmlNode specificNode = xmlSummary.SelectSingleNode("//Encoding/BIG5Code[text()='" + code.ToString("X") + "']");
                            if (specificNode == null)
                            {

                                Abnormal ukw = new Abnormal
                                {
                                    WordCount = (i + 1).ToString(),
                                    OriginWord = GetUnknowString(content, i, 10),
                                    TransWord = strFillUnknowWord
                                };
                                UnknowWords.Add(ukw);
                                strUnknowMsg.AppendLine(ukw.ToString());
                                strResult.Append(strFillUnknowWord);
                            }
                            else
                            {
                                //    //specificNode = specificNode.ParentNode;
                                XmlNode Big5Node = specificNode.ParentNode.SelectSingleNode("UTF8Code");
                                if (Big5Node.InnerText.Length > 0)
                                {
                                    //testOutUTF8(specificNode.ParentNode.SelectSingleNode("UTF8Code").InnerText);
                                    //testOutUnicode(specificNode.ParentNode.SelectSingleNode("UnicodeCode").InnerText);
                                    //Encoding u8 = Encoding.UTF8;
                                    //Encoding u = Encoding.Unicode;
                                    //byte[] utf8Bytes = Util.HexToBytes(specificNode.ParentNode.SelectSingleNode("UnicodeCode").InnerText);
                                    //Console.WriteLine(u.GetString(utf8Bytes));
                                    // Convert the string into a byte[]. 
                                    //byte[] utf8Bytes = utf8.GetBytes(specificNode.ParentNode.SelectSingleNode("UnicodeCode").InnerText);
                                    //// Perform the conversion from one encoding to the other. 
                                    //byte[] defaultBytes = Encoding.Convert(u, u8, utf8Bytes);
                                    //char[] defaultChars = new char[u8.GetCharCount(defaultBytes, 0, defaultBytes.Length)];
                                    //u8.GetChars(defaultBytes, 0, defaultBytes.Length, defaultChars, 0);
                                    //string defaultString = new string(defaultChars);

                                    ////byte[] bytes = new byte[UTf8Node.InnerText.Length * sizeof(char)];
                                    ////Buffer.BlockCopy(UTf8Node.InnerText.ToCharArray(), 0, bytes, 0, bytes.Length);
                                    //int byteLength = UTf8Node.InnerText.Length / 2;
                                    //byte[] bytes = new byte[byteLength];
                                    //string hex;
                                    //int j = 0;
                                    //for (int k = 0; k < bytes.Length; k++)
                                    //{
                                    //    hex = new String(new Char[] { UTf8Node.InnerText[j], UTf8Node.InnerText[j + 1] });
                                    //    bytes[k] = HexToByte(hex);
                                    //    j = j + 2;
                                    //}
                                    //運算後的位元組長度:16進位數字字串長/2
                                    byte[] byteOUT = new byte[Big5Node.InnerText.Length / 2];
                                    for (int k = 0; k < Big5Node.InnerText.Length; k = k + 2)
                                    {
                                        //每2位16進位數字轉換為一個10進位整數
                                        byteOUT[k / 2] = Convert.ToByte(Big5Node.InnerText.Substring(k, 2), 16);
                                    }

                                    strResult.Append(utf8.GetString(byteOUT));
                                    //紀錄已轉碼文字
                                    Abnormal tfw = new Abnormal
                                    {
                                        WordCount = (i + 1).ToString(),
                                        OriginWord = GetUnknowString(content, i, 10),
                                        TransWord = utf8.GetString(byteOUT)
                                    };
                                    TransferedWords.Add(tfw);
                                }
                                else
                                {

                                }
                            }
                        }
                        else if (code >= 0xA140 && code <= 0xA3BF)
                        { //標點符號、希臘字母及特殊符號，包括在0xA259-0xA261，安放了九個計量用漢字：兙兛兞兝兡兣嗧瓩糎
                            strResult.Append(TransferWordB2U(arr[i].ToString().ToCharArray()));
                        }
                        else if (code >= 0xA3C0 && code <= 0xA3FE)
                        { //保留。此區沒有開放作造字區用                            
                            Abnormal ukw = new Abnormal
                            {
                                WordCount = (i + 1).ToString(),
                                OriginWord = GetUnknowString(content, i, 10),
                                TransWord = strFillUnknowWord
                            };
                            UnknowWords.Add(ukw);
                            strUnknowMsg.AppendLine(ukw.ToString());
                            strResult.Append(strFillUnknowWord);
                            //strResult.AppendLine(string.Format(@"第{0}行，第{1}個字，起始位置為：" + StartTag + "，來源(UTF-8)內容為{2}", LineCount, i + 1, GetUnknowString(line, i, 10)));
                        }
                        else if (code >= 0xA440 && code <= 0xC67E)
                        { //常用漢字
                            strResult.Append(TransferWordB2U(arr[i].ToString().ToCharArray()));
                        }
                        //else if (code >= 0xC6A1 && code <= 0xC8FE)
                        //{ //保留給使用者自定義字元(造字區)

                        //    Abnormal ukw = new Abnormal
                        //    {
                        //        WordCount = (i + 1).ToString(),
                        //        OriginWord = GetUnknowString(content, i, 10)
                        //    };
                        //    UnknowWords.Add(ukw);
                        //    strUnknowMsg.AppendLine(ukw.ToString());
                        //    strResult.Append(strFillUnknowWord);
                        //    //strResult.AppendLine(string.Format(@"第{0}行，第{1}個字，起始位置為：" + StartTag + "，來源(UTF-8)內容為{2}", LineCount, i + 1, GetUnknowString(line, i, 10)));
                        //}
                        else if (code >= 0xC940 && code <= 0xF9D5)
                        { //次常用漢字
                            strResult.Append(TransferWordB2U(arr[i].ToString().ToCharArray()));
                        }
                        else if (code >= 0xF9D6 && code <= 0xF9FE)
                        { //保留給使用者自定義字元(造字區) (1)CP950 添加了 7 個倚天中文系統增加的字元「碁銹裏墻恒粧嫺」（俗稱「倚天字」）和 34 個畫圖和製表符號(ref: https://zh.wikipedia.org/wiki/代碼頁950)
                            strResult.Append(TransferWordB2U(arr[i].ToString().ToCharArray()));
                        }
                        else if (code > 0xF9FE && code <= 0xFEFE)
                        { //保留給使用者自定義字元(造字區) (2)
                            Abnormal ukw = new Abnormal
                            {
                                WordCount = (i + 1).ToString(),
                                OriginWord = GetUnknowString(content, i, 10),
                                TransWord = strFillUnknowWord
                            };
                            UnknowWords.Add(ukw);
                            strUnknowMsg.AppendLine(ukw.ToString());
                            strResult.Append(strFillUnknowWord);
                            //strResult.AppendLine(string.Format(@"第{0}行，第{1}個字，起始位置為：" + StartTag + "，來源(UTF-8)內容為{2}", LineCount, i + 1, GetUnknowString(line, i, 10)));
                        }
                        else
                        {
                            Abnormal ukw = new Abnormal
                            {
                                WordCount = (i + 1).ToString(),
                                OriginWord = GetUnknowString(content, i, 10),
                                TransWord = strFillUnknowWord
                            };
                            UnknowWords.Add(ukw);
                            strUnknowMsg.AppendLine(ukw.ToString());
                            strResult.Append(strFillUnknowWord);
                            //strResult.AppendLine(string.Format(@"第{0}行，第{1}個字，起始位置為：" + StartTag + "，來源(UTF-8)內容為{2}", LineCount, i + 1, GetUnknowString(line, i, 10)));
                        }
                    }
                    else
                    {
                        strResult.Append(utf8.GetString(ByteArrayUtf8));
                    }
                }


                return strResult.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private void testOutUTF8(string str)
        {
            try
            {

            }
            catch
            {

            }
        }
        private void testOutUnicode(string str)
        {
            try
            {

            }
            catch
            {

            }
        }

        /// <summary>
        /// 判斷是否在常用字集區間內
        /// </summary>
        /// <param name="utfCode"></param>
        /// <returns></returns>
        public bool GetUtf8CodeRange(string utfCode)
        {
            try
            {
                Int32 utf8 = Int32.Parse(utfCode, NumberStyles.HexNumber);
                //中文漢字範圍
                //Int32 ChineseWordStart = Int32.Parse("E4B880", NumberStyles.HexNumber);
                //Int32 ChineseWordEnd = Int32.Parse("E9BEA0", NumberStyles.HexNumber);
                //常用符號範圍
                //Int32 NormalSymboStart = Int32.Parse("EFBC80", NumberStyles.HexNumber);
                //Int32 NormalSymboEnd = Int32.Parse("EFBFAF", NumberStyles.HexNumber);
                foreach (KeyValuePair<int, string[]> item in dicUTF8Range)
                {
                    Int32 StartPoint = Int32.Parse(item.Value[0], NumberStyles.HexNumber);
                    Int32 EndPoint = Int32.Parse(item.Value[1], NumberStyles.HexNumber);
                    if (utf8 >= StartPoint && utf8 <= EndPoint)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        protected string TransferWordB2U(char[] arr)
        {
            try
            {
                Encoding big5 = Encoding.GetEncoding("big5");
                Encoding utf8 = Encoding.UTF8;
                byte[] bdata = big5.GetBytes(arr);
                byte[] temp = Encoding.Convert(big5, utf8, bdata);
                return utf8.GetString(temp);
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        protected StreamWriter TransferWordU2B(char[] arr, StreamWriter fsReturn)
        {
            try
            {
                Encoding big5 = Encoding.GetEncoding("big5");
                Encoding utf8 = Encoding.UTF8;
                byte[] bdata = utf8.GetBytes(arr);
                byte[] temp = Encoding.Convert(utf8, big5, bdata);
                fsReturn.Write(big5.GetString(temp));
            }
            catch (Exception ex)
            {

            }
            return fsReturn;
        }
        protected string UnKnowWord(Encoding encoding)
        {
            try
            {
                return encoding.GetString(ALTERATE_BIG5_CHAR);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private string GetUnknowString(string line, int position, int count)
        {
            string strReturn = string.Empty;
            try
            {
                if (position - count >= 0)
                {
                    strReturn = line.Substring(position - count + 1, count);
                }
                else
                {
                    strReturn = line.Substring(0, line.Length);
                }
            }
            catch (Exception ex)
            {

            }
            return strReturn;
        }
        private string GetStartTag(string line)
        {
            string strReturn = string.Empty;
            try
            {
                string STag = "<";
                string ETag = ">";
                int iStartPosition = 0;
                int iEndPosition = 0;
                iStartPosition = line.IndexOf(STag);
                iEndPosition = line.IndexOf(ETag);
                if (iStartPosition != -1 && iEndPosition != -1)
                {
                    strReturn = line.Substring(iStartPosition, iEndPosition - iStartPosition + 1);
                }
                else
                {
                    strReturn = line;
                }
            }
            catch (Exception ex)
            {

            }
            return strReturn;
        }
    }
}
