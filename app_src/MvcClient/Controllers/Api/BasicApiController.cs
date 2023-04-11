using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcLib;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace MvcClient.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasicApiController : ControllerBase
    {
        public BasicApiController()
        { 

        }

        protected IWebHostEnvironment _env;
        protected string saving_dir = Constants.SavingPath + "\\pdffile";

        protected void CreateDir(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }


        protected Task<string> save_file(IFormFile file, IWebHostEnvironment environment)
        {
            if (file != null)
            {
                _env = environment;
                string path = Path.Combine(saving_dir,
                     Path.GetRandomFileName() + Path.GetExtension(file.FileName));
                string finalpath = Path.Combine(_env.WebRootPath, path);
                using (var stream = System.IO.File.Create(finalpath))
                {
                    file.CopyToAsync(stream);
                }
                return Task.FromResult(path);
            }

            return Task.FromResult(string.Empty);
        }

    }
}
