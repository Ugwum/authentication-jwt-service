using AuthService.Core.DataAccess.Data;
using AuthService.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.Utils
{
    public class Transform
    {
        public static UserVM From(LoginUserResponse loginUser)
        {
            return new UserVM
            {
                user_type = loginUser.user_type,
                biz_captain_property = loginUser.biz_captain_property,
                biz_creation = loginUser.biz_creation,
                date_joined = loginUser.date_joined,
                email = loginUser.email,
                first_name = loginUser.first_name,
                last_name = loginUser.last_name,
                phone = loginUser.phone,
                unregistered_creation = loginUser.unregistered_creation,
                prospa_user_id = loginUser.prospa_user_id,

            };
        }

        //public static UserVM From(CustomUser loginUser)
        //{
        //    return new UserVM
        //    {
        //        user_type = loginUser.user_Type,
        //        biz_captain_property = loginUser.biz_captain_property,
        //        biz_creation = loginUser.biz_creation,
        //        date_joined = loginUser.date_joined,
        //        email = loginUser.email,
        //        first_name = loginUser.first_name,
        //        last_name = loginUser.last_name,
        //        phone = loginUser.phone,
        //        unregistered_creation = loginUser.unregistered_creation,
        //        prospa_user_id = loginUser.prospa_user_id,

        //    };
        //}
    }
}
