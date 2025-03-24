using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DormDomain.Model;

public partial class Faculty : Entity
{
    [Display(Name = "ID факультету")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public byte FacultyId { get; set; }

    [Display(Name = "Назва факультету")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public string? FacultyName { get; set; }

    [Display(Name = "Декан")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public string? Dean { get; set; }

    [Display(Name = "Дата створення")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "Дата оновлення")]
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
