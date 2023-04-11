using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcLib.Dto.PdfDtos
{
    public class PagedPdfDto : PagedModelDto
    {
        public int bindid { get; set; }
    }


    public enum FilesType
    {
        main_cvr,
        attaches,
        preview_url_cvr,
        all
    }

    public class PdfDto
    {
        [Required]
        public int pdf_id { get; set; }
        public FilesType uploadType { get; set; }
        public IFormFile file { get; set; }
        public string destUrl { get; set; }
        public string title { get; set; }
    }


    public struct FileInfoVm
    {
        public FileInfoVm(string p, long s, string title, int id)
        {
            path = p;
            size = s;
            name = title;
            this.id = id;
        }

        public string path { get; }
        public long size { get; }
        public string name { get; set; }
        public int id { get; }
    }
}
