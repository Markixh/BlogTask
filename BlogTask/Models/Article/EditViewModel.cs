﻿using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace BlogTask.Models.Article
{
    public class EditViewModel
    {
        [Required(ErrorMessage = "Поле Заголовок обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Заголовок", Prompt = "Введите заголовок статьи")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Поле Контент обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Контент", Prompt = "Введите текст статьи")]
        public string Text { get; set; }

        public Guid Guid { get; set; }
    }
}
