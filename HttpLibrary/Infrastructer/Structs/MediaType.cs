namespace HttpLibrary.Infrastructer.Structs
{
    public struct MediaType
    {
        public static readonly MediaType Json   = new MediaType("application/json");
        public static readonly MediaType Xml    = new MediaType("application/xml");
        public static readonly MediaType Text   = new MediaType("application/text");

        private readonly string _value;

        private MediaType(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}