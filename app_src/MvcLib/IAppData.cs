//to client data
using MvcLib.Tools;

namespace MvcLib
{
    /// <summary>
    /// for api returned data mainly
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAppData
    {
        int statusCode { get; set; }
        string msg { get; set; }
        string jsonData { get; set; }
    }


}
