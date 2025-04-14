using System.ComponentModel.DataAnnotations;

namespace DormInfrastructure.Models;
public class RegisterViewModel
{
    [Required]
    [Display(Name = "Повне ім'я")]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Display(Name = "Пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Повторіть пароль")]
    public string ConfirmPassword { get; set; }
}
