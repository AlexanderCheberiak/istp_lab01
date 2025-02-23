using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DormDomain.Model;

public partial class Room
{
    [Display(Name = "ID кімнати")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public short RoomId { get; set; }

    [Display(Name = "Номер кімнати")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public short? RoomNumber { get; set; }

    [Display(Name = "Місткість")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public byte? Capacity { get; set; }

    [Display(Name = "Дата створення")]
    [Required(ErrorMessage = "Поле є обов'язковим!")]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "Дата оновлення")]
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
