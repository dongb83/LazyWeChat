using System.Net;

namespace LazyWeChat.Models.Exception
{
    public class BadHttpResponseException : System.Exception
    {
        string _message = "";

        public BadHttpResponseException(string requestUrl, HttpStatusCode statusCode)
        {
            _message = $"http request('{requestUrl}') return statuscode('{(int)statusCode}')";
        }

        public override string Message => _message;
    }
}
