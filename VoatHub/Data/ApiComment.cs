using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Data
{
    class ApiComment
    {
        public int Id { get; }
        public int ParentID { get; }
        public int SubmissionID { get; }
        public string Subverse { get; }
        public DateTime Date { get; }
        public DateTime LastEditDate { get; }
        public int UpVotes { get; }
        public int DownVotes { get; }
        public string UserName { get; }
        public int ChildCount { get; }
        public int Level { get; }
        public string Content { get; }
        public string FormattedContent { get; }
    }
}
