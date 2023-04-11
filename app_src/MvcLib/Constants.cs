using MvcLib.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvcLib
{
    public static class Constants
    {
        #region Table row js, with the table row js function of the front end.

        public const string JsClassName = "menuController";
        public const string JsRowEdit = "rowEdit(this)";
        public const string JsRowDel = "rowDel(this)";
        public const string JsRowAdd = "rowAdd(this)";
        public const string JsHeaderCheckAll = JsClassName + ".checkAll()";
        internal static string JsSelectRow = JsClassName + ".selectRow(this)";

        public const string SavingPath = "\\upload";

        /// <summary>
        /// itemcategory key.
        /// </summary>
        public const string Splitor = "_";
        public const string Lx = "lx";
        public const string Mk = "mk";
        public const string Xk = "xk";
        public const string Bb = "bb";


        public enum OperType
        {
            Add,
            Del,
            Update,
            Query
        }   

        public enum TableButtonTypes
        {
            btns_table_pdf,
            btns_table_category,
            btns_table_testpaper_props
        }

        public static Dictionary<string,string> TableConstants
        {
            get
            {
                var dic = new Dictionary<string, string>();
                dic.Add("True", "Y");
                dic.Add("False", "N");
                dic.Add(string.Empty, "-");
                return dic;
            }
        }

        /// <summary>
        /// reset showing value in the td, True==> Y, 
        /// Category ==>1: 模块 2：学科.....
        /// </summary>
        /// <param name="type"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static string Reset_Td_Value(this string  k)
        {
            if (TableConstants.ContainsKey(k))
            {
                return TableConstants[k];
            }
            return k;
        }


        #endregion

    }
}
