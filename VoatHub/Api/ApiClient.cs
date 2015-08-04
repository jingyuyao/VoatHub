using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

using VoatHub.Data;

namespace VoatHub.Api
{
    class ApiClient
    {
        private HttpClient httpClient;
        private string baseUri;

        public ApiClient(string apiKey, string baseUri)
        {
            httpClient = new HttpClient();
            this.baseUri = baseUri;

            var headers = httpClient.DefaultRequestHeaders;

            headers.Add("Voat-ApiKey", apiKey);
        }

        public async Task<ApiGetResponseList<ApiSubmission>> GetSubmissions(string subverse)
        {
            Uri uri = new Uri(this.baseUri + "v/" + subverse);
            var result = await this.httpClient.GetAsync(uri);
            var transformedResult = this.transformResultList<ApiSubmission>(result);

            return new Task<ApiGetResponseList<ApiSubmission>>(transformedResult);
        }

        private ApiBaseResponse transformResultList<T>(HttpResponseMessage result)
        {
            try
            {
                result.EnsureSuccessStatusCode();
                return (ApiGetResponseList<T>) result;
            }
            catch(Exception e)
            {
                return new ApiBaseResponse();
            }
        }
    }
}
