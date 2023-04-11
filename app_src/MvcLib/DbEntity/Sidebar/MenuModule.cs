using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLib.Sidebar
{
    public abstract class BasicData
    {     
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ID")]
        public int id { get; set; }

        [Display(Name = "名称")]
        [Required]
        public string title { get; set; }

        [Display(Name ="序号")]
        [Required]
        public int orderNo { get; set; }
        [Display(Name = "展示")]
        public bool isShow { get; set; }
    }

 
    //lots of properties can be implemented
    public class MenuModule :BasicData
    {
        public List<AppSidebar> Sidebars { get; set; }
    }

}