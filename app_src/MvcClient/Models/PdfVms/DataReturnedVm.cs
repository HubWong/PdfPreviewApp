using MvcLib;
using MvcLib.MainContent;
using MvcLib.Sidebar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcClient.Models.PdfVms
{
    public interface IDataReturnedVm
    {
        IRightCategoryView CategosView { get; }
        IRightBindingView RightBindingView { get; }
        string appSidebar { get; set; }
    }


    /// <summary>
    /// for just returned main content with list data.
    /// </summary>
    public class DataReturnedVm : IDataReturnedVm
    {
        #region constructure
        public DataReturnedVm(string sidebar)
        {
            appSidebar = sidebar;
        }

        public DataReturnedVm(string sidebar, IRightCategoryView rightCategoryView) : this(sidebar)
        {
            CategosView = rightCategoryView;
        }

        public DataReturnedVm(string sidebar, IRightBindingView rightBindingView) : this(sidebar)
        {
            RightBindingView = rightBindingView;
        }

        #endregion

        public string appSidebar { get; set; }
        public IRightCategoryView CategosView { get; private set; }
        public IRightBindingView RightBindingView { get; private set; }

    }

}
