namespace HttpLibrary.Infrastructer.Extension
{

    public static class HttpClientSenderExtension
    {
        /// <summary>
        /// Response içerisinde, API'dan dönen reponse content verisini okur ve geriye <see cref="string"/> olarak döndürür.
        /// </summary>
        /// <param name="responseMessage"><see cref="HttpResponseMessage"/> nesnesi</param>
        /// <returns><see cref="string"/> türünde response content verisi döner.</returns>
        public static async Task<string> GetContent(this HttpResponseMessage responseMessage)
        {
            return await responseMessage.Content.ReadAsStringAsync();
        }
    }
}