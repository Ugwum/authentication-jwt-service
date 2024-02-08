using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prospa.AuthService.Core.Model.Configuration
{
    public class JWTSettings
    {
        public int JWTAccessTokenExpiry { get; set; }
        public string JWTIssuer { get; set; }
        public string JWTAudience { get; set; }
        public int JWTRefreshTokenExpiry { get; set; }


    }
}
