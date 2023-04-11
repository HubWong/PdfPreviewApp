using MvcLib.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MvcLib
{
    
    public interface IFormViewModel
    {       
        public byte IsAdd { get; }
        public string TimeStr { get; }
    }

    /// <summary>
    /// view model interface
    /// </summary>
    public interface IViewModel
    {

    }
}
