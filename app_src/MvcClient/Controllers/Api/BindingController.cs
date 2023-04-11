using Microsoft.AspNetCore.Mvc;
using MvcClient.Models.Data.PdfData;
using MvcLib;
using MvcLib.Dto.PdfDtos;
using System.Text.Json;
using System.Threading.Tasks;

namespace MvcClient.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BindingController : BasicApiController
    {
        private IBindingRepo _bindingRepos;

        public BindingController(IBindingRepo bindingRepo)
        {
            _bindingRepos = bindingRepo;
        }

        [HttpPost("save")]
        public async Task<IActionResult> post([FromBody] BindingSavingDto SavingDto)
        {
            if (ModelState.IsValid)
            {
                SavingDto.maker = User.Identity?.Name;
                var c = await _bindingRepos.SaveBindings(SavingDto);
                return Ok(c);
            }
            return BadRequest(0);
        }

        /// <summary>
        /// delete many binding records
        /// </summary>
        /// <param name="bindingSavingDto"></param>
        /// <returns></returns>
        [HttpPost("del")]
        public async Task<IActionResult> delete([FromBody] BindingSavingDto bindingSavingDto)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _bindingRepos.DelBindings(bindingSavingDto));
            }
            return Ok(0);
        }


        [HttpPost("loadBinding")]
        public async Task<IActionResult> post([FromBody] BindingDto bindingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("model invalid");
            }

            if (_bindingRepos.Bindings == null)
            {
                return Ok(0);
            }

            return Ok(await _bindingRepos.GetDownLevel(bindingDto.Selected_Id, bindingDto.Upper_Ids));

        }
    }
}
