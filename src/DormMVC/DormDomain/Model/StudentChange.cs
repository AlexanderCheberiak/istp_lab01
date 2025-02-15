using System;
using System.Collections.Generic;

namespace DormDomain.Model;

public partial class StudentChange
{
    public short ChangeId { get; set; }

    public short StudentId { get; set; }

    public DateOnly? ChangeDate { get; set; }

    public string? ChangeField { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public virtual Student Student { get; set; } = null!;
}
