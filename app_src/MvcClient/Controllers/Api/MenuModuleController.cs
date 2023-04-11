using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuModuleController : BasicApiController
    {
        // GET: api/<MenuModuleController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<MenuModuleController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<MenuModuleController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<MenuModuleController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MenuModuleController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
