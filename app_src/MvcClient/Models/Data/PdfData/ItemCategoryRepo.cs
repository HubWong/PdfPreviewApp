using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MvcClient.Models.Data.PdfData;
using MvcClient.Models.PdfVms;
using MvcLib;
using MvcLib.Db;
using MvcLib.Dto;
using MvcLib.MainContent;
using MvcLib.Tools;

namespace MvcClient.Models.Data
{
    public interface IItemCategoryRepo :      
        IRightCategoryView 
    {
        IEnumerable<ItemCategory> GetCategList (string type, string orderby, int pg, out int ttl, bool isAsc);
        IEnumerable<ItemCategory> GetCategList();       
        Task<int> Del_Many(string[] id_array);
        Task<List<ItemCategory>> GetLeixingData();
        Task<int> Update(ItemCategory itemCategory);
        Task<List<ItemCategory>> GetTopLeixing(List<int> lxs);
        Task<int> Del<KT>(KT K, Action<List<string>> ActionPathsToDel);
    }

    public class ItemCategoryRepo : IItemCategoryRepo
    {
        private AppDbContext _db;        
        public List<ItemCategory> ModelList { get; set; }
     
        public ITableVm TableCreate { get; set; }

        public ItemCategoryRepo (AppDbContext appDbContext) {
            _db = appDbContext;
            ModelList = new List<ItemCategory>();           
        }

        public Task<int> Add (ItemCategory model) {
            if (Existed(model).Result)
            {
                return Task.FromResult(0);
            }
            _db.categories.Add(model);
            return _db.SaveChangesAsync ();
        }

   
        public Task<List<ItemCategory>> GetAll () {
            return Task.FromResult (_db.categories.ToList ());
        }      



        public Task<List<ItemCategory>> PagedData (Func<ItemCategory, bool> func, out int ttl, int pg = 1) {
            var list = _db.categories.Where (func).ToList ();
            ttl = list.Count;
            return Task.FromResult (list);
        }

        public async Task<int> Update (ItemCategory itemCategory) {
         
            var mdl = await Get(itemCategory.id);
            if (mdl != null)
            {
                mdl.isShow = itemCategory.isShow;
                mdl.orderNo = itemCategory.orderNo;
                mdl.title = itemCategory.title;
                mdl.memo = itemCategory.memo;
                mdl.update_day = DateTime.Now;
                //mdl.category = itemCategory.category;
            }
            _db.categories.Update(mdl);
            return _db.SaveChanges();
        } 



        /// <summary>
        /// get different category table.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="orderby"></param>
        /// <param name="pg"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public IEnumerable<ItemCategory> GetCategList (string type, string orderby, int pg, out int ttl,bool isAsc=true) {
            try
            {
                //get certain types
                //see lib in Utility static method               
                int index = Utility.EnumGetDisplayIndex<categoryType>(type);        

                categoryType thisType = (categoryType)index;
               
                var q = from n in _db.categories
                        where n.category == thisType
                        select n;

                ttl = q.Count();
                var m =q
                    .OrderBy(a => a.orderNo)
                    .ThenBy(a => a.make_day).Skip((pg-1)*10).Take(10).ToList();

                
              
                return m;
            }
            catch (Exception)
            {
                ttl = 0;
                return null;
            }       

        }

        public Task GetInitList(string p, int pg)
        {
            ModelList =GetCategList(p, null, pg, out int i).ToList();
            return Task.CompletedTask;
        }

        public Task GetInitList(string sb, int pg, out int ttl)
        {
            ModelList = GetCategList(sb, null, pg, out ttl).ToList();
            TableCreate = new TableVm<ItemCategory>(ModelList, sb,()=>new TableButonCreator().GenButton(Constants.TableButtonTypes.btns_table_category));
            TableCreate.GenTable(sb, ttl, pg);
            return Task.CompletedTask;

        }

        public Task<ItemCategory> Get<KT>(KT k)
        {
            return Task.FromResult(_db.categories.Find(k));
        }

        /// <summary>
        /// get all categories .
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ItemCategory> GetCategList()
        {
            return _db.categories.AsEnumerable();
        }


        Func<string, int?> to_int_delegete = x =>
        {
            if(int.TryParse(x, out int m))
            {
                return m;
            }
            return null;
        };


 

        /// <summary>
        /// del category with many relative pdfs     
        /// </summary>
        /// <typeparam name="KT"></typeparam>
        /// <param name="K"></param>
        /// <returns></returns>
        public async Task<int> Del<KT>(KT K, Action<List<string>> ActionPathsToDel)
        {
       
            ItemCategory m = _db.categories.Find(K);
            
            _db.Remove(m);
            var list =_db.bindings.Where(a => a.banben_id == m.id || a.leixing_id == m.id || a.mokuai_id == m.id || a.xueke_id == m.id);
            var pdfs= _db.pdf_upload_logs.Where(a=>list.Select(x=>x.id).Contains(a.id));
            //del pdf relative files.
            await PdfRepo.DelPdfFiles(pdfs.ToList(), ActionPathsToDel);
            _db.pdf_upload_logs.RemoveRange(pdfs);
            _db.bindings.RemoveRange(list);        
            return await _db.SaveChangesAsync();
        }

        /// <summary>
        /// del many with string array from js.
        /// </summary>
        /// <param name="id_array"></param>
        /// <returns></returns>
        public Task<int> Del_Many(string[] id_array)
        {
            List<int> int_array = new List<int>();
            id_array.ToList().ForEach(x => {
                int_array.Add(to_int_delegete(x).Value);
            });
            if (int_array.Count > 0)
            {
                var list = new List<ItemCategory>();
                foreach (var item in int_array)
                {
                    list.Add(_db.categories.Find(item));
                }

                if (list.Count > 0)
                {
                    _db.categories.RemoveRange(list);
                }
            }
            return _db.SaveChangesAsync();       
        }

        public Task<bool> Existed(ItemCategory model)
        {
            return Task.FromResult( _db.categories.Any(a => a.title.EndsWith(model.title.Trim())));
        }

        public Task<List<ItemCategory>> GetLeixingData()
        {
            var list = _db.categories.Where(a => a.category == categoryType.lei_xing).ToList();
            return Task.FromResult(list);
        }

        public Task<List<ItemCategory>> GetTopLeixing(List<int> lxs)
        {
            if (lxs != null)
            {
                var list = _db.categories.Where(a => lxs.Contains(a.id)).OrderBy(a => a.make_day).ToList();
                return Task.FromResult(list);
            }
            return null;
        }

        public Task<int> Del<KT>(KT K)
        {
            throw new NotImplementedException();
        }
    }
}