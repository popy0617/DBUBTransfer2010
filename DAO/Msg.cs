using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MSDAO.Properties;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace MSDAO
{
    public class Msg
    {
        //private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string MSG_TITLE = Resources.MSG_TITLE;
        public static string NO_DATA_FOUND = "查無資料";
        public static string DATA_DUPLICATE = "資料已存在";
        public static string CHECK_CLICK_DATA = "請點選資料";

        #region 訊息視窗

        /// <summary>
        /// 提示訊息
        /// </summary>
        public static void ShowMessage(string msg)
        {
            HttpContext.Current.Response.Write(" <script language='javascript'>");
            HttpContext.Current.Response.Write(" alert('" + msg + "');");
            HttpContext.Current.Response.Write(" </script>");
        }

        public static void ShowMessage(Control ctrl, Exception ex = null)
        {
            if (ctrl == null)
                return;

            if (ex != null)
            {
                //logger.Error(ex.Message, ex);
                HttpContext.Current.Response.Write(" <script language='javascript'>");
                HttpContext.Current.Response.Write(" alert('" + ex.Message + "');");
                HttpContext.Current.Response.Write(" </script>");
            }
            else
            {
                if (ctrl is Button)
                {
                    string tmp = ((Button)ctrl).Text;

                    //清除按鈕不顯示訊息
                    if (Resources.CLEAR_E_AREA.Equals(tmp) || Resources.CLEAR_Q_AREA.Equals(tmp))
                        return;
                }

                //動態修改過後的新值不會帶到後端，只好丟回前端才取
                //ctrl.ID:BTN_CLEAR
                //ctrl.ClientID：App.cph_content1_BTN_CLEAR
                //X.Js.Call("showMessageByCtrl", ctrl.ClientID);
                HttpContext.Current.Response.Write(" <script language='javascript'>");
                HttpContext.Current.Response.Write(" alert('" + ctrl.ClientID + "');");
                HttpContext.Current.Response.Write(" </script>");
            }
        }

        /// <summary>
        /// 彈出式視窗
        /// </summary>
        /// <param name="content"></param>
        public static void ShowAlert(string content)
        {
            //X.Msg.Alert(MSG_TITLE, content).Show();
            HttpContext.Current.Response.Write(" <script language='javascript'>");
            HttpContext.Current.Response.Write(" alert('" + content + "');");
            HttpContext.Current.Response.Write(" </script>");
        }

        /// <summary>
        /// 彈出式YES/NO視窗
        /// </summary>
        public static void ShowConfirmDialog(string msg, string yesCallbackScript = null, string noCallbackScript = null)
        {
            //X.Msg.Confirm(MSG_TITLE, msg, new MessageBoxButtonsConfig
            //{
            //    Yes = new MessageBoxButtonConfig
            //    {
            //        Handler = yesCallbackScript,
            //        Text = "是"
            //    },
            //    No = new MessageBoxButtonConfig
            //    {
            //        Handler = noCallbackScript,
            //        Text = "否"
            //    }
            //}).Show();
        }

        #endregion 訊息視窗

        #region 組合訊息

        /// <summary>
        /// 訊息內容-空值
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>string</returns>
        public static string EmptyMsgContent(string fieldName)
        {
            return fieldName + "不可為空";
        }

        /// <summary>
        /// 訊息內容-格式不正確
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>string</returns>
        public static string IncorrectFormatMsgContent(string fieldName = "")
        {
            return fieldName + "格式不正確";
        }

        #endregion 組合訊息
    }
}
