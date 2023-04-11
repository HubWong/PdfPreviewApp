using MvcLib.DbEntity.MainContent;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcLib.Dto.ColumnDto
{
    public class ColumnDataDto
    {
        public int? Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public int pid { get; set; }
        public string Maker { get; set; }
    }

    public class ColumnFormVm : ColumnDataDto
    {
        public ColumnFormVm()
        {
            IsAdd = 1;
        }
        public byte IsAdd { get; set; }
    }

    public class ColumnVm
    {         
        public ColumnVm(ColumnData columnData)
        {
            this.text = columnData.Name;
            this.pid = columnData.Pid;
            this.id = columnData.Id;
            this.nodes = new List<ColumnVm>();
        }   
        public string text { get; set; }
        public int id { get; set; }
        public int pid { get; set; }
        public List<ColumnVm> nodes { get; set; }
    }
}
