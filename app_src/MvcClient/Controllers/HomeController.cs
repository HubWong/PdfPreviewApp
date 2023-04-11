using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using MvcClient.Models.Data;
using MvcClient.Models.Data.PdfData;
using MvcLib.Dto.PdfDtos;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeIndex _homeIndexVm;
        private IBindingRepo _bindingRepos;

        public HomeController(IHomeIndex homeIndexData, IBindingRepo bindingRepo)
        {
            _homeIndexVm = homeIndexData;
            _bindingRepos = bindingRepo;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "pdf&试卷";
            _homeIndexVm.GetDefaultVm();
            return View(_homeIndexVm.HomeIndexVm);
        }

        /// <summary>
        /// partial view when page index clicked for ajax.
        /// </summary>
        /// <param name="pdfTableDto"></param>
        /// <returns></returns>
        public async Task<IActionResult> _PdfListView(PagedPdfDto pdfTableDto)
        {
            await _homeIndexVm.GetPagedVm(pdfTableDto);
            return PartialView(_homeIndexVm);
        }

        /// <summary>
        /// get view by clicking top button
        /// not test yet
        /// </summary>
        /// <returns></returns>
        [HttpPost("home/default")]
        public async Task<IActionResult> PdfList(BindingRestDto bindingRestDto)
        {
            var topRest = await _bindingRepos.GetDownLevels(bindingRestDto);
            var pdfTableDto = new PagedPdfDto
            {
                menu = "home",
                bindid = _bindingRepos.BindId,
                pg = 1,
                orderby = nameof(MvcLib.MainContent.PdfUploadLog.id),
                isAsc = true,
                ttl = 1
            };

            var vm = await _homeIndexVm.GetPagedVm(pdfTableDto);
            _homeIndexVm.HomeIndexVm.Pdfs = vm.Pdfs;
            _homeIndexVm.HomeIndexVm.Pager = vm.Pager;
            _homeIndexVm.HomeIndexVm.BindData = topRest;
            _homeIndexVm.HomeIndexVm.BindId = pdfTableDto.bindid;

            return PartialView("_PdfListView", _homeIndexVm.HomeIndexVm);
        }



        public async Task<IActionResult> Pdf(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var pdf = await _homeIndexVm.GetByPdfId(id);
                if (pdf == null)
                {
                    return NotFound();
                }

                return View(pdf);
            }
            else
            {
                return Error();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorVm { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
