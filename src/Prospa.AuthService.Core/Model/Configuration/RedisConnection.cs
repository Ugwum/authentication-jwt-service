using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prospa.AuthService.Core.Model.Configuration
{
    public class RedisConnection
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Password { get; set; }
        public bool UseSSL { get; set; }
        public int DefaultCacheExpiration { get; set; }
    }
}
