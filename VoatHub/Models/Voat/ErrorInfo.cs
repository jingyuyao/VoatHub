using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Models.Voat
{
    /// <summary>
    /// Container for an error information.
    /// </summary>
    /// <remarks>http://fakevout.azurewebsites.net/api/help/resourcemodel?modelName=ErrorInfo</remarks>
    public class ErrorInfo
    {
        public string type { get; set; }
        public string message { get; set; }
    }
}
