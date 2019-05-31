using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUBTransfer
{
    public class FieldModule
    {
        private bool _Selected;
        private string _FieldName;
        private string _MS950_FieldName;
        private string _DataType;
        private string _DataLength;

        /// <summary>
        /// 欄位建構子
        /// </summary>
        /// <param name="selected"></param>
        /// <param name="fieldname"></param>
        public FieldModule(bool selected, string fieldname, string datatype, string datalength)
        {
            try
            {
                Selected = selected;
                FieldName = fieldname;
                _DataType = datatype;
                _DataLength = datalength;
                _MS950_FieldName = "MS950_" + fieldname;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; }
        }
        public string FieldName
        {
            get { return _FieldName; }
            set { _FieldName = value; }
        }
        public string MS950_FieldName
        {
            get { return _MS950_FieldName; }
        }
        public string DataType
        {
            get { return _DataType; }
        }
        public string DataLength
        {
            get { return _DataLength; }
        }
    }
}
