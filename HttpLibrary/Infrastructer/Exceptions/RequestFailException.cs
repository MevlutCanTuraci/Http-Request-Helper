using System.Net;

namespace HttpLibrary.Infrastructer.Exceptions
{
    public class RequestFailException : Exception
    {
        public int StatusCodeInt { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string DeveloperMessage { get; set; }


        public RequestFailException(string message, int statusCodeInt, HttpStatusCode statusCode, string developerMessage): base(message)
        {
            StatusCodeInt       = statusCodeInt;
            StatusCode          = statusCode;
            DeveloperMessage    = developerMessage;
        }

    }
}