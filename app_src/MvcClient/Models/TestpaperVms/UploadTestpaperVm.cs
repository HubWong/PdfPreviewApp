using MvcLib;
using MvcLib.DbEntity.MainContent;
using MvcLib.Dto;
using MvcLib.Dto.ColumnDto;
using MvcLib.Dto.PropDto;
using MvcLib.Dto.UploadDto;
using System.Collections.Generic;

namespace MvcClient.Models.TestpaperVms
{
    /// <summary>
    /// upload testpaper page vms 
    /// </summary>
    public class UploadTPVm : IFormViewModel
    {

        private byte _isAdd;
        private ITestpaperUploadRepo uploaderRepo;
        public UploadTPVm(IPropRepo db, 
            IColumnDataRepo dataRepo, 
            ITestpaperUploadRepo testpaperUploadRepo, 
            byte isAdd = 1)
        {
            props = db.GetAll().Result;
            columnVms = dataRepo.GetLevelData(0);
            _isAdd = isAdd;
            uploaderRepo = testpaperUploadRepo;
        }

        public int id { get; set; }
        public byte IsAdd { get; set; }
        public string TimeStr { get; set; }

        public TestpaperUpload uploaddata
        {
            get
            {
                if (this._isAdd == 1)
                {
                    return new TestpaperUpload();                   
                }
                return uploaderRepo.Get(id).Result;
            }
        }


        public UploadedFileDto FileInfo { 
            get
            {
                if (this.IsAdd == 1)
                {
                    return new UploadedFileDto();
                }
                return new UploadedFileDto {                     
                    size = uploaddata.Vm.size,
                    path = uploaddata.Vm.path
                };

            }
        }
        public List<TestpaperProps> props { get; set; }

        public List<ColumnVm> columnVms
        {
            get; set;
        }
    }
}
