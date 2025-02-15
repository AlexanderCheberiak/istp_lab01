using System;
using System.Collections.Generic;

namespace DormDomain.Model;

public partial class PaymentType
{
    public byte PaymentTypeId { get; set; }

    public string PaymentName { get; set; } = null!;

    public virtual ICollection<StudentPayment> StudentPayments { get; set; } = new List<StudentPayment>();
}
