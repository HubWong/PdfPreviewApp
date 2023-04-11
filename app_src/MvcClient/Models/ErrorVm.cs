using System;

namespace MvcClient.Models
{
    public class ErrorVm
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string msg { get; set; }
    }
}
