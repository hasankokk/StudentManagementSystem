using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StudentManagementSystem.Models;

[Index(nameof(ClassroomId), IsUnique = true)]
public class Classroom
{
    public int ClassroomId { get; set; }
    [MaxLength(100)]
    public string? Name { get; set; }
    
    public ICollection<Student>? Students { get; set; } = new List<Student>();
    public ICollection<Teacher>? Teachers { get; set; } = new List<Teacher>();
    public ICollection<Lesson>? Lessons { get; set; } = new List<Lesson>();
}