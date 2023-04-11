
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MvcLib.Db;
using MvcLib.DbEntity.MainContent;
using MvcLib.Dto.ColumnDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Models.Data.TestpaperData
{
    public class ColumnDataRepo : IColumnDataRepo
    {
        private AppDbContext _db;
        private DbSet<ColumnData> _ColumnData;

        public ColumnDataRepo(AppDbContext appDbContext)
        {
            _db = appDbContext;
            _ColumnData = appDbContext.column_datas;
        }

        public Task<int> Add(ColumnDataDto model)
        {
            ColumnData columnData = new ColumnData();
            columnData.Pid = model.pid;
            columnData.Name = model.Name;
            columnData.MakeDay = DateTime.Now;
            columnData.Maker = model.Maker;
            _ColumnData.Add(columnData);
            return _db.SaveChangesAsync();
        }

        public async Task<int> Del<KT>(KT K)
        {
            var d = await _ColumnData.FindAsync(K);
            if (d == null) return 0;
            _ColumnData.Remove(d);
            return _db.SaveChanges();
        }

        public Task<int> Del(int[] ids)
        {
            var list = _ColumnData.Where(a => ids.Contains(a.Id));
            if (list.Any())
            {
                _ColumnData.RemoveRange(list);
            }
            return _db.SaveChangesAsync();           
        }

        public async Task<bool> Existed(ColumnDataDto model)
        {
            return await _ColumnData.AnyAsync(a => a.Id == model.Id);         
        }

        public async Task<ColumnDataDto> Get<KT>(KT k)
        {
            var d = await _ColumnData.FindAsync(k);
            if (d != null)
            {
                return new ColumnDataDto
                {
                    Maker = d.Maker,
                    pid = d.Pid,
                    Name = d.Name
                };

            }
            return null;
            //throw new System.NotImplementedException();
        }

        public async Task<List<ColumnData>> GetAll()
        {
            return await _ColumnData.ToListAsync();
        }

        public Task<List<ColumnData>> PagedData(Func<ColumnData, bool> func, out int ttl, int pg = 1)
        {
            ttl = _ColumnData.Where(func).Count();
            var list = _ColumnData.Skip(--pg * 10).Take(10).ToList();
            return Task.FromResult(list);
        }

        /// <summary>
        /// get all children of clm
        /// </summary>
        /// <param name="pid">pid of the children</param>
        /// <returns></returns>
        public Task<List<ColumnData>> GetChildren(int pid)
        {
            return _ColumnData.Where(a => a.Pid == pid).ToListAsync();
        }

        /// <summary>
        /// get vm from column data of the base.only one level
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public IEnumerable<ColumnVm> GetColumnVms(List<ColumnData> datas)
        {
            if (datas != null)
            {
                foreach (var item in datas)
                {
                    yield return new ColumnVm(item);
                }
            }
        }

        /// <summary>
        /// get two levels of columns.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public List<ColumnVm> GetLevelData(int pid=0)
        {
            var list_rslt = new List<ColumnVm>();
            var list = GetColumnVms(GetChildren(pid).Result);
            if (list != null)
            {
                foreach (var item in list)
                {
                    var ch = GetColumnVms(GetChildren(item.id).Result);
                    if (ch != null && ch.Any())
                        item.nodes.AddRange(ch);
                    list_rslt.Add(item);
                }
            }
            return list_rslt;
        }  
  
        public Task<int> Update(ColumnDataDto columnDataDto)
        {
            if (columnDataDto.pid.Equals(columnDataDto.Id))
            {
                return Task.FromResult(0);
            }
            var entity = _ColumnData.Find(columnDataDto.Id);

            if (entity != null)
            {
                entity.Pid = columnDataDto.pid;
                entity.MakeDay = DateTime.Now;
                entity.Id = columnDataDto.Id.Value;
            }
            _ColumnData.Update(entity);
            return _db.SaveChangesAsync();
        }
    }
}
