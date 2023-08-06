using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogTask.Data.Queries
{
    public class UpdateUserQuery
    {
        public string NewLogin { get; set; }
        public string NewFirstName { get; set; }
        public string NewLastName { get; set; }
        public string NewSurName { get; set; }
        public string NewPassword { get; set; }
        public UpdateUserQuery(string newLogin, string newFirstName, string newLastName, string newSurName, string newPassword)
        {
            NewLogin = newLogin;
            NewFirstName = newFirstName;
            NewLastName = newLastName;
            NewSurName = newSurName;
            NewPassword = newPassword;
        }
    }
}
