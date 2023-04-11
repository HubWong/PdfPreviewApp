using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Models.Data.PdfData;
using MvcClient.Models.PdfVms;
using MvcLib.Dto.PdfDtos;
using MvcLib.MainContent;
using MvcLib.Tools;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MvcClient.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : BasicApiController
    {
        private IBindingRepo _bindingRepos;
        private IPdfRepo _pdfRepo;
        private IPdfFileRepo _fileRepo;
        private IPdfUrlRepo _urlPreviewRepo;
        private string wwwFolder;

        public PdfController(IBindingRepo bindingRepo, 
            IPdfRepo pdfRepo, 
            IPdfFileRepo pdfFileRepo, 
            IWebHostEnvironment hostEnvironment, 
            IPdfUrlRepo urlRepo)
        {
            _bindingRepos = bindingRepo;
            _fileRepo = pdfFileRepo;
            wwwFolder = hostEnvironment.WebRootPath;
            _pdfRepo = pdfRepo;
            _urlPreviewRepo = urlRepo;
        }



        /// <summary>
        /// click a at the top in pdfpage 
        /// </summary>
        /// <param name="bindingRestDto"></param>
        /// <returns></returns>
        [HttpPost("rest")]
        public async Task<IActionResult> Post(BindingRestDto bindingRestDto)
        {
            var vm = new RgtBooksVm();
            string sidebar = "电子书列表";
            IDataReturnedVm dataView = new DataReturnedVm(sidebar, this._bindingRepos);

            vm.BindData = await _bindingRepos.GetDownLevels(bindingRestDto);
            var pdfTableDto = new PagedPdfDto
            {
                menu = sidebar,
                bindid = dataView.RightBindingView.BindId,
                pg = 1,
                orderby = nameof(PdfUploadLog.id),
                isAsc = true,
                ttl = 1
            };
            var listPdf = await dataView.RightBindingView.GetBindingTable(pdfTableDto, false);

            vm.BindingPdfTable = new BindingPdfTable(_bindingRepos.BindId)
            {
                BookTable = new TableVm<PdfUploadLog>(listPdf, sidebar, () => new TableButonCreator().GenButton(MvcLib.Constants.TableButtonTypes.btns_table_pdf))
            };

            vm.BindingPdfTable.BookTable.GenTable(sidebar, pdfTableDto.ttl, 1);
            RghtBookApi rghtBookApi = new()
            {
                BindData = vm.BindData,
                bid = _bindingRepos.BindId,
                Table = vm.BindingPdfTable.BookTable.TableDom,
            };
            return Ok(rghtBookApi);
        }

        /// <summary>
        /// get json data of paged table 
        /// </summary>
        /// <param name="pdfTableDto"></param>
        /// <returns></returns>
        [HttpPost("page")]
        public async Task<IActionResult> getPdfs(PagedPdfDto pdfTableDto)
        {
            if (pdfTableDto.bindid != 0)
            {
                var list = await _bindingRepos.GetBindingTable(pdfTableDto, false);
                var vm = new TableVm<PdfUploadLog>(list, pdfTableDto.menu, () => new TableButonCreator().GenButton(MvcLib.Constants.TableButtonTypes.btns_table_pdf));
                vm.GenTable(pdfTableDto.menu, pdfTableDto.ttl, pdfTableDto.pg);
                var v_m = new RghtBookApi
                {
                    BindData = null,
                    bid = pdfTableDto.bindid,
                    Table = vm.TableDom
                };
                return Ok(v_m);
            }
            return null;
        }



        /// <summary>
        /// form submit 
        /// </summary>
        /// <param name="pdfVm"></param>
        /// <returns></returns>
        public async Task<IActionResult> post([FromBody] PdfRowVm pdfVm)
        {
            int i = await _pdfRepo.AddOrUpdate(pdfVm);
            return Ok(i);
        }

        #region Get      
        //with the pattern: http://localhost:5002/api/pdf/attach/1 
        [HttpGet("attach/{id:int}")]
        public async Task<IActionResult> getPdfAttatches(int id)
        {
            PdfUploadLog data = await _pdfRepo.Get(id);

            var ret = new ApiResponse();
            foreach (var item in data.pdf_files)
            {
                ret.setNestedProp(item);
            }

            foreach (var item in data.pdf_Urls)
            {
                ret.setNestedProp(item);
            }

            return Ok(JsonConvert.SerializeObject(data, Formatting.Indented));
        }

        /// <summary>
        /// get pdf cover image or pdf files
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cvr"></param>
        /// <returns></returns>
        [HttpGet("files/{reqType:int}/{id:int}")]
        public async Task<IActionResult> getFiles(FilesType reqType, int id)
        {
            if (id == -1)
            {
                return Ok();
            }
            var mdl = await _pdfRepo.Get(id);
            if (mdl != null)
            {
                if (reqType == FilesType.main_cvr)
                { 
                    long lng = 0;
                    //if the cvr image is not null
                    if (!string.IsNullOrWhiteSpace(mdl.image_path))
                    {
                        var fe = new FileInfo(wwwFolder + mdl.image_path);
                        if (fe.Exists)
                        {
                            lng = fe.Length;
                            return Ok(new FileInfoVm(mdl.image_path, lng, mdl.title, mdl.id));
                        }                    
                    }
                    else
                    {
                        return Ok(0);
                    }

                   
                }
                else if (reqType == FilesType.attaches)
                {
                    List<FileInfoVm> list = new();
                    mdl.pdf_files.ForEach(a => list.Add(new FileInfoVm(a.saving_path, a.file_size, a.title, a.id)));
                    return Ok(list);
                }
                else if (reqType == FilesType.preview_url_cvr)
                {
                    List<FileInfoVm> list = new List<FileInfoVm>();
                    mdl.pdf_Urls.ForEach(a => list.Add(new FileInfoVm(a.image_path, Utility.FileSize(wwwFolder + a.image_path).Result, a.pdf_url + "[" + a.title + "]", a.id)));
                    return Ok(list);
                }
            }

            return Ok("");
        }
        #endregion
        #region Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            int c = 0;
            if (id.Contains(','))
            {
                string[] arr = id.Split(',');
                for (int i = 0; i < arr.Length; i++)
                {
                    if (int.TryParse(arr[i], out int y))
                    {
                        c += await _pdfRepo.Del(y, r => Utility.DelFiles(wwwFolder, r).Wait());
                    }
                }
            }
            else
            {
                if (int.TryParse(id, out int y))
                {
                    c = await _pdfRepo.Del(y, r => Utility.DelFiles(wwwFolder, r).Wait());
                }
            }

            return Ok(c);
        }

        [HttpDelete("cvr/{id:int}")]
        public async Task<IActionResult> DeleteCvr(int id)
        {
            var mdl = await _pdfRepo.Get(id);
            if (mdl != null)
            {
                Utility.DelFile(this.wwwFolder + mdl.image_path);
                mdl.image_path = string.Empty;
                return Ok(await _pdfRepo.UpdatePdf(mdl));
            }
            return Ok(1);
        }

        /// <summary>
        /// delete file , url preview or a whole upload log preview image of pdf.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("file/{type:int}/{id:int}")]
        public async Task<IActionResult> DeleteFile(FilesType type, int id)
        {
            List<string> path_to_del = new List<string>();
            int i = 0;
            if (type == FilesType.attaches)
            {
                var mdl = await _fileRepo.getFile(id);
                if(mdl == null)
                {
                    return Ok(0);
                }
                path_to_del.Add(wwwFolder + mdl.saving_path);
                i = await _fileRepo.Del(id);
            }     
            else if (type == FilesType.preview_url_cvr)
            {
                var mdl = await _urlPreviewRepo.Get(id);
                path_to_del.Add(wwwFolder + mdl.image_path);
                if (mdl == null)
                {
                    return Ok(0);
                }

                i = await _urlPreviewRepo.Del(id);
            }
            else if(type==FilesType.all) //del the whole pdf record
            {
                var mdl = await _pdfRepo.Get(id);

                if (mdl == null)
                {
                    return Ok(0);
                }

                foreach (var item in mdl.pdf_files)
                {
                    path_to_del.Add(wwwFolder + item.saving_path);
                }

                foreach (var item in mdl.pdf_Urls)
                {
                    path_to_del.Add(wwwFolder + item.image_path);
                }

                mdl.image_path = string.Empty;
                i = await _pdfRepo.UpdatePdf(mdl);
            }
            Utility.DelFiles(path_to_del);
            return Ok(i);
        }
    }

    #endregion

}
