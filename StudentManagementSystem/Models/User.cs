using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StudentManagementSystem.Models;

[Index(nameof(Tckn), IsUnique = true)]
public class User
{
    public int UserId { get; set; }
    [MaxLength(11)]
    public long Tckn { get; set; }
    public string? Password { get; set; }
    [MaxLength(50)]
    public required string Name { get; set; }
    [MaxLength(50)]
    public required string Surname { get; set; }
    [MaxLength(50)]

    public Role Role { get; set; } = Role.Student;
    public bool IsTempPassword { get; set; } = true;
}