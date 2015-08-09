using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Data.Voat
{
    public class ApiUserPreferences
    {
        public bool disableCustomCSS { get; set; }
        public bool enableNightMode { get; set; }
        public string language { get; set; }
        public bool openLinksNewWindow { get; set; }
        public bool enableAdultContent { get; set; }
        public bool publiclyDisplayVotes { get; set; }
        public bool publiclyDisplaySubscriptions { get; set; }
    }
}
