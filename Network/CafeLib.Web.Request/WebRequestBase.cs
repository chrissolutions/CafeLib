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
        public WebHeaders Headers { get; protected set; }

        #region Constructors

        /// <summary>
        /// WebRequestBase default constructor.
        /// </summary>
        protected WebRequestBase()
        {
            Headers = new WebHeaders();
        }

        /// <summary>
        /// ApiRequest constructor
        /// </summary>
        /// <param name="headers">request headers</param>
        protected WebRequestBase(WebHeaders headers)
        {
            Headers = headers ?? new WebHeaders();
        }

        #endregion

        #region Methods

        protected Task<TOut> GetAsync<TOut>(Uri endpoint, WebHeaders headers = null, object parameters = null)
        {
            return WebRequestImpl.GetAsync<TOut>(endpoint, headers ?? Headers, parameters);
        }

        protected Task<TOut> PostAsync<TIn, TOut>(Uri endpoint, TIn body, WebHeaders headers = null, object parameters = null)
        {
            var json = JsonConvert.SerializeObject(body);
            return WebRequestImpl.PostAsync<TOut>(endpoint, headers ?? Headers, json, parameters);
        }

        protected Task<TOut> PutAsync<TIn, TOut>(Uri endpoint, TIn body, WebHeaders headers = null, object parameters = null)
        {
            var json = JsonConvert.SerializeObject(body);
            return WebRequestImpl.PutAsync<TOut>(endpoint, headers ?? Headers, json, parameters);
        }

        protected Task<bool> DeleteAsync<TIn>(Uri endpoint, TIn body, WebHeaders headers = null, object parameters = null)
        {
            var json = JsonConvert.SerializeObject(body);
            return WebRequestImpl.DeleteAsync(endpoint, headers ?? Headers, json, parameters);
        }

        #endregion
    }
}
