using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcClient.Models.TestpaperVms
{
    
    /// <summary>
    /// form uploading dto for testpaper
    /// </summary>
    public class TestpaperFormVmDto
    {
        public TestpaperFormVmDto()
        {
            isAdd = 1;
        }

        [Required]
        public int id { get; set; }

        [Required]
        public int nf { get; set; }
        
        [Required]
        public int sf { get; set; }
        public int pid { get; set; }
        public int isAdd { get; set; }
        public List<IFormFile> formfiles { get; set; }
    }
}
