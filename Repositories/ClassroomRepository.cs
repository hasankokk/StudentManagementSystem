using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Helpers;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Repositories;

public class ClassroomRepository
{
    private readonly AppDbContext _context;

    public ClassroomRepository(AppDbContext context)
    {
        _context = context;
    }
    public List<Classroom> GetClassroomList()
    {
        return _context.Classrooms.ToList();
    }

    public Classroom? GetClassroomById(int id)
    {
        var findClassroom = _context.Classrooms.FirstOrDefault(c => c.ClassroomId == id);
        if (findClassroom != null)
            return findClassroom;
        return null;
    }

    public bool AddClassroom(Classroom classroom)
    {
        if (_context.Classrooms.Any(c => c.Name == classroom.Name))
            return false;
        _context.Classrooms.Add(classroom);
        _context.SaveChanges();
        return true;
    }
    public bool UpdateClassroom(Classroom classroom)
    {
        var existing = _context.Classrooms.FirstOrDefault(c => c.ClassroomId == classroom.ClassroomId);
        if (existing == null)
            return false;

        existing.Name = classroom.Name;
        _context.SaveChanges();
        return true;
    }
    public bool DeleteClassroom(int classroomId)
    {
        var classroom = _context.Classrooms
            .Include(c => c.Students)
            .Include(c => c.Teachers)
            .FirstOrDefault(c => c.ClassroomId == classroomId);

        if (classroom == null)
            return false;

        if ((classroom.Students != null && classroom.Students.Any()) ||
            (classroom.Teachers != null && classroom.Teachers.Any()))
        {
            if (classroom.Students != null)
            {
                foreach (var student in classroom.Students.ToList())
                {
                    student.Classrooms.Remove(classroom);
                }
            }
            if (classroom.Teachers != null)
            {
                foreach (var teacher in classroom.Teachers.ToList())
                {
                    teacher.Classrooms.Remove(classroom);
                }
            }
            _context.SaveChanges();
        }

        _context.Classrooms.Remove(classroom);
        _context.SaveChanges();
        return true;
    }

}