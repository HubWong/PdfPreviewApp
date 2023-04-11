using MvcLib.Dto;
using MvcLib.Dto.UploadDto;
using System;
using System.Collections.Generic;

namespace MvcLib.DbEntity.MainContent
{
    public class TestpaperUpload : BaseEntity<UploadVm>
    {
        public int id { get; set; }
        public DateTime make_day { get; set; }
        public ColumnData column { get; set; }
        public int columnId { get; set; }
        public TestpaperProps nf { get; set; }
        public int nfId { get; set; }
        public TestpaperProps sf { get; set; }
        public int sfId { get; set; }
        
        public override UploadVm Vm
        {
            get
            {
                return new UploadVm(this);
            }
        }

        public List<FileEntity>  testpaper_docs { get; set; }
    }
}
