using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace VoatHub.Data
{
    [DataContract]
    class ApiSubmission
    {
        [DataMember(Name = "id")]
        public int Id { get; }
        public int CommentCount { get; }
        public DateTime Date { get; }
        public int UpVotes { get; }
        public int DownVotes { get; }
        public DateTime LastEditDate { get; }
        public int Views { get; }
        public string UserName { get; }
        public string Subverse { get; }
        public string Thumbnail { get; }
        public string Title { get; }
        public int Type { get; }
        public string Url { get; }
        public string Content { get; }
        public string FormattedContent { get; }
    }
}
