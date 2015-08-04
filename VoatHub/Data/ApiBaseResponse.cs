using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Data
{
    class ApiBaseResponse
    {
        public bool Success { get; }
        public ErrorInfo error { get; }
    }
}
