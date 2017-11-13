using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionExtendedServer.WebAPI.Models
{
    public class BaseResult
    {
        public bool IsError { get; set; }
        public string Message { get; set; }

        public BaseResult(bool error = false, string message = "Success")
        {

            IsError = error;
            Message = message;
        }

    }
}
