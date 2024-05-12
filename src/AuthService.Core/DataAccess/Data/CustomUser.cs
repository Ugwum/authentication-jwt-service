using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Core.DataAccess.Data
{
    [Table("hoodregapp_customuser")]
    public class CustomUser
    {
        [Key]
        public long id { get; set; }
        public string first_Name { get; set; }
        public string last_Name { get; set; }
        public string email { get; set; }       
        public bool is_Active { get; set; }
        public DateTime date_Joined { get; set; }
        public string phone { get; set; }
        public string username { get; set; }
        public string user_Type { get; set; } = "ecosystem";
        public bool email_Confirmed { get; set; }         
        public string password { get; set; }
    }

}
