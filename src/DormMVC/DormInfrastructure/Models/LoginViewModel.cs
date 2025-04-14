using System.ComponentModel.DataAnnotations;

namespace DormInfrastructure.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Адреса електронної пошти")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Не виходити")]
        public bool RememberMe { get; set; }
    }
}