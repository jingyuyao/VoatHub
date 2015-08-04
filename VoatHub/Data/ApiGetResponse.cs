using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Data
{
    class ApiGetResponse<T> : ApiBaseResponse
    {
        public T Data { get; }
    }
}
