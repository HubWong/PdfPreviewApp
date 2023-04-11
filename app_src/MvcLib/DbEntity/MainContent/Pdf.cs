using MvcLib.Sidebar;
using MvcLib.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLib.MainContent
{
    /// <summary>
    /// db records of pdf table
    /// </summary>
    public class PdfUploadLog : BasicData
    {
        public int bindingId { get; set; }
        public Binding binding { get; set; }
        [Display(Name = "日期")]
        public DateTime make_day { get; set; }
        public string maker { get; set; }
        public string memo { get; set; }
        public string image_path { get; set; }  //cover image
        public DateTime? update_day { get; set; }
        public List<PdfFile> pdf_files { get; set; }
        public List<Pdf_Url> pdf_Urls { get; set; }
    }


    public class BasePdf
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public PdfUploadLog pdf { get; set; }
        public int pdfId { get; set; }
        public string title { get; set; }
        public DateTime make_day { get; set; }
    }


    /// <summary>
    /// pdf urls for viewing
    /// </summary>
    public class Pdf_Url : BasePdf
    {       
        public string image_path { get; set; }  //cover image

        [DataType(DataType.Url)]
        public string pdf_url { get; set; }
    }


    /// <summary>
    /// attachments of pdf
    /// </summary>
    public class PdfFile : BasePdf
    {
        public string saving_path { get; set; }

        public string file_type
        {
            get; set;
        }


        public long file_size { get; set; }
    }



}