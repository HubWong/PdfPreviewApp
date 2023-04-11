using MvcLib;
using MvcLib.MainContent;
using MvcLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcClient.Models.PdfVms
{
    public class ItemCategoryVm : ItemCategory, IFormViewModel
    {
        public ItemCategoryVm()
        {

        }

        public byte IsAdd
        {
            get
            {
                if (id.Equals(0))
                {
                    return 1;
                }
                return 0;
            }
        }


        public int DaysAgo
        {
            get
            {
                if (update_day != null)
                    return DateTime.Now.Subtract(update_day.Value).Days;
                else
                {
                    return DateTime.Now.Subtract(make_day).Days;
                }
            }
        }

        public string TimeStr
        {
            get
            {
                if (IsAdd == 1)
                {
                    return DateTime.Now.ToString("D");
                }
                else
                {
                    return DaysAgo + "天前";
                }
            }
        }

        public Dictionary<int, string> DicCategories
        {
            get
            {
                Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
                var arr = Enum.GetValues(typeof(categoryType))
                    .Cast<categoryType>();
                for (int i = 0; i < 4; i++)
                {
                    keyValuePairs.Add(i, arr.ToArray()[i].EnumExtension());
                }
                return keyValuePairs;
            }
        }


    }
}
