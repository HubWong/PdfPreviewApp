using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Models.Data;
using MvcClient.Models.Data.PdfData;
using MvcClient.Models.PdfVms;
using MvcLib;
using MvcLib.Dto.PdfDtos;
using MvcLib.MainContent;
using MvcLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Controllers
{
    /// <summary>
    /// view of dashboard
    /// </summary>
    public class DashboardController : BaseController
    {   
        private IBindingRepo _bindingRepo;
        private IPdfRepo _pdfRepo;
        private IItemCategoryRepo _categoryRepo;
        private IBindingVm _bindingVm;
        private IDataReturnedVm dataReturnedVm;

        public DashboardController(IMenuModuleRepo moduleRepo,
            IItemCategoryRepo itemCategory,
            IPdfRepo pdfRepo,
            IBindingVm bindingVm,
            IBindingRepo bindingRepo) : base(moduleRepo)
        {
            _bindingVm = bindingVm;
            _categoryRepo = itemCategory;       
            _bindingRepo = bindingRepo;
            _pdfRepo = pdfRepo;
        }



        /// <summary>
        /// pdf page starts.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ConsoleModuleVm indexVm = new ConsoleModuleVm();
            indexVm.ConsoleTitle = "Pdf";
            ViewData["Title"] = indexVm.ConsoleTitle;
            indexVm.Sidebars = base.SortedMenus.Take(2).ToList();
            return View(indexVm);
        }

        #region main content

        public IActionResult _RgtTblView(PagingVm p)
        {
            if (string.IsNullOrEmpty(p.menu))
            {
                return BadRequest("sidebar is null");
            }
            int x = 0;
            var r = _categoryRepo.GetCategList(p.menu, p.orderby, p.pg, out x, p.isAsc);
            var tableVm = new TableVm<ItemCategory>(r.ToList(), p.menu, () => new TableButonCreator().GenButton(Constants.TableButtonTypes.btns_table_category), "table table-bordered table-dark");
            tableVm.GenTable(p.menu, x, p.pg);
            return PartialView(tableVm.TableDom);
        }

        /// <summary>
        /// item category table view with paged model
        /// 2021.12.22 停止开发，转向sscms留言开发
        /// 2021-12-28 11--16时左右留言开放结束，发送主任
        /// </summary>
        /// <param name="sidebar"></param>
        /// <param name="pg"></param>
        /// <returns></returns>
        public IActionResult _RgtBindView()
        {
            return PartialView(_bindingVm.Typed_Categories);
        }

        /// <summary>
        /// add or edit View of Itemcategory
        /// </summary>
        /// <param name="itemCategory"></param>
        /// <returns></returns>
        public async Task<IActionResult> _CategoryFormView(string id)
        {
            var Vm = new ItemCategoryVm();

            if (id == null)  //add 
            {
                GenSelectList(Vm.DicCategories);
                ViewBag.Categ = SelectListItems;
                return PartialView(Vm);
            }
            else
            {
                if (int.TryParse(id, out int k))
                {
                    var data = await _categoryRepo.Get(k);
                    if (data != null) //edit
                    {
                        GenSelectList(Vm.DicCategories, (int)data.category);
                        ViewBag.Categ = SelectListItems;
                        Vm.id = data.id;
                        Vm.isShow = data.isShow;
                        Vm.make_day = data.make_day;
                        Vm.memo = data.memo;
                        Vm.orderNo = data.orderNo;
                        Vm.title = data.title;

                        return PartialView(Vm);
                    }
                }

            }
            return BadRequest();
        }

        [Route("pdf/attach/{id:int}")]
        public IActionResult _PdfAttachsView(int id)
        {
            return PartialView();
        }

        /// <summary>
        /// catched!! add or edit pdf view.
        /// submit fn is in pdf api ctlr.
        /// </summary>6
        /// <returns></returns>    
        [Route("pdf/{bid:int}/{id?}")]
        public async Task<IActionResult> _pdfEditView(int bid, string id)
        {
            if (bid == -1 && string.IsNullOrEmpty(id) || bid == 0)
            {
                return PartialView("error", new ErrorVm
                {
                    msg = "need argument :bid and key to get view",
                    RequestId = HttpContext.Request.Host + HttpContext.Request.Path
                });
            }

            var vm = new PdfRowVm();
            if (string.IsNullOrWhiteSpace(id)) //add
            {
                vm.bindingId = bid;
                vm.IsAdd = 1;
                return PartialView(vm);
            }
            else
            {
                PdfUploadLog pdf = await _pdfRepo.Get(int.Parse(id));
                vm.IsAdd = 0;
                vm.id = int.Parse(id);
                vm.bindingId = pdf.bindingId;
                vm.make_day = pdf.make_day;
                vm.orderNo = pdf.orderNo;
                vm.title = pdf.title;
                vm.image_path = pdf.image_path;
                vm.update_day = pdf.update_day;
                vm.memo = pdf.memo;
                vm.isShow = pdf.isShow;
                vm.pdf_files = pdf.pdf_files;
                vm.pdf_Urls = pdf.pdf_Urls;
                return PartialView(vm);
            }
        }

        /// <summary>
        /// ajax get bind id with its table
        /// </summary>
        /// <returns></returns>
        public IActionResult GetBindTable(PagedPdfDto pdfTableDto)
        {
            BindingPdfTable bindingPdfTable = new BindingPdfTable(pdfTableDto.bindid);
            return Ok(bindingPdfTable);
        }


        /// <summary>
        /// default/index 电子书列表
        /// initial view.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> _RgtBooksView(PagedPdfDto pdfTableDto)
        {
            var vm = new RgtBooksVm();
            dataReturnedVm = new DataReturnedVm(pdfTableDto.menu, _bindingRepo);
            var topLx = await dataReturnedVm.RightBindingView.GetTopLeixings();
            if (topLx.Count == 0)
            {
                var tbl = new TableVm<PdfUploadLog>(null, pdfTableDto.menu, () => new TableButonCreator().GenButton(Constants.TableButtonTypes.btns_table_pdf));
                tbl.GenTable(pdfTableDto.menu, 0, 1);
                var pdfTable = new BindingPdfTable(-1) { BookTable = tbl };
                vm.BindingPdfTable = pdfTable;
            }
            else
            {
                try
                {
                    Dictionary<categoryType, List<ItemCategory>> dicTopRestData = await dataReturnedVm.RightBindingView.GetDownLevels(new BindingRestDto
                    {
                        clk = "lx_" + topLx.First().id,
                        leixing_id = topLx.First().id
                    });

                    dicTopRestData.Add(categoryType.lei_xing, topLx);
                    BindingPdfTable bindingPdfTable;
                    //if there are binded table data.
                    if (dataReturnedVm.RightBindingView.BindId > 0)
                    {
                        pdfTableDto.bindid = dataReturnedVm.RightBindingView.BindId;

                        var listPdf = await dataReturnedVm.RightBindingView.GetBindingTable(pdfTableDto,false);

                        bindingPdfTable = new BindingPdfTable(dataReturnedVm.RightBindingView.BindId)
                        {
                            BookTable = new TableVm<PdfUploadLog>(listPdf, pdfTableDto.menu,
                            () => new TableButonCreator().GenButton(Constants.TableButtonTypes.btns_table_pdf), "table table-striped")
                        };

                        bindingPdfTable.BookTable.GenTable(pdfTableDto.menu, pdfTableDto.ttl, 1);
                        vm.BindData = dicTopRestData;
                        vm.BindingPdfTable = bindingPdfTable;
                    }
                    else
                    {
                        vm.BindData = dicTopRestData;
                        vm.BindingPdfTable = new BindingPdfTable(0)
                        {
                            BookTable = new TableVm<PdfUploadLog>(new List<PdfUploadLog>(), pdfTableDto.menu,
                            () => new TableButonCreator().GenButton(Constants.TableButtonTypes.btns_table_pdf))
                        };
                    }

                }
                catch (Exception)
                {
                    return PartialView("Error");
                }


            }
            return PartialView(vm);
        }


        #endregion

    }
}