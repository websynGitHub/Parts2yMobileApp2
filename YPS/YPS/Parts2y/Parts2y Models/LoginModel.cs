using System;
using System.Collections.Generic;
using System.Text;

namespace YPS.Parts2y.Parts2y_Models
{
    public class LoginModel
    {
        public bool status { get; set; }
        public UserDetails UserDetails { get; set; }
        
    }
    public class UserDetails
    {
        //public int UniqueId { get; set; }

        public int ID { get; set; }
        public string username { get; set; }
        public string user_email { get; set; }
        public string user_password { get; set; }
        public int roleid { get; set; }
        public string EntityName { get; set; }
        public string Colorcode { get; set; }
    }
}
