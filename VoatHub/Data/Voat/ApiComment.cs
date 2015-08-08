using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Data.Voat
{
    /// <summary>
    /// Container for a comment.
    /// </summary>
    /// <remarks>http://fakevout.azurewebsites.net/api/help/resourcemodel?modelName=ApiComment</remarks>
    public class ApiComment
    {
        public int id { get; set; }
        public int parentID { get; set; }
        public int submissionID { get; set; }
        public string subverse { get; set; }
        public DateTime date { get; set; }
        public DateTime lastEditDate { get; set; }
        public int upVotes { get; set; }
        public int downVotes { get; set; }
        public string userName { get; set; }

        /// <summary>
        /// Child comment count. This is a count of direct decedents only.
        /// </summary>
        public int childCount { get; set; }

        /// <summary>
        /// Level of the comment. 0 is root. This value is relative to the parent comment. If you are loading mid-branch 0 will be returned for the starting position comment.
        /// </summary>
        public int level { get; set; }
        public string content { get; set; }
        public string formattedContent { get; set; }
    }
}
