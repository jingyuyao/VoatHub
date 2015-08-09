using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Api
{
    /// <summary>
    /// Wraps <see cref="UriBuilder"/> class to add an apiPath to before every path.
    /// </summary>
    public class ApiUriBuilder
    {
        private UriBuilder uriBuilder;
        private string apiPath;

        public ApiUriBuilder(string scheme, string host, string apiPath)
        {
            uriBuilder = new UriBuilder();
            uriBuilder.Scheme = scheme;
            uriBuilder.Host = host;
            this.apiPath = apiPath;
        }

        /// <summary>
        /// Get the absolute API uri by combining domain, apiPath, relative uri and a query string.
        /// </summary>
        /// <param name="path">A relative uri. Relative uri should not be prefixed with a '/'.</param>
        /// <param name="query">Query string with key=value separated by &. Should not prefix '?'.</param>
        /// <returns>The absolute uri of the API endpoint.</returns>
        public Uri Uri(string path, string query)
        {
            uriBuilder.Path = apiPath + path;
            uriBuilder.Query = query;

            return uriBuilder.Uri;
        }

        /// <summary>
        /// Get the absolute API uri by combining domain, apiPath and a relative uri.
        /// </summary>
        /// <param name="path">A relative uri. Relative uri should not be prefixed with a '/'.</param>
        /// <returns>The absolute uri of the API endpoint.</returns>
        public Uri Uri(string path)
        {
            return Uri(path, null);
        }
    }
}
