using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

using Newtonsoft.Json;

namespace VoatHub.Api.Client
{
    /// <summary>
    /// Extends <see cref="ApiClient"/> to send and receive Json data.
    /// </summary>
    public abstract class JsonApiClient : ApiClient
    {
        private static readonly HttpMediaTypeHeaderValue REQUEST_HEADER_VALUE = new HttpMediaTypeHeaderValue("application/json");

        /// <summary>
        /// Deserializes Json response into user defined type.
        /// </summary>
        /// <typeparam name="T">Type the response is deserialized to.</typeparam>
        /// <param name="response">Response to deserialize.</param>
        /// <returns>Serialized response.</returns>
        /// <exception cref="SerializationException"></exception>
        protected override async Task<T> deserializeResponse<T>(HttpResponseMessage response)
        {
            try
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseContent, "JsonApiClient");
                
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch(JsonException e)
            {
                Debug.WriteLine(e);
                throw new SerializationException();
            }
        }

        protected override HttpMediaTypeHeaderValue requestContentType
        {
            get
            {
                return REQUEST_HEADER_VALUE;
            }
        }
    }
}
