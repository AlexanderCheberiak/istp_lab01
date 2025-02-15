using System;
using System.Collections.Generic;

namespace DormDomain.Model;

public partial class StudentPayment
{
    public short PaymentId { get; set; }

    public short StudentId { get; set; }

    public int? Amount { get; set; }

    public byte? PaymentTypeId { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public virtual PaymentType? PaymentType { get; set; }

    public virtual Student Student { get; set; } = null!;
}
