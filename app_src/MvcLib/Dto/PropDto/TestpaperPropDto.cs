using MvcLib.DbEntity.MainContent;
using System.ComponentModel.DataAnnotations;

namespace MvcLib.Dto.PropDto
{
    public class TestpaperPropDto
    {
        public enum properType
        {
            nf,
            sf
        }
        [Display(Name = "类型")]
        public properType nf_or_sf { get; set; }

        [Display(Name = "名称")]
        [Required] public string prop_name { get; set; }
    }

    public class TestpaperVm
    {
        public TestpaperVm()
        {
            IsAdd = 1;

        }
        public TestpaperVm(TestpaperProps testpaperProps)
        {
            id = testpaperProps.id;
            type = (int)testpaperProps.nianfen_or_shengfen;
            title = testpaperProps.value;
            IsAdd = 0;
        }
        [Display(Name = "类型")]
        public string ch_type
        {
            get
            {
                if (type == 0)
                {
                    return "年份";
                }
                else
                {
                    return "省份";
                }
            }
        }
        public int type { get; set; }
        [Display(Name = "ID")]
        public int id { get; set; }
        [Display(Name = "名称")]
        public string title { get; set; }

        public byte IsAdd { get; set; }


    }
}
