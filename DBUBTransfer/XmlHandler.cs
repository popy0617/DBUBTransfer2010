using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DBUBTransfer
{
    public class XmlHandler
    {
        private string _FileName = String.Empty;
        private XmlDocument _XmlDocument = new XmlDocument();
        private XmlDeclaration _Declaration;
        private XmlDocumentType _DocType;

        private List<XmlEntity> _XmlEntity = new List<XmlEntity>();
        private List<XmlNotation> _XmlNotation = new List<XmlNotation>();
        #region NodeType

        #endregion
        /// <summary>
        /// 物件建構函式二
        /// </summary>
        /// <param name="pFileName">檔案名稱</param>
        public XmlHandler()
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }//end of constructor.
        /// <summary>
        /// 物件建構函式二
        /// </summary>
        /// <param name="pFileName">檔案名稱</param>
        public XmlHandler(string pFileName)
        {
            try
            {
                _FileName = pFileName;
            }
            catch (Exception ex)
            {
            }

        }//end of constructor.

        /// <summary>
        /// 將 Xml 檔案資料載入 XmlDocument 物件
        /// </summary>
        /// <returns>成功傳回 True, 否則為 False</returns>
        public bool LoadFile()
        {
            bool oResult = true;
            try
            {
                _XmlDocument.Load(_FileName);
            }//end of try.
            catch (Exception ex)
            {
                oResult = false;
            }//end of catch.
            return oResult;
        }//end of LoadFile.

        public XmlDocument LoadRoot()
        {
            XmlDocument oResult = new XmlDocument();
            try
            {
                oResult.Load(_FileName);
            }//end of try.
            catch (Exception ex)
            {

            }//end of catch.
            return oResult;
        }
        /// <summary>
        /// 取得根目錄的所有子節點物件列表
        /// </summary>
        /// <returns></returns>
        public XmlNodeList GetRootXmlNodes()
        {
            return _XmlDocument.ChildNodes[1].ChildNodes;
        }//end of GetAllXmlNodes.

        /// <summary>
        /// 釋放 XML 文件資源
        /// </summary>
        public void Close()
        {
            _XmlDocument = null;
            //GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
        }//end of Close.

        public XmlNode SelectSingleNode(string path)
        {
            try
            {
                return _XmlDocument.SelectSingleNode(path);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public XmlNode SelectSingleNode(string path, XmlNamespaceManager xnm)
        {
            try
            {
                return _XmlDocument.SelectSingleNode(path, xnm);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string RebuildContent()
        {
            try
            {
                string strReturn = string.Empty;

                //Declaration
                strReturn += _Declaration.ToString();
                //DocType
                strReturn += _DocType.ToString();
                //Entity
                foreach (XmlEntity item in _XmlEntity)
                {
                    strReturn += item.ToString();
                }
                //Notation
                //Node Content
                strReturn += _XmlDocument.ToString();

                return strReturn;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

    #region Class XmlTagConvert
    /// <summary>
    /// 標簽轉換抽象物件
    /// </summary>
    public abstract class XmlTagConvert : XPathNode
    {

        #region Class Member
        protected XmlNode _XmlNode = null;
        public XPathNode RootNode = null;
        protected static string _SecretLevel = string.Empty;
        protected static string _DecodeLimitation = string.Empty;
        public XmlTagConvert(string pName)
            : base(pName)
        {
        }//end of constructor.
        public XmlTagConvert(string pName, string pValue)
            : base(pName, pValue)
        {
        }//end of constructor.

        #endregion

        #region Method Member

        /// <summary>
        /// 取得段落父節點
        /// </summary>
        /// <param name="pCurrentTagLength"></param>
        /// <param name="pNode1"></param>
        /// <param name="pNode2"></param>
        /// <param name="pNode3"></param>
        /// <param name="pNode4"></param>
        /// <param name="pNode5"></param>
        /// <returns></returns>
        private XPathNode GetLastNode(int pCurrentTagLength, XPathNode pNode1, XPathNode pNode2, XPathNode pNode3, XPathNode pNode4, XPathNode pNode5)
        {
            XPathNode oResult = null;
            switch (pCurrentTagLength)
            {
                case 2:
                    if (pNode1 != null) oResult = pNode1;
                    break;
                case 3:
                    if (pNode2 != null) oResult = pNode2;
                    break;
                case 4:
                    if (pNode3 != null) oResult = pNode3;
                    break;
                case 5:
                    if (pNode4 != null) oResult = pNode4;
                    break;
            }//end of switch.
            return oResult;
        }//end of GetLastNode().

        #endregion

    }//end of class.
    #endregion
    #region Class XPathNode
    /// <summary>
    /// XPath 字串節點 -- XPathNode
    /// </summary>
    public class XPathNode
    {

        #region Data Member
        /// <summary>
        /// 節點名稱
        /// </summary>
        public string Name = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Value = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public List<XPathNode> Attributes = new List<XPathNode>();
        /// <summary>
        /// 
        /// </summary>
        public List<XPathNode> SubNodes = new List<XPathNode>();
        /// <summary>
        /// 紀錄單一節點是否無值( 顯示如<密等/>)
        /// </summary>
        public bool NoValue = false;
        #endregion

        #region Method Member

        /// <summary>
        /// XPathNode 物件建構函式
        /// </summary>
        /// <param name="pName">節點名稱</param>
        public XPathNode(string pName)
        {
            try
            {
                Name = pName;
            }
            catch (Exception ex)
            {
            }
        }//end of constructor.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="pValue"></param>
        public XPathNode(string pName, string pValue)
        {
            Name = pName;
            Value = pValue;
            if (Value == string.Empty)
                NoValue = true;
        }//end of constructor.

        /// <summary>
        /// 取得兩層標簽結構的物件
        /// </summary>
        /// <param name="Layer1Name">第一層標簽字串</param>
        /// <param name="Layer2Name">第二層標簽字串</param>
        /// <returns>XPathNode 物件</returns>
        public static XPathNode CreateEntity(string Layer1Name, string Layer2Name)
        {
            try
            {
                XPathNode oXPathNode = new XPathNode(Layer1Name);
                oXPathNode.AddSubNode(new XPathNode(Layer2Name));
                return oXPathNode;
            }
            catch (Exception ex)
            {
                return null;
            }
        }//end of GetEntity.

        /// <summary>
        /// 取得標簽結構的物件
        /// </summary>
        /// <param name="Layer1Name">第一層標簽字串</param>
        /// <param name="Layer2Name">第二層標簽字串</param>
        /// <returns>XPathNode 物件</returns>
        public static XPathNode CreateEntity(string xpath)
        {
            try
            {
                XPathNode oXPathNode = new XPathNode(xpath);
                return oXPathNode;
            }
            catch (Exception ex)
            {
                return null;
            }
        }//end of GetEntity.
        /// <summary>
        /// 加入單一 XPathNode 節點屬性
        /// </summary>
        /// <param name="pAttributeName">屬性名稱字串</param>
        /// <param name="pAttributeValue">屬性值字串</param>
        public void AddAttribute(string pAttributeName, string pAttributeValue)
        {
            bool oAddFlag = true;
            foreach (XPathNode oAttributes in Attributes)
            {
                if (oAttributes.Name == pAttributeName) oAddFlag = false;
            }//end of foreach.
            if (oAddFlag)
            {
                Attributes.Add(new XPathNode(pAttributeName, pAttributeValue));
            }//end of if.
        }//end of AddAttribute.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAttribute"></param>
        public void AddAttribute(XPathNode pAttribute)
        {
            AddAttribute(pAttribute.Name, pAttribute.Value);
        }//end of AddAttribute.

        /// <summary>
        /// 加入單一 XPathNode 子節點並且回傳該子節點物件
        /// </summary>
        /// <param name="pSubNode">XPathNode 物件</param>
        /// <returns>XPathNode 物件</returns>
        public XPathNode AddSubNode(XPathNode pSubNode)
        {
            if (SubNodes.IndexOf(pSubNode) == -1)
            {
                SubNodes.Add(pSubNode);
            }//end of if.
            return pSubNode;
        }//end of AddSubNode.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pNameArray"></param>
        public void AddMultiLayerSubNode(string[] pNameArray)
        {
            XPathNode oParentNode = this;
            for (int i = 0; i < pNameArray.Length; i++)
            {
                XPathNode oChildNode = new XPathNode(pNameArray[i]);
                oParentNode.AddSubNode(oChildNode);
                oParentNode = oChildNode;
            }//end of for.
        }//end of AddMultiLayerSubNode.

        /// <summary>
        /// 組合 Xml 字串
        /// </summary>
        /// <param name="pBreakLine">子節點間是否要斷行</param>
        /// <returns></returns>
        public string ToXmlString(bool pBreakLine)
        {
            bool oHaveEndFlag = false;
            string oResult = "<" + Name;
            int oPos = 0;
            foreach (XPathNode oAttribute in Attributes)
            {
                oResult += " " + oAttribute.Name + "=\"" + oAttribute.Value + "\"";
            }//end of foreach.
            if (NoValue)
            {
                oHaveEndFlag = true;
                oResult += "/";
            }
            oResult += ">" + Value;
            foreach (XPathNode oSubNode in SubNodes)
            {
                if (pBreakLine && oPos == 0) oResult += Environment.NewLine + "    ";
                oResult += oSubNode.ToXmlString(pBreakLine);
                if (pBreakLine)
                {
                    oResult += Environment.NewLine;
                    if (oPos != SubNodes.Count - 1) oResult += "    ";
                }
                oPos++;
            }//end of foreach.
            if (!oHaveEndFlag)
                oResult += "</" + Name + ">";
            return oResult;
        }//end of ToXmlString.

        /// <summary>
        /// 組合 Xml 字串 (提高一個節點，附件tag使用)
        /// </summary>
        /// <returns></returns>
        public string ToXmlUpString()
        {
            string oResult = string.Empty;
            int oPos = 0;
            foreach (XPathNode oAttribute in Attributes)
            {
                oResult += " " + oAttribute.Name + "=\"" + oAttribute.Value + "\"";
            }//end of foreach.
            foreach (XPathNode oSubNode in SubNodes)
            {
                oPos++;
                oResult += oSubNode.ToXmlString(false);
                if (oPos != SubNodes.Count) oResult += Environment.NewLine;
            }//end of foreach.
            return oResult;
        }//end of ToXmlString.

        /// <summary>
        ///  組合節點字串: Sample -- //核判區分列表//核判區分選項[@名稱='官長_伺服器' and @排列='單排']
        /// </summary>
        /// <returns>xPath 字串</returns>
        public override string ToString()
        {
            string oResult = "//" + Name;
            if (Attributes.Count > 0)
            {
                oResult += "[";
                for (int i = 0; i < Attributes.Count; i++)
                {
                    if (i > 0) oResult += " and ";
                    oResult += "@" + Attributes[i].Name + "='" + Attributes[i].Value + "'";
                }//end of for.
                oResult += "]";
            }//end of if.
            foreach (XPathNode oSubNode in SubNodes)
            {
                oResult += oSubNode.ToString();
            }//end of foreach.
            return oResult;
        }//end of ToString.
        #endregion

    }//end of class.
    #endregion
}
