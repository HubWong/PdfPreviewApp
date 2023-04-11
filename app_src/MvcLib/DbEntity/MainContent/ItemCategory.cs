using MvcLib.Sidebar;
using System;
using System.ComponentModel.DataAnnotations;

namespace MvcLib.MainContent
{

    public enum categoryType
    {
        [Display(Name ="类型")]
        lei_xing,

        [Display(Name = "模块")]
        mo_kuai,

        [Display(Name = "学科")]
        xue_ke,

        [Display(Name = "版本")]
        ban_ben,

        [Display(Name ="电子")]
        pdf_book =4
       // bid,
    }

    public class ItemCategory : BasicData
    {
        public ItemCategory()
        {
            make_day = DateTime.Now;
        }
        [Display(Name ="类型")]
        public categoryType category { get; set; }
        public DateTime make_day {
            get;set;            
        }
        public DateTime? update_day { get; set; }
        public string memo { get; set; }
    }  
}
