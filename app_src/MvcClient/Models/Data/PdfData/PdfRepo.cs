using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MvcClient.Models.PdfVms;
using MvcLib.Db;
using MvcLib.MainContent;
using MvcLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Models.Data.PdfData
{
    public interface IPdfRepo : IRightPdfRepo
    {
        //add or update pdf model.
        Task<int> AddOrUpdate(PdfRowVm pdfVm);
        Task<int> UpdatePdf(PdfUploadLog pdf);
        Task<int> Del<KT>(KT K, Action<List<string>> ActionDelFiles);
    }

    public class PdfRepo : IPdfRepo
    {
        private AppDbContext _db;
        public DbSet<PdfUploadLog> Pdfs { get; private set; }

        private IItemCategoryRepo CategRepo;
        private readonly string _wwwFolder;

        public IBindingRepo BindingRepo { get; private set; }

        public int Bindid { get; set; }

        public PdfRepo(AppDbContext appDbContext,
            IItemCategoryRepo categoryRepo,
            IWebHostEnvironment webHostEnvironment
           )
        {
            _db = appDbContext;
            Pdfs = _db.pdf_upload_logs;
            CategRepo = categoryRepo;
            _wwwFolder = webHostEnvironment.WebRootPath;
        }

        public Task<int> Add(PdfUploadLog model)
        {
            if (Pdfs.Any(a => a.id.Equals(model.id)))
            {
                Pdfs.Update(model);
            }
            else
            {
                Pdfs.Add(model);
            }
            return _db.SaveChangesAsync();
        }

        public Task<int> Add(PdfRowVm pdfVm)
        {
            return Add(pdfVm);
        }


        public Task<bool> Existed(PdfUploadLog model)
        {
            return Pdfs.AnyAsync(a => a.title.Equals(model.title) || a.bindingId.Equals(model.bindingId));

        }

        public Task<PdfUploadLog> Get<KT>(KT k)
        {
            var m = from n in _db.pdf_upload_logs.Include(a => a.pdf_files).Include(a => a.pdf_Urls)
                    where n.id.Equals(k)
                    select n;
            return m.FirstOrDefaultAsync();
        }

        public Task<List<PdfUploadLog>> GetAll()
        {
            return Pdfs.ToListAsync();
        }

        public Task<List<PdfUploadLog>> PagedData(Func<PdfUploadLog, bool> func, out int ttl, int pg = 1)
        {
            var p = Pdfs.Where(func);
            ttl = p.Count();
            var list = p.Skip((pg - 1) * 10).Take(10).ToList();
            return Task.FromResult(list);
        }

        public Task<RgtBooksVm> GetRgtBooksVm(int bindid, out int ttl, string orderby = "makeDay", int pg = 1)
        {
            var p = Pdfs.Where(o => o.bindingId.Equals(bindid)).OrderBy(a => a.make_day);
            ttl = p.Count();
            var list = p.Skip((pg - 1) * 10).Take(10).ToList();
            if (list != null)
            {

            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// add or update
        /// </summary>
        /// <param name="pdfVm"></param>
        /// <returns>model id</returns>
        public async Task<int> AddOrUpdate(PdfRowVm pdfVm)
        {
            if (await Existed(pdfVm))
            {
                pdfVm.update_day = DateTime.Now;
                _db.pdf_upload_logs.Update(pdfVm);
            }
            else
            {
                _db.pdf_upload_logs.Add(pdfVm);
            }
            await _db.SaveChangesAsync();

            return pdfVm.id;
        }

        public async Task<int> UpdatePdf(PdfUploadLog pdf)
        {
            if (await Existed(pdf))
            {
                _db.pdf_upload_logs.Update(pdf);
                return await _db.SaveChangesAsync();
            }
            return 0;
        }

        private Task Del_MultipuleFiles(PdfUploadLog pdf)
        {
            foreach (var item in pdf.pdf_files)
            {
                Utility.DelFile(_wwwFolder + item.saving_path);
            }
            Utility.DelFile(_wwwFolder + pdf.image_path);
            return Task.CompletedTask;
        }


        public async Task<int> Del<KT>(KT K)
        {
            var m = await Get(K);
            if (m != null)
            {
                await Del_MultipuleFiles(m);
                Pdfs.Remove(m);
            }
            return _db.SaveChanges();
        }


        public static Task DelPdfFiles(List<PdfUploadLog> pdfs, Action<List<string>> ActionPathsToDel)
        {
            var ListPath = new List<string>();
            ListPath.AddRange(pdfs.Select(a => a.image_path));
            foreach (var item in pdfs)
            {
                ListPath.Add(item.image_path);

                if (item.pdf_files != null && item.pdf_files.Count > 0)
                {
                    foreach (var file in item.pdf_files)
                    {
                        ListPath.Add(file.saving_path);
                    }
                }

                if (item.pdf_Urls != null && item.pdf_Urls.Count > 0)
                {
                    foreach (var url in item.pdf_Urls)
                    {
                        ListPath.Add(url.image_path);
                    }
                }
            }

            ActionPathsToDel(ListPath);
            return Task.CompletedTask;
        }
        public async Task<int> Del<KT>(KT K, Action<List<string>> ActionDelFiles)
        {
            var m = await Get(K);
            if (m != null)
            {
                Pdfs.Remove(m);
                await DelPdfFiles(new List<PdfUploadLog> { m }, ActionDelFiles);
            }
            return _db.SaveChanges();

        }
    }
}
