using MvcLib.Db;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Models.Data
{
    /// <summary>
    /// test convieniet way of saving /query db.
    /// not used yet.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BasicDbRepo<T>
    {
        public BasicDbRepo(AppDbContext dbContext, Func<IQueryable<T>> dbsets)
        {
            DB = dbContext;
            Datas = dbsets;

        }

        public AppDbContext DB { get; private set; }

        public Func<IQueryable<T>> Datas { get; }      

    }
}
