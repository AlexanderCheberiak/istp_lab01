using System;
using System.Collections.Generic;

namespace DormDomain.Model;

public partial class Student
{
    public short StudentId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public byte? FacultyId { get; set; }

    public byte? Course { get; set; }

    public short? RoomId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Faculty? Faculty { get; set; }

    public virtual ICollection<GuestVisit> GuestVisits { get; set; } = new List<GuestVisit>();

    public virtual Room? Room { get; set; }

    public virtual ICollection<StudentChange> StudentChanges { get; set; } = new List<StudentChange>();

    public virtual ICollection<StudentPayment> StudentPayments { get; set; } = new List<StudentPayment>();
}
