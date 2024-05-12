using AuthService.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core
{
    public class CustomException : Exception
    {
        public string code { get; set; }
        public CustomException(string? errorcode)
        {
            this.code = errorcode ?? GeneralStatusCodes.UNEXPECTED_ERROR.code;
        }

        public CustomException(string? errorcode, string? message) : base(message)
        {
            this.code = errorcode ?? GeneralStatusCodes.UNEXPECTED_ERROR.code;
        }

        public CustomException(string? errorcode, string? message, Exception? innerException) : base(message, innerException)
        {
            this.code = errorcode ?? GeneralStatusCodes.UNEXPECTED_ERROR.code;
        }

        protected CustomException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
