using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace BlogTask.Models.Role
{
    public class EditViewModel
    {
        [Required(ErrorMessage = "Поле Название обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Название", Prompt = "Введите название роли")]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Описание", Prompt = "Введите описание роли")]
        public string Description { get; set; }

        public int Id;
    }
}
