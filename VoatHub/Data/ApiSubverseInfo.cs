using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Data
{
    class ApiSubverseInfo
    {
        public string Name { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTime CreationDate { get; }
        public int SubscriberCount { get; }
        public bool RatedAdult { get; }
        public string Sidebar { get; }
        public string Type { get; }
    }
}
