using System.ComponentModel.DataAnnotations;

namespace DormInfrastructure.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "Пошта")]
        [EmailAddress]
        public string Email { get; set; }
    }
}