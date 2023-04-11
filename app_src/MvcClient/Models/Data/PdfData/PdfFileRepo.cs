using MvcLib.Db;
using MvcLib.MainContent;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Models.Data.PdfData
{
    public interface IPdfFileRepo : IDataBase<PdfFile>
    {
        Task<int> DelUrl(int id);
        Task<int> remove(string[] fileIds);
        Task<bool> DelFile(string pth);
        Task<PdfFile> getFile(int id);
    }
    public class PdfFileRepo : IPdfFileRepo
    {
        private AppDbContext _db;

        public PdfFileRepo(AppDbContext appContext)
        {
            _db = appContext;
        }


        public Task<int> Add(PdfFile model)
        {
            var e = _db.pdf_upload_logs.Any(a => a.id.Equals(model.pdfId));
            if (e)
            {
                _db.pdf_files.Add(model);
            }

            return _db.SaveChangesAsync();

        }

        public Task<int> Add(Pdf_Url model)
        {
            var e = _db.pdf_upload_logs.Any(a => a.id.Equals(model.pdfId));
            if (e)
            {
                _db.pdf_urls.Add(model);
            }
            return _db.SaveChangesAsync();
        }

        /// <summary>
        /// del pdf file
        /// </summary>
        /// <typeparam name="KT"></typeparam>
        /// <param name="K"></param>
        /// <returns></returns>
        public Task<int> Del<KT>(KT K)
        {
            if (_db.pdf_files.Any(a => a.id.Equals(K)))
            {
                var m = _db.pdf_files.Find(K);
                _db.pdf_files.Remove(m);
            }
            return _db.SaveChangesAsync();
        }



        public Task<int> DelUrl(int id)
        {
            if (_db.pdf_urls.Any(a => a.id.Equals(id)))
            {
                var m = _db.pdf_urls.Find(id);
                _db.pdf_urls.Remove(m);
            }
            return _db.SaveChangesAsync();
        }

        public Task<PdfFile> Get<KT>(KT k)
        {
            var md = _db.pdf_files.Single(a => a.id.Equals(k));
            return Task.FromResult(md);

        }

        public Task<bool> DelFile(string pth)
        {
            throw new NotImplementedException();
        }

        public async Task<int> remove(string[] fileIds)
        {
            var tsk = new Task[fileIds.Length];
            for (int K = 0; K < fileIds.Length; K++)
            {
                if (_db.pdf_files.Any(a => a.id.Equals(K)))
                {
                    var m = _db.pdf_files.Find(K);
                    await DelFile(m.saving_path);
                    _db.pdf_files.Remove(m);
                }
            }

            return _db.SaveChanges();
        }


        Task<PdfFile> IPdfFileRepo.getFile(int id)
        {
            return Get(id);
        }

        public Task<bool> Existed(PdfFile model)
        {
            throw new NotImplementedException();
        }
    }
}
