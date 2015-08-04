using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Data
{
    class ApiGetResponseList<T> : ApiBaseResponse
    {
        public List<T> Data { get; set; }
    }
}
