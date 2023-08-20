using System.ComponentModel.DataAnnotations;

namespace BlogTask.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Поле Логин обязательно для заполнения")]
        [DataType(DataType.Text)]
        public string Login { get; set; }

        [Required(ErrorMessage = "Поле Пароль обязательно для заполнения")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
