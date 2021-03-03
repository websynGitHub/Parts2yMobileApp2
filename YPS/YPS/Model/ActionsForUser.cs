using System;
using System.Collections.Generic;
using System.Text;

namespace YPS.Model
{
    public class ActionsForUser
    {
        public string message { get; set; }
        public int status { get; set; }
        public List<ActionsForUserData> data { get; set; }
    }
    public class ActionsForUserData
    {
        public int ID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public int ActionID { get; set; }
        public string ActionName { get; set; }
        public string ActionCode { get; set; }
        public int Status { get; set; }
    }
}
