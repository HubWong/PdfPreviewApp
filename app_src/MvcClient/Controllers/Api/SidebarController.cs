using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models.Data;
using MvcLib.Sidebar;

namespace MvcClient.Controllers.Api {
    [Route ("api/[controller]")]
    [ApiController]
    public class SidebarController : BasicApiController
    {
        private ISidebarRepo _SidebarRepo;
        private IMenuModuleRepo _ModuleRepo;

        public List<MenuModule> Menus { get; set; }
        public SidebarController (ISidebarRepo sidebarRepo, IMenuModuleRepo menuModuleRepo) {
            _SidebarRepo = sidebarRepo;
            _ModuleRepo = menuModuleRepo;
        }

        public async Task<IActionResult> Get () {
            return Ok (await _SidebarRepo.QueryAll ());
        }

        [HttpPost]
        public IActionResult Post (AppSidebar appSidebar) {
            return Ok ();
        }

      

    }
}