using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StudentManagementSystem.Models;

public class Student
{
    public int StudentId { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public ICollection<Classroom>? Classrooms { get; set; } = new List<Classroom>();
}