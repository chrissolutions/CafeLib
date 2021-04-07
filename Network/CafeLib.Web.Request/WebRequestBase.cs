using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Web.Request
{
    /// <summary>
    /// Abstract class to handle Http requests.
    /// </summary>
    public abstract class WebRequestBase : IDisposable
    {
        private bool _disposed;
        private readonly HttpClient _client;

        #region Automatic Properties

        public WebHeaders Headers { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// WebRequestBase default constructor.
        /// </summary>
        protected WebRequestBase()
            : this(null)
        {
        }

        /// <summary>
        /// WebRequestBase constructor
        /// </summary>
        /// <param name="headers">request headers</param>
        protected WebRequestBase(WebHeaders headers)
        {
            _client = new HttpClient();
            Headers = headers ?? new WebHeaders();
        }

        #endregion

        #region Methods

        protected Task<TOut> GetAsync<TOut>(Uri endpoint, WebHeaders headers = null, object parameters = null)
        {
            return WebRequestImpl.GetAsync<TOut>(_client, endpoint, headers ?? Headers, parameters);
        }

        protected Task<TOut> PostAsync<TIn, TOut>(Uri endpoint, TIn body, WebHeaders headers = null, object parameters = null)
        {
            var json = JsonConvert.SerializeObject(body);
            return WebRequestImpl.PostAsync<TOut>(_client, endpoint, headers ?? Headers, json, parameters);
        }

        protected Task<TOut> PutAsync<TIn, TOut>(Uri endpoint, TIn body, WebHeaders headers = null, object parameters = null)
        {
            var json = JsonConvert.SerializeObject(body);
            return WebRequestImpl.PutAsync<TOut>(_client, endpoint, headers ?? Headers, json, parameters);
        }

        protected Task<bool> DeleteAsync<TIn>(Uri endpoint, TIn body, WebHeaders headers = null, object parameters = null)
        {
            var json = JsonConvert.SerializeObject(body);
            return WebRequestImpl.DeleteAsync(_client, endpoint, headers ?? Headers, json, parameters);
        }

        #endregion

        #region IDisposible

        public void Dispose()
        {
            Dispose(!_disposed);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose concurrent queue.
        /// </summary>
        /// <param name="disposing">indicate whether the queue is disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            try
            {
                _client.Dispose();
            }
            catch
            {
                // ignore
            }
        }

        #endregion
    }
}
