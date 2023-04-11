using Microsoft.EntityFrameworkCore;
using MvcLib.Db;
using MvcLib.DbEntity.MainContent;
using MvcLib.Dto.PropDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Models.Data.TestpaperData
{
    public class PropsRepo : IPropRepo
    {
        private readonly AppDbContext dbContext;

        public PropsRepo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<int> Add(TestpaperProps model)
        {
            dbContext.Add(model);
            return dbContext.SaveChangesAsync();
            throw new NotImplementedException();
        }

        public Task<int> Del<KT>(KT K)
        {
            var m = this.dbContext.testpaper_props.Find(K);
            if (m != null)
            {
                this.dbContext.testpaper_props.Remove(m);
                return this.dbContext.SaveChangesAsync();
            }
            return null;
            //throw new NotImplementedException();
        }

        public Task<bool> Existed(TestpaperProps model)
        {
            var r= this.dbContext.testpaper_props.Any(k => model.id.Equals(k.id));
            return Task.FromResult(r);
            //throw new NotImplementedException();
        }

        public Task<TestpaperProps> Get<KT>(KT k)
        {
            return dbContext.testpaper_props.FindAsync(k).AsTask();
            //throw new NotImplementedException();
        }

        public Task<List<TestpaperProps>> GetAll()
        {
            return dbContext.testpaper_props.ToListAsync();
            throw new NotImplementedException();
        }

        public Task<List<TestpaperProps>> PagedData(Func<TestpaperProps, bool> func, out int ttl, int pg = 1)
        {
            ttl = dbContext.testpaper_props.Where(func).Count();
            var list = dbContext.testpaper_props.Where(func).Skip(--pg * 10).Take(10).ToList();
            return Task.FromResult(list);

        }
    }
}
