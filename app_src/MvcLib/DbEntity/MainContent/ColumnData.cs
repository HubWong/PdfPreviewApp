using MvcLib.Dto.ColumnDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MvcLib.DbEntity.MainContent
{
    /// <summary>
    /// in clm mgr of testpaper
    /// </summary>
    public class ColumnData:BaseEntity<ColumnVm>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public int Pid { get; set; }
        
        [Required]
        public string Name { get; set; }
        public string Memo { get; set; }
        public string Maker { get; set; }
        public DateTime MakeDay { get; set; }      

        public override ColumnVm Vm { 
            get {
                return new ColumnVm(columnData: this);
            } 
        }    
    }
}
