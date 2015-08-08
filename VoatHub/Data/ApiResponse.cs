using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Data
{
    /// <summary>
    /// Generic response returned by the Voat API.
    /// </summary>
    /// <remarks>http://fakevout.azurewebsites.net/api/help</remarks>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        public T data { get; set; }
        public bool success { get; set; }
        public ErrorInfo error { get; set; }
    }

    /// <summary>
    /// Generic response returned by the Voat API with no data.
    /// </summary>
    public class ApiResponse
    {
        public bool success { get; set; }
        public ErrorInfo error { get; set; }
    }
}
