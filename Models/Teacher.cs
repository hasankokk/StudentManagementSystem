using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StudentManagementSystem.Models;

public class Teacher
{
    public int TeacherId { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public ICollection<Classroom>? Classrooms { get; set; } =  new List<Classroom>();
    public ICollection<Lesson>? Lessons { get; set; } =  new List<Lesson>();
}