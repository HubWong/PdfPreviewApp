using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models.Data;
using MvcClient.Models.Data.PdfData;
using MvcClient.Models.PdfVms;
using MvcLib.Dto;
using MvcLib.MainContent;
using MvcLib.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MvcClient.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemCategoryController : BasicApiController
    {
        private readonly IItemCategoryRepo _categoryRepos;
        private IBindingRepo _bindingRepo;
        private static  IWebHostEnvironment _wwwFolder;

        public ItemCategoryController(IItemCategoryRepo itemCategoryRepo,IBindingRepo bindingRepo, IWebHostEnvironment hostEnvironment)
        {
            _categoryRepos = itemCategoryRepo;
            _bindingRepo = bindingRepo;
            _wwwFolder = hostEnvironment;
        }

        // GET api/<ItemCategoryController>/5
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            if (id == 0)
            {
                return BadRequest("argument of id is 0");
            }
            return Ok(await _categoryRepos.Get(id));
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ItemCategoryVm itemCategory)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _categoryRepos.Add(itemCategory));
            }
            return Ok();
        }

        // PUT api/<ItemCategoryController>/5 
        /// <summary>
        /// form valid bug 
        /// </summary>
        /// <param name="itemCategory"></param>
        /// <returns></returns>
        [HttpPost("update")]
        public async Task<IActionResult> update([FromBody] ItemCategoryVm itemCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(0);
            }
            int i = 0;
            if (itemCategory.IsAdd == 1)
            {
                i = await _categoryRepos.Add(itemCategory);
            }
            else
            {         
                i = await _categoryRepos.Update(itemCategory);
            }


            return Ok(i);
        }

        /// <summary>
        /// if there is binding or binding pdfs.        
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true:has binding , false: no binding</returns>
        [HttpGet("PreDel/{id:int}")]
        public IActionResult PreDel(int id)
        {          
            return Ok(_bindingRepo.Any(id));
        }

        Action<List<string>> delpdfFiles =async r=>await Utility.DelFiles(_wwwFolder.WebRootPath, r);

        // DELETE api/<ItemCategoryController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("param error");
            }

            if (id.IndexOf(",") == -1)
            {
                if (int.TryParse(id, out int x))
                {
                    return Ok(await _categoryRepos.Del(x, delpdfFiles));
                }
            }
            else
            {
                string[] id_array = id.Split(",");
                return Ok(await _categoryRepos.Del_Many(id_array));
            }
            return Ok();
        }

    }
}
