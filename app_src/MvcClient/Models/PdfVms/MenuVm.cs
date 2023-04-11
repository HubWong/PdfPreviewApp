using MvcLib.Sidebar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Models.PdfVms
{
    public interface IMenuVm
    {
     
    }

    /// <summary>
    /// dashboard sidebar view-model
    /// </summary>
    public class MenuVm : IMenuVm
    {
        public MenuVm(MenuModule menuModule)
        {
            MenuModule = menuModule;
        }

        public MenuVm(List<MenuModule> menuModules)
        {
            Modules = menuModules;         
        }

        private List<MenuModule> Modules;     

        public MenuModule MenuModule { get; set; }     

    }
}
