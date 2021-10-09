using System;
using System.Collections.Generic;
using System.Text;

namespace CafeLib.BsvSharp.Api.WhatsOnChain.Models
{
    public class Response
    {
        public bool IsSuccessful { get; }

        public Exception Exception { get; }

        internal Response()
        {
            IsSuccessful = true;
        }

        internal Response(Exception exception)
        {
            IsSuccessful = false;
            Exception = exception;
        }
    }
}
