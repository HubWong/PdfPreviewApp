using MvcLib.Dto.PropDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MvcLib.DbEntity.MainContent
{
    public class TestpaperProps:BaseEntity<TestpaperVm>
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public TestpaperPropDto.properType nianfen_or_shengfen { get; set; }
        [Required]public string value { get; set; }

        public override TestpaperVm Vm 
        { 
            get
            {
                return new TestpaperVm(testpaperProps:this);
            } 
        }
    }
}
