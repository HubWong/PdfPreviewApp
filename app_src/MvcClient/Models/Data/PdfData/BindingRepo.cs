using Microsoft.EntityFrameworkCore;
using MvcClient.Models.PdfVms;
using MvcLib;
using MvcLib.Db;
using MvcLib.Dto.PdfDtos;
using MvcLib.MainContent;
using MvcLib.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Models.Data.PdfData
{
    public interface IBindingRepo : IRightBindingView
    {
        List<ItemCategory> GetCategList();
        public IQueryable<Binding> Bindings { get; set; }
        Task<KeyValuePair<string, IEnumerable<int>>> GetDownLevel(string clickedId, string upperIds);
        Task<int> SaveBindings(BindingSavingDto bindingSavingDto);
        Task<int> DelBindings(BindingSavingDto bindingSavingDto);
        Task<Dictionary<categoryType, List<ItemCategory>>> getDicBindings();
        bool Any(int categoryId);
        Task<int> bindCount(ItemCategory itemCategory);
        IPdfRepo Pdfs { get; }
    }

    public class BindingRepo : IBindingRepo
    {
        private AppDbContext _db;
        private IItemCategoryRepo CategRepo;
        public int BindId { get; set; }
        public IQueryable<Binding> Bindings { get; set; }
        public IPdfRepo Pdfs { get; private set; }


        public ITableVm TableCreate { get; set; }
        public BindingRepo(AppDbContext appDbContext, IItemCategoryRepo ItemCategoryRepo, IPdfRepo pdfRepo)
        {
            _db = appDbContext;
            CategRepo = ItemCategoryRepo;
            Bindings = _db.bindings.AsQueryable();
            Pdfs = pdfRepo;
        }

        public Task<int> Add(Binding model)
        {
            _db.bindings.Add(model);
            return _db.SaveChangesAsync();
        }



        /// <summary>
        /// order:  类型 / 模块 / 学科 / 版本
        /// </summary>
        /// <param name="clickedId"></param>
        /// <param name="upperIds">1,2,3....</param>
        /// <returns></returns>
        public Task<KeyValuePair<string, IEnumerable<int>>> GetDownLevel(string clickedId, string upperIds)
        {
            if (string.IsNullOrWhiteSpace(clickedId))
            {
                throw new ArgumentNullException();
            }
            var res = new KeyValuePair<string, IEnumerable<int>>();
            string[] upperIdArray;
            if (string.IsNullOrWhiteSpace(upperIds))  //top level clicked.
            {
                upperIdArray = new string[0];
            }
            else
            {
                upperIdArray = upperIds.Split(",");
            }

            int last_a = int.Parse(clickedId.Split(Constants.Splitor)[1]);
            var query = from n in _db.bindings
                        select n;
            if (clickedId.StartsWith(Constants.Lx) && string.IsNullOrEmpty(upperIds)) //get level mokuai
            {
                query = query.Where(a => a.leixing_id.Equals(last_a));
                res = new KeyValuePair<string, IEnumerable<int>>(Constants.Mk, query.ToList().Select(a => a.mokuai_id).Distinct());
            }
            else if (upperIdArray.Length == 1 && clickedId.StartsWith(Constants.Mk)) //get level xueke
            {
                query = query.Where(a => a.mokuai_id.Equals(last_a) && a.leixing_id.Equals(int.Parse(upperIdArray[0])));
                query = query.Distinct();
                res = new KeyValuePair<string, IEnumerable<int>>(Constants.Xk,
                    query.Select(a => a.xueke_id).Distinct());
            }
            else if (upperIdArray.Length == 2 && clickedId.StartsWith(Constants.Xk)) //get level of banben
            {
                query = query.Where(a => a.xueke_id.Equals(last_a) && a.leixing_id.Equals(int.Parse(upperIdArray[0])) && a.mokuai_id.Equals(int.Parse(upperIdArray[1])));
                query = query.Distinct();
                res = new KeyValuePair<string, IEnumerable<int>>(Constants.Bb,
                    query.Select(a => a.banben_id).Distinct());
            }
            return Task.FromResult(res);
        }



        /// <summary>
        /// get data title & id of lower level
        /// </summary>
        /// <param name="nxtType"></param>
        /// <param name="bindingRest"></param>
        /// <returns></returns>
        private async Task<List<ItemCategory>> getItemCategories(categoryType clkType, BindingRestDto bindingRest)
        {
            var b = from x in _db.bindings
                    select x;

            List<int> qids = new List<int>();
            // binding data need to be ordered by its id, 'cause pdf list need it.
            switch (clkType)
            {
                case categoryType.lei_xing:
                    b = b.Where(a => a.leixing_id.Equals(bindingRest.leixing_id)).OrderBy(a => a.id);
                    qids = await b.Select(a => a.mokuai_id).ToListAsync();
                    break;

                case categoryType.mo_kuai:
                    b = b.Where(a => a.leixing_id.Equals(bindingRest.leixing_id)
                    && a.mokuai_id.Equals(bindingRest.mokuai_id)).OrderBy(a => a.id);
                    qids = await b.Select(a => a.xueke_id).ToListAsync();
                    break;

                case categoryType.xue_ke:
                    b = b.Where(a => a.leixing_id.Equals(bindingRest.leixing_id)
                    && a.mokuai_id.Equals(bindingRest.mokuai_id)
                    && a.xueke_id.Equals(bindingRest.xueke_id)).OrderBy(a => a.id);
                    qids = await b.Select(a => a.banben_id).ToListAsync();
                    break;
            }

            return _db.categories.Where(a => qids.Contains(a.id)).Distinct().ToList();

        }


        /// <summary>
        /// get rest bdings.
        /// </summary>
        /// <param name="bindingRestDto"></param>
        /// <returns></returns>
        public async Task<Dictionary<categoryType, List<ItemCategory>>> GetDownLevels(BindingRestDto bindingRestDto)
        {
            var dicResult = new Dictionary<categoryType, List<ItemCategory>>();
            int id = -1;
            List<ItemCategory> downLevelData;

            if (bindingRestDto.clk.StartsWith(Constants.Lx))
            {
                downLevelData = await getItemCategories(categoryType.lei_xing, bindingRestDto);
                if (downLevelData.Any())
                {
                    id = downLevelData.First().id;
                    bindingRestDto.clk = "mk_" + id;
                    bindingRestDto.mokuai_id = id;
                }
                else
                {
                    //throw new NullReferenceException("no downlevel data found");

                }
                dicResult.Add(categoryType.mo_kuai, downLevelData);
            }

            if (bindingRestDto.clk.StartsWith(Constants.Mk) && bindingRestDto.mokuai_id != -1)
            {
                downLevelData = await getItemCategories(categoryType.mo_kuai, bindingRestDto);
                if (downLevelData.Any())
                {
                    id = downLevelData.First().id;
                    bindingRestDto.clk = "xk_" + id;
                    bindingRestDto.xueke_id = id;
                }
                else
                {
                    // throw new NullReferenceException("no downlevel data found");
                }
                dicResult.Add(categoryType.xue_ke, downLevelData);

            }

            if (bindingRestDto.clk.StartsWith(Constants.Xk) && bindingRestDto.xueke_id != -1)
            {
                downLevelData = await getItemCategories(categoryType.xue_ke, bindingRestDto);
                if (downLevelData.Any())
                {
                    id = downLevelData.First().id;
                }
                else
                {
                    //throw new NullReferenceException("no downlevel data found");
                }
                bindingRestDto.clk = "bb_" + id;
                dicResult.Add(categoryType.ban_ben, downLevelData);
            }

            if (bindingRestDto.clk.StartsWith(Constants.Bb))
            {
                id = int.Parse(bindingRestDto.clk.Split("_")[1]);
            }

            var bd = await _db.bindings.SingleOrDefaultAsync(a => a.leixing_id == bindingRestDto.leixing_id
            && a.mokuai_id == bindingRestDto.mokuai_id
            && a.xueke_id == bindingRestDto.xueke_id
            && a.banben_id == id);

            if (bd != null)
            {
                BindId = bd.id;
            }

            return dicResult;
        }

        public Task<List<Binding>> GetAll()
        {
            return Task.FromResult(_db.bindings.OrderBy(a => a.id).ToList());
        }

        public Task<List<Binding>> PagedData(Func<Binding, bool> func, out int ttl, int pg = 1)
        {
            ttl = _db.bindings.Where(func).ToList().Count;
            var lst = _db.bindings.Where(func).ToList();
            return Task.FromResult(lst);
        }


        public List<BindingVm> ToViewModels(IEnumerable<Binding> list)
        {
            throw new Exception();
        }

        public Task GetInitList(string p, int pg, out int ttl)
        {
            return PagedData(null, out ttl, pg);
        }

        public Task<Binding> Get(string key)
        {
            return Task.FromResult(_db.bindings.Find(key));
        }

        public Task<Binding> Get<KT>(KT k)
        {
            return _db.bindings.FindAsync(k).AsTask();
        }

        public List<ItemCategory> GetCategList()
        {
            return _db.categories.ToList();
        }

        public Task<int> Del<KT>(KT K)
        {
            var m = _db.bindings.Find(K);
            if (m != null)
            {
                _db.bindings.Remove(m);
            }
            return _db.SaveChangesAsync();
        }

        private List<Binding> genBinding(BindingSavingDto bindingSavingDto)
        {
            var list = new List<Binding>();
            foreach (int item in bindingSavingDto.getLastIdsArray())
            {
                list.Add(new Binding
                {
                    maker = bindingSavingDto.maker,
                    banben_id = item,
                    leixing_id = bindingSavingDto.leixing_id,
                    mokuai_id = bindingSavingDto.mokuai_id,
                    xueke_id = bindingSavingDto.xueke_id
                });
            }
            return list;
        }

        public Task<int> SaveBindings(BindingSavingDto bindingSavingDto)
        {
            List<Binding> list = genBinding(bindingSavingDto);
            foreach (var item in list)
            {
                if (!Existed(item).Result)
                {
                    _db.bindings.Add(item);
                }
            }

            return _db.SaveChangesAsync();
        }

        public Task<bool> Existed(Binding binding)
        {
            return Task.FromResult(_db.bindings.Any(a => a.banben_id == binding.banben_id && a.leixing_id == binding.leixing_id
           && a.mokuai_id == binding.mokuai_id && a.xueke_id == binding.xueke_id));
        }

        /// <summary>
        /// get binding by related id;
        /// </summary>
        /// <param name="fk"></param>
        /// <returns></returns>
        private Binding GetBinding(Binding binding)
        {
            return Bindings.SingleOrDefault(a => a.leixing_id.Equals(binding.leixing_id) &&
            a.mokuai_id.Equals(binding.mokuai_id) &&
            a.xueke_id.Equals(binding.xueke_id) &&
            a.banben_id.Equals(binding.banben_id));
        }

        public Task<int> DelBindings(BindingSavingDto bindingSavingDto)
        {
            var list = genBinding(bindingSavingDto);

            //del single binding 
            foreach (var item in list)
            {
                var b = GetBinding(item);
                if (b != null)
                {
                    _db.bindings.Remove(b);
                }
            }
            return _db.SaveChangesAsync();
        }


        /// <summary>
        /// get dic of top categs in pdf table view.
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<categoryType, List<ItemCategory>>> getDicBindings()
        {
            Dictionary<categoryType, List<ItemCategory>> dicResult = new Dictionary<categoryType, List<ItemCategory>>();

            var ids = _db.bindings.ToList();
            var lxs = await _db.categories.OrderBy(a => a.id)
                .Where(a => ids.Select(x => x.leixing_id).Contains(a.id)).ToListAsync();
            dicResult.Add(categoryType.lei_xing, lxs);

            var mk = await _db.categories.OrderBy(a => a.id)
                .Where(a => ids.Select(x => x.mokuai_id).Contains(a.id)).ToListAsync();
            dicResult.Add(categoryType.mo_kuai, mk);

            var xk = await _db.categories.OrderBy(a => a.id)
                .Where(a => ids.Select(x => x.xueke_id).Contains(a.id)).ToListAsync();
            dicResult.Add(categoryType.xue_ke, xk);

            var bb = await _db.categories.OrderBy(a => a.id)
                .Where(a => ids.Select(x => x.banben_id).Contains(a.id)).ToListAsync();
            dicResult.Add(categoryType.ban_ben, bb);

            return dicResult;
        }


        public async Task<List<PdfUploadLog>> GetBindingTable(PagedPdfDto pdfTableDto, bool Isfront = false)
        {
            int ttl = 0;
            var list = await GetPagedBindingPdfs(pdfTableDto.bindid, pdfTableDto.orderby, pdfTableDto.pg, Isfront, out ttl);
            pdfTableDto.ttl = ttl;

            return list;
        }

        /// <summary>
        /// get pdf list for Front and Console end.
        /// </summary>
        /// <param name="bindid"></param>
        /// <param name="orderby"></param>
        /// <param name="pg"></param>
        /// <param name="isfront"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public Task<List<PdfUploadLog>> GetPagedBindingPdfs(int bindid, string orderby, int pg, bool isfront, out int ttl)
        {
            IQueryable<PdfUploadLog> p;
            if (!isfront)
            {
                p = _db.pdf_upload_logs.Where(o => o.bindingId.Equals(bindid));
            }
            else
            {
                //isShow: can be shown, isfront : from front
                p = _db.pdf_upload_logs.Where(o => o.bindingId.Equals(bindid) && o.isShow.Equals(true));
            }

            ttl = p.Count();
            var list = p.Skip((pg - 1) * 10).Take(10).ToListAsync();
            return list;
        }



        public Task<List<PdfUploadLog>> GetPdfTables(int bindid, string orderby, out int ttl, int pg = 1)
        {
            throw new NotImplementedException();
        }

        public Task<RgtBooksVm> getBindingTable(BindingRestDto bindingRestDto)
        {
            throw new NotImplementedException();
        }

        public Task<List<ItemCategory>> GetTopLeixings()
        {
            var list = Bindings.
                OrderBy(a => a.id).
                Select(a => a.leixing_id).Distinct().ToList();
            var list_cates = CategRepo.GetTopLeixing(list);
            return list_cates;
        }


        public Task<int> bindCount(ItemCategory itemCategory)
        {
            return _db.bindings.CountAsync(a => a.mokuai_id.Equals(itemCategory.id) || a.leixing_id.Equals(itemCategory.id)
            || a.banben_id.Equals(itemCategory.id) || a.xueke_id.Equals(itemCategory.id));
        }

        public bool Any(int categoryId)
        {
            return _db.bindings.Any(a => a.banben_id.Equals(categoryId) || a.leixing_id.Equals(categoryId) || a.banben_id.Equals(categoryId) || a.mokuai_id.Equals(categoryId));

        }
    }
}
