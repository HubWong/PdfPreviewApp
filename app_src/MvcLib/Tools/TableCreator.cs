using MvcLib.MainContent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MvcLib.Constants;

namespace MvcLib.Tools
{

    public class TableButonCreator
    {
        public const string CheckBox = "<input class=\"check-input\" type=\"checkbox\" value=\"\" id=\"wyb_checkbox\">";
        public ButtonDom[] GenButton(TableButtonTypes type)
        {
            var buttonDoms = new List<ButtonDom>();
            ButtonDom buttonDom;
            string clsXs = "btn-sm btn-xs";
                       

            switch (type)
            {
                case TableButtonTypes.btns_table_pdf:
                    buttonDom = new ButtonDom(Constants.JsClassName, Constants.JsRowEdit, "编辑", "btn-primary " + clsXs);
                    buttonDoms.Add(buttonDom);
                    buttonDom = new ButtonDom(Constants.JsClassName, Constants.JsRowDel, "删除", "btn-danger " + clsXs);
                    buttonDoms.Add(buttonDom);
                    buttonDom = new ButtonDom("PdfPage", "cvrMgr(this)", "封面图", "btn-outline-primary " + clsXs);
                    buttonDoms.Add(buttonDom);
                    buttonDom = new ButtonDom("PdfPage", "pdfFilesPage(this)", "附件&预览", "btn-warning " + clsXs);
                    buttonDoms.Add(buttonDom);
                    break;

                case TableButtonTypes.btns_table_category:
                    buttonDom = new ButtonDom(Constants.JsClassName, Constants.JsRowEdit, "编辑", "btn-primary " + clsXs);
                    buttonDoms.Add(buttonDom);
                    buttonDom = new ButtonDom(Constants.JsClassName, Constants.JsRowDel, "删除", "btn-danger " + clsXs);
                    buttonDoms.Add(buttonDom);
                    break;
                case TableButtonTypes.btns_table_testpaper_props:
                    buttonDom = new ButtonDom("menuController", "addProp(this)", "新增", "btn-success " + clsXs);
                    buttonDoms.Add(buttonDom);
                    buttonDom = new ButtonDom(Constants.JsClassName, "delProp(this)", "删除", "btn-danger " + clsXs);
                    buttonDoms.Add(buttonDom);
                    break;
            }
            return buttonDoms.ToArray();

        }

        /// <summary>
        /// gen string of button dom
        /// </summary>
        /// <param name="btnArray"></param>
        /// <returns></returns>
        public string Gen_BtnDom(ButtonDom[] btnArray)
        {           
            StringBuilder _sb = new StringBuilder();
            if (btnArray == null || btnArray.Length == 0)
            {
                return string.Empty;
            }
            foreach (var item in btnArray)
            {
                _sb.Append(item.ToString());
            }
            return _sb.ToString();
        }
    }

    /*****************
    table creator ,these classes work together with bootstrap 5.1 to generate table with 
    css classes defined in the front end.
              by wyb 2021/9/28
     ****************/
    public class Td
    {
        public string DomClass { get; set; }
        private string _td_val;
        public Td(string value)
        {
            _td_val =value;
        }


        public Td(string value, string domCls,bool isHead=true):this(value)
        {          
            _td_val = value;
            DomClass = domCls; 
            IsHeader = isHead;
        }

        public Td(string value, string domCls, Dictionary<string,string> props, bool isHead = true):this(value,domCls,isHead)
        {
            Td_Props = props;
        }

        public static Td TdCheckBox()
        {
            return new Td(TableButonCreator.CheckBox, "checkbox_td", false);
        }

        /// <summary>
        /// get all prop dic in td  to string.
        /// </summary>
        /// <returns></returns>
        private string _Dic_props_to_string()
        {
            var sb = new StringBuilder();
            if (this.Td_Props!=null && this.Td_Props.Count>0)
            {
                foreach (var item in this.Td_Props)
                {
                    sb.Append($"{ item.Key} =\"{ item.Value} \" ");
                }
                return sb.ToString();
            }
            return string.Empty;
        }

        public string td_value { 
            get
            {
                if (IsHeader)
                {
                    string moreProps =_Dic_props_to_string();
                    return $"<th scope='col' class='{DomClass}' {moreProps}>" + _td_val + "</th>";
                }
                else
                {                 
                    
                    return $"<td class='{DomClass}'>" + Constants.Reset_Td_Value(_td_val) + "</td>";
                }
            } 
        }

        public bool IsHeader { get; private set; }
        public Dictionary<string,string> Td_Props { get; }
    }

    public class ButtonDom
    {
        public ButtonDom(string jsClsName, string jsMthd, string Txt, string cssCls)
        {
            JsClassName = jsClsName;
            JsMthod = jsMthd;
            CssName = cssCls;
            BtnText = Txt;
            
        }
        private StringBuilder stringBuilder;
        public string BtnText { get; set; }
        public string JsClassName { get; set; }
        public string JsMthod { get; set; }
        public string CssName { get; set; }    

