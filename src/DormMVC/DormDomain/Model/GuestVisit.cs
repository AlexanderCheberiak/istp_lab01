using System;
using System.Collections.Generic;

namespace DormDomain.Model;

public partial class GuestVisit
{
    public int VisitId { get; set; }

    public string? GuestName { get; set; }

    public short StudentId { get; set; }

    public DateOnly? VisitDate { get; set; }

    public virtual Student Student { get; set; } = null!;
}
