using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DormDomain.Model;

public partial class Student
{
    [Display(Name = "ID студента")]
    [Key]
    public short StudentId { get; set; }

    [Display(Name = "Повне ім'я")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public string FullName { get; set; } = null!;

    [Display(Name = "Дата народження")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public DateOnly? BirthDate { get; set; }

    [Display(Name = "Факультет")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public byte? FacultyId { get; set; }

    [Display(Name = "Курс")]
    [Required(ErrorMessage = "Недопустиме значення!")]
    public byte? Course { get; set; }

    [Display(Name = "Кімната")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public short? RoomId { get; set; }

    [Display(Name = "Дата заселення")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Дата оновлення")]
    public DateTime? UpdatedAt { get; set; }

    [Display(Name = "Факультет")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public virtual Faculty? Faculty { get; set; }

    public virtual ICollection<GuestVisit> GuestVisits { get; set; } = new List<GuestVisit>();

    [Display(Name = "Кімната")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public virtual Room? Room { get; set; }

    public virtual ICollection<StudentChange> StudentChanges { get; set; } = new List<StudentChange>();

    public virtual ICollection<StudentPayment> StudentPayments { get; set; } = new List<StudentPayment>();
}
