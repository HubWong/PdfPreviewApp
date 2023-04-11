using System.ComponentModel.DataAnnotations;

namespace MvcLib.Sidebar {

    public class AppSidebar : BasicData 
    {
        public string Route { get; set; }
        public MenuModule Module { get; set; }
        public int ModuleId { get; set; }

    }
}