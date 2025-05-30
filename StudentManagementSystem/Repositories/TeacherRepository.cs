using StudentManagementSystem.Data; 
using StudentManagementSystem.Models; 
using Microsoft.EntityFrameworkCore;
namespace StudentManagementSystem.Repositories
{
    public class TeacherRepository
    {
        private readonly AppDbContext _context; 
        public TeacherRepository(AppDbContext context)
        {
            _context = context;
        }

        public Teacher AddTeacher(int userId)
        {
            var teacher = new Teacher
            {
                UserId = userId
            };
            _context.Teachers.Add(teacher);
            _context.SaveChanges();
            return teacher;
        }
        
        public Teacher? GetByTckn(long tckn)
        {
            return _context.Teachers
                .Include(x => x.User)
                .Include(x => x.Lessons)!
                    .ThenInclude(l => l.Classrooms)
                .Include(x => x.Classrooms)
                .FirstOrDefault(x => x.User.Tckn == tckn)!;
        }
        public List<Classroom> GetClassroomsByTeacherId(int teacherId)
        {
            var teacher = _context.Teachers.Include(s => s.Classrooms).FirstOrDefault(s => s.TeacherId == teacherId);
            return teacher.Classrooms.ToList();
        }
        public List<Teacher> All()
        {
            return _context.Teachers
                .Include(t => t.Classrooms)
                .Include(t => t.Lessons)
                .Include(t => t.User)
                .ToList();
        }

        public void RemoveTeacher(Teacher teacher)
        {
            _context.Teachers.Remove(teacher);
            _context.SaveChanges();
        }

        public void UpdateTeacher(Teacher teacher, string? name, string? surname)
        {
            if (name != null && surname != null)
            {
                teacher.User.Name = name;
                teacher.User.Surname = surname;
            }
            _context.SaveChanges();
        }

        public bool AddClass(int teacherId, int classRoomId)
        {
            var teacher = _context.Teachers.Include(x => x.Classrooms).FirstOrDefault(x => x.TeacherId == teacherId);
            var teacherClass=_context.Classrooms.Find(classRoomId); 
            if(teacher == null || teacherClass == null|| teacher.Classrooms.Any(c => c.ClassroomId == classRoomId))
                return false; 
            teacher.Classrooms.Add(teacherClass);
            _context.SaveChanges();
            return true;
        }
        public void RemoveClass(int teacherId,int classRoomId)
        {
            var teacher = _context.Teachers.Include(x => x.Classrooms).FirstOrDefault(x => x.TeacherId == teacherId);
            var teacherClass = teacher?.Classrooms.FirstOrDefault(c => c.ClassroomId == classRoomId); 
            if(teacherClass == null) 
                return; 
            teacher.Classrooms.Remove(teacherClass);
            _context.SaveChanges();
        }
        
        

        public void RemoveLessonFromTeacher(int teacherId, int lessonId)
        {
            var teacher = _context.Teachers.Include(t => t.Lessons).FirstOrDefault(t => t.TeacherId == teacherId);
            var lesson = _context.Lessons.Find(lessonId);

            if (teacher != null && lesson != null && teacher.Lessons.Contains(lesson))
            {
                teacher.Lessons.Remove(lesson);
                _context.SaveChanges();
            }
        }
    }
}