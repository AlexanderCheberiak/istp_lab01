using System;
using System.Collections.Generic;

namespace DormDomain.Model;

public partial class Room
{
    public short RoomId { get; set; }

    public short? RoomNumber { get; set; }

    public byte? Capacity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
