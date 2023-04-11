using Microsoft.AspNetCore.Http;
using MvcLib;
using MvcLib.Dto.PdfDtos;
using MvcLib.MainContent;
using MvcLib.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcClient.Models.PdfVms
{
    /// <summary>
    ///  pdf table row data
    /// </summary>
    public class PdfRowVm : PdfUploadLog, IFormViewModel
    {

        public PdfRowVm(PdfUploadLog pdf)
        {

        }

        public PdfRowVm()
        {
            make_day = DateTime.Now;
            IsAdd = 0;
        }

        public byte IsAdd { get; set; }

        public int DaysAgo
        {
            get
            {
                if (update_day != null)
                    return DateTime.Now.Subtract(update_day.Value).Days;
                else
                {
                    return DateTime.Now.Subtract(make_day).Days;
                }
            }
        }

        public string TimeStr
        {
            get
            {
                if (IsAdd == 1)
                {
                    return DateTime.Now.ToString("D");
                }
                else
                {
                    return DaysAgo + "天前";
                }

            }
        }
    }



    /// <summary>
    /// binding paf table.
    /// </summary>
    public class BindingPdfTable
    {
        public BindingPdfTable(int bid)
        {
            BindId = bid;
        }

        public int BindId { get; set; }       
        public TableVm<PdfUploadLog> BookTable { get; set; }
    }

    public interface IPdf
    {
        Task<List<ItemCategory>> GetTopLeixings();
        Task<List<PdfUploadLog>> GetPdfTables(int bindid, string orderby, out int ttl, int pg = 1);
        Task<List<PdfUploadLog>> GetBindingTable(PagedPdfDto pdfTableDto, bool IsFront);
        Task<Dictionary<categoryType, List<ItemCategory>>> GetDownLevels(BindingRestDto bindingRestDto);
        int BindId { get; set; }
    }

    public interface IPdfVm : IPdf
    {
        Task<RgtBooksVm> getBindingTable(BindingRestDto bindingRestDto);
    }


    /// <summary>
    /// not done yet
    /// </summary>
    public class RgtBooksVm
    {
        public RgtBooksVm()
        {

        }
        public Dictionary<categoryType, List<ItemCategory>> BindData { get; set; }
        public BindingPdfTable BindingPdfTable { get; set; }
    }




    public class RghtBookApi
    {
        public Dictionary<categoryType, List<ItemCategory>> BindData { get; set; }
        public TableDom Table { get; set; }
        public int bid { get; set; }
    }


}
