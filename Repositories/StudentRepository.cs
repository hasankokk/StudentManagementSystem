using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Helpers;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Repositories;

public class StudentRepository
{
    private readonly AppDbContext _context;
    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public Student AddStudent(int userId)
    {
        var student = new Student { UserId = userId };
        _context.Students.Add(student);
        _context.SaveChanges();
        return student;
    }

    public Student? GetByTckn(long tckn)
    {
        return _context.Students.Include(s => s.User).FirstOrDefault(s => s.User.Tckn == tckn);
    }

    public List<Student> GetAll()
    {
        return _context.Students.Include(s => s.User).Include(s => s.Classrooms).ToList();
    }

    public List<Classroom> GetClassroomsByStudentId(int studentId)
    {
        var student = _context.Students.Include(s => s.Classrooms).FirstOrDefault(s => s.StudentId == studentId);
        return student.Classrooms.ToList();
    }

    public void RemoveStudent(Student student)
    {
        _context.Students.Remove(student);
        _context.SaveChanges();
    }

    public void UpdateStudent(Student student, string? name, string? surname)
    {
        if (!string.IsNullOrWhiteSpace(name)) student.User.Name = name;
        if (!string.IsNullOrWhiteSpace(surname)) student.User.Surname = surname;
        _context.SaveChanges();
    }

    public bool AddClassroom(int studentId, int classroomId)
    {
        var student = _context.Students.Include(s => s.Classrooms).FirstOrDefault(s => s.StudentId == studentId);
        var classroom = _context.Classrooms.Find(classroomId);
        if (student == null || classroom == null || student.Classrooms.Any(c => c.ClassroomId == classroomId))
            return false;

        student.Classrooms.Add(classroom);
        _context.SaveChanges();
        return true;
    }

    public bool RemoveClassroom(int studentId, int classroomId)
    {
        var student = _context.Students.Include(s => s.Classrooms).FirstOrDefault(s => s.StudentId == studentId);
        var classroom = student?.Classrooms.FirstOrDefault(c => c.ClassroomId == classroomId);
        if (classroom == null) 
            return false;

        student.Classrooms.Remove(classroom);
        _context.SaveChanges();
        return true;
    }
}
