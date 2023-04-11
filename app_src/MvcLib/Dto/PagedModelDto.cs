namespace MvcLib.Dto
{
    public class PagedModelDto
    {
        public PagedModelDto()
        {
            pg = 1;
            isAsc = true;
        }
        public string menu { get; set; }
        public int pg { get; set; }
        public string orderby { get; set; }
        public int ttl { get; set; }
        public bool isAsc { get; set; }
    }


}
