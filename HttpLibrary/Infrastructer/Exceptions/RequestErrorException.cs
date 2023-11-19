namespace HttpLibrary.Infrastructer.Exceptions
{
    public class RequestErrorException : Exception
    {
        public string DeveloperMessage { get; set; }

        public RequestErrorException(string message, string devMessage) : base(message)
        {
            DeveloperMessage = devMessage;
        }
    }
}