using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DormDomain.Model;

public partial class StudentChange
{
    [Display(Name = "ID зміни")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public short ChangeId { get; set; }

    [Display(Name = "ID студента")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public short StudentId { get; set; }

    [Display(Name = "Дата зміни")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public DateOnly? ChangeDate { get; set; }

    [Display(Name = "Змінене поле")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public string? ChangeField { get; set; }

    [Display(Name = "Старе значення")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public string? OldValue { get; set; }

    [Display(Name = "Нове значення")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public string? NewValue { get; set; }

    public virtual Student Student { get; set; } = null!;
}
