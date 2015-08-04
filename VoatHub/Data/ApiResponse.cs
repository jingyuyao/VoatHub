using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Data
{
    interface ApiResponse<T>
    {
        T Data { get; }
        bool Success { get; }
        ErrorInfo Error { get; }
    }
}
