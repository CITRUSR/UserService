using System.ComponentModel.DataAnnotations;

namespace UserService.Domain.Entities;

public class Student
{
    public long Id { get; set; }
    [MaxLength(32)] [Required] public string FirstName { get; set; }
    [MaxLength(32)] [Required] public string LastName { get; set; }
    [MaxLength(32)] public string? PatronymicName { get; set; }
    [Required] public Group Group { get; set; }
    [Required] public int GroupId { get; set; }
    public DateTime? DroppedOutAt { get; set; }
}