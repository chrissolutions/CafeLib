using System;
using System.Threading.Tasks;
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
            return await WebRequestImpl.GetAsync<TOut>(Endpoint, headers ?? Headers, parameters);
        }

        protected async Task<TOut> PostAsync<TIn, TOut>(TIn body, WebHeaders headers = null, object parameters = null)
        {
            var json = JsonConvert.SerializeObject(body);
            return await WebRequestImpl.PostAsync<TOut>(Endpoint, headers ?? Headers, json, parameters);
        }

        protected async Task<TOut> PutAsync<TIn, TOut>(TIn body, WebHeaders headers = null, object parameters = null)
        {
            var json = JsonConvert.SerializeObject(body);
            return await WebRequestImpl.PutAsync<TOut>(Endpoint, headers ?? Headers, json, parameters);
        }

        protected async Task<bool> DeleteAsync<TIn>(TIn body, WebHeaders headers = null, object parameters = null)
        {
            var json = JsonConvert.SerializeObject(body);
            return await WebRequestImpl.DeleteAsync(Endpoint, headers ?? Headers, json, parameters);
        }

        #endregion
    }
}
