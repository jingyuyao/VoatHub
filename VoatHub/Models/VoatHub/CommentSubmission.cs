using System;
using System.Collections.Generic;

using VoatHub.Models.Voat.v1;

namespace VoatHub.Models.VoatHub
{
    public class CommentSubmission
    {
        public ApiSubmission Submission { get; set; }
        public List<ApiComment> Comments { get; set; }
    }
}
