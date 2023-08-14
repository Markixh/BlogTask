using BlogTask.Data.Models;

namespace BlogTask.Models.Account
{    
    public class UserViewModel : User
    {
        public string GetFullName()
        {
            return String.Concat(FirstName, " ", LastName, " ", SurName);
    }
    }
}
