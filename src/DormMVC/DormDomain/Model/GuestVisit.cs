using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DormDomain.Model;

public partial class GuestVisit : Entity
{
    [Display(Name = "ID відвідування")]
    public int VisitId { get; set; }

    [Display(Name = "Ім'я гостя")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public string? GuestName { get; set; }

    [Display(Name = "ID студента")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public short StudentId { get; set; }

    [Display(Name = "Дата відвідування")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public DateOnly? VisitDate { get; set; }

    [Display(Name = "Студент")]
    public virtual Student Student { get; set; } = null!;
}
