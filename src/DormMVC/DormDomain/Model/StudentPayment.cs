using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DormDomain.Model;

public partial class StudentPayment
{
    [DisplayName("ID оплати")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public short PaymentId { get; set; }

    [DisplayName("ID студента")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public short StudentId { get; set; }

    [DisplayName("Сума")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public int? Amount { get; set; }

    [DisplayName("Тип платежу")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public byte? PaymentTypeId { get; set; }

    [DisplayName("Дата платежу")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public DateOnly? PaymentDate { get; set; }

    [DisplayName("Тип платежу")]
    public virtual PaymentType? PaymentType { get; set; }

    [DisplayName("Студент")]
    public virtual Student Student { get; set; } = null!;
}
