using System.Collections.Generic;

namespace MvcLib.Tools
{

    public interface ITableDom
    {
        string TableCss { get; set; }
        string TableHeadDom { get; set; }
        string TableRowDom { get; set; }
        string TableFooter { get; set; }
        string PagerDom { get; set; }
    }

    public class TableDom : ITableDom
    {
        public string TableCss { get; set; }
        public string TableHeadDom { get; set; }
        public string TableRowDom { get; set; }
        public string TableFooter { get; set; }
        public string PagerDom { get; set; }
    }

    public interface ITableVm
    {      
        void GenTable(string t, int ttl, int pg);       
        TableDom TableDom { get; set; }
    }



    public interface ITableCreator<T>
    {
        List<T> TableMeta { get; set; }      
        Dictionary<string,string> HeaderDic { get; }
    }
}
