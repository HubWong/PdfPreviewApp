using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcLib.Dto.PdfDtos
{
    public class BindingDto
    {
        //public int Type { get; set; }
        [Required]
        public string Selected_Id { get; set; }
        public string Upper_Ids { get; set; } //i.e: xk_1, xd_2
    }

    public class BindingBaseDto
    {
        [Required] public int leixing_id { get; set; }
        [Required] public int mokuai_id { get; set; }
        [Required] public int xueke_id { get; set; }
    }

    public class BindingRestDto : BindingBaseDto
    {
        public string clk { get; set; }
    }

    /// <summary>
    /// last_ids is like: [bb_1,bb_2]
    /// </summary>
    public class BindingSavingDto : BindingBaseDto
    {
        [Required]
        public string[] last_ids { get; set; }  //many ids in the last category.
        public string maker { get; set; }

        private int _getIntId(string item)
        {
            if (int.TryParse(item.Split(Constants.Splitor)[1], out int k))
            {
                return k;
            };
            return -1;
        }

        /// <summary>
        /// last_ids : [xd_xx, mk_xx]
        /// </summary>
        /// <returns></returns>
        public List<int> getLastIdsArray()
        {
            var list = new List<int>();
            if (last_ids.Length > 0)
            {
                foreach (string item in last_ids)
                {
                    int k = _getIntId(item);
                    if (k != -1)
                    {
                        list.Add(k);
                    }
                }
            }
            return list;
        }
    }
}
