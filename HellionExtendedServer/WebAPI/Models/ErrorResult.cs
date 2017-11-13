using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionExtendedServer.WebAPI.Models
{
    public class ErrorResult : BaseResult
    {
        public Exception Exception { get; set; }

        public ErrorResult(Exception exception, bool error = true, string message = "Error")
            : base(error, message)
        {
            Exception = exception;
        }

    }
}
