using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MvcLib.Db;
using MvcLib.DbEntity.MainContent;
using MvcLib.Dto.UploadDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Models.Data.TestpaperData
{    
    public class TestpaperUploadRepo : ITestpaperUploadRepo
    {
        public TestpaperUploadRepo(AppDbContext appContext)
        {
            this.Db = appContext;
        }

        public AppDbContext Db { get; set; }      

        public Task<int> Add(TestpaperUpload model)
        {
            this.Db.testpaper_upload_logs.Add(model);
            return this.Db.SaveChangesAsync();
        }



        public Task<int> Del<KT>(KT K)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Existed(TestpaperUpload model)
        {
            var b = this.Db.testpaper_upload_logs.Any(a => model.id.Equals(a.id));
            return Task.FromResult(b);
        }

        public Task<TestpaperUpload> Get<KT>(KT k)
        {
            var md = this.Db.testpaper_upload_logs.Find(k);
            return Task.FromResult(md);
        }

        public Task<List<TestpaperUpload>> GetAll()
        {
            return this.Db.testpaper_upload_logs.ToListAsync();
        }

        public Task<List<TestpaperUpload>> PagedData(Func<TestpaperUpload, bool> func, out int ttl, int pg = 1)
        {
            ttl = this.Db.testpaper_upload_logs.Where(func).Count();
            var list_prp = from n in this.Db.testpaper_upload_logs.Include("nf").Include("sf").Include("column")                          
                           select n;
            var list = list_prp.Where(func).ToList();
            return Task.FromResult(list);
        }

        public Task<int> update(TestpaperUpload testpaperUpload)
        {
            var md = this.Db.Find<TestpaperUpload>(testpaperUpload.id);
            if (md != null)
            {
                this.Db.testpaper_upload_logs.Update(testpaperUpload);
            }
            return this.Db.SaveChangesAsync();       
        }
    }
}
