using MvcLib.Db;
using MvcLib.DbEntity.MainContent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcLib.Dto.ColumnDto
{
    public interface IColumnDataRepo : IDataBase<ColumnDataDto>, IDbQuery<ColumnData>
    {
        Task<List<ColumnData>> GetChildren(int pid);
        List<ColumnVm> GetLevelData(int pid);
        Task<int> Del(int[] ids);
        Task<int> Update(ColumnDataDto columnDataDto);
        IEnumerable<ColumnVm> GetColumnVms(List<ColumnData> datas);
    }
}
