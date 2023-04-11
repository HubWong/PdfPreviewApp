using MvcLib.DbEntity.MainContent;
using System.ComponentModel.DataAnnotations;

namespace MvcLib.Dto.UploadDto
{
    /// <summary>
    /// shown in the table row
    /// </summary>
    public class UploadVm : UploadedFileDto,IViewModel
    {
        public UploadVm(TestpaperUpload testpaperUpload_Db)
        {
            this.DBModel = testpaperUpload_Db;
        }

        [Display(Name = "年份")]
        public string PropNf
        {
            get
            {
                return this.DBModel.nf.value;
            }
        }

        [Display(Name = "省份")]
        public string PropSf
        {
            get
            {
                return this.DBModel.sf.value;
            }
        }

        [Display(Name = "栏目")]
        public string ColumnStr
        {
            get
            {
                return this.DBModel.column.Name;
            }
        }
        
        private TestpaperUpload DBModel { get; }
    }
}
