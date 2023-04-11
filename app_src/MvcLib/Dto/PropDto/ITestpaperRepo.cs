using MvcLib.Db;
using MvcLib.DbEntity.MainContent;

namespace MvcLib.Dto.PropDto
{
    public interface IPropRepo : IDataBase<TestpaperProps>, IDbQuery<TestpaperProps>
    {
        //Task<List<TestpaperProps>> GetCategData(TestpaperPropDto.properType properType);
    }
}
