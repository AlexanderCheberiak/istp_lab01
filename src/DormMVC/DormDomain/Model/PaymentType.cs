using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DormDomain.Model;

public partial class PaymentType : Entity
{
    [Display(Name = "ID платежу")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public byte PaymentTypeId { get; set; }

    [Display(Name = "Назва платежу")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public string PaymentName { get; set; } = null!;

    public virtual ICollection<StudentPayment> StudentPayments { get; set; } = new List<StudentPayment>();
}
