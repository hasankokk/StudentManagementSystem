// Helpers/HomeworkHelper.cs

using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Helpers;
using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;

public class HomeworkHelper
{
    private readonly AppDbContext _context;
    private readonly HomeworkRepository _homeworkRepository;
    private readonly LessonRepository _lessonRepository;
    private readonly GradeRepository _gradeRepository;
    private readonly StudentRepository _studentRepository;
    private readonly TeacherRepository _teacherRepository;

    public HomeworkHelper(AppDbContext context)
    {
        _context = context;
        _homeworkRepository = new HomeworkRepository(context);
        _lessonRepository = new LessonRepository(context);
        _gradeRepository = new GradeRepository(context);
        _studentRepository = new StudentRepository(context);
        _teacherRepository = new TeacherRepository(context);
    }
    public void CreateHomework(long teacherTckn)
    {
        var teacher = _teacherRepository.GetByTckn(teacherTckn);
        if (teacher == null || !teacher.Lessons.Any())
        {
            ColoredHelper.ShowErrorMsg("Ders bulunamadı.");
            return;
        }

        var lessons = teacher.Lessons.ToList();
        var lessonOptions = lessons.Select(l => l.LessonName).ToArray();
        var selectedLessonId = Helper.AskOption(lessonOptions, "Hangi ders için ödev verilecek?");
        var selectedLesson = lessons[selectedLessonId - 1];

        var task = Helper.Ask("Ödev başlığı", true);
        var desc = Helper.Ask("Açıklama (boş olabilir)");
        var due= Helper.AskDate("Son teslim tarihi (gg.aa.yyyy)");


        var homework = new Homework
        {
            Task = task,
            Description = desc,
            EndDate = due,
            LessonId = selectedLesson.LessonId
        };

        _homeworkRepository.AddHomework(homework);
        ColoredHelper.ShowSuccessMsg("Ödev başarıyla eklendi.");
    }
    public void ListHomeworksByTeacher(long teacherTckn)
    {
        var teacher = _teacherRepository.GetByTckn(teacherTckn);
        if (teacher == null)
        {
            ColoredHelper.ShowErrorMsg("Öğretmen bulunamadı.");
            return;
        }

        var homeworks = _homeworkRepository.GetByTeacher(teacher.TeacherId);
        if (!homeworks.Any())
        {
            ColoredHelper.ShowInfoMsg("Kayıtlı ödev bulunamadı.");
            return;
        }

        foreach (var hw in homeworks)
        {
            ColoredHelper.ShowListMsg($"Ders: {hw.Lesson.LessonName}");
            ColoredHelper.ShowListMsg($"Başlık: {hw.Task}");
            ColoredHelper.ShowListMsg($"Açıklama: {hw.Description}");
            ColoredHelper.ShowListMsg($"Teslim Tarihi: {hw.EndDate:dd.MM.yyyy}");
            ColoredHelper.ShowListMsg("-----------------------------------------------");
        }
    }
    public void UpdateHomework(long teacherTckn)
    {
        var teacher = _teacherRepository.GetByTckn(teacherTckn);
        if (teacher == null)
        {
            ColoredHelper.ShowErrorMsg("Öğretmen bulunamadı.");
            return;
        }

        var homeworks = _homeworkRepository.GetByTeacher(teacher.TeacherId);
        if (!homeworks.Any())
        {
            ColoredHelper.ShowInfoMsg("Güncellenecek ödev bulunamadı.");
            return;
        }

        var hwList = homeworks.ToList();
        int selectHomework = Helper.AskOption(hwList.Select(h => h.Task).ToArray(), "Güncellenecek ödevi seç:");
        var selected = hwList[selectHomework - 1];

        string newTitle = Helper.Ask("Yeni başlık (boş = aynı)");
        string newDesc = Helper.Ask("Yeni açıklama (boş = aynı)");
        var newDateInput = Helper.AskDate("Yeni teslim tarihi (gg.aa.yyyy, boş = aynı)");

        if (!string.IsNullOrWhiteSpace(newTitle)) 
            selected.Task = newTitle;
        if (!string.IsNullOrWhiteSpace(newDesc)) 
            selected.Description = newDesc;
        if (newDateInput != null) 
            selected.EndDate = newDateInput;

        _homeworkRepository.Update(selected);
        ColoredHelper.ShowSuccessMsg("Ödev güncellendi.");
    }
    public void DeleteHomework(long teacherTckn)
    {
        var teacher = _teacherRepository.GetByTckn(teacherTckn);
        if (teacher == null)
        {
            ColoredHelper.ShowErrorMsg("Öğretmen bulunamadı.");
            return;
        }

        var homeworks = _homeworkRepository.GetByTeacher(teacher.TeacherId);
        if (!homeworks.Any())
        {
            ColoredHelper.ShowInfoMsg("Silinecek ödev yok.");
            return;
        }

        var hwList = homeworks.ToList();
        int selectHomework = Helper.AskOption(hwList.Select(h => h.Task).ToArray(), "Silinecek ödevi seç:");
        var selected = hwList[selectHomework - 1];

        _homeworkRepository.Delete(selected.HomeworkId);
        ColoredHelper.ShowSuccessMsg("Ödev silindi.");
    }
    
