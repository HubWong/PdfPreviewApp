using MvcLib.MainContent;
using System;

namespace MvcLib.Sidebar
{

    public interface ISidebar :ISideModule {     
      
        bool isShow { get; set; }
    }

}