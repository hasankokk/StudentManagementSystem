namespace StudentManagementSystem.Models;

public class Lesson
{
    public int LessonId { get; set; }
    public string LessonName { get; set; }
    
    public ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();
    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    public ICollection<Homework> Homeworks { get; set; } = new List<Homework>();
}