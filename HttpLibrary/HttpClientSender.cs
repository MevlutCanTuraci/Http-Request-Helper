#region Imports

using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using HttpLibrary.Infrastructer.Extension;
using HttpLibrary.Infrastructer.Structs;
using System.Text;
using System.Net.Http.Json;
using System.Text.Json;
using HttpLibrary.Infrastructer.Exceptions;

#endregion


namespace HttpLibrary
{
    public class HttpClientSender : IDisposable
    {
        public Uri Url { get; set; }
        private List<(string Name, string Value)> _headers { get; set; }


        private HttpClientHandler _handler { get; init; }
        private HttpClient _client { get; init; }
        private StringContent _content { get; set; }


        public HttpClientSender(WebProxy proxy = null, DecompressionMethods decompressionMethods = default, TimeSpan timeout = default)
        {
            //SSL hatası alınmaması için bir ayar yapılır.
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, errors) => { return true; };


            //SSL hatası alınmaması için bir ayar yapılır.
            _handler = new HttpClientHandler
            {
                #pragma warning disable
                ServerCertificateCustomValidationCallback = (HttpRequestMessage message, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
                {
                    return true;
                }
            };


            //Kullanıcı custom bir proxy vermişse if içerisine girer
            //iligi webproxy ataması gerçekleştirir.
            if (proxy != null && proxy != default)
            {
                _handler.Proxy = proxy!;
            }


            //Cutom bir sıkıştırma methodu verilmişse if içerisine girer
            //ilgili methodları ekler.
            #pragma warning disable
            if (decompressionMethods != default && decompressionMethods != null)
            {
                _handler.AutomaticDecompression = decompressionMethods;
            }


            //Yeni bir client nesnesi oluşturulur.
            _client = new HttpClient(_handler);


            //Kullanıcı custom bir timeout belirlediyse if içerisine girer,
            //ilgili client nesnesinde yeni timeout değerini atar.
            if (timeout != default && timeout != TimeSpan.Zero && timeout != null)
            {
                _client.Timeout = timeout;
            }

        }


        /// <summary>
        /// Bir API endpoint adresine istek atar ve istek başarılıysa geriye <see cref="HttpResponseMessage"/> nesnesini döner.
        /// </summary>
        /// <remarks>
        /// <see cref="RequestFailException"/> exception'i, <b>200 kodu</b> haricinde bütün durumlarda fırlatılır. 
        /// Başarısız olan her istekte fırlatılır.
        /// 
        /// <see cref="RequestErrorException"/> exception'i, http isteği atılamadığında fırlatılır.
        /// 
        /// <see cref="ArgumentNullException"/> exception'i, <b><see cref="Url"/></b> özelliği <see cref="null"/> olduğu zaman veya geçersiz olduğunda fırlatılır.
        /// 
        /// </remarks>
        /// <param name="method">Hangi methodta istek atılacağı belirlenir.</param>
        /// <returns>Başarılı olursa <b>(200 / Ok)</b> dönerse geriye <see cref="HttpRequestMessage"/> nesnesi döner.</returns>
        /// <exception cref="RequestFailException">200 harici her status kodunda fırlatılır.</exception>
        /// <exception cref="RequestErrorException">Http isteği atılamadığında fırlatılır.</exception>
        /// <exception cref="ArgumentNullException">Url adresi null olduğu zaman fırlatılır.</exception>
        public async Task<HttpResponseMessage> SendAsync(HttpMethod method)
        {
            try
            {
                if (Url == default || Url == null)
                {
                    throw new ArgumentNullException($"{nameof(Url)} alanı geçersiz!!! Geçersiz url adresi. Url adresini kontrol edin");
                }

                if (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch || method == HttpMethod.Delete)
                {
                    if (_content == null || _content == default)
                    {
                        throw new ArgumentNullException($"Herhangi bir body content bilgisi verilmedi!");
                    }
                }

                SetHeaders();
                HttpResponseMessage _responseMessage = new HttpResponseMessage();

                if (method == HttpMethod.Get)
                {
                    _responseMessage = await _client.GetAsync(Url);
                }
                else if (method == HttpMethod.Post)
                {
                    if (_content != null && _content != default)
                    {
                        _responseMessage = await _client.PostAsync(Url, _content);
                    }
                }
                else if (method == HttpMethod.Put)
                {
                    if (_content != null && _content != default)
                    {
                        _responseMessage = await _client.PutAsync(Url, _content);
                    }
                }
                else if (method == HttpMethod.Delete)
                {
                    if (_content != null && _content != default)
                    {
                        _responseMessage = await _client.DeleteAsync(Url);
                    }
                }
                else if (method == HttpMethod.Patch)
                {
                    if (_content != null && _content != default)
                    {
                        _responseMessage = await _client.PatchAsync(Url, _content);
                    }
                }
                else
                {
                    throw new Exception("Geçersiz http method türü!");
                }


                if (_responseMessage.IsSuccessStatusCode)
                {
                    return _responseMessage;
                }
                else
                {
                    throw new RequestFailException(
                        message:            await _responseMessage.Content.ReadAsStringAsync(),
                        statusCode:         _responseMessage.StatusCode,
                        statusCodeInt:      (int)_responseMessage.StatusCode,
                        developerMessage:   await _responseMessage.Content.ReadAsStringAsync()
                     );
                }
            }
            catch (RequestFailException e)
            {
                throw e;
            }
            catch (ArgumentNullException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new RequestErrorException(e.Message, e.ToString());
            }
        }


