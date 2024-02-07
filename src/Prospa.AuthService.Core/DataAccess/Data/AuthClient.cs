using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prospa.AuthService.Core.DataAccess.Data
{
    [Table("auth_client")]
    public class AuthClient
    {
        [Key]
        public long id { get; set; }
        public string service_name { get; set; }
        public string service_type { get; set; }
        public string secret_Key { get; set; }
        public string secret_Id { get; set; }
        public bool is_Active { get; set; }
        public DateTime date_Joined { get; set; }

    }

}