    public void SubmitHomework(long studentTckn)
    {
        var student = _studentRepository.GetByTckn(studentTckn);
        if (student == null)
        {
            ColoredHelper.ShowErrorMsg("Öğrenci bulunamadı.");
            return;
        }

        var homeworks = _homeworkRepository.GetHomeworksByStudentId(student.StudentId);
        if (!homeworks.Any())
        {
            ColoredHelper.ShowInfoMsg("Size atanmış ödev bulunamadı.");
            return;
        }

        var pending = homeworks
            .Where(h => !_gradeRepository
                .GetGradesByHomework(h.HomeworkId)
                .Any(g => g.StudentId == student.StudentId && g.IsSubmitted))
            .OrderBy(h => h.EndDate)
            .ToList();

        if (!pending.Any())
        {
            ColoredHelper.ShowInfoMsg("Teslim edilecek ödeviniz bulunmamaktadır.");
            return;
        }

        var selectHw = Helper.AskOption(pending.Select(h => $"{h.Lesson.LessonName} - {h.Task}").ToArray(), "Teslim edilecek ödevi seç:");
        var selectedHw = pending[selectHw - 1];

        // Cevap girişi yok, sadece teslim edildi olarak işaretlenecek
        _gradeRepository.SubmitHomework(selectedHw.HomeworkId, student.StudentId);
        ColoredHelper.ShowSuccessMsg("Ödev başarıyla teslim edildi.");
    }
    public void ListHomeworksByStudent(long studentTckn)
    {
        var student = _studentRepository.GetByTckn(studentTckn);
        if (student == null)
        {
            ColoredHelper.ShowErrorMsg("Öğrenci bulunamadı.");
            return;
        }

        var homeworks = _homeworkRepository.GetHomeworksByStudentId(student.StudentId);
        if (!homeworks.Any())
        {
            ColoredHelper.ShowInfoMsg("Size atanmış ödev bulunamadı.");
            return;
        }

        foreach (var hw in homeworks.OrderBy(h => h.EndDate))
        {
            ColoredHelper.ShowListMsg($"Ders: {hw.Lesson.LessonName}");
            ColoredHelper.ShowListMsg($"Başlık: {hw.Task}");
            ColoredHelper.ShowListMsg($"Açıklama: {hw.Description}");
            ColoredHelper.ShowListMsg($"Teslim Tarihi: {hw.EndDate:dd.MM.yyyy}");

            var grade = _gradeRepository
                .GetGradesByHomework(hw.HomeworkId)
                .FirstOrDefault(g => g.StudentId == student.StudentId);

            if (grade is not null && grade.IsSubmitted)
                ColoredHelper.ShowListMsg($"Teslim Durumu: Teslim edildi ({grade.SubmittedAt?.ToString("dd.MM.yyyy")})");
            else
                ColoredHelper.ShowListMsg("Teslim Durumu: Henüz teslim edilmedi");

            ColoredHelper.ShowListMsg(new string('-', 30));
        }
    }

}

