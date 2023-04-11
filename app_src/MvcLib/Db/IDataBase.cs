using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MvcLib.Db
{
    public  interface IDataBase<T> where T:class
    {
        Task<int> Add(T model);     
        Task<int> Del<KT>(KT K);      
        Task<T> Get<KT>(KT k);
        Task<bool> Existed(T model);
    }
}
