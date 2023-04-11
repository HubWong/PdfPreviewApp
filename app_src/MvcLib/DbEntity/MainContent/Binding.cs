using MvcLib.Dto.PdfDtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLib.MainContent
{
    /// <summary>
    /// binding Entity for saving
    /// id is updated after db created.
    /// </summary>
    public class Binding:BindingBaseDto
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required] 
        public int banben_id { get; set; }
        public string maker { get; set; }
    }

}