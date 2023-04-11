namespace MvcLib.ConfigModels
{
    public class Nav
    {
        public string title { get; set; }
        public string[] menus { get; set; }
    }

    public class Initial_Navs
    {
        public const string Initial_Data = "initial_data";
        public Nav[] pdf_sidebar { get; set; }
    }
}