        /// <summary>
        /// Bir API endpoint adresine istek atar ve istek başarılıysa geriye <see cref="T"/> nesnesini döner.
        /// </summary>
        /// <remarks>
        /// <see cref="RequestFailException"/> exception'i, <b>200 kodu</b> haricinde bütün durumlarda fırlatılır. 
        /// Başarısız olan her istekte fırlatılır.
        /// 
        /// <see cref="RequestErrorException"/> exception'i, http isteği atılamadığında fırlatılır.        
        /// 
        /// <see cref="ArgumentNullException"/> exception'i, <b><see cref="Url"/></b> özelliği <see cref="null"/> olduğu zaman veya geçersiz olduğunda fırlatılır.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">Hangi methodta istek atılacağı belirlenir.</param>
        /// <returns>Başarılı olursa <b>(200 / Ok)</b> dönerse geriye <see cref="T"/> nesnesi döner.</returns>
        /// <exception cref="RequestFailException">200 harici her status kodunda fırlatılır.</exception>
        /// <exception cref="RequestErrorException">Http isteği atılamadığında fırlatılır.</exception>
        /// <exception cref="ArgumentNullException">Url adresi null olduğu zaman fırlatılır.</exception>
        /// <exception cref="JsonException">İstenilen model ile API'dan dönen model farklı ise fırlatılır.</exception>
        public async Task<T> SendAsync<T>(HttpMethod method)
        {
            try
            {
                if (Url == default || Url == null)
                {
                    throw new ArgumentNullException("Geçersiz url adresi. Url adresini kontrol edin");
                }

                if (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch || method == HttpMethod.Delete)
                {
                    if (_content == null || _content == default)
                    {
                        throw new ArgumentNullException("Herhangi bir content bilgisi verilmedi!");
                    }
                }

                SetHeaders();
                T responseT;
                HttpResponseMessage responseMessage = new HttpResponseMessage();

                if (method == HttpMethod.Get)
                {
                    responseMessage = await _client.GetAsync(Url);
                }
                else if (method == HttpMethod.Post)
                {
                    if (_content != null && _content != default)
                    {
                        responseMessage = await _client.PostAsync(Url, _content);
                    }
                }
                else if (method == HttpMethod.Put)
                {
                    if (_content != null && _content != default)
                    {
                        responseMessage = await _client.PutAsync(Url, _content);
                    }
                }
                else if (method == HttpMethod.Delete)
                {
                    if (_content != null && _content != default)
                    {
                        responseMessage = await _client.DeleteAsync(Url);
                    }
                }
                else if (method == HttpMethod.Patch)
                {
                    if (_content != null && _content != default)
                    {
                        responseMessage = await _client.PatchAsync(Url, _content);
                    }
                }
                else
                {
                    throw new Exception("Geçersiz http method türü!");
                }


                if (responseMessage.IsSuccessStatusCode)
                {
                    return await responseMessage.Content.ReadFromJsonAsync<T>(JsonConvertExtension.JsonSettings);
                }

                else
                {
                    throw new RequestFailException(
                        message:            await responseMessage.Content.ReadAsStringAsync(),
                        statusCode:         responseMessage.StatusCode,
                        statusCodeInt:      (int)responseMessage.StatusCode,
                        developerMessage:   await responseMessage.Content.ReadAsStringAsync()
                    );
                }
            }
            catch (RequestFailException e)
            {
                throw e;
            }
            catch (ArgumentNullException e)
            {
                throw e;
            }
            catch(JsonException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new RequestErrorException(e.Message, e.ToString());
            }
        }


        /// <summary>
        /// İsteğe header değeri ekler.
        /// </summary>
        /// <param name="name">Header adı</param>
        /// <param name="value">Header değeri</param>
        public void AddHeader(string name, string value)
        {
            if (_headers == null || _headers == default)
            {
                _headers = new();
            }

            _headers.Add((name, value));
        }


        /// <summary>
        /// Cache'leri kaldıran header verileri ekler.
        /// </summary>
        public void AddDisabledCacheHeaders()
        {
            AddHeader("Cache-Control", "no-cache");
            AddHeader("Cache-Control", "no-store");
            AddHeader("Cache-Control", "max-age=1");
            AddHeader("Cache-Control", "s-maxage=1");
            AddHeader("Pragma", "no-cache");
        }


        /// <summary>
        /// Client'a kullanıcının verdiği header verilerini ekler
        /// </summary>
        private void SetHeaders()
        {
            if (_headers != null || _headers != default)
            {
                if (_headers.Count > 0)
                {
                    foreach ((string Name, string Value) i in _headers)
                    {
                        _client.DefaultRequestHeaders.Add(i.Name, i.Value);
                    }
                }
            }
        }


        /// <summary>
        /// Post body'sine content eklemek için kullanılır.
        /// </summary>
        /// <param name="content">Bir model verilir. Bu model json formatına çevrilir ve body içerisine eklenir.</param>
        /// <param name="mediaType">Gönderilecek olan content'in türü.</param>
        /// <param name="encoding">Content'in encoding bilgisi. <b>Default değeri <see cref="Encoding.UTF8"/> olarak ayarlanmıştır. </b></param>
        public void AddContent(object content, MediaType mediaType, Encoding encoding = default)
        {
            if (encoding == null || encoding == default)
            {
                encoding = Encoding.UTF8;
            }

            _content = new StringContent(content.ConvertObjectToJson(), encoding, mediaType.ToString());
        }


        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Burada managed kaynakları serbest bırak
                if (_client != null)
                {
                    _client.Dispose();
                }
                if (_handler != null)
                {
                    _handler.Dispose();
                }
                if (_content != null)
                {
                    _content.Dispose();
                }
            }

            Url         = null;
            _headers    = null;
            //_handler    = null;
            //_content    = null;
            //_client     = null;
            // Burada unmanaged kaynakları serbest bırak
        }


        ~HttpClientSender()
        {
            Dispose(false);
        }

        #endregion


    }
}