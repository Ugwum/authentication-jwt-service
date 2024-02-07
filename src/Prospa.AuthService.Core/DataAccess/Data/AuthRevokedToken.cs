using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Prospa.AuthService.Core.DataAccess.Data
{
    [Table("auth_revokedtoken")]
    public class AuthRevokedToken
    {
        [Key]
        public long id { get; set; }
        public string auth_token { get; set; }
        public DateTime revoked_at { get; set; }
        public DateTime revoken_expiry { get; set; }

    }

}
