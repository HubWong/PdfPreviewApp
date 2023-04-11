using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcLib.Db
{
    /// <summary>
    /// sidebar content, get query list.
    /// </summary>
    public interface IDbQuery<T>
    {
        Task<List<T>> GetAll();
        Task<List<T>> PagedData(Func<T, bool> func, out int ttl, int pg = 1);      
    }
}
