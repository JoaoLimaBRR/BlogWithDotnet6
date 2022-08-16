using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatorio")]
        [StringLength(maximumLength: 100, MinimumLength = 10,ErrorMessage = "O nome deve conter entre 10 a 100 caracteres")]
        public string Name { get; set; }
        [Required(ErrorMessage = "O email é obrigatorio")]
        [EmailAddress(ErrorMessage = "o e-mail é invalido")]
        public string Email { get; set; }

    }
}
