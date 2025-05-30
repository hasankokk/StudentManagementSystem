using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Repositories;

public class LessonRepository
{
    private readonly AppDbContext _context;

    public LessonRepository(AppDbContext context)
    {
        _context = context;
    }

    public Lesson? GetByName(string name)
    {
        return _context.Lessons.FirstOrDefault(l => l.LessonName == name);
    }

    public Lesson GetByIdWithClassroom(int lessonId)
    {
        return _context.Lessons
            .Include(l => l.Classrooms)
            .Include(l => l.Teachers)
            .FirstOrDefault(l => l.LessonId == lessonId)!;
    }

    public List<Lesson> GetAll()
    {
        return _context.Lessons
            .Include(l => l.Classrooms)
            .Include(l => l.Teachers)
            .ToList();
    }

    public Lesson? GetById(int id)
    {
        return _context.Lessons
            .Include(l => l.Classrooms)
            .Include(l => l.Teachers)
            .FirstOrDefault(l => l.LessonId == id);
    }
    
    public Lesson AddLesson(Lesson lesson)
    {
        _context.Lessons.Add(lesson);
        _context.SaveChanges();
        return lesson;
    }
    public void UpdateName(int lessonId, string newName)
    {
        var lesson = _context.Lessons.Find(lessonId);
        if (lesson == null) return;
        lesson.LessonName = newName;
        _context.SaveChanges();
    }

    public void DeleteLesson(int id)
    {
        var lesson = _context.Lessons.Find(id);
        if (lesson != null)
        {
            _context.Lessons.Remove(lesson);
            _context.SaveChanges();
        }
    }
    public bool AddClassroom(int lessonId, int classroomId)
    {
        var lesson    = _context.Lessons
            .Include(l => l.Classrooms)
            .FirstOrDefault(l => l.LessonId == lessonId);
        var classroom = _context.Classrooms.Find(classroomId);

        if (lesson != null && classroom != null &&
            !lesson.Classrooms.Any(c => c.ClassroomId == classroomId))
        {
            lesson.Classrooms.Add(classroom);
            _context.SaveChanges();
            return true;
        }
        return false;
    }
    public bool RemoveClassroom(int lessonId, int classroomId)
    {
        var lesson = _context.Lessons.Include(l => l.Classrooms)
            .FirstOrDefault(l => l.LessonId == lessonId);
        var classroom = lesson?.Classrooms.FirstOrDefault(c => c.ClassroomId == classroomId);
        if (classroom == null) return false;

        lesson.Classrooms.Remove(classroom);
        _context.SaveChanges();
        return true;
    }
    public bool AddLessonToTeacher(int teacherId, int lessonId)
    {
        var teacher = _context.Teachers.Include(t => t.Lessons).FirstOrDefault(t => t.TeacherId == teacherId);
        var lesson = _context.Lessons.Find(lessonId);

        if (teacher != null && lesson != null && !teacher.Lessons.Contains(lesson))
        {
            teacher.Lessons.Add(lesson);
            _context.SaveChanges();
            return true;
        }
        return false;
    }
    public bool RemoveTeacher(int lessonId, int teacherId)
    {
        var lesson = _context.Lessons.Include(l => l.Teachers).FirstOrDefault(l => l.LessonId == lessonId);
        var teacher = lesson?.Teachers.FirstOrDefault(t => t.TeacherId == teacherId);
        if (teacher == null) return false;

        lesson.Teachers.Remove(teacher);
        _context.SaveChanges();
        return true;
    }
}