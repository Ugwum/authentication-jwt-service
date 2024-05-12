using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.Model
{
   
    public class GenerateTokenRequest
    {
        public string username { get; set; }

        public string password { get; set; }
    }

    public class RefreshTokenReq
    {
        public string expiredToken { get; set; }
    }


}
