using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Repositories;

public class HomeworkRepository
{
    private readonly AppDbContext _context;

    public HomeworkRepository(AppDbContext context)
    {
        _context = context;
    }
    public Homework? GetById(int homeworkId)
    {
        return _context.Homeworks
            .Include(h => h.Lesson)
            .ThenInclude(l => l.Classrooms)
            .ThenInclude(c => c.Students)
            .ThenInclude(s => s.User)
            .FirstOrDefault(h => h.HomeworkId == homeworkId);
    }
    public List<Homework> GetHomeworksByStudentId(int studentId)
    {
        var student = _context.Students
            .Include(s => s.Classrooms)
            .FirstOrDefault(s => s.StudentId == studentId);

        if (student == null)
            return new();

        var classroomIds = student.Classrooms.Select(c => c.ClassroomId).ToList();

        return _context.Homeworks
            .Include(h => h.Lesson)
            .ThenInclude(l => l.Classrooms)
            .Where(h => h.Lesson.Classrooms.Any(c => classroomIds.Contains(c.ClassroomId)))
            .ToList();
    }
    public List<Homework> GetByTeacher(int teacherId)
    {
        return _context.Homeworks
            .Include(h => h.Lesson)
            .ThenInclude(t => t.Teachers)
            .Where(h => h.Lesson.Teachers.Any(t => t.TeacherId == teacherId))
            .ToList();
    }
    public bool AddHomework(Homework homework)
    {
        _context.Homeworks.Add(homework);
        _context.SaveChanges();
        return true;
    }

    public void Update(Homework homework)
    {
        var existing = _context.Homeworks.Find(homework.HomeworkId);
        if (existing == null) 
            return;

        existing.Task = homework.Task;
        existing.Description = homework.Description;
        existing.EndDate = homework.EndDate;

        _context.SaveChanges();
    }


    public bool Delete(int id)
    {
        var homework = _context.Homeworks.Find(id);
        if (homework == null) 
            return false;

        _context.Homeworks.Remove(homework);
        _context.SaveChanges();
        return true;
    }
}