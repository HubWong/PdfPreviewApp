using MvcLib.Sidebar;
using System.Collections.Generic;

namespace MvcClient.Models
{
    public class ConsoleModuleVm
    {
        public string ConsoleTitle { get; set; }
        public List<MenuModule> Sidebars { get; set; }
    }

}
