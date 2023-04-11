using Microsoft.Extensions.Configuration;
using MvcLib.ConfigModels;
using MvcLib.MainContent;
using MvcLib.Sidebar;
using System;
using System.Collections.Generic;
using System.IO;

namespace MvcLib.Db
{
    public class DummyData
    {
        public static List<AppSidebar> Sidebars;
        public static List<MenuModule> Modules;
        public static List<ItemCategory> categories;

        public static IConfiguration GetSettingsJson()
        {
            var filePath = AppDbContext.BaseDir;
            // if it is test ,  change file path
#if DEBUG
            if (filePath.IndexOf("TestProj") != -1)
            {
                filePath = "D:\\Projects\\CORE\\Assignment\\ElecBook\\app_src\\MvcClient\\appsettings.json";

            }
#endif

            if (!File.Exists(filePath))
            {
                throw new Exception("no json found");
            }

            IConfiguration configuration;
            var builder = new ConfigurationBuilder();

            builder.AddJsonFile(filePath, false, true);
            configuration = builder.Build();


            return configuration;
        }

        public IConfiguration Configuration { get; set; }

        internal T GetSection<T>(string[] key) where T : class
        {
            Configuration = GetSettingsJson();
            foreach (var item in key)
            {
                Configuration = Configuration.GetSection(item);
            }
            return Configuration.Get<T>();
        }  

        public static void DataInit()
        {
            var dmy = new DummyData();
            dmy.getConfigsData();
            
            if (dmy.Navs != null)
            {
                Modules = new List<MenuModule>();
                MenuModule md ;
                AppSidebar sb;

                for (int i = 0; i < dmy.Navs.pdf_sidebar.Length; i++)
                {                    
                    md = new MenuModule();
                    md.Sidebars = new List<AppSidebar>();
                    md.isShow = true;
                    md.orderNo = i + 1;
                    md.title = dmy.Navs.pdf_sidebar[i].title;

                    var module_sidebars = dmy.Navs.pdf_sidebar[i];
                    for (int x =  0; x < module_sidebars.menus.Length; x++)
                    {
                        sb = new AppSidebar();
                        sb.Module =md;
                        sb.orderNo = x + 1;
                        sb.title = module_sidebars.menus[x];
                        sb.isShow = true;
                        md.Sidebars.Add(sb);
                    }                    
                  
                    Modules.Add(md);
                }            

            }

            categories = new List<ItemCategory> {
                new ItemCategory { id = 1, isShow = true, orderNo = 1, category = categoryType.ban_ben, title = "banben1" },
                new ItemCategory { id = 2, isShow = true, orderNo = 2, category = categoryType.ban_ben, title = "banben2" },
                new ItemCategory { id = 3, isShow = true, orderNo = 3, category = categoryType.ban_ben, title = "banben3" },
                new ItemCategory { id = 4, isShow = true, orderNo = 4, category = categoryType.ban_ben, title = "banben4" },
                new ItemCategory { id = 5, isShow = true, orderNo = 5, category = categoryType.lei_xing, title = "leixing1" },
                new ItemCategory { id = 6, isShow = true, orderNo = 6, category = categoryType.lei_xing, title = "leixing2" },
                new ItemCategory { id = 7, isShow = true, orderNo = 7, category = categoryType.lei_xing, title = "leixing3" },
            };
        }

        public DummyData()
        {
            
        }

     
        public Initial_Navs Navs { get; set; }

        public void getConfigsData()
        {
            Navs = GetSection<Initial_Navs>(new string[]{ Initial_Navs.Initial_Data });        
            
        }
    }
}