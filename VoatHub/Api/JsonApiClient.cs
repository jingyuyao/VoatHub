using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

using Newtonsoft.Json;

namespace VoatHub.Api
{
    public abstract class JsonApiClient : ApiClient
    {
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
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch(JsonSerializationException)
            {
                throw new SerializationException();
            }
        }
    }
}
