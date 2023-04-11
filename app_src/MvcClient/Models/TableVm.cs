using MvcLib.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvcClient.Models
{
    /// <summary>
    /// table with meta data of T 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TableVm<T> : ITableVm
        where T : class
    {
        public TableVm(List<T> datas, string sidebar, Func<ButtonDom[]> funcButton, string TableCss = "table table-dark")
        {
            TableMeta = datas;
            TableName = sidebar;
            GetButons = funcButton;
            this.TableCss = TableCss;
            TableDom = new TableDom();
        }

        public TableCreator<T> TableCreater
        {
            get
            {
                return new TableCreator<T>(TableMeta, TableName, buttonDoms);
            }
        }

        public string TableCss { get; set; }
        public List<T> TableMeta { get; private set; }
        public string TableName { get; }


        public ButtonDom[] buttonDoms
        {
            get
            {
                return GetButons();
            }
        }
        public Func<ButtonDom[]> GetButons { get; set; }
        public TableDom TableDom { get; set; }

        public void GenTable(string t, int ttl, int pg)
        {
            TableDom.TableCss = this.TableCss;
            TableDom.TableHeadDom = TableCreater.GenTableHeadDom();
            if (TableMeta != null && TableMeta.Count > 0)
            {
                TableDom.TableRowDom = TableCreater.GenRowsDom().Result;
            }
            else
            {
                TableDom.TableRowDom = TableCreater.GenRowsDom().Result;
            }
            TableDom.TableFooter = TableCreater.GenTableFooter();
            TableDom.PagerDom = TableCreater.GetPagerDom(t, pg, ttl).Result;
        }
    }
}
