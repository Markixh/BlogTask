using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace BlogTask.Models.Article
{
    public class AddViewModel
    {
        public string Title { get; set; }

        public string Text { get; set; }
    }
}
