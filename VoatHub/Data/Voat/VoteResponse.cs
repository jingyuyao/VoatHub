using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Data.Voat
{
    public class VoteResponse
    {
        public int recordedValue { get; set; }
        public bool success { get; set; }
        public ProcessResult result { get; set; }
        public string resultName { get; set; }
        public string message { get; set; }
    }

    public enum ProcessResult
    {
        NotProcessed, Success, Ignored, Denied
    }
}
