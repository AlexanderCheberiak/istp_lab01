using System;
using System.Collections.Generic;

namespace DormDomain.Model;

public partial class Faculty
{
    public byte FacultyId { get; set; }

    public string? FacultyName { get; set; }

    public string? Dean { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
