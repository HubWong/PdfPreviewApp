using MvcLib.Db;
using MvcLib.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Models.Data
{
    public interface ISidebarRepo : IDataBase<AppSidebar>
    {       
        Task<IQueryable<AppSidebar>> QueryAll();
    }

    public  class SidebarRepo : ISidebarRepo
    {
        private AppDbContext _db;
    

        public SidebarRepo(AppDbContext appDbContext)
        {
            _db = appDbContext;         
        }

        public Task<int> Add(AppSidebar model)
        {
            _db.sidebars.Add(model);
            return _db.SaveChangesAsync();
        }

        public Task<IQueryable<AppSidebar>> QueryAll()
        {
             var r = _db.sidebars.AsQueryable<AppSidebar>();
            return Task.FromResult(r);            
        }

        public Task<List<AppSidebar>> GetInitList(string p, int pg)
        {
            throw new NotImplementedException();
        }

        public Task<AppSidebar> Get(string key)
        {
            return Task.FromResult(_db.sidebars.Find(key));
        }

        public Task<AppSidebar> Get<KT>(KT k)
        {
            throw new NotImplementedException();
        }

        public Task<int> Del<KT>(KT K)
        {
            throw new NotImplementedException();
        }

        public Task<int> Update<Dto>(Dto model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Existed(AppSidebar model)
        {
            throw new NotImplementedException();
        }
    }
}
