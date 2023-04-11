using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Controllers.Api;
using MvcClient.Models.TestpaperVms;
using MvcClient.Models;
using MvcLib.DbEntity.MainContent;
using MvcLib.DbEntity;
using MvcLib.Dto.ColumnDto;
using MvcLib.Dto.PropDto;
using MvcLib.Dto.UploadDto;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace MvcClient.Controllers.TestpaperApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestPaperController : BasicApiController
    {
 
        private IColumnDataRepo _ColumnDb;
        private IPropRepo testpaperPropRepo;
        protected ITestpaperUploadRepo uploadRepo;
        private readonly IFileRepos fileRepos;

        public TestPaperController(IColumnDataRepo columnDataDto,
            IPropRepo testpaperPropRepo,
            ITestpaperUploadRepo testpaperUploadRepo,
            IWebHostEnvironment env, IFileRepos fileRepos)
        {
            _ColumnDb = columnDataDto;
            uploadRepo = testpaperUploadRepo;
            this.fileRepos = fileRepos;
            this.testpaperPropRepo = testpaperPropRepo;
            this._env = env;
        }

        #region Columns

        /// <summary>
        /// get all children of parents
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("children/{id?}")]
        public async Task<IActionResult> children(int id = -1)
        {
            var list = await _ColumnDb.GetChildren(id);
            if (list != null)
            {
                var vms = new List<ColumnVm>();
                for (int i = 0; i < list.Count; i++)
                {
                    vms.Add(list[i].Vm);
                }
                return Ok(vms);
            }
            return Ok(list);
        }

        [HttpGet]
        public async Task<IActionResult> get(int id = -1, int pg = 1)
        {
            return Ok(await _ColumnDb.PagedData(a => a.Pid.Equals(id), out int ttl, pg));
        }

        [HttpPost]
        public async Task<IActionResult> post(ColumnDataDto columnDataDto)
        {
            columnDataDto.Maker = User.Identity.Name ?? "sys";
            return Ok(await _ColumnDb.Add(columnDataDto));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> del(int id)
        {
            return Ok(await _ColumnDb.Del(id));
        }

        [HttpPut]
        public async Task<IActionResult> put(ColumnFormVm columnDataDto)
        {
            if (!ModelState.IsValid)
            {
                throw new BadHttpRequestException("data error");
            }

            if (columnDataDto.IsAdd == 1)
            {
                return Ok(await _ColumnDb.Add(columnDataDto));
            }
            else
            {
                return Ok(await _ColumnDb.Update(columnDataDto));
            }
        }
        #endregion


        #region Propery
        [HttpGet("props/{id}/{pg}")]
        public async Task<IActionResult> getProperty(int id = 1, int pg = 1)
        {
            TestpaperPropDto.properType type = (TestpaperPropDto.properType)id;

            return Ok(await this.testpaperPropRepo.PagedData(a => a.nianfen_or_shengfen.Equals(type), out int ttl, pg));
        }

        //delete property.
        [HttpDelete("props/{id:int}")]
        public async Task<IActionResult> delprop(int id)
        {
            return Ok(await this.testpaperPropRepo.Del(id));
        }

        /// <summary>
        /// add new props in db
        /// </summary>
        /// <param name="tstPropDto"></param>
        /// <returns></returns>
        [HttpPost("props")]
        public async Task<IActionResult> postProperty(TestpaperPropDto tstPropDto)
        {
            TestpaperProps testpaperProps = new TestpaperProps
            {
                nianfen_or_shengfen = tstPropDto.nf_or_sf,
                value = tstPropDto.prop_name
            };
            return Ok(await this.testpaperPropRepo.Add(testpaperProps));
        }
        #endregion


        #region upload page
        /// <summary>
        /// upload not test yet???????
        /// </summary>
        /// <param name="formVmDto"></param>
        /// <returns></returns>
        [HttpPost("tpUploadEdit")]
        public async Task<IActionResult> uploadpaper([FromBody] TestpaperFormVmDto formVmDto)
        {
            if (ModelState.IsValid)
            {
                TestpaperUpload testpaperUpload = new TestpaperUpload
                {
                    nfId = formVmDto.nf,
                    make_day = DateTime.Now,
                    sfId = formVmDto.sf,
                    columnId = formVmDto.pid
                };

                if (formVmDto.isAdd == 1)
                {
                    await uploadRepo.Add(testpaperUpload);
                }
                else
                {
                    await uploadRepo.update(testpaperUpload);
                }

                var saveFile = new SaveFileUploaded(base.saving_dir, formVmDto.formfiles, this.fileRepos.AddorUpdatePath);

                await saveFile.Save();
                return Ok(saveFile.fileDbModels);

            }
            return BadRequest();
        }

        private async Task<List<string>> SaveFiles(List<IFormFile> files)
        {
            var paths = new List<string>();
            foreach (var item in files)
            {
                paths.Add(await save_file(item, _env));
            }
            return paths;
        }

        /// <summary>
        /// testpaper upload multipully  
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("tpUpload")]
        public async Task<IActionResult> uploadTestppr([FromForm] List<IFormFile> file)
        {
            if (file != null)
            {
                return Ok(await SaveFiles(file));
            }
            return BadRequest();
        }

        #endregion
    }
}
