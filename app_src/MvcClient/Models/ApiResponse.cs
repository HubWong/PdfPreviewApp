
namespace MvcClient.Models
{
    public class ApiData
    {
        public int id { get; set; }
        public int status { get; set; }
        public string data { get; set; }
    }
    public class ApiResponse
    {      
        public void setPropValtoNull<T>(T data, string prop)
        {
            var info = typeof(T).GetProperty(prop);

            if (info != null)
            {
                info.SetValue(data, null);
            }
        }

        public void setNestedProp<T>(T data)
        {         
            foreach (var item in data.GetType().GetProperties())
            {
                if (item.Name == "pdf")
                {
                    setPropValtoNull<T>(data, item.Name);
                }
            }
        }


    }
}
