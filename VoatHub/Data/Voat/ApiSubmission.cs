using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace VoatHub.Data.Voat
{
    /// <summary>
    /// Container for a submission.
    /// </summary>
    /// <remarks>http://fakevout.azurewebsites.net/api/help/resourcemodel?modelName=ApiSubmission</remarks>
    public class ApiSubmission
    {
        public int id { get; set; }
        public int commentCount { get; set; }
        public DateTime date { get; set; }
        public int upVotes { get; set; }
        public int downVotes { get; set; }
        public string lastEditDate { get; set; }
        public int views { get; set; }
        public string userName { get; set; }
        public string subverse { get; set; }
        public Uri thumbnail { get; set; }
        public string title { get; set; }

        /// <summary>
        /// The type of submission. Values: 1 for Self Posts, 2 for Link Posts
        /// </summary>
        public int type { get; set; }
        public Uri url { get; set; }
        public string content { get; set; }
        public string formattedContent { get; set; }
    }
}
