using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Helpers;
using StudentManagementSystem.Models;

public class GradeRepository
{
    private readonly AppDbContext _context;

    public GradeRepository(AppDbContext context)
    {
        _context = context;
    }

    public bool AddOrUpdateGrade(int homeworkId, int studentId, int grade, string? feedback)
    {
        var existing = _context.Grades.FirstOrDefault(g => g.HomeworkId == homeworkId && g.StudentId == studentId);

        // Eğer kayıt varsa ama teslim edilmemişse, not verilmesin
        if (existing != null)
        {
            if (!existing.IsSubmitted)
            {
                ColoredHelper.ShowErrorMsg("Bu ödev henüz teslim edilmemiş. Not verilemez.");
                return false;
            }

            existing.Score = grade;
            existing.Feedback = feedback;
        }
        else
        {
            ColoredHelper.ShowErrorMsg("Ödev teslim edilmemiş. Önce öğrenci teslim etmelidir.");
            return false;
        }

        _context.SaveChanges();
        return true;
    }

    
    public void DeleteGrade(int homeworkId, int studentId)
    {
        var grade = _context.Grades.FirstOrDefault(g => g.HomeworkId == homeworkId && g.StudentId == studentId);
        if (grade != null)
        {
            _context.Grades.Remove(grade);
            _context.SaveChanges();
        }
    }
    public void SubmitHomework(int homeworkId, int studentId)
    {
        var grade = _context.Grades.FirstOrDefault(g => g.HomeworkId == homeworkId && g.StudentId == studentId);

        if (grade != null)
        {
            grade.IsSubmitted = true;
            grade.SubmittedAt = DateTime.Now;
        }
        else
        {
            grade = new Grade
            {
                HomeworkId = homeworkId,
                StudentId = studentId,
                IsSubmitted = true,
                SubmittedAt = DateTime.Now
            };
            _context.Grades.Add(grade);
        }

        _context.SaveChanges();
    }

    public List<Grade> GetGradesByTeacher(int teacherId)
    {
        return _context.Grades
            .Include(g => g.Homework)
            .ThenInclude(h => h.Lesson)
            .ThenInclude(l => l.Teachers)
            .Include(g => g.Student)
            .ThenInclude(s => s.User)
            .Where(g => g.Homework.Lesson.Teachers.Any(t => t.TeacherId == teacherId))
            .ToList();
    }
    public List<Grade> GetGradesByStudentId(int studentId)
    {
        return _context.Grades
            .Include(g => g.Homework)
            .ThenInclude(h => h.Lesson)
            .Where(g => g.StudentId == studentId)
            .ToList();
    }
    public List<Grade> GetGradesByHomework(int homeworkId)
    {
        return _context.Grades
            .Include(g => g.Student)
            .ThenInclude(s => s.User)
            .Where(g => g.HomeworkId == homeworkId)
            .ToList();
    }
    public List<Grade> GetSubmittedGradesByStudentId(int studentId)
    {
        return _context.Grades
            .Include(g => g.Homework)
            .ThenInclude(h => h.Lesson)
            .Where(g => g.StudentId == studentId && g.IsSubmitted)
            .ToList();
    }
}