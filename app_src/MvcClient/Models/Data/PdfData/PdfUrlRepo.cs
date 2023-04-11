using MvcLib.Db;
using MvcLib.MainContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Models.Data.PdfData
{
    public interface IPdfUrlRepo : IDataBase<Pdf_Url>
    {
        IEnumerable<Pdf_Url> GetList(int pk);
    }

    public class PdfUrlRepo : IPdfUrlRepo
    {
        private AppDbContext _db;

        public PdfUrlRepo(AppDbContext db)
        {
            _db = db;
        }

        public Task<int> Add(Pdf_Url model)
        {
            _db.pdf_urls.Add(model);
            return _db.SaveChangesAsync();
        }

        public Task<int> Del<KT>(KT K)
        {
            var mdl = _db.pdf_urls.Find(K);
            _db.pdf_urls.Remove(mdl);
            return _db.SaveChangesAsync();
        }

        public Task<bool> Existed(Pdf_Url model)
        {
            throw new NotImplementedException();
        }

        public async Task<Pdf_Url> Get<KT>(KT k)
        {
            return await _db.pdf_urls.FindAsync(k);

        }

        public IEnumerable<Pdf_Url> GetList(int pk)
        {
            return _db.pdf_urls.Where(a => a.pdfId.Equals(pk));
        }
    }
}
