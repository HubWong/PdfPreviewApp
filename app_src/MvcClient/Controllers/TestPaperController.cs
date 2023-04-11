using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Models.Data;
using MvcClient.Models.TestpaperVms;
using MvcLib.DbEntity.MainContent;
using MvcLib.Dto;
using MvcLib.Dto.ColumnDto;
using MvcLib.Dto.PropDto;
using MvcLib.Dto.UploadDto;
using MvcLib.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MvcClient.Controllers
{
    /*
     * 2022.08.13 new project of director Gng :
         upload test paper with props of 省份，年份 
        test paper also need to belong to one column(栏目) with unlimited levels.    
     */
    public class TestPaperController : BaseController
    {
        public IColumnDataRepo ColumRepo { get; }

        private IPropRepo TestpaperPropRepo;
        protected ITestpaperUploadRepo UploadRepo;

        public TestPaperController(IMenuModuleRepo moduleRepo,
            IColumnDataRepo columnDataDto,
            IPropRepo testpaperRepo,
            ITestpaperUploadRepo uploadRepo) : base(moduleRepo)
        {
            ColumRepo = columnDataDto;
            UploadRepo = uploadRepo;
            TestpaperPropRepo = testpaperRepo;
        }


        public IActionResult Index()
        {
            ViewData["Title"] = "试卷";
            ConsoleModuleVm indexVm = new ConsoleModuleVm();
            indexVm.ConsoleTitle = "试卷";
            indexVm.Sidebars = base.SortedMenus.Skip(2).ToList();
            return View(indexVm);
        }

        #region Columns

        /// <summary>
        /// cln table .
        /// </summary>
        /// <returns></returns>   
        public async Task<IActionResult> ColumnList(int pid = 0, int pg = 1)
        {          
            //get column table data.
            var clmData = await this.ColumRepo.GetAll();
            ViewBag.rootsData = this.ColumRepo.GetColumnVms(clmData);
            return PartialView();
        }

        /// <summary>
        /// cln table .
        /// </summary>
        /// <returns></returns>   
        public async Task<IActionResult> ColumnEdit(int id = 0)
        {
            var model = await ColumRepo.Get(id);
            var fm = new ColumnFormVm
            {
                IsAdd = 0,
                Id = id,
                Name = model.Name,
                pid = model.pid
            };

            ViewBag.rootsData = this.getClms();
            return PartialView(fm);
        }


        public IActionResult ColumnCreate(int pid = 0)
        {
            var fm = new ColumnFormVm
            {
                IsAdd = 1,
                pid = pid
            };
            ViewBag.rootsData = this.getClms(pid);
            return PartialView("ColumnEdit", fm);
        }

      
        private List<ColumnVm> getClms(int pid = 0)
        {
            var listVm = this.ColumRepo.GetLevelData(pid);
            return listVm;
        }
        #endregion


        #region Property of Sf and Nf
        /// <summary>
        /// get shengfen or nianfen 
        /// </summary>
        /// <param name="pg"></param>
        /// <param name="categ">sf / nf</param>
        /// <returns></returns>
        [Route("testpaper/props/{categ}")]
        public async Task<IActionResult> PropertyList(string categ = "sf",
            PagedModelDto pageModel = null)
        {
            var list = new List<TestpaperProps>();
            int t = pageModel.ttl;
            var listVm = new List<TestpaperVm>();
            if (categ.Equals("sf"))
            {
                list = await this.TestpaperPropRepo.PagedData(a => a.nianfen_or_shengfen.Equals(TestpaperPropDto.properType.sf), out t, pageModel.pg);
                list.ForEach(a => listVm.Add(new TestpaperVm(a)));
            }
            if (categ.Equals("nf"))
            {
                list = await this.TestpaperPropRepo.PagedData(a => a.nianfen_or_shengfen.Equals(TestpaperPropDto.properType.nf), out t, pageModel.pg);
                list.ForEach(a => listVm.Add(new TestpaperVm(a)));
            }

            string sidebar = categ == "sf" ? "省份" : "年份";
            ITableVm tableVm = new TableVm<TestpaperVm>(listVm, sidebar, () =>
            {
                return new TableButonCreator().GenButton(MvcLib.Constants.TableButtonTypes.btns_table_testpaper_props);
            }, TableCss: "bootstrap-table table");
            tableVm.GenTable(sidebar, t, pageModel.pg);
            return PartialView(tableVm.TableDom);
        }

        public async Task<IActionResult> PropertyEdit(int? id = null)
        {
            if (id.HasValue)
            {
                var m = await this.TestpaperPropRepo.Get(id);

                return PartialView(m.Vm);
            }
            else
            {
                return PartialView(new TestpaperVm());
            }

        }
        #endregion

        #region Uploading

        /// <summary>
        /// edit or add
        /// </summary>
        /// <returns></returns>
        //[HttpGet("/{id:int}")]
        public IActionResult UploadEdit(int id=-1)
        {
            var vm = new UploadTPVm(this.TestpaperPropRepo, this.ColumRepo,this.UploadRepo,id==-1?(byte)1:(byte)0);
       
            return PartialView(vm);
        }

        /// <summary>
        /// cln table .not used yet
        /// </summary>
        /// <returns></returns>
        public IActionResult UploadPage(string id)
        {
            return PartialView();
        }

        /// <summary>
        /// page of upload list
        /// </summary>
        /// <param name="pagedModelDto"></param>
        /// <returns></returns>
        public async Task<IActionResult> UploadFileList(PagedModelDto pagedModelDto)
        {
            int ttl = pagedModelDto.ttl;
            List<TestpaperUpload> rawList =await this.UploadRepo.PagedData(a=>a.id>0, out ttl, pagedModelDto.pg);
            var vmList = new List<UploadVm>();
            rawList.ForEach(a =>vmList.Add(a.Vm));
            
            var tableUploaded = new TableVm<UploadVm>(
                vmList, pagedModelDto.menu, () =>
            {
                return new TableButonCreator().GenButton(MvcLib.Constants.TableButtonTypes.btns_table_category);
            });

            tableUploaded.GenTable(pagedModelDto.menu,ttl,pagedModelDto.pg);
            ITableDom tableDom = tableUploaded.TableDom;
            return PartialView(tableDom);
        }
        #endregion



    }
}
