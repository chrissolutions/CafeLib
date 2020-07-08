﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
    internal static class WebRequestImpl
    {
        #region Methods

        /// <summary>
        /// Perform a GET call
        /// </summary>
        /// <param name="endpoint">uri endpoint</param>
        /// <param name="headers">http headers</param>
        /// <param name="parameters">parameters</param>
        /// <returns>web response</returns>
        public static async Task<T> GetAsync<T>(Uri endpoint, WebHeaders headers, object parameters = null)
        {
            var uri = CombineUri(endpoint, parameters);
            using var httpResponse = await SendRequest(uri, HttpMethod.Get, headers, null);
            var response = new WebResponse(httpResponse);
            response.EnsureSuccessStatusCode(httpResponse);
            return await ConvertContent<T>(response, await httpResponse.Content.ReadAsStreamAsync());
        }

        /// <summary>
        /// Perform a POST call
        /// </summary>
        /// <param name="endpoint">uri endpoint</param>
        /// <param name="headers">http headers</param>
        /// <param name="body">body data</param>
        /// <param name="parameters">parameters</param>
        /// <returns>web response</returns>
        public static async Task<T> PostAsync<T>(Uri endpoint, WebHeaders headers, object body, object parameters = null)
        {
            var uri = CombineUri(endpoint, parameters);
            using var httpResponse = await SendRequest(uri, HttpMethod.Post, headers, body);
            var response = new WebResponse(httpResponse);
            response.EnsureSuccessStatusCode(httpResponse);
            return await ConvertContent<T>(response, await httpResponse.Content.ReadAsStreamAsync());
        }

        /// <summary>
        /// Perform a PUT call
        /// </summary>
        /// <param name="endpoint">uri endpoint</param>
        /// <param name="headers">http headers</param>
        /// <param name="body">body data</param>
        /// <param name="parameters">parameters</param>
        /// <returns>web response</returns>
        public static async Task<T> PutAsync<T>(Uri endpoint, WebHeaders headers, object body, object parameters = null)
        {
            var uri = CombineUri(endpoint, parameters);
            using var httpResponse = await SendRequest(uri, HttpMethod.Put, headers, body);
            var response = new WebResponse(httpResponse);
            response.EnsureSuccessStatusCode(httpResponse);
            return await ConvertContent<T>(response, await httpResponse.Content.ReadAsStreamAsync());
        }

        /// <summary>
        /// Perform a DELETE call
        /// </summary>
        /// <param name="endpoint">uri endpoint</param>
        /// <param name="headers">authentication cookie</param>
        /// <param name="body">body data</param>
        /// <param name="parameters">parameters</param>
        /// <returns>web response</returns>
        public static async Task<bool> DeleteAsync(Uri endpoint, WebHeaders headers, object body, object parameters = null)
        {
            var uri = CombineUri(endpoint, parameters);
            using var httpResponse = await SendRequest(uri, HttpMethod.Delete, headers, body);
            var response = new WebResponse(httpResponse);
            response.EnsureSuccessStatusCode(httpResponse);
            return await ConvertContent<bool>(response, await httpResponse.Content.ReadAsStreamAsync());
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets a uri combining the endpoint and parameters.
        /// </summary>
        /// <param name="endpoint">uri endpoint</param>
        /// <param name="parameters">parameters</param>
        /// <returns>combined uri</returns>
        private static Uri CombineUri(Uri endpoint, object parameters)
        {
            var queryParameters = new StringBuilder();
            var parameterMap = ToObjectMap(parameters);
            if (parameterMap.Any())
            {
                queryParameters = new StringBuilder("?");

                foreach (var param in parameterMap)
                {
                    queryParameters.Append($"{param.Key}={param.Value}&");
                }

                queryParameters.Remove(queryParameters.Length - 1, 1);
            }

            return new Uri(endpoint, queryParameters.ToString());
        }

        /// <summary>
        /// Converts anonymous object properties to an object map.
        /// </summary>
        /// <param name="parameters">parameters</param>
        /// <returns>object map</returns>
        private static IDictionary<string, object> ToObjectMap(object parameters)
        {
            var objectMap = new Dictionary<string, object>();

            if (parameters != null)
            {
                if (parameters.GetType().IsAnonymousType())
                {
                    TypeDescriptor.GetProperties(parameters)
                        .OfType<PropertyDescriptor>()
                        .ToList()
                        .ForEach(x => objectMap.Add(x.Name, ConvertValueToStringParameter(x.GetValue(parameters))));
                }
                else if (parameters is string)
                {
                    foreach (var param in parameters.ToString().Split('&'))
                    {
                        var assignment = param.Split('=');
                        objectMap.Add(assignment[0], assignment.Length > 1 ? Uri.UnescapeDataString(assignment[1]) : null);
                    }
                }
                else
                {
                    objectMap = (Dictionary<string, object>)parameters;
                }
            }

            return objectMap;
        }

        /// <summary>
        /// Converts value to string parameter.
        /// </summary>
        /// <param name="value">parameter value</param>
        /// <returns>string parameter</returns>
        private static string ConvertValueToStringParameter(object value)
        {
            var parameterBuilder = new StringBuilder();

            switch (value)
            {
                case IList list:
                    if (list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            parameterBuilder.Append(item);
                            parameterBuilder.Append(",");
                        }

                        parameterBuilder.Remove(parameterBuilder.Length - 1, 1);
                    }
                    break;

                // Convert dictionary parameter to comma separated "key|value" string.
                case IDictionary dictionary:
                    if (dictionary.Count > 0)
                    {
                        foreach (var item in dictionary.Keys)
                        {
                            parameterBuilder.Append($"{item}|{dictionary[item]}");
                            parameterBuilder.Append(",");
                        }

                        parameterBuilder.Remove(parameterBuilder.Length - 1, 1);
                    }
                    break;

                // Convert all other value to string.
                default:
                    parameterBuilder.Append(value);
                    break;
            }

            return parameterBuilder.ToString();
        }

        /// <summary>
        /// Sets up the HttpClient request header.
        /// </summary>
        /// <param name="client">http client</param>
        /// <param name="uri">endpoint uri</param>
        /// <param name="headers">http headers</param>
        /// <param name="body">body data</param>
        private static void SetupRequestHeader(HttpClient client, Uri uri, WebHeaders headers, object body)
        {
            var index = uri.AbsoluteUri.IndexOf(uri.AbsolutePath, StringComparison.Ordinal);
            client.BaseAddress = new Uri(uri.AbsoluteUri.Remove(index));
            client.DefaultRequestHeaders.Accept.Clear();

            if (body != null)
            {
                switch (body)
                {
                    case byte[] _:
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(WebContentType.Octet));
                        break;

                    case string _:
                        var data = body.ToString().TrimStart();
                        if (data.StartsWith("{") || data.StartsWith("["))
                        {
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(WebContentType.Json));
                        }
                        else if (data.StartsWith("<"))
                        {
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(WebContentType.Xml));
                        }
                        else
                        {
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(WebContentType.Text));
                        }
                        break;
                }

                client.DefaultRequestHeaders.TryAddWithoutValidation("Access-Control-Allow-Origin", "*");
            }

            headers.ForEach(x => client.DefaultRequestHeaders.TryAddWithoutValidation(x.Key, x.Value));
        }

        /// <summary>
        /// Issue a http request
        /// </summary>
        /// <param name="uri">endpoint uri</param>
        /// <param name="method">http method</param>
        /// <param name="headers">http headers</param>
        /// <param name="body">body data</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>response message</returns>
        private static async Task<HttpResponseMessage> SendRequest(Uri uri, HttpMethod method, WebHeaders headers, object body)
        {
            using var client = new HttpClient();
            SetupRequestHeader(client, uri, headers, body);

            if (method.Method == HttpMethod.Get.Method)
            {
                return await client.GetAsync(uri.PathAndQuery);
            }

            if (method.Method == HttpMethod.Post.Method)
            {
                var content = body != null
                    ? body is byte[] bytes
                        ? new ByteArrayContent(bytes)
                        : new StringContent(body.ToString(), Encoding.UTF8, WebContentType.Json)
                    : null;

                return await client.PostAsync(uri.PathAndQuery, content);
            }

            if (method.Method == HttpMethod.Put.Method)
            {
                var content = body != null
                    ? body is byte[] bytes
                        ? new ByteArrayContent(bytes)
                        : new StringContent(body.ToString(), Encoding.UTF8, WebContentType.Json)
                    : null;

                return await client.PutAsync(uri.PathAndQuery, content);
            }

            if (method.Method == HttpMethod.Delete.Method)
            {
                return await client.DeleteAsync(uri.PathAndQuery);
            }

            throw new MissingMethodException(nameof(method));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="contentStream"></param>
        /// <returns></returns>
        private static async Task<T> ConvertContent<T>(WebResponse response, Stream contentStream)
        {
            if (typeof(T) == typeof(bool))
            {
                return (T)(object)(contentStream != null);
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
            var content = await reader.ReadToEndAsync();
            if (content == null) return default;

            switch (response.ContentType)
            {
                case WebContentType.Json:
                    return JsonConvert.DeserializeObject<T>(content);

                case WebContentType.Xml:
                {
                    var serializer = new XmlSerializer(typeof(T));
                    using var stringReader = new StringReader(content);
                    return (T)serializer.Deserialize(stringReader);
                }

                default:
                    return (T)(object)content;
            }
        }

        #endregion
    }
}
