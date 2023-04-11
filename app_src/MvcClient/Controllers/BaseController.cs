using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcClient.Models.Data;
using MvcLib.Db;
using MvcLib.Sidebar;
using System.Collections.Generic;

namespace MvcClient.Controllers
{
    public class BaseController : Controller
    {
        private IMenuModuleRepo _modules;
        protected List<SelectListItem> SelectListItems { get; set; }


        public BaseController(IMenuModuleRepo menuModuleRepo)
        {
            _modules = menuModuleRepo;
            SelectListItems = new List<SelectListItem>();
        }

        protected List<MenuModule> SortedMenus
        {
            get
            {
                DummyData.DataInit();
                return DummyData.Modules;
            }
        }

        public IActionResult _SidebarView()
        {
            return PartialView(SortedMenus);
        }


        protected void GenSelectList<T>(T list) where T : IDictionary<int, string>
        {
            foreach (KeyValuePair<int, string> item in list)
            {
                SelectListItems.Add(new SelectListItem(item.Value, item.Key.ToString()));
            }
        }

        protected void GenSelectList<T>(T list, int selectedItemKey) where T : IDictionary<int, string>
        {
            foreach (KeyValuePair<int, string> item in list)
            {
                SelectListItems.Add(new SelectListItem(item.Value, item.Key.ToString(), item.Key == selectedItemKey));
            }

        }


    }
}
