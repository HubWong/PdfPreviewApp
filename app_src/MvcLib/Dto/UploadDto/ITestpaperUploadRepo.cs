using MvcLib.Db;
using MvcLib.DbEntity.MainContent;
using System.Threading.Tasks;

namespace MvcLib.Dto.UploadDto
{

    public interface ITestpaperUploadRepo :
        IDataBase<TestpaperUpload>,
        IDbQuery<TestpaperUpload>
    {
        Task<int> update(TestpaperUpload testpaperUpload);        
    }
}
