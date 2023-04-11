using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcLib.DbEntity.MainContent;
using MvcLib.Dto;
using MvcLib.Dto.ColumnDto;
using System.Threading.Tasks;

namespace MvcClient.Controllers.Api
{
    /// <summary>
    /// columns unlimited data
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ColumnController : BasicApiController
    {
        private IColumnDataRepo _dbColumnData;

        public ColumnController(IColumnDataRepo columnData)
        {
            _dbColumnData = columnData;
        }

        /// <summary>
        /// get root data list
        /// </summary>
        /// <returns></returns>
        [HttpGet("/{pg:int}")]
        public async Task<IActionResult> get(int pg=1)
        {
            return Ok(await _dbColumnData.PagedData(a=>a.Pid==-1,out int ttl,pg));
        }

        [HttpGet("/children/{id:int}")]
        public async Task<IActionResult> getChildren(int id) {
            return Ok(await _dbColumnData.GetChildren(id));
        }

        [HttpPost]  
        public async Task<IActionResult> add(ColumnDataDto columnData)
        {
            columnData.Maker = User.Identity.Name;
            return Ok(await _dbColumnData.Add(columnData));
        }


        [HttpDelete]
        public async Task<IActionResult> del(int[] ids)
        {
            return Ok(await _dbColumnData.Del(ids));
        }

    }
}
