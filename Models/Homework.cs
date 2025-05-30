using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models;

public class Homework
{
    public int HomeworkId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Task { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; }
    
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}