using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Models.Data.PdfData;
using MvcLib.Dto.PdfDtos;
using MvcLib.MainContent;
using MvcLib.Tools;
using System;
using System.IO;
using System.Threading.Tasks;
using UEditor.Core;

namespace MvcClient.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : BasicApiController
    {
        private UEditorService _ueditorService;
        private IPdfFileRepo _pdfFileRepo;
        private IPdfUrlRepo _pdfUrlRepo;
        private IPdfRepo _pdfRepo;

        public UploadController(UEditorService uEditorService, IPdfUrlRepo pdfUrlRepo, IPdfFileRepo pdfFileRepo, IPdfRepo pdfRepo, IWebHostEnvironment hostEnvironment)
        {
            _ueditorService = uEditorService;
            _pdfUrlRepo = pdfUrlRepo;
            _pdfFileRepo = pdfFileRepo;
            _pdfRepo = pdfRepo;
            _env = hostEnvironment;
        }


        public string Cur_Folder
        {
            get
            {
                return _env.WebRootPath;
            }
        }

        #region general file uploading

        public static string FormatBytes(long bytes)
        {
            string[] Suffix = { "Byte", "KB", "MB", "GB", "TB" };
            int i = 0;
            double dblSByte = bytes;
            if (bytes > 1024)
                for (i = 0; (bytes / 1024) > 0; i++, bytes /= 1024)
                    dblSByte = bytes / 1024.0;
            return String.Format("{0:0.##}{1}", dblSByte, Suffix[i]);
        }

        /// <summary>
        /// saving file uploaded with pdf id.
        /// </summary>
        /// <param name="pdfFileDto"></param>
        /// <returns>if cvr, returns image path, else returns changes count</returns>
        [HttpPost("fileupload")]
        public async Task<IActionResult> fileUpload([FromForm] PdfDto pdfDto)
        {
            var dataReturn = new ApiData();
            string filePath;
            if (pdfDto.file.Length > 0 && pdfDto.pdf_id != -1)
            {
                CreateDir(Cur_Folder + this.saving_dir);
                filePath = Path.Combine(saving_dir,
                     Path.GetRandomFileName() + Path.GetExtension(pdfDto.file.FileName)).Replace("\\","/");

                using (var stream = System.IO.File.Create(Cur_Folder + filePath))
                {
                    await pdfDto.file.CopyToAsync(stream);
                }
            }
            else
            {
                throw new ArgumentException("pdfid error or no file uploaded");
            }

            if (pdfDto.uploadType == FilesType.main_cvr) //uploading cvr image.
            {
                if (pdfDto.pdf_id != 0)
                {
                    var mdl = await _pdfRepo.Get(pdfDto.pdf_id);  //del and update
                    if (mdl != null)
                    {
                        Utility.DelFile(mdl.image_path);
                    }
                    mdl.image_path = filePath;
                    await _pdfRepo.UpdatePdf(mdl);
                    dataReturn.id = mdl.id;
                }

            }

            else if (pdfDto.uploadType == FilesType.attaches)
            {
                PdfFile pdfFile = new PdfFile();
                pdfFile.pdfId = pdfDto.pdf_id;
                pdfFile.saving_path = filePath;
                pdfFile.make_day = DateTime.Now;
                pdfFile.title = pdfDto.file.FileName;
                pdfFile.file_type = Path.GetExtension(pdfDto.file.FileName);
                pdfFile.file_size = pdfDto.file.Length;
                await _pdfFileRepo.Add(pdfFile);
                dataReturn.id = pdfFile.id;
            }
            else if (pdfDto.uploadType == FilesType.preview_url_cvr)
            {
                Pdf_Url pdf_Url = new Pdf_Url();
                pdf_Url.pdfId = pdfDto.pdf_id;
                pdf_Url.title = pdfDto.title;
                pdf_Url.image_path = filePath;
                pdf_Url.make_day = DateTime.Now;
                pdf_Url.pdf_url = pdfDto.destUrl;
                await _pdfUrlRepo.Add(pdf_Url);
                dataReturn.id = pdf_Url.id;              
            }
            dataReturn.status = 200;
            dataReturn.data = filePath;
            return Ok(dataReturn);
        }

        #endregion


        #region Pdf RichText editor
        /// <summary>
        /// accept type:doc docx pptx ppt zip rar
        /// </summary>
        /// <param name="formFiles"></param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public IActionResult upload()
        {
            var response = _ueditorService.UploadAndGetResponse(HttpContext);
            return Ok(response.Result);
        }

        #endregion

    }
}
