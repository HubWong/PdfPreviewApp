using Microsoft.EntityFrameworkCore;
using MvcClient.Models.PdfVms;
using MvcLib.Db;
using MvcLib.Sidebar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Models.Data
{
    public interface IMenuModuleRepo : 
        IDataBase<MenuModule>,
        IMenuVm
    {
        IQueryable<MenuModule> MenuModules { get; set; }       
        Task QueryMenuModules();       
    }

    public class MenuModuleRepo : IMenuModuleRepo
    {
        private AppDbContext _db;
        private ISidebarRepo sidebarRepo;
        public IQueryable<MenuModule> MenuModules { get; set; }
     
        public MenuModuleRepo(AppDbContext appDbContext, ISidebarRepo sidebarRepo)
        {
            _db = appDbContext;
            this.sidebarRepo = sidebarRepo;
        }

        public async Task<int> Add(MenuModule model)
        {
            _db.menu_modules.Add(model);
           return await  _db.SaveChangesAsync();           
        }


        public async Task<int> Update(MenuModule model)
        {
             _db.Update(model);
            return await _db.SaveChangesAsync();         
        }

        public Task QueryMenuModules()
        {
            this.MenuModules = _db.menu_modules.Include(a=>a.Sidebars);
           
            return Task.CompletedTask;
        }

        public Task<List<MenuModule>> GetInitList(string p, int pg)
        {
            throw new System.NotImplementedException();
        }

        public async Task<MenuModule> Get(string key)
        {
            return await _db.menu_modules.FindAsync(key); 
        }

        public async Task<MenuModule> Get<KT>(KT k)
        {
            return await _db.menu_modules.FindAsync(k);           
        }

        public async Task<int> Del<KT>(KT K)
        {
            var m = _db.menu_modules.Find(K);
            if (m != null)
            {
                _db.menu_modules.Remove(m);
                return await _db.SaveChangesAsync();
            }
            return await Task.FromResult(0);
        }

        public Task<int> Update<Dto>(Dto model)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Existed(MenuModule model)
        {
            return Task.FromResult(_db.menu_modules.Any(a => a.title.Equals(model.title)));
        }
    }
}
