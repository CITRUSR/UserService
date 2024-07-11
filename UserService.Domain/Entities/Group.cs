using System.ComponentModel.DataAnnotations;

namespace UserService.Domain.Entities;

public class Group
{
    public int Id { get; set; }
    [Required] public int SpecialityId { get; set; }
    [Required] public Speciality Speciality { get; set; }
    [Required] public Guid CuratorId { get; set; }
    [Required] public Teacher Curator { get; set; }
    [Required] [Range(1, byte.MaxValue)] public byte CurrentCourse { get; set; }
    [Required] [Range(1, byte.MaxValue)] public byte CurrentSemester { get; set; }
    [Required] [Range(1, byte.MaxValue)] public byte SubGroup { get; set; }
    [Required] public DateTime StartedAt { get; set; }
    public ICollection<Student> Students { get; } = new List<Student>();
    public DateTime? GraduatedAt { get; set; }

    public override string ToString()
    {
        return $"{CurrentCourse}-{Speciality.Abbreavation}{SubGroup}";
    }
}