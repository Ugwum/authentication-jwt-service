using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prospa.AuthService.Core.Model.Configuration
{
    public class AppSettings
    {
        public string CertificateFilePath { get; set; }
        public string CertificatePassword { get; set; }
        public string PublicKeyPair { get; set; }
        public string PrivateKeyPair { get; set; }

        public string ProspaLoginAPIUrl { get; set; }
    }
}
