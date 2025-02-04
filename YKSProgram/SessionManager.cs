using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YKSProgram
{
    public static class SessionManager
    {
        public static int CurrentUserID { get; set; }
        public static User CurrentUser { get; set; }
    }

    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool EmailVerified { get; set; }
    }

}