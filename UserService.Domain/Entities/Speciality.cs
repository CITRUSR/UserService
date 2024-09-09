using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserService.Domain.Entities;

public class Speciality
{
    public int Id { get; set; }
    public ICollection<Group> Groups { get; } = new List<Group>();

    [MaxLength(256)]
    [Required]
    public string Name { get; set; }

    [MaxLength(10)]
    [Required]
    public string Abbreviation { get; set; }

    [DefaultValue(0)]
    public Decimal Cost { get; set; }

    [Required]
    [Range(1, byte.MaxValue)]
    public byte DurationMonths { get; set; }
    public bool IsDeleted { get; set; } = false;
}
