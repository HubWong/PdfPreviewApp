using MvcClient.Models.Data.PdfData;
using MvcClient.Models.Home;
using MvcLib.Dto.PdfDtos;
using MvcLib.MainContent;
using MvcLib.Tools;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Models.Data
{
    public interface IHomeIndex
    {
        HomeIndexVm HomeIndexVm { get; }
        void GetDefaultVm();
        Task<PdfListVm> GetPagedVm(PagedPdfDto pdfTableDto);
        Task<PdfUploadLog> GetByPdfId(string id);
    }

    public class HomeRepo : IHomeIndex
    {
        private readonly IBindingRepo _bindRepo;

        public HomeRepo(IBindingRepo bindingRepo)
        {
            _bindRepo = bindingRepo;
            HomeIndexVm = new HomeIndexVm();
        }

        public HomeIndexVm HomeIndexVm { get; private set; }

        public async Task<PdfListVm> GetPagedVm(PagedPdfDto pdfTableDto)
        {
            PdfListVm pdfListVm = new()
            {
                Pdfs = await _bindRepo.GetBindingTable(pdfTableDto, true),
                Pager = Utility.PageDom("homePdf", pdfTableDto.ttl, pdfTableDto.pg, 10)
            };
            return pdfListVm;
        }


        public async void GetDefaultVm()
        {
            var topLx = await _bindRepo.GetTopLeixings();
            if (topLx != null && topLx.Count != 0)
            {
                HomeIndexVm.BindData = await _bindRepo.GetDownLevels(new BindingRestDto
                {
                    clk = "lx_" + topLx.First().id,
                    leixing_id = topLx.First().id
                });

                HomeIndexVm.BindData.Add(categoryType.lei_xing, topLx);
                HomeIndexVm.BindId = _bindRepo.BindId;

                var pdfTableDto = new PagedPdfDto()
                {
                    bindid = _bindRepo.BindId,
                    pg = 1,
                    orderby = nameof(PdfUploadLog.id),
                    isAsc = true,
                    ttl = 1
                };

                if (HomeIndexVm.BindId > 0)
                {
                    var list = await _bindRepo.GetBindingTable(pdfTableDto, true);
                    var page = Utility.PageDom("homePdf", pdfTableDto.ttl, 1);
                    HomeIndexVm.Pdfs = list;
                    HomeIndexVm.Pager = page;
                }
            }

        }

        public Task<PdfUploadLog> GetByPdfId(string id)
        {
            return _bindRepo.Pdfs.Get(int.Parse(id));
        }
    }
}
