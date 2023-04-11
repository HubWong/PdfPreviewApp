using MvcLib.Db;
using MvcLib.MainContent;
using System.Threading.Tasks;

namespace MvcClient.Models.PdfVms
{
    public interface IRightCategoryView :
        IDataBase<ItemCategory>,
        IDbQuery<ItemCategory>
    {
        Task GetInitList(string menu, int pg, out int ttl);
    }

    public interface IRightBindingView :
        IDataBase<Binding>,
        IDbQuery<Binding>,
        IPdfVm
    {

    }

    public interface IRightPdfRepo :
        IDataBase<PdfUploadLog>,
        IDbQuery<PdfUploadLog>
    {
        int Bindid { get; set; }
        Task<RgtBooksVm> GetRgtBooksVm(int bindid, out int ttl, string orderby = "makeDay", int pg = 1);
    }

}