        /// <summary>
        /// change btn to dom string 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            stringBuilder = new StringBuilder();
            stringBuilder.Append("<a");
            stringBuilder.Append($" href='javascript:;' ");
            stringBuilder.Append($" class='btn {CssName}'");
            stringBuilder.Append($" onClick='{JsClassName}.{JsMthod}'");
            stringBuilder.Append($">{BtnText}");
            stringBuilder.Append("</a>");
            return stringBuilder.ToString();
        }

    }

    public class Tr<T> where T:class
    {
        public Tr()
        {
            Sb = new StringBuilder();
            Sb.Append($"<tr onClick=\'{Constants.JsSelectRow}\'>");
            Tds = new List<Td>();
        }

        public List<Td> Tds { get; set; }
        public StringBuilder Sb { get; set; }
        public Dictionary<string,string> HeaderTitleDic {
            get {
                return Utility.GetDisplayName(typeof(T));
            }
        }

        /// <summary>
        /// <th>XX</th>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<Td> GetHeaderThs() 
        {
            var listTdHeader = new List<Td>();
            if (HeaderTitleDic != null)
            {
                var ht = new Dictionary<string,string>();
                ht.Add("onClick", Constants.JsHeaderCheckAll);
                listTdHeader.Add(new Td("全选 "+ TableButonCreator.CheckBox,"",ht,true));
                foreach (KeyValuePair<string, string> item in HeaderTitleDic)
                {
                    listTdHeader.Add(new Td(item.Value, item.Key, true));
                }              
            }
            return listTdHeader;
        }


        /// <summary>
        /// gen single row string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if(this.Tds==null || Tds.Count == 0)
            {
                return Sb.Append("</tr>").ToString();
            }         
           
            foreach (Td item in Tds)
            {
                Sb.Append(item.td_value);
            }
            Sb.Append("</tr>");
            return Sb.ToString() ;
        }

        public string ToString(Td newTd)
        {
            if (this.Tds == null || Tds.Count == 0)
            {
                return Sb.Append("</tr>").ToString();
            }

            Tds.Add(newTd);
             
            foreach (Td item in Tds)
            {
                Sb.Append(item.td_value);
            }

            Sb.Append("</tr>");
            return Sb.ToString();
        }

        /// <summary>
        ///  when get row data by header title
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="buttons"></param>
        public Tr<T> Gen_tr_by_header(T data, params ButtonDom[] buttons) 
        {           
            if (HeaderTitleDic!=null)
            {
                var dic =Utility.GetDisplayName(data, HeaderTitleDic.Keys.ToArray());
                var tr_Button = new TableButonCreator();
                Tr<T> tr = new Tr<T>();
                tr.Tds.Add(Td.TdCheckBox());
                foreach (var item in dic)
                {
                    tr.Tds.Add(new Td(Constants.Reset_Td_Value(item.Value), item.Key, false));
                }
                var btnStr = tr_Button.Gen_BtnDom(buttons);
                tr.Tds.Add(new Td(btnStr, "", false));
                return tr;
            }
            return null;
        }
    }

   
    public class TableCreator<T> :ITableCreator<T>
        where T : class
    {
        private string _tableTitle;  //table footer title

        public TableCreator(List<T> datas, params ButtonDom[] buttons)
        {
            TableMeta = datas;
            ButtonDoms = buttons;
            RowMaker = new Tr<T>();
            HeaderTds = RowMaker.GetHeaderThs();
            Action_pager = (a, b, c) => Utility.PageDom(a, b, c);
        }


        public TableCreator(List<T> datas,string tableName, params ButtonDom[] buttons):this(datas,buttons)
        {
            _tableTitle = tableName;
        }


        public Dictionary<string,string> HeaderDic 
        { 
            get {                        
                return RowMaker.HeaderTitleDic;
            }
        }
    
        public List<Td> HeaderTds
        {
            get;
        }    
      
        public int ColumnInTable         
        { 
            get {
                return HeaderTds.Count + (this.ButtonDoms!=null ? 1 : 0);
            }
        }

        public Func<string,int,int,string> Action_pager { get; set; }
        public string Pager { get; set; }
        public List<T> TableMeta { get; set; }
        public ButtonDom[] ButtonDoms { get; private set; }
        public Tr<T> RowMaker { get;private set; }

        public string GenTableHeadDom()
        {
            if (this.HeaderTds.Count>0)
            {            
                this.RowMaker.Tds = this.HeaderTds;           
            }
            return RowMaker.ToString(new Td(string.Empty, "", true));
        }

        public string GenTableFooter()
        {
            return $"<tr data-type='{this._tableTitle}'><td colspan='{this.ColumnInTable}'>{this._tableTitle}</td></tr>";
        }

        /// <summary>
        /// get pager dom   
        /// </summary>
        /// <param name="ttlRecords"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<string> GetPagerDom(string type, int pg, int ttl)
        {
            if (Action_pager != null)
            {
                int t = Utility.EnumGetDisplayIndex<categoryType>(type.Substring(0,2));
               Pager = Action_pager(t.ToString(),ttl,pg);
            }

            return Task.FromResult(Pager);          
        }

        public string EmptyDataRow
        { 
            get
            {
                return $"<tr><td class='text-center' colspan='{this.ColumnInTable}'>没有数据</td></tr>";
            }           
        }

        public Task<string> GenRowsDom()
        {         
            var stringBuilder = new StringBuilder();
            if (RowMaker.HeaderTitleDic!=null)
            {
                if (TableMeta != null && TableMeta.Count > 0) {
                    Tr<T> tr;
                    foreach (T item in TableMeta)
                    {
                        tr = RowMaker.Gen_tr_by_header(item, this.ButtonDoms);
                        if (tr != null && tr.Tds.Count > 0)
                        {
                            stringBuilder.Append(tr.ToString());
                        }
                    }
                }
                else
                {
                    stringBuilder.Append(EmptyDataRow);
                }

            }
            else
            {
                throw new Exception("table header not created");
            }
           
            return Task.FromResult(stringBuilder.ToString());
        }

    }
}
