using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prospa.AuthService.Core.Model
{

    public class UserVM
    {
        public string acceesstoken { get; set; }
        public string email { get; set; }
        
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string user_type { get; set; }       
        public string biz_creation { get; set; }
        public string unregistered_creation { get; set; }
        public int prospa_user_id { get; set; }
        public DateTime date_joined { get; set; }
        public string phone { get; set; }
        public object biz_captain_property { get; set; }

    }


    public class RequestResult
    {
        public string code { get; set; }
        public string message { get; set; }
        public bool Succeeded { get; set; }
        public dynamic data { get; set; }

    }

    public class GenericErrorResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public dynamic data { get; set; }
    }

    public class LoginUserResponse
    {
        public string email { get; set; }
        public string token { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string user_type { get; set; }
        public int status_code { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public string biz_creation { get; set; }
        public string unregistered_creation { get; set; }
        public int prospa_user_id { get; set; }
        public DateTime date_joined { get; set; }
        public string phone { get; set; }
        public object biz_captain_property { get; set; }

    }
}
