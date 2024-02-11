using System.Net;

namespace NetMinimalAPI.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMsgs = new List<string>();
        }

        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public object Result { get; set; }
        public IList<string> ErrorMsgs { get; set; }

    }
}
