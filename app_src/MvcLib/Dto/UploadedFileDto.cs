using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MvcLib.Dto
{
    public class UploadedFileDto
    {
        public float size { get; set; }
        public string path { get; set; }
        public string name {
            get 
            {
                return  Path.GetFileName(path);
            }
        }
    }
}
