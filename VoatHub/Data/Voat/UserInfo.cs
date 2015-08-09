using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Data.Voat
{
    public class UserInfo
    {
        public string userName { get; set; }
        public string registrationDate { get; set; }
        public string bio { get; set; }
        public string profilePicture { get; set; }
        public CommentPoints commentPoints { get; set; }
        public SubmissionPoints submissionPoints { get; set; }
        public CommentVoting commentVoting { get; set; }
        public SubmissionVoting submissionVoting { get; set; }
        public List<Badge> badges { get; set; }
    }

    public class CommentPoints
    {
        public int sum { get; set; }
        public int upCount { get; set; }
        public int downCount { get; set; }
    }

    public class SubmissionPoints
    {
        public int sum { get; set; }
        public int upCount { get; set; }
        public int downCount { get; set; }
    }

    public class CommentVoting
    {
        public int sum { get; set; }
        public int upCount { get; set; }
        public int downCount { get; set; }
    }

    public class SubmissionVoting
    {
        public int sum { get; set; }
        public int upCount { get; set; }
        public int downCount { get; set; }
    }

    public class Badge
    {
        public string name { get; set; }
        public string awardedDate { get; set; }
        public string title { get; set; }
        public string badgeGraphic { get; set; }
    }

    public class ApiSubscription
    {
        public int type { get; set; }
        public string typeName { get; set; }
        public string name { get; set; }
    }
}
