using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace MSDAO
{
    public class GeneralDao
    {
        //private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _connString = null;
        public SqlConnection _conn = null;
        public SqlTransaction _trans = null;
        public string Server = string.Empty;
        public string DBName = string.Empty;
        public string ID = string.Empty;
        public string Pwd = string.Empty;

        public GeneralDao(string sConn)
        {
            _connString = sConn;
        }

        public GeneralDao()
        {
            _connString =
                "Data Source=localhost;Initial Catalog=MOA;Persist Security Info=True;User ID=sa;Password=sa;Max Pool Size=5;Connection Timeout=600";
        }
        /// <summary>
        /// 初始連線字串
        /// </summary>
        /// <param name="server">主機位址</param>
        /// <param name="db">資料庫名稱</param>
        /// <param name="id">使用者名稱</param>
        /// <param name="pwd">使用者密碼</param>
        public GeneralDao(string server, string db, string id, string pwd)
        {
            _connString =
                @"Data Source="+server+";Initial Catalog="+db+";Persist Security Info=True;User ID="+id+";Password="+pwd+";Max Pool Size=5";
            Server = server;
            DBName = db;
            ID = id;
            Pwd = pwd;
        }

        public string ConnectionString { get { return _connString; } }
        /// <summary>
        /// 開啟資料庫連線
        /// </summary>
        public void CreateConnection()
        {
            try
            {
                if ((_conn == null))
                {
                    _conn = new SqlConnection(_connString);
                    _conn.Open();
                }
                else if (_conn.State == ConnectionState.Closed)
                {
                    _conn.Open();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 關閉資料庫連線
        /// </summary>
        public void CloseConnection()
        {
            if (((_conn != null)) && (_conn.State == ConnectionState.Open))
            {
                _conn.Close();
                _conn.Dispose();
            }
            _conn = null;
        }

        /// <summary>
        /// 資料庫交易開始
        /// </summary>
        public void BeginTrans()
        {
            try
            {
                CreateConnection();
                _trans = _conn.BeginTransaction();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 資料庫交易執行
        /// </summary>
        public void Commit()
        {
            try
            {
                if (_trans == null)
                {
                    throw new Exception("transaction為null");
                }
                _trans.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 資料庫交易回復
        /// </summary>
        public void Rollback()
        {
            try
            {
                if (_trans != null)
                    _trans.Rollback();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 執行STATEMENT
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objCommand"></param>
        /// <returns></returns>
        public int Execute(string sql, TaSqlCommand objCommand = null)
        {
            try
            {
                if (objCommand == null)
                    objCommand = new TaSqlCommand();

                CreateConnection();
                objCommand.DbCmd.CommandText = sql;
                objCommand.DbCmd.Connection = this._conn;
                objCommand.DbCmd.Transaction = this._trans;
                int c = objCommand.DbCmd.ExecuteNonQuery();

                return c;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_trans == null) CloseConnection();
            }
        }

        #region Query
        public DataTable QueryTables(GeneralDao dao)
        {
            try
            {
                DBName = dao.DBName;
                //預設將所有varchar型態欄位都納入轉碼
                //GeneralDao dao = new GeneralDao("139.223.24.61", "moa", "sysadm", "sysadm");                
                string strSql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG = '" + DBName + "'";
                return dao.QueryForDataTable(strSql);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objCommand"></param>
        /// <returns></returns>
        public DataTable QueryForDataTable(string sql, TaSqlCommand objCommand = null)
        {
            try
            {
                if (objCommand == null)
                    objCommand = new TaSqlCommand();

                // 實作分頁
                if (objCommand.IsPaging())
                {
                    // ORACLE ROWNUM從1開始
                    sql = @"SELECT * FROM (SELECT ROWNUM AS ROWN, ORITBL.* FROM (" + sql + @") ORITBL)
                             WHERE ROWN >= " + ((objCommand.PageIdx * objCommand.PageSize) - (objCommand.PageSize - 1)) +
                          @" AND ROWN <= " + (objCommand.PageIdx * objCommand.PageSize);
                }

                CreateConnection();
                objCommand.DbCmd.CommandText = sql;
                objCommand.DbCmd.Connection = this._conn;
                objCommand.DbCmd.Transaction = this._trans;
                DataSet myDataSet = new DataSet();
                SqlDataAdapter myAdapter = new SqlDataAdapter(objCommand.DbCmd);
                myAdapter.Fill(myDataSet);

                return myDataSet.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_trans == null)
                    CloseConnection();
            }
        }
        /// <summary>
        /// 使用VO取出DataTable
        /// </summary>
        /// <param name="wvo"></param>
        /// <returns></returns>
        public DataTable QueryForDataTable(BaseVO wvo)
        {
            TaSqlCommand objCommand = new TaSqlCommand();
            SqlHelper.SetParametersByVO(wvo, objCommand.DbCmd);

            return this.QueryForDataTable(SqlHelper.GetSelectString(wvo), objCommand);
        }

        /// <summary>
        /// 使用SqlCommand取出DataTable
        /// 取出DataTable轉成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="objCommand"></param>
        /// <returns></returns>
        public List<T> QueryForList<T>(string sql, TaSqlCommand objCommand = null) where T : BaseVO, new()
        {
            DataTable dt = QueryForDataTable(sql, objCommand);
            return DataTableToList<T>(dt);
        }

        /// <summary>
        /// 使用vo取出DataTable
        /// 取出DataTable轉成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wvo"></param>
        /// <returns></returns>
        public List<T> QueryForList<T>(BaseVO wvo) where T : BaseVO, new()
        {
            DataTable dt = QueryForDataTable(wvo);
            return DataTableToList<T>(dt);
        }

        /// <summary>
        /// 執行SQL，回傳AGGREGATE VALUE
        /// 取出int的欄位值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objCommand"></param>
        /// <returns></returns>
        public int QueryForInt(string sql, TaSqlCommand objCommand = null)
        {
            try
            {
                if (objCommand == null)
                { objCommand = new TaSqlCommand(); }
                DataTable dt = QueryForDataTable(sql, objCommand);
                return dt.Rows.Count > 0 ? ConvertUtil.ToInt(dt.Rows[0][0].ToString()) : -1;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 取出long的欄位值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objCommand"></param>
        /// <returns></returns>
        public long QueryForLong(string sql, TaSqlCommand objCommand = null)
        {
            try
            {
                if (objCommand == null)
                { objCommand = new TaSqlCommand(); }
                return ConvertUtil.ToInt(QueryForDataTable(sql, objCommand).Rows[0][0].ToString());
            }
            catch (Exception)
            {
                return 0;
            }

        }

        /// <summary>
        /// 取出string的欄位值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objCommand"></param>
        /// <returns></returns>
        public string QueryForString(string sql, TaSqlCommand objCommand = null)
        {
            try
            {
                if (objCommand == null)
                { objCommand = new TaSqlCommand(); }
                return ConvertUtil.ToString(QueryForDataTable(sql, objCommand).Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// 取出double的欄位值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="objCommand"></param>
        /// <returns></returns>
        public double QueryForDouble(string sql, TaSqlCommand objCommand = null)
        {
            try
            {
                if (objCommand == null)
                { objCommand = new TaSqlCommand(); }
                return ConvertUtil.ToDouble((object)QueryForDataTable(sql, objCommand).Rows[0][0].ToString());
            }
            catch (Exception)
            {
                return 0;
            }

        }

        /// <summary>
        /// 取出單一欄位值
        /// 取到的資料會是第一列的特定欄位名稱的值
        /// </summary>
        /// <param name="sql">T-SQL</param>
        /// <param name="fieldname">Field Name</param>
        /// <param name="objCommand">SQL Command</param>
        /// <returns></returns>
        public string QueryForSingleField(string sql, string fieldname, TaSqlCommand objCommand = null)
        {
            try
            {
                if (objCommand == null)
                { objCommand = new TaSqlCommand(); }
                DataTable dt = QueryForDataTable(sql, objCommand);
                return dt.Rows.Count > 0 ? dt.Rows[0][fieldname].ToString() : null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        /// <summary>
        /// 使用VO內資料進行新增
        /// </summary>
        /// <param name="vo"></param>
        /// <returns></returns>
        public int Insert(BaseVO vo)
        {
            TaSqlCommand objCommand = new TaSqlCommand();
            SqlHelper.SetParametersByVO(vo, objCommand.DbCmd);

            return this.Execute(SqlHelper.GetInsertString(vo), objCommand);
        }

        /// <summary>
        /// 使用VO內資料進行刪除
        /// </summary>
        /// <param name="wvo"></param>
        /// <returns></returns>
        public int Delete(BaseVO wvo)
        {
            TaSqlCommand objCommand = new TaSqlCommand();
            SqlHelper.SetParametersByVO(wvo, objCommand.DbCmd);

            return this.Execute(SqlHelper.GetDeleteString(wvo), objCommand);
        }

        /// <summary>
        /// 使用VO內資料及WVO條件進行更新
        /// </summary>
        /// <param name="vo"></param>
        /// <param name="wvo"></param>
        /// <returns></returns>
        public int Update(BaseVO vo, BaseVO wvo)
        {
            TaSqlCommand objCommand = new TaSqlCommand();
            SqlHelper.SetParametersByVO(vo, wvo, objCommand.DbCmd);

            return this.Execute(SqlHelper.GetUpdateString(vo, wvo), objCommand);
        }

        /// <summary>
        /// 使用VO內資料及指定欄位條件進行更新
        /// </summary>
        /// <param name="vo"></param>
        /// <param name="pkField"></param>
        /// <param name="pkVal"></param>
        /// <returns></returns>
        public int Update(BaseVO vo, string pkField, object pkVal)
        {
            string sql = SqlHelper.GetUpdateString(vo) + " where " + pkField + " = :F_" + pkField;

            TaSqlCommand objCommand = new TaSqlCommand();
            SqlHelper.SetParametersByVO(vo, objCommand.DbCmd);
            objCommand.DbCmd.Parameters.AddWithValue(":F_" + pkField, pkVal);

            return this.Execute(sql, objCommand);
        }

        /// <summary>
        /// 取出DataTable回傳List資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<T> DataTableToList<T>(DataTable dt) where T : BaseVO, new()
        {
            string[] excludeProp = { "TargetTable", "OrderBy", "UpdateString" };
            DataColumnCollection dtColumns = dt.Columns;

            List<T> tmpList = new List<T>();
            foreach (DataRow tmpRow in dt.Rows)
            {
                T vo = new T();
                Type objType = vo.GetType();
                PropertyInfo[] props = objType.GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    if (excludeProp.Contains(prop.Name))
                        continue;

                    if (!dtColumns.Contains(prop.Name))
                        continue;

                    if (prop.PropertyType.IsGenericType &&
                        prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(System.Int64))
                        {
                            prop.SetValue(vo, ConvertUtil.ToNLong(tmpRow[prop.Name]),null);
                        }
                        else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(System.Int32))
                        {
                            prop.SetValue(vo, ConvertUtil.ToNInt(tmpRow[prop.Name]), null);
                        }
                        else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(System.Double))
                        {
                            prop.SetValue(vo, ConvertUtil.ToNDouble(tmpRow[prop.Name]), null);
                        }
                        else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(System.DateTime))
                        {
                            if (typeof(System.DateTime) == tmpRow[prop.Name].GetType())
                                prop.SetValue(vo, tmpRow[prop.Name], null);
                            //if (typeof(System.DBNull) == tmpRow[prop.Name].GetType())
                            //    prop.SetValue(vo, null);
                        }
                    }
                    else
                    {
                        prop.SetValue(vo, Convert.ChangeType(tmpRow[prop.Name], prop.PropertyType), null);
                    }
                }

                tmpList.Add(vo);
            }
            return tmpList;
        }

        #region 回傳不同資料格式
        /// <summary>
        /// @desc : query database by SqlDataAdapter and DataTable
        /// @return : json string by Json.Net
        /// @param : startDate, e.g 1960-06-01
        /// @param : endDate, e.g. 1960-06-20
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        internal string FetchDbDataByDateRetJson(string startDate, string endDate)
        {
            // 取得 App.config 內的連接字串
            string connectionString = _connString;

            // save json string prepared as returned one
            String jsonData = "";

            // connect to the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                // prepare the sql syntax
                // use parameter-based syntax to prevent SQL-injection attacks
                string sqlStr = "select * from dbo.employees where birth_date >= @StartDate and birth_date <= @EndDate;";
                SqlCommand sqlCmd = new SqlCommand(sqlStr, conn);
                sqlCmd.Parameters.AddWithValue("StartDate", startDate);
                sqlCmd.Parameters.AddWithValue("EndDate", endDate);

                // use sql data adapter to query the database
                SqlDataAdapter sda = new SqlDataAdapter(sqlCmd);

                try
                {
                    // start connection
                    conn.Open();

                    // query data and load into a data table
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    conn.Close();

                    // try to transfer SQL data to the data table by the JSON.NET
                    // it might use Json.Net library
                    jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
                }
                catch
                {
                    // use a dictionary to save status
                    Dictionary<string, string> statusLog = new Dictionary<string, string>();
                    statusLog.Add("status", "Fetching SQL data is failure.");
                    jsonData = JsonConvert.SerializeObject(statusLog, Newtonsoft.Json.Formatting.Indented);
                }

            }
            return jsonData;
            #region 使用方法
            //// generate a object
            //GeneralDao tsqlquery = new GeneralDao();

            //
            //// Example.1 : fetch sql data into a datatable and transform the data into the json string
            //
            //System.Console.WriteLine(tsqlquery.fetchDBDataByDateRetJson("1962-06-01", "1962-06-20"));
            #endregion
        }

        /// <summary>
        /// @desc : fetch each row of data to find the detail information, this example would save emp_no
        /// @return : a list to employee whose first name is the same with parameter
        /// @param : getFirstName is the name for ssearching
        /// </summary>
        /// <param name="getFirstName"></param>
        /// <returns></returns>
        internal List<string> FetchDbDataByNameRetList(string getFirstName)
        {
            string connectionString = _connString;

            // Return as enum list
            List<string> objectList = new List<string> { };

            // connect to the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                // prepare the sql syntax
                string sqlStr = "select * from dbo.employees where first_name = @first_name;";
                SqlCommand sqlCmd = new SqlCommand(sqlStr, conn);
                sqlCmd.Parameters.AddWithValue("first_name", getFirstName);

                try
                {
                    // start connection
                    conn.Open();

                    // use sql data reader to fetch data row by row
                    SqlDataReader reader = sqlCmd.ExecuteReader();

                    // search all rows
                    while (reader.Read())
                    {
                        // use index, e.g. reader[0], reader[1], to get corresponding contents
                        objectList.Add(string.Format("{0}", reader[0]));
                    }
                    reader.Close();
                    conn.Close();
                }
                catch
                {
                    objectList = null;
                }

            }
            return objectList;
            #region 使用方法
            //// generate a object
            //GeneralDao tsqlquery = new GeneralDao();

            //// Example.2 : fetch sql data row by row

            //List<string> ttlObjs = tsqlquery.fetchDBDataByNameRetList("Georgi");
            //int ttlEmts = ttlObjs.Count;
            //for (int eachEmployee = 0; eachEmployee < ttlEmts; eachEmployee++)
            //{
            //    System.Console.WriteLine(string.Format("Item No. {0} is {1}.", eachEmployee, ttlObjs[eachEmployee]));
            //}
            #endregion
        }

        /// <summary>
        /// @desc : fetch the sql data into a datatable in dataset, and modified the data into another datatable
        /// @return : a dataset conserving two datatables, one is querying from sql server, second is the modified datatable
        /// @param : getFirstName is the name for ssearching
        /// </summary>
        /// <param name="getFirstName"></param>
        /// <returns></returns>
        internal DataSet fetchDBDataByDateAndCal(string getFirstName)
        {
            string connectionString = _connString;

            // Return as a complete data entry
            Dictionary<string, string> objectList = new Dictionary<string, string> { };

            // save returned data
            DataSet ret_ds = new DataSet();

            // connect to the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                // prepare the sql syntax
                string sqlStr = "select * from dbo.employees where first_name = @first_name;";
                SqlCommand sqlCmd = new SqlCommand(sqlStr, conn);
                sqlCmd.Parameters.AddWithValue("first_name", getFirstName);

                try
                {

                    // start connection
                    conn.Open();

                    // query the result and save into the table
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                    da.Fill(ds);
                    da.Dispose();

                    // pick up the most earilest to the company but the youngest one
                    ds.Tables[0].DefaultView.Sort = "hire_date asc, birth_date desc";

                    // create a new table and get the sorting result
                    DataTable newdt = ds.Tables[0].DefaultView.ToTable();
                    newdt.TableName = "sortOrder";

                    // add a new column and give a new id
                    newdt.Columns.Add("id", typeof(string));
                    for (int idIndex = 0; idIndex < newdt.Rows.Count; idIndex++)
                    {
                        newdt.Rows[idIndex]["id"] = String.Format("{0}", idIndex);
                    }

                    // copy tables in the origin dataset into the returned dataset
                    for (int dt_index = 0; dt_index < ds.Tables.Count; dt_index++)
                    {
                        DataTable dt_add = ds.Tables[dt_index].Copy();
                        ds.Tables.RemoveAt(dt_index);

                        // notice it must be rename to prevent override the table by function add()
                        dt_add.TableName = string.Format("{0}", dt_index);

                        ret_ds.Tables.Add(dt_add);
                    }

                    // sort table must proceed again 
                    ret_ds.Tables["0"].DefaultView.Sort = "hire_date asc, birth_date desc";

                    // add the second table
                    ret_ds.Tables.Add(newdt);

                    conn.Close();
                }
                catch
                {
                    objectList = null;
                }

            }
            return ret_ds;
            #region 使用方法
            // generate a object
            //GeneralDao tsqlquery = new GeneralDao();


            //// Example.3 : DataSet conserving two data tables, one is origin and the other is modified table

            //// use defaultview to get sorting results
            //DataSet ds = tsqlquery.fetchDBDataByDateAndCal("Georgi");

            //// use the DefaultView to see the sorting result
            //for (int i = 0; i < ds.Tables.Count; i++)
            //{
            //    System.Console.WriteLine(ds.Tables[i].TableName);
            //}

            //// show DefaultView.Sort result
            //for (int eachEmplopee = 0; eachEmplopee < ds.Tables["0"].DefaultView.Count; eachEmplopee++)
            //{
            //    if (eachEmplopee > 10) { break; }
            //    System.Console.WriteLine(string.Format("Item No. {0}, {1}, {2}.", ds.Tables["0"].DefaultView[eachEmplopee]["emp_no"], ds.Tables["0"].DefaultView[eachEmplopee]["hire_date"], ds.Tables["0"].DefaultView[eachEmplopee]["birth_date"]));
            //}

            //// use modified table, adding a new column, to show the result
            //for (int eachEmplopee = 0; eachEmplopee < ds.Tables[1].Rows.Count; eachEmplopee++)
            //{
            //    if (eachEmplopee > 10) { break; }
            //    System.Console.WriteLine(string.Format("Item No. {0}, {1}, {2}, {3}.", ds.Tables["sortOrder"].Rows[eachEmplopee]["emp_no"], ds.Tables["sortOrder"].Rows[eachEmplopee]["hire_date"], ds.Tables["sortOrder"].Rows[eachEmplopee]["birth_date"], ds.Tables["sortOrder"].Rows[eachEmplopee]["id"]));
            //}
            #endregion
        }

        #endregion
    }
}
