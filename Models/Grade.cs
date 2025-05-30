using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models;

public class Grade
{
    public int GradeId { get; set; }
    
    [Range(0, 100)]
    public int? Score { get; set; }
    public string? Feedback { get; set; }
    
    public bool IsSubmitted { get; set; } = false;
    public DateTime? SubmittedAt { get; set; } 
    public int HomeworkId { get; set; }
    public Homework Homework { get; set; }
    
    public int StudentId { get; set; }
    public Student Student { get; set; }
}