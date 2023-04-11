using MvcClient.Models.Data.PdfData;
using MvcLib;
using MvcLib.Dto;
using MvcLib.MainContent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MvcClient.Models.PdfVms
{
    /// <summary>
    /// list all kind of itmcatgs
    /// </summary>
    public class BindingVm : IBindingVm
    {
        public BindingVm(IBindingRepo bindingRepo)
        {
            BindingRepo = bindingRepo;
            List<ItemCategory> list = BindingRepo.GetCategList();
            ItemCategories_Banben = list.Where(a => a.category == categoryType.ban_ben).ToList();
            ItemCategories_LeiXing = list.Where(a => a.category == categoryType.lei_xing).ToList();
            ItemCategories_Mokuai = list.Where(a => a.category == categoryType.mo_kuai).ToList();
            ItemCategories_Xueke = list.Where(a => a.category == categoryType.xue_ke).ToList();
        }

        public IBindingRepo BindingRepo { get; set; }

        public Dictionary<categoryType, List<ItemCategory>> Typed_Categories
        {
            get
            {
                var dic = new Dictionary<categoryType, List<ItemCategory>>();
                dic.Add(categoryType.ban_ben, ItemCategories_Banben);
                dic.Add(categoryType.lei_xing, ItemCategories_LeiXing);
                dic.Add(categoryType.mo_kuai, ItemCategories_Mokuai);
                dic.Add(categoryType.xue_ke, ItemCategories_Xueke);
                return dic;

            }
        }


        #region Item Props
        private List<ItemCategory> ItemCategories_LeiXing
        {
            get;
        }

        private List<ItemCategory> ItemCategories_Mokuai
        {
            get;
        }

        private List<ItemCategory> ItemCategories_Xueke
        {
            get;
        }

        private List<ItemCategory> ItemCategories_Banben
        {
            get;
        }

        #endregion


    }

    public interface IBindingVm
    {
        Dictionary<categoryType, List<ItemCategory>> Typed_Categories
        {
            get;
        }
    }
}