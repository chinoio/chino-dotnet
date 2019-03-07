using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chino
{
    class ChinoApiException : Exception
    {
        public ChinoApiException(String message) : base(message)
        {
            
        }
    }
}
