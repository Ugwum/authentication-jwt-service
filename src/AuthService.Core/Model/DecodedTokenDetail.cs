using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.Model
{ 
    public class DecodedTokenDetail
    {
        public string aud { get; set; }
        public string username { get; set; }
        public string usertype { get; set; }
        public DateTime iss { get; set; }
        public DateTime exp { get; set; }
        public string refexp { get; set; }

    }
}
