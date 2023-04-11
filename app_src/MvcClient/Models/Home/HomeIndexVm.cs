using MvcLib.MainContent;
using System.Collections.Generic;

namespace MvcClient.Models.Home
{
    /// <summary>
    /// for client showing.
    /// </summary>
    public class PdfListVm
    {       
        public List<PdfUploadLog> Pdfs { get; set; }
        public string Pager { get; set; }
    }

    /// <summary>
    /// index vm of home
    /// </summary>
    public class HomeIndexVm:PdfListVm
    {
        public Dictionary<categoryType, List<ItemCategory>> BindData { get; set; }
        
        public int BindId { get; set; }
    }
}
