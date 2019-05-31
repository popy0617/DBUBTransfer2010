using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSDAO.Properties;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace MSDAO
{
    public class OracleCommonUtil
    {
        /// <summary>
        /// 取得seqName.NextVal
        /// MS SQL、ACCESS、MYSQL不適用
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="seqName"></param>
        /// <returns></returns>
        public static long GetSequenceNextVal(string tableName, string seqName)
        {
            string sql = string.Format("SELECT {0}.NEXTVAL FROM {1}", seqName, tableName);
            return new GeneralDao().QueryForLong(sql);
        }
    }
    public class CommonUtil
    {
        //private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Grid & Store

        public enum OP_MODE
        {
            I = 1,
            U = 2,
            D = 3,
            P = 4
        }

        #region EXT
        /// <summary>
        /// 將DataTable資料塞給Grid
        /// 如果DataTable筆數大於0則回傳true，反之回傳false
        /// </summary>
        //public static bool SetGrid(Ext.Net.GridPanel grid, DataTable dt, bool resetGrid = true)
        //{
        //    bool isDtHasRec = false;

        //    //建立STORE以及綁定資料
        //    bool createStore = false;
        //    if (grid.Store.Primary == null)
        //    {
        //        createStore = true;
        //        Model m = new Model();
        //        m.ID = grid.ID + "_MODEL";
        //        m.IDProperty = "";
        //        foreach (DataColumn dc in dt.Columns)
        //        {
        //            m.Fields.Add(new ModelField()
        //            {
        //                Name = dc.ColumnName
        //            });
        //        }
        //        Store s = new Store();
        //        s.ID = grid.ID + "_STORE";
        //        s.PageSize = ConvertUtil.ToInt(Resource.GRID_PAGE_SIZE);
        //        s.Model.Add(m);
        //        grid.Store.Add(s);
        //    }
        //    if (dt == null)
        //    {
        //        grid.GetStore().DataSource = "";
        //    }
        //    else
        //    {
        //        grid.GetStore().DataSource = dt;
        //        if (dt.Rows.Count > 0)
        //            isDtHasRec = true;
        //        else
        //            isDtHasRec = false;
        //    }
        //    grid.GetStore().DataBind();
        //    if (createStore)
        //        grid.Render();

        //    if (resetGrid)
        //    {
        //        if (grid.GetSelectionModel() != null)
        //            grid.GetSelectionModel().ClearSelection();
        //        if (grid.BottomBar.Toolbar != null)
        //            ((PagingToolbar)grid.FindControl(grid.BottomBar.Toolbar.ID)).MoveFirst();
        //    }
        //    else
        //    {
        //        X.Js.Call("moveToSelectionStartPage", grid.ClientID);
        //    }

        //    return isDtHasRec;
        //}

        //清除Grid資料
        //public static void ClearGrid(GridPanel grid)
        //{
        //    ResetGridRelateObjs(grid);

        //    //後續可以接續其他DataBind
        //    grid.GetStore().LoadData("", false);
        //}

        //public static void SelectGridRecById(Ext.Net.GridPanel grid, object pk)
        //{
        //    CommonUtil.SelectGridRecById(grid, new object[] { pk });
        //}

        //public static void SelectGridRecById(Ext.Net.GridPanel grid, object[] pks)
        //{
        //    if (pks == null || pks.Length == 0)
        //        return;

        //    if (grid.GetSelectionModel() == null)
        //        return;

        //    grid.GetSelectionModel().Select(pks);
        //    X.Js.Call("moveToSelectionStartPage", grid.ClientID);
        //}

        //#endregion 根據根據IDProperty的值點選GRID資料

        //private static void ResetGridRelateObjs(GridPanel grid)
        //{
        //    if (grid.GetSelectionModel() != null)
        //        grid.GetSelectionModel().ClearSelection();

        //    if (grid.BottomBar.Toolbar != null)
        //        ((PagingToolbar)grid.FindControl(grid.BottomBar.Toolbar.ID)).MoveFirst();
        //}

        ////將DataTable資料綁定進Store
        //public static void SetStore(Control ctrl, DataTable dt)
        //{
        //    bool createStore = false;

        //    switch (ctrl.GetType().ToString())
        //    {
        //        case "Ext.Net.MultiSelect":
        //        case "Ext.Net.ItemSelector":
        //            MultiSelectBase ms = (MultiSelectBase)ctrl;
        //            createStore = false;
        //            if (ms.Store.Primary == null)
        //            {
        //                createStore = true;
        //                Model m = new Model();
        //                m.ID = ms.ID + "_MODEL";
        //                m.Fields.Add(new ModelField()
        //                {
        //                    Name = ms.DisplayField
        //                });
        //                Store s = new Store();
        //                s.ID = ms.ID + "_STORE";
        //                s.Model.Add(m);
        //                ms.Store.Add(s);
        //            }
        //            if (dt == null)
        //                ms.GetStore().DataSource = "";
        //            else
        //                ms.GetStore().DataSource = dt;
        //            ms.GetStore().DataBind();
        //            if (createStore)
        //                ms.Render();
        //            break;

        //        case "Ext.Net.ComboBox":
        //        case "Ext.Net.MultiCombo":
        //            ComboBoxBase cb = (ComboBoxBase)ctrl;
        //            createStore = false;
        //            if (cb.Store.Primary == null)
        //            {
        //                createStore = true;
        //                Model m = new Model();
        //                m.ID = cb.ID + "_MODEL";
        //                m.Fields.Add(new ModelField()
        //                {
        //                    Name = cb.DisplayField
        //                });
        //                Store s = new Store();
        //                s.ID = cb.ID + "_STORE";
        //                s.Model.Add(m);
        //                cb.Store.Add(s);
        //            }

        //            cb.GetStore().ClearFilter();

        //            if (dt == null)
        //            {//清除資料
        //                cb.GetStore().DataSource = "";
        //                cb.GetStore().DataBind();
        //            }
        //            else
        //            {
        //                if (cb.GetStore().DataSource == null)
        //                {//首次綁定
        //                    cb.GetStore().DataSource = dt;
        //                    cb.GetStore().DataBind();
        //                }
        //                else
        //                {//重複綁定，清除後重LOAD
        //                    cb.GetStore().LoadData("", false);
        //                    cb.GetStore().LoadData(dt, true);
        //                }
        //            }

        //            if (createStore)
        //                cb.Render();
        //            break;
        //    }
        //}

        ////清除Store
        //public static void ClearStore(Store store)
        //{
        //    store.LoadData("", false);
        //}

        //public static Dictionary<string, string> GridRecToContainer(string gridRec, AbstractPanel panel, string fieldPrefix)
        //{
        //    Dictionary<string, string> dic = JSON.Deserialize<Dictionary<string, string>[]>(gridRec)[0];

        //    foreach (KeyValuePair<string, string> kvp in dic)
        //    {
        //        Control ct = FindControlUnder(panel, fieldPrefix + kvp.Key);
        //        if (ct != null)
        //            CommonUtil.SetObjectValue(ct, kvp.Value);
        //    }

        //    return dic;
        //}

        //private static void ResetGridRelateObjs(GridPanel grid)
        //{
        //    if (grid.GetSelectionModel() != null)
        //        grid.GetSelectionModel().ClearSelection();

        //    if (grid.BottomBar.Toolbar != null)
        //        ((PagingToolbar)grid.FindControl(grid.BottomBar.Toolbar.ID)).MoveFirst();
        //}

        ////將DataTable資料綁定進Store
        //public static void SetStore(Control ctrl, DataTable dt)
        //{
        //    bool createStore = false;

        //    switch (ctrl.GetType().ToString())
        //    {
        //        case "Ext.Net.MultiSelect":
        //        case "Ext.Net.ItemSelector":
        //            MultiSelectBase ms = (MultiSelectBase)ctrl;
        //            createStore = false;
        //            if (ms.Store.Primary == null)
        //            {
        //                createStore = true;
        //                Model m = new Model();
        //                m.ID = ms.ID + "_MODEL";
        //                m.Fields.Add(new ModelField()
        //                {
        //                    Name = ms.DisplayField
        //                });
        //                Store s = new Store();
        //                s.ID = ms.ID + "_STORE";
        //                s.Model.Add(m);
        //                ms.Store.Add(s);
        //            }
        //            if (dt == null)
        //                ms.GetStore().DataSource = "";
        //            else
        //                ms.GetStore().DataSource = dt;
        //            ms.GetStore().DataBind();
        //            if (createStore)
        //                ms.Render();
        //            break;

        //        case "Ext.Net.ComboBox":
        //        case "Ext.Net.MultiCombo":
        //            ComboBoxBase cb = (ComboBoxBase)ctrl;
        //            createStore = false;
        //            if (cb.Store.Primary == null)
        //            {
        //                createStore = true;
        //                Model m = new Model();
        //                m.ID = cb.ID + "_MODEL";
        //                m.Fields.Add(new ModelField()
        //                {
        //                    Name = cb.DisplayField
        //                });
        //                Store s = new Store();
        //                s.ID = cb.ID + "_STORE";
        //                s.Model.Add(m);
        //                cb.Store.Add(s);
        //            }

        //            cb.GetStore().ClearFilter();

        //            if (dt == null)
        //            {//清除資料
        //                cb.GetStore().DataSource = "";
        //                cb.GetStore().DataBind();
        //            }
        //            else
        //            {
        //                if (cb.GetStore().DataSource == null)
        //                {//首次綁定
        //                    cb.GetStore().DataSource = dt;
        //                    cb.GetStore().DataBind();
        //                }
        //                else
        //                {//重複綁定，清除後重LOAD
        //                    cb.GetStore().LoadData("", false);
        //                    cb.GetStore().LoadData(dt, true);
        //                }
        //            }

        //            if (createStore)
        //                cb.Render();
        //            break;
        //    }
        //}

        ////清除Store
        //public static void ClearStore(Store store)
        //{
        //    store.LoadData("", false);
        //}

        //public static Dictionary<string, string> GridRecToContainer(string gridRec, AbstractPanel panel, string fieldPrefix)
        //{
        //    Dictionary<string, string> dic = JSON.Deserialize<Dictionary<string, string>[]>(gridRec)[0];

        //    foreach (KeyValuePair<string, string> kvp in dic)
        //    {
        //        Control ct = FindControlUnder(panel, fieldPrefix + kvp.Key);
        //        if (ct != null)
        //            CommonUtil.SetObjectValue(ct, kvp.Value);
        //    }

        //    return dic;
        //}

        //#endregion Grid & Store

        //#region 控制項操作

        ////跟據id取得ctrl下的物件，找不到則回傳null
        //public static Control FindControlUnder(Control ctrl, string id)
        //{
        //    if (id.ToUpper().Equals(ConvertUtil.ToString(ctrl.ID).ToUpper()))
        //        return ctrl;
        //    foreach (Control c in ctrl.Controls)
        //    {
        //        Control t = FindControlUnder(c, id);
        //        if (t != null)
        //            return t;
        //    }
        //    return null;
        //}

        ////取出控制項的值
        //public static string GetObjectValue(Control ctrl)
        //{
        //    string value = string.Empty;

        //    if (ctrl != null)
        //    {
        //        switch (ctrl.GetType().ToString())
        //        {
        //            case "Ext.Net.DateField":
        //                DateField df = (DateField)ctrl;
        //                if (!df.IsEmpty)
        //                {
        //                    value = df.Text;
        //                }
        //                else
        //                {
        //                    if (!string.IsNullOrEmpty(df.RawText))  //IsEmpty但RawText卻有值 >> 輸入的日期格式不正確
        //                        throw new Exception(Msg.IncorrectFormatMsgContent(GetObjectLabel(df)));
        //                }
        //                break;

        //            case "Ext.Net.Button":
        //                Ext.Net.Button bt = (Ext.Net.Button)ctrl;
        //                value = bt.Text;
        //                break;

        //            case "Ext.Net.Label":
        //                Ext.Net.Label lbl = (Ext.Net.Label)ctrl;
        //                value = lbl.Text;
        //                break;

        //            case "Ext.Net.Hidden":
        //                Ext.Net.Hidden hd = (Ext.Net.Hidden)ctrl;
        //                value = hd.Text;
        //                break;

        //            default:
        //                if (ctrl is Ext.Net.TextFieldBase)
        //                {
        //                    TextFieldBase tfb = (TextFieldBase)ctrl;
        //                    value = tfb.Text;
        //                }
        //                break;
        //        }
        //    }
        //    return value;
        //}

        //public static void SetObjectValue(Control ctrl, string value)
        //{
        //    if (ctrl == null)
        //        return;

        //    if (string.IsNullOrEmpty(value) || value.ToLower().Equals("null"))
        //        value = string.Empty;

        //    switch (ctrl.GetType().ToString())
        //    {
        //        case "Ext.Net.TextField":       //避免塞值時又發動EVENT
        //        case "Ext.Net.TriggerField":
        //        case "Ext.Net.TextArea":
        //            if (ctrl is Observable)
        //                ((Observable)ctrl).SuspendEvents();

        //            TextFieldBase tf = (TextFieldBase)ctrl;

        //            //數字型態的TextField如果SuspendEvents後不能觸發加上千分位，因此在後端進行format
        //            if ("INTEGER".Equals(tf.Vtype) || "NUMBER".Equals(tf.Vtype))
        //                tf.Text = ConvertUtil.FormatMoney(value);
        //            else
        //                tf.Text = value;

        //            if (ctrl is Observable)
        //                ((Observable)ctrl).ResumeEvents();

        //            break;

        //        case "Ext.Net.ComboBox":    //特殊控制項EXT有許多背景動作，不可SuspendEvents
        //        case "Ext.Net.DateField":
        //            TextFieldBase tfb = (TextFieldBase)ctrl;
        //            tfb.Text = value;
        //            break;

        //        case "Ext.Net.Hidden":
        //            Hidden h = (Hidden)ctrl;
        //            h.Text = value;
        //            break;
        //    }
        //}

        ////取出控制項的Label
        //public static string GetObjectLabel(Control ctrl)
        //{
        //    if (ctrl == null)
        //        return null;

        //    string label = string.Empty;

        //    if (ctrl is Ext.Net.TextFieldBase)
        //    {
        //        TextFieldBase tfb = (TextFieldBase)ctrl;
        //        label = GetLabelText(tfb.FieldLabel);
        //    }
        //    else if (ctrl is Ext.Net.Button)
        //    {
        //        Ext.Net.Button btn = (Ext.Net.Button)ctrl;
        //        label = btn.Text;
        //    }

        //    return label;
        //}

        ////取得Label值
        //private static string GetLabelText(string label)
        //{
        //    return string.IsNullOrEmpty(label) ? string.Empty : label.Replace(Resource.REQUIRE_FLAG, "").Replace(Resource.REQUIRE_FLAG, "");
        //}

        //public static void SetRequiredFlag(Control[] ctrls, bool flag = true)
        //{
        //    if (ctrls == null)
        //        return;

        //    foreach (Control ctrl in ctrls)
        //        CommonUtil.SetRequiredFlag(ctrl, flag);
        //}

        ////加上必填欄位FLAG
        //public static void SetRequiredFlag(Control ctrl, bool flag = true)
        //{
        //    if (ctrl == null)
        //        return;

        //    string required = Resource.REQUIRE_FLAG;
        //    switch (ctrl.GetType().ToString())
        //    {
        //        case "Ext.Net.TextField":
        //        case "Ext.Net.NumberField":
        //        case "Ext.Net.ComboBox":
        //        case "Ext.Net.DateField":
        //        case "Ext.Net.TextArea":
        //        case "Ext.Net.TriggerField":
        //            TextFieldBase tfb = (TextFieldBase)ctrl;
        //            tfb.FieldLabel = tfb.FieldLabel.Replace(required, "");   //避免重複加上flag，先移除之
        //            if (flag)
        //                tfb.FieldLabel = required + tfb.FieldLabel;
        //            break;
        //    }
        //}

        //#region 檢核

        ///// <summary>
        ///// 檢查物件是否為空值
        ///// 如為空值則丟出exception
        ///// </summary>
        ///// <param name="ctrl"></param>
        //public static void EnsureObjectNotEmpty(Control ctrl, string fieldLabel = null)
        //{
        //    if (ctrl != null)
        //    {
        //        switch (ctrl.GetType().ToString())
        //        {
        //            case "Ext.Net.TextField":
        //            case "Ext.Net.TextArea":
        //            case "Ext.Net.NumberField":
        //            case "Ext.Net.ComboBox":
        //            case "Ext.Net.TriggerField":
        //                TextFieldBase tf = (TextFieldBase)ctrl;
        //                if (tf.IsEmpty)
        //                {
        //                    if (string.IsNullOrEmpty(fieldLabel))
        //                        fieldLabel = GetLabelText(tf.FieldLabel);

        //                    throw new Exception(Msg.EmptyMsgContent(fieldLabel));
        //                }
        //                break;

        //            case "Ext.Net.DateField":
        //                DateField df = (DateField)ctrl;
        //                if (df.IsEmpty)
        //                {
        //                    if (string.IsNullOrEmpty(fieldLabel))
        //                        fieldLabel = GetLabelText(df.FieldLabel);

        //                    if (!string.IsNullOrEmpty(df.RawText))
        //                        throw new Exception(Msg.IncorrectFormatMsgContent(fieldLabel));
        //                    else
        //                        throw new Exception(Msg.EmptyMsgContent(fieldLabel));
        //                }
        //                break;

        //            default:
        //                logger.Error("不支援的物件類型" + ctrl.ID);
        //                break;
        //        }
        //    }
        //}

        ///// <summary>
        ///// 檢核容器內有 * 的必填欄位是否為空值，如為空值則丟出exception
        ///// hidden欄位不做檢核，如有特殊需求請使用ensureObjectNotEmpty()
        /////
        ///// A client side hidden property is not submitted to server automatically. So, no way to maintain a server side Hidden property.
        ///// 因server端無法取得contorl hidden屬性，因此無法進行判斷
        ///// </summary>
        ///// <param name="cont">要檢核的容器</param>
        ///// <param name="includeHiddenObj">是否要檢核隱藏物件</param>
        //public static void EnsureContainerObjectNotEmpty(Control ctrl)
        //{
        //    if (ctrl == null)
        //        return;

        //    switch (ctrl.GetType().ToString())
        //    {
        //        case "Ext.Net.TextField":
        //        case "Ext.Net.TextArea":
        //        case "Ext.Net.NumberField":
        //        case "Ext.Net.MultiCombo":
        //        case "Ext.Net.ComboBox":
        //        case "Ext.Net.TriggerField":
        //            TextFieldBase tf = (TextFieldBase)ctrl;
        //            if (tf.IsEmpty && IsRequiredField(tf.FieldLabel) && !tf.Hidden)
        //                throw new Exception(Msg.EmptyMsgContent(GetLabelText(tf.FieldLabel)));
        //            break;

        //        case "Ext.Net.DateField":
        //            DateField df = (DateField)ctrl;
        //            if (df.IsEmpty && IsRequiredField(df.FieldLabel) && !df.Hidden)
        //            {
        //                if (!string.IsNullOrEmpty(df.RawText))
        //                    throw new Exception(Msg.IncorrectFormatMsgContent(GetLabelText(df.FieldLabel)));
        //                else
        //                    throw new Exception(Msg.EmptyMsgContent(GetLabelText(df.FieldLabel)));
        //            }
        //            break;

        //        default:
        //            foreach (Control child in ctrl.Controls)
        //                EnsureContainerObjectNotEmpty(child);
        //            break;
        //    }
        //}

        ////確保物件內容值不相等於val
        //public static void EnsureObjectNotEqualToVal(Control ctrl, object[] val)
        //{
        //    if (val == null || val.Length == 0)
        //        return;

        //    if (ctrl is TextFieldBase)
        //    {
        //        TextFieldBase tfb = (TextFieldBase)ctrl;
        //        foreach (object s in val)
        //        {
        //            if (ConvertUtil.ToString(s).Equals(tfb.RawText))
        //                throw new Exception(CommonUtil.GetObjectLabel(tfb) + "不可為" + tfb.RawText);
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception("不支援的物件類型" + ctrl.ID);
        //    }
        //}

        ////確保dfEnd時間大於dfBeg
        //public static void CompareDateFields(DateField dfBeg, DateField dfEnd, string dateLabel = null)
        //{
        //    if (dfBeg.IsEmpty || dfEnd.IsEmpty)
        //        return;

        //    if (DateTime.Parse(dfBeg.Text).CompareTo(DateTime.Parse(dfEnd.Text)) > 0)
        //    {
        //        if (string.IsNullOrEmpty(dateLabel))
        //            throw new Exception(GetObjectLabel(dfEnd) + "不可早於" + GetObjectLabel(dfBeg));
        //        else
        //            throw new Exception(dateLabel + "迄日不可早於起日");
        //    }
        //}

        ////以<font color=red>*</font>來判斷是否為必填欄位
        //private static bool IsRequiredField(string label)
        //{
        //    bool b = false;
        //    if (!string.IsNullOrEmpty(label))
        //    {
        //        if (label.ToLower().IndexOf(Resource.REQUIRE_FLAG) != -1 || label.ToLower().IndexOf(Resource.REQUIRE_FLAG) != -1)
        //            b = true;
        //    }
        //    return b;
        //}

        //#endregion 檢核

        ////將Container下控制項轉成Dictionary<ctrlName, ctrlValue>
        //public static void ContainerToDictionary(Control ctrl, Dictionary<string, string> dic)
        //{
        //    foreach (Control c in ctrl.Controls)
        //    {
        //        if (c is Ext.Net.TextFieldBase)
        //            dic.Add(c.ID, CommonUtil.GetObjectValue(c));
        //        else
        //            CommonUtil.ContainerToDictionary(c, dic);
        //    }
        //}

        //#endregion 控制項操作

        ////取得seqName.NextVal
        //public static long GetSequenceNextVal(string seqName)
        //{
        //    string sql = string.Format("SELECT {0}.NEXTVAL FROM DUAL", seqName);
        //    return new GeneralDAO().QueryForLong(sql);
        //}

        ////根據傳入的Dictionary組出標準HTTP URL字串
        //public static string BuildQueryString(string url, Dictionary<string, string> para)
        //{
        //    //移除不正常結尾
        //    if (url.EndsWith("?") || url.EndsWith("&"))
        //        url = url.Remove(url.Length - 1);

        //    if (url.IndexOf('?') == -1)
        //        url += "?";
        //    else
        //        url += "&";

        //    //組合Para字串
        //    foreach (KeyValuePair<string, string> item in para)
        //        url += item.Key + "=" + item.Value + "&";
        //    if (para.Count > 0)
        //        url = url.Remove(url.Length - 1);

        //    return url;
        //}

        ////導頁
        //public void RedirectToPrg()
        //{
        //    //TODO
        //}

        #endregion
        /// <summary>
        /// 將DataTable資料塞給Grid
        /// 如果DataTable筆數大於0則回傳true，反之回傳false
        /// </summary>
        public static bool SetGrid(DataGrid grid, DataTable dt, bool resetGrid = true)
        {
            bool isDtHasRec = false;

            //建立STORE以及綁定資料
            bool createStore = false;
            //if (grid.Store.Primary == null)
            //{
            //    createStore = true;
            //    Model m = new Model();
            //    m.ID = grid.ID + "_MODEL";
            //    m.IDProperty = "";
            //    foreach (DataColumn dc in dt.Columns)
            //    {
            //        m.Fields.Add(new ModelField()
            //        {
            //            Name = dc.ColumnName
            //        });
            //    }
            //    Store s = new Store();
            //    s.ID = grid.ID + "_STORE";
            //    s.PageSize = ConvertUtil.ToInt(Resource.GRID_PAGE_SIZE);
            //    s.Model.Add(m);
            //    grid.Store.Add(s);
            //}
            //if (dt == null)
            //{
            //    grid.GetStore().DataSource = "";
            //}
            //else
            //{
            //    grid.GetStore().DataSource = dt;
            //    if (dt.Rows.Count > 0)
            //        isDtHasRec = true;
            //    else
            //        isDtHasRec = false;
            //}
            //grid.GetStore().DataBind();
            //if (createStore)
            //    grid.Render();

            //if (resetGrid)
            //{
            //    if (grid.GetSelectionModel() != null)
            //        grid.GetSelectionModel().ClearSelection();
            //    if (grid.BottomBar.Toolbar != null)
            //        ((PagingToolbar)grid.FindControl(grid.BottomBar.Toolbar.ID)).MoveFirst();
            //}
            //else
            //{
            //    X.Js.Call("moveToSelectionStartPage", grid.ClientID);
            //}

            return isDtHasRec;
        }

        /// <summary>
        /// 清除Grid資料
        /// </summary>
        /// <param name="grid"></param>
        public static void ClearGrid(DataGrid grid)
        {
            ResetGridRelateObjs(grid);

            ////後續可以接續其他DataBind
            //grid.GetStore().LoadData("", false);
        }

        #region 根據根據IDProperty的值點選GRID資料
                
        public static void SelectGridRecById(DataGrid grid, object pk)
        {
            CommonUtil.SelectGridRecById(grid, new object[] { pk });
        }

        public static void SelectGridRecById(DataGrid grid, object[] pks)
        {
            //if (pks == null || pks.Length == 0)
            //    return;

            //if (grid.GetSelectionModel() == null)
            //    return;

            //grid.GetSelectionModel().Select(pks);
            //X.Js.Call("moveToSelectionStartPage", grid.ClientID);
        }

        #endregion 根據根據IDProperty的值點選GRID資料

        private static void ResetGridRelateObjs(DataGrid grid)
        {
            //if (grid.GetSelectionModel() != null)
            //    grid.GetSelectionModel().ClearSelection();

            //if (grid.BottomBar.Toolbar != null)
            //    ((PagingToolbar)grid.FindControl(grid.BottomBar.Toolbar.ID)).MoveFirst();
        }

        //將DataTable資料綁定進Store
        public static void SetStore(Control ctrl, DataTable dt)
        {
            bool createStore = false;

            //switch (ctrl.GetType().ToString())
            //{
            //    case "Ext.Net.MultiSelect":
            //    case "Ext.Net.ItemSelector":
            //        MultiSelectBase ms = (MultiSelectBase)ctrl;
            //        createStore = false;
            //        if (ms.Store.Primary == null)
            //        {
            //            createStore = true;
            //            Model m = new Model();
            //            m.ID = ms.ID + "_MODEL";
            //            m.Fields.Add(new ModelField()
            //            {
            //                Name = ms.DisplayField
            //            });
            //            Store s = new Store();
            //            s.ID = ms.ID + "_STORE";
            //            s.Model.Add(m);
            //            ms.Store.Add(s);
            //        }
            //        if (dt == null)
            //            ms.GetStore().DataSource = "";
            //        else
            //            ms.GetStore().DataSource = dt;
            //        ms.GetStore().DataBind();
            //        if (createStore)
            //            ms.Render();
            //        break;

            //    case "Ext.Net.ComboBox":
            //    case "Ext.Net.MultiCombo":
            //        ComboBoxBase cb = (ComboBoxBase)ctrl;
            //        createStore = false;
            //        if (cb.Store.Primary == null)
            //        {
            //            createStore = true;
            //            Model m = new Model();
            //            m.ID = cb.ID + "_MODEL";
            //            m.Fields.Add(new ModelField()
            //            {
            //                Name = cb.DisplayField
            //            });
            //            Store s = new Store();
            //            s.ID = cb.ID + "_STORE";
            //            s.Model.Add(m);
            //            cb.Store.Add(s);
            //        }

            //        cb.GetStore().ClearFilter();

            //        if (dt == null)
            //        {//清除資料
            //            cb.GetStore().DataSource = "";
            //            cb.GetStore().DataBind();
            //        }
            //        else
            //        {
            //            if (cb.GetStore().DataSource == null)
            //            {//首次綁定
            //                cb.GetStore().DataSource = dt;
            //                cb.GetStore().DataBind();
            //            }
            //            else
            //            {//重複綁定，清除後重LOAD
            //                cb.GetStore().LoadData("", false);
            //                cb.GetStore().LoadData(dt, true);
            //            }
            //        }

            //        if (createStore)
            //            cb.Render();
            //        break;
            //}
        }

        //清除Store
        //public static void ClearStore(Store store)
        //{
        //    store.LoadData("", false);
        //}

        //public static Dictionary<string, string> GridRecToContainer(string gridRec, AbstractPanel panel, string fieldPrefix)
        //{
        //    Dictionary<string, string> dic = JSON.Deserialize<Dictionary<string, string>[]>(gridRec)[0];

        //    foreach (KeyValuePair<string, string> kvp in dic)
        //    {
        //        Control ct = FindControlUnder(panel, fieldPrefix + kvp.Key);
        //        if (ct != null)
        //            CommonUtil.SetObjectValue(ct, kvp.Value);
        //    }

        //    return dic;
        //}

        #endregion Grid & Store

        #region 控制項操作
        
        /// <summary>
        /// 跟據id取得ctrl下的物件，找不到則回傳null
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Control FindControlUnder(Control ctrl, string id)
        {
            if (id.ToUpper().Equals(ConvertUtil.ToString(ctrl.ID).ToUpper()))
                return ctrl;
            foreach (Control c in ctrl.Controls)
            {
                Control t = FindControlUnder(c, id);
                if (t != null)
                    return t;
            }
            return null;
        }
       
        /// <summary>
        /// 取出控制項的值
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public static string GetObjectValue(Control ctrl)
        {
            string value = string.Empty;

            if (ctrl != null)
            {
                switch (ctrl.GetType().ToString())
                {
                    case "DateField":
                        //DateField df = (DateField)ctrl;
                        //if (!df.IsEmpty)
                        //{
                        //    value = df.Text;
                        //}
                        //else
                        //{
                        //    if (!string.IsNullOrEmpty(df.RawText))  //IsEmpty但RawText卻有值 >> 輸入的日期格式不正確
                        //        throw new Exception(Msg.IncorrectFormatMsgContent(GetObjectLabel(df)));
                        //}
                        break;

                    case "Button":
                        Button bt = (Button)ctrl;
                        value = bt.Text;
                        break;

                    case "Label":
                        Label lbl = (Label)ctrl;
                        value = lbl.Text;
                        break;

                    case "Hidden":
                        HiddenField hd = (HiddenField)ctrl;
                        value = hd.Value;
                        break;

                    default:
                        if (ctrl is TextBox)
                        {
                            TextBox tfb = (TextBox)ctrl;
                            value = tfb.Text;
                        }
                        break;
                }
            }
            return value;
        }

        /// <summary>
        /// 設定控制項的值
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="value"></param>
        public static void SetObjectValue(Control ctrl, string value)
        {
            if (ctrl == null)
                return;

            if (string.IsNullOrEmpty(value) || value.ToLower().Equals("null"))
                value = string.Empty;

            //switch (ctrl.GetType().ToString())
            //{
            //    case "Ext.Net.TextField":       //避免塞值時又發動EVENT
            //    case "Ext.Net.TriggerField":
            //    case "Ext.Net.TextArea":
            //        if (ctrl is Observable)
            //            ((Observable)ctrl).SuspendEvents();

            //        TextFieldBase tf = (TextFieldBase)ctrl;

            //        //數字型態的TextField如果SuspendEvents後不能觸發加上千分位，因此在後端進行format
            //        if ("INTEGER".Equals(tf.Vtype) || "NUMBER".Equals(tf.Vtype))
            //            tf.Text = ConvertUtil.FormatMoney(value);
            //        else
            //            tf.Text = value;

            //        if (ctrl is Observable)
            //            ((Observable)ctrl).ResumeEvents();

            //        break;

            //    case "Ext.Net.ComboBox":    //特殊控制項EXT有許多背景動作，不可SuspendEvents
            //    case "Ext.Net.DateField":
            //        TextFieldBase tfb = (TextFieldBase)ctrl;
            //        tfb.Text = value;
            //        break;

            //    case "Ext.Net.Hidden":
            //        Hidden h = (Hidden)ctrl;
            //        h.Text = value;
            //        break;
            //}
        }

        //取出控制項的Label
        public static string GetObjectLabel(Control ctrl)
        {
            if (ctrl == null)
                return null;

            string label = string.Empty;

            if (ctrl is TextBox)
            {
                TextBox tfb = (TextBox)ctrl;
                label = GetLabelText(tfb.Text);
            }
            else if (ctrl is Button)
            {
                Button btn = (Button)ctrl;
                label = btn.Text;
            }

            return label;
        }

        //取得Label值
        private static string GetLabelText(string label)
        {
            return string.IsNullOrEmpty(label) ? string.Empty : label.Replace(Resources.REQUIRE_FLAG, "").Replace(Resources.REQUIRE_FLAG, "");
        }

        public static void SetRequiredFlag(Control[] ctrls, bool flag = true)
        {
            if (ctrls == null)
                return;

            foreach (Control ctrl in ctrls)
                CommonUtil.SetRequiredFlag(ctrl, flag);
        }

        /// <summary>
        /// 加上必填欄位FLAG
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="flag"></param>
        public static void SetRequiredFlag(Control ctrl, bool flag = true)
        {
            if (ctrl == null)
                return;

            string required = Resources.REQUIRE_FLAG;
            switch (ctrl.GetType().ToString())
            {
                case "TextField":
                case "NumberField":
                case "ComboBox":
                case "DateField":
                case "TextArea":
                case "TriggerField":
                    TextBox tfb = (TextBox)ctrl;
                    tfb.Text = tfb.Text.Replace(required, "");   //避免重複加上flag，先移除之
                    if (flag)
                        tfb.Text = required + tfb.Text;
                    break;
            }
        }

        #region 檢核

        /// <summary>
        /// 檢查物件是否為空值
        /// 如為空值則丟出exception
        /// </summary>
        /// <param name="ctrl"></param>
        public static void EnsureObjectNotEmpty(Control ctrl, string fieldLabel = null)
        {
            if (ctrl != null)
            {
                switch (ctrl.GetType().ToString())
                {
                    case "TextField":
                    case "TextArea":
                    case "NumberField":
                    case "ComboBox":
                    case "TriggerField":
                        TextBox tf = (TextBox)ctrl;
                        if (tf.Text.Equals(string.Empty))
                        {
                            if (string.IsNullOrEmpty(fieldLabel))
                                fieldLabel = GetLabelText(tf.Text);

                            throw new Exception(Msg.EmptyMsgContent(fieldLabel));
                        }
                        break;

                    case "DateField":
                        TextBox df = (TextBox)ctrl;
                        if (df.Text.Equals(string.Empty))
                        {
                            if (string.IsNullOrEmpty(fieldLabel))
                                fieldLabel = GetLabelText(df.Text);

                            //if (!string.IsNullOrEmpty(df.RawText))
                            //    throw new Exception(Msg.IncorrectFormatMsgContent(fieldLabel));
                            //else
                            //    throw new Exception(Msg.EmptyMsgContent(fieldLabel));
                        }
                        break;

                    default:
                        //logger.Error("不支援的物件類型" + ctrl.ID);
                        break;
                }
            }
        }

        /// <summary>
        /// 檢核容器內有 * 的必填欄位是否為空值，如為空值則丟出exception
        /// hidden欄位不做檢核，如有特殊需求請使用ensureObjectNotEmpty()
        ///
        /// A client side hidden property is not submitted to server automatically. So, no way to maintain a server side Hidden property.
        /// 因server端無法取得contorl hidden屬性，因此無法進行判斷
        /// </summary>
        /// <param name="cont">要檢核的容器</param>
        /// <param name="includeHiddenObj">是否要檢核隱藏物件</param>
        public static void EnsureContainerObjectNotEmpty(Control ctrl)
        {
            if (ctrl == null)
                return;

            switch (ctrl.GetType().ToString())
            {
                case "Ext.Net.TextField":
                case "Ext.Net.TextArea":
                case "Ext.Net.NumberField":
                case "Ext.Net.MultiCombo":
                case "Ext.Net.ComboBox":
                case "Ext.Net.TriggerField":
                    TextBox tf = (TextBox)ctrl;
                    if (tf.Text.Equals(string.Empty) && IsRequiredField(tf.Text) && !tf.Visible)
                        throw new Exception(Msg.EmptyMsgContent(GetLabelText(tf.Text)));
                    break;

                case "Ext.Net.DateField":
                    TextBox df = (TextBox)ctrl;
                    if (df.Text.Equals(string.Empty) && IsRequiredField(df.Text) && !df.Visible)
                    {
                        //if (!string.IsNullOrEmpty(df.RawText))
                        //    throw new Exception(Msg.IncorrectFormatMsgContent(GetLabelText(df.FieldLabel)));
                        //else
                        //    throw new Exception(Msg.EmptyMsgContent(GetLabelText(df.FieldLabel)));
                    }
                    break;

                default:
                    foreach (Control child in ctrl.Controls)
                        EnsureContainerObjectNotEmpty(child);
                    break;
            }
        }

        //確保物件內容值不相等於val
        public static void EnsureObjectNotEqualToVal(Control ctrl, object[] val)
        {
            if (val == null || val.Length == 0)
                return;

            if (ctrl is TextBox)
            {
                TextBox tfb = (TextBox)ctrl;
                foreach (object s in val)
                {
                    //if (ConvertUtil.ToString(s).Equals(tfb.RawText))
                    //    throw new Exception(CommonUtil.GetObjectLabel(tfb) + "不可為" + tfb.RawText);
                }
            }
            else
            {
                throw new Exception("不支援的物件類型" + ctrl.ID);
            }
        }

        //確保dfEnd時間大於dfBeg
        public static void CompareDateFields(TextBox dfBeg, TextBox dfEnd, string dateLabel = null)
        {
            if (dfBeg.Text.Equals(string.Empty) || dfEnd.Text.Equals(string.Empty))
                return;

            if (DateTime.Parse(dfBeg.Text).CompareTo(DateTime.Parse(dfEnd.Text)) > 0)
            {
                if (string.IsNullOrEmpty(dateLabel))
                    throw new Exception(GetObjectLabel(dfEnd) + "不可早於" + GetObjectLabel(dfBeg));
                else
                    throw new Exception(dateLabel + "迄日不可早於起日");
            }
        }

        //以<font color=red>*</font>來判斷是否為必填欄位
        private static bool IsRequiredField(string label)
        {
            bool b = false;
            if (!string.IsNullOrEmpty(label))
            {
                if (label.ToLower().IndexOf(Resources.REQUIRE_FLAG) != -1 || label.ToLower().IndexOf(Resources.REQUIRE_FLAG) != -1)
                    b = true;
            }
            return b;
        }

        #endregion 檢核

        //將Container下控制項轉成Dictionary<ctrlName, ctrlValue>
        public static void ContainerToDictionary(Control ctrl, Dictionary<string, string> dic)
        {
            foreach (Control c in ctrl.Controls)
            {
                if (c is TextBox)
                    dic.Add(c.ID, CommonUtil.GetObjectValue(c));
                else
                    CommonUtil.ContainerToDictionary(c, dic);
            }
        }

        #endregion 控制項操作
        
        /// <summary>
        /// 根據傳入的Dictionary組出標準HTTP URL字串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public static string BuildQueryString(string url, Dictionary<string, string> para)
        {
            //移除不正常結尾
            if (url.EndsWith("?") || url.EndsWith("&"))
                url = url.Remove(url.Length - 1);

            if (url.IndexOf('?') == -1)
                url += "?";
            else
                url += "&";

            //組合Para字串
            foreach (KeyValuePair<string, string> item in para)
                url += item.Key + "=" + item.Value + "&";
            if (para.Count > 0)
                url = url.Remove(url.Length - 1);

            return url;
        }

        //導頁
        public void RedirectToPrg()
        {
            //TODO
        }
    }
}
