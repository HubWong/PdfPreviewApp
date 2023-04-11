using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MvcClient.Models;
using MvcClient.Models.TestpaperVms;
using MvcLib.DbEntity;
using MvcLib.DbEntity.MainContent;
using MvcLib.Dto.ColumnDto;
using MvcLib.Dto.PropDto;
using MvcLib.Dto.UploadDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcClient.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestpaperController : BasicApiController
    {
       
    }
}
