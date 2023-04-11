using MvcLib.Db;
using MvcLib.DbEntity;
using MvcLib.Dto.UploadDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcClient.Models.Data.TestpaperData
{
    public class TestpaperFileRepo : IFileRepos
    {
        public TestpaperFileRepo(AppDbContext appDbContext)
        {
            AppDbContext = appDbContext;
        }

        public AppDbContext AppDbContext { get; }

        public Task<int> Add(FileEntity model)
        {
            AppDbContext.app_files.Add(model);
            return AppDbContext.SaveChangesAsync();
            throw new NotImplementedException();
        }

        //???????
        public string AddorUpdatePath(string path)
        {
           
            throw new NotImplementedException();
        }

        public Task<int> Del<KT>(KT K)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Existed(FileEntity model)
        {
            throw new NotImplementedException();
        }

        public Task<FileEntity> Get<KT>(KT k)
        {
            throw new NotImplementedException();
        }

        public Task<List<FileEntity>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<List<FileEntity>> PagedData(Func<FileEntity, bool> func, out int ttl, int pg = 1)
        {
            throw new NotImplementedException();
        }
    }
}
