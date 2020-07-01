using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CafeLib.Core.Extensions;
using Newtonsoft.Json;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Web.Request
{
    /// <summary>
    /// Abstract class to handle Http requests.
    /// </summary>
    public abstract class WebRequestBase
    {
        public Uri Endpoint { get; }
        public WebHeaders Headers { get; protected set; }

        #region Constructors

        /// <summary>
        /// ApiRequest constructor
        /// </summary>
        /// <param name="endpoint">service endpoint</param>
        /// <param name="headers">request headers</param>
        protected WebRequestBase(string endpoint, WebHeaders headers = null)
            : this(new Uri(endpoint), headers)
        {
        }

        /// <summary>
        /// WebRequestBase constructor
        /// </summary>
        /// <param name="endpoint">service endpoint</param>
        /// <param name="headers">request headers</param>
        protected WebRequestBase(Uri endpoint, WebHeaders headers = null)
        {
            Endpoint = endpoint;
            Headers = headers ?? new WebHeaders();
        }

        #endregion

        #region Methods

        protected async Task<TOut> GetAsync<TOut>(WebHeaders headers = null, object parameters = null)
        {
            var response = await WebRequestImpl.GetAsync(Endpoint, headers ?? Headers, parameters);
            return await ConvertContent<TOut>(await response.GetContent());
        }

        protected async Task<TOut> PostAsync<TIn, TOut>(TIn body, WebHeaders headers = null, object parameters = null)
        {
            var json = JsonConvert.SerializeObject(body);
            var response = await WebRequestImpl.PostAsync(Endpoint, headers ?? Headers, json, parameters);
            return await ConvertContent<TOut>(await response.GetContent());
        }

        protected async Task<TOut> PutAsync<TIn, TOut>(TIn body, WebHeaders headers = null, object parameters = null)
        {
            var json = JsonConvert.SerializeObject(body);
            var response = await WebRequestImpl.PutAsync(Endpoint, headers ?? Headers, json, parameters);
            return await ConvertContent<TOut>(await response.GetContent());
        }

        protected async Task<TOut> DeleteAsync<TIn, TOut>(TIn body, WebHeaders headers = null, object parameters = null)
        {
            var json = JsonConvert.SerializeObject(body);
            var response = await WebRequestImpl.DeleteAsync(Endpoint, headers ?? Headers, json, parameters);
            return await ConvertContent<TOut>(await response.GetContent());
        }

        #endregion

        #region Helpers

        private static async Task<T> ConvertContent<T>(Stream contentStream)
        {
            if (typeof(T) == typeof(bool))
            {
                return (T)(object)true;
            }

            if (contentStream == null)
            {
                return default;
            }

            if (typeof(T) == typeof(byte[]))
            {
                return (T)(object)await contentStream.ToByteArrayAsync();
            }

            var reader = new StreamReader(contentStream, Encoding.UTF8);
            var response = await reader.ReadToEndAsync();
            if (response == null) return default;

            var data = response.TrimStart();
            switch (data[0])
            {
                case '{':
                case '[':
                    return JsonConvert.DeserializeObject<T>(data);

                case '<':
                {
                    var serializer = new XmlSerializer(typeof(T));
                    using var stringReader = new StringReader(data);
                    return (T)serializer.Deserialize(stringReader);
                }

                default:
                    return (T)(object)response;
            }
        }

        #endregion
    }
}
