using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.DataAccess.Data
{
    [Table("auth_serviceclient")]
    public class AuthClient
    {
        [Key]
        public long id { get; set; }
        public string service_name { get; set; }
        public string service_description { get; set; }
        public string service_type { get; set; }
        public string secretKey { get; set; }
        public string secretId { get; set; }
        public bool isactive { get; set; }
        public DateTime datecreated { get; set; }

    }

}
