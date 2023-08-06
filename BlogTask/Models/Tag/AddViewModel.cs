using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace BlogTask.Models.Tag
{
    public class AddViewModel
    {
        [Required(ErrorMessage = "Поле Название обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Название", Prompt = "Введите название тега")]
        public string Name { get; set; }
    }
}
