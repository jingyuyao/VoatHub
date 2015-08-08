using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Data.Voat
{
    /// <summary>
    /// Container for information pertaining to a subverse.
    /// </summary>
    /// <remarks>http://fakevout.azurewebsites.net/api/help/resourcemodel?modelName=ApiSubverseInfo</remarks>
    public class ApiSubverseInfo
    {
        public string name { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime creationDate { get; set; }
        public int subscriberCount { get; set; }
        public bool ratedAdult { get; set; }
        public string sidebar { get; set; }
        public string type { get; set; }
    }
}
