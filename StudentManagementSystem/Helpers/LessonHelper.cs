using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;

namespace StudentManagementSystem.Helpers;

public class LessonHelper
{
    private readonly AppDbContext _context;
    private readonly LessonRepository _lessonRepository;
    private readonly ClassroomRepository _classroomRepository;
    private readonly TeacherRepository _teacherRepository;
    private readonly ClassroomHelper _classroomHelper;
    private readonly TeacherHelper _teacherHelper;

    public LessonHelper(AppDbContext context)
    {
        _context = context;
        _lessonRepository = new LessonRepository(_context);
        _classroomRepository = new ClassroomRepository(_context);
        _teacherRepository = new TeacherRepository(_context);
        _classroomHelper = new ClassroomHelper(_context);
        _teacherHelper = new TeacherHelper(_context);
    }

    public void AddOrAssignLesson()
    {
        var name = Helper.Ask("Ders Adı", true);
        var lesson = _lessonRepository.GetByName(name) ?? _lessonRepository.AddLesson(new Lesson { LessonName = name });

        var classroom = PickClassroom("Hangi sınıfa atansın?", lesson.LessonId);
        if (classroom == null) return;

        if (_lessonRepository.AddClassroom(lesson.LessonId, classroom.ClassroomId))
            ColoredHelper.ShowSuccessMsg("Ders sınıfa atandı.");
        else
            ColoredHelper.ShowErrorMsg("Bu ders zaten o sınıfta tanımlı.");

        AssignTeacher(lesson.LessonId);
    }
    public void EditLesson()
    {
        var lesson = PickLesson();
        if (lesson == null) 
            return;

        new ConsoleMenu($"Ders Düzenle: {lesson.LessonName}")
            .AddOption("İsim Değiştir",      () => {
                var newName = Helper.Ask("Yeni ad", true);
                _lessonRepository.UpdateName(lesson.LessonId, newName);
                ColoredHelper.ShowSuccessMsg("İsim güncellendi.");
            })
            .AddOption("Sınıf Ekle", () => {
                var classroom = PickClassroom("Hangi sınıf eklensin?", lesson.LessonId);
                if (classroom != null && _lessonRepository.AddClassroom(lesson.LessonId, classroom.ClassroomId))
                    ColoredHelper.ShowSuccessMsg("Sınıf eklendi.");
                else
                    ColoredHelper.ShowErrorMsg("Zaten o sınıfta.");
            })
            .AddOption("Sınıf Çıkar", () => {
                var classroom = _classroomHelper.PickClassroomFrom(lesson.Classrooms);
                if (classroom != null && _lessonRepository.RemoveClassroom(lesson.LessonId, classroom.ClassroomId))
                    ColoredHelper.ShowSuccessMsg("Sınıf çıkarıldı.");
            })
            .AddOption("Öğretmen Ekle",     () => AssignTeacher(lesson.LessonId))
            .AddOption("Öğretmen Çıkar", () => {
                var teacher = _teacherHelper.PickTeacherFrom(lesson.Teachers);
                if (teacher != null && _lessonRepository.RemoveTeacher(lesson.LessonId, teacher.TeacherId))
                    ColoredHelper.ShowSuccessMsg("Öğretmen çıkarıldı.");
            })
            .Show();
    }
    public void DeleteLesson()
    {
        var lesson = PickLesson();
        if (lesson == null) return;
        _lessonRepository.DeleteLesson(lesson.LessonId);
        ColoredHelper.ShowSuccessMsg("Ders silindi.");
    }
    public Lesson? PickLesson()
    {
        var lessons = _lessonRepository.GetAll();
        if (!lessons.Any())
        {
            ColoredHelper.ShowErrorMsg("Hiç ders yok."); 
            return null;
        }

        var selectLesson = Helper.AskOption(lessons
            .Select(l => $"{l.LessonName} (Sınıf: {string.Join("/", l.Classrooms.Select(c=>c.Name))})")
            .ToArray(), "Ders seç:");
        return lessons[selectLesson-1];
    }
    public Classroom? PickClassroom(string message, int lessonId)
    {
        var lesson = _lessonRepository.GetById(lessonId);
        var existingClassroomIds = lesson.Classrooms.Select(c => c.ClassroomId).ToList();
        var allClassrooms = _classroomRepository.GetClassroomList();
        var available = allClassrooms
            .Where(c => !existingClassroomIds.Contains(c.ClassroomId))
            .ToList();

        if (!available.Any())
        {
            ColoredHelper.ShowInfoMsg("Atanabilecek sınıf kalmadı.");
            return null;
        }

        int idx = Helper.AskOption(available.Select(c => c.Name).ToArray(), message);
        return available[idx - 1];
    }


    private void AssignTeacher(int lessonId)
    {
        var lesson = _lessonRepository.GetById(lessonId);
        var allTeachers = _teacherRepository.All();
        var assignedIds = lesson.Teachers.Select(t => t.TeacherId).ToList();

        var availableTeachers = allTeachers
            .Where(t => !assignedIds.Contains(t.TeacherId))
            .ToList();

        if (!availableTeachers.Any())
        {
            ColoredHelper.ShowInfoMsg("Atanabilecek öğretmen kalmadı.");
            return;
        }

        var idx = Helper.AskOption(
            availableTeachers.Select(t => $"{t.User?.Name} {t.User?.Surname}").ToArray(),
            "Hangi öğretmen eklensin?");

        var selected = availableTeachers[idx - 1];

        if (_lessonRepository.AddLessonToTeacher(selected.TeacherId, lessonId))
            ColoredHelper.ShowSuccessMsg("Öğretmen eklendi.");
        else
            ColoredHelper.ShowErrorMsg("Zaten atanmış.");
    }
}