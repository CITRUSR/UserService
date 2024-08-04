using System.ComponentModel.DataAnnotations;

namespace UserService.Domain.Entities;

public class Teacher
{
    [Key]
    public Guid Id { get; set; }

    public Guid SsoId { get; set; }

    [MaxLength(32)]
    [Required]
    public string FirstName { get; set; }

    [MaxLength(32)]
    [Required]
    public string LastName { get; set; }

    [MaxLength(32)]
    public string? PatronymicName { get; set; }

    [Required]
    public short RoomId { get; set; }
    public ICollection<Group> Groups { get; } = new List<Group>();
    public DateTime? FiredAt { get; set; }
}
