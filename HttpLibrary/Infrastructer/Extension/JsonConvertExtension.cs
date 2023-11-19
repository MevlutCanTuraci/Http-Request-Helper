using System.Text.Json;

namespace HttpLibrary.Infrastructer.Extension
{
    public static class JsonConvertExtension
    {
        #pragma warning disable
        public static T ConvertTObject<T>(this string stringJson)
        {
            T result = JsonSerializer.Deserialize<T>(stringJson, JsonSettings);

            return result;
        }


        public static string ConvertObjectToJson(this object object_)
        {
            var result = JsonSerializer.Serialize(object_, JsonSettings);

            return result;
        }


        public static JsonSerializerOptions JsonSettings = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}