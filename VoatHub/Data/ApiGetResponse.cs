using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Data
{
    class ApiGetResponse<T> : ApiResponse<T>
    {
        private T _Data;

        public ApiGetResponse (T data)
        {
            _Data = data;
        }

        public T Data
        {
            get
            {
                return _Data;
            }
        }

        public ErrorInfo Error
        {
            get
            {
                return null;
            }
        }

        public bool Success
        {
            get
            {
                return true;
            }
        }
    }
}
