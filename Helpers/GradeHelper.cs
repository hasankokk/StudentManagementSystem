using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;

namespace StudentManagementSystem.Helpers;

public class GradeHelper
{
    private readonly HomeworkRepository _homeworkRepository;
    private readonly GradeRepository _gradeRepository;
    private readonly TeacherRepository _teacherRepository;
    private readonly StudentRepository _studentRepository;

    public GradeHelper(AppDbContext context)
    {
        _homeworkRepository = new HomeworkRepository(context);
        _gradeRepository = new GradeRepository(context);
        _teacherRepository = new TeacherRepository(context);
        _studentRepository = new StudentRepository(context);
    }

    public void GiveGrades(long teacherTckn)
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
        ColoredHelper.ShowInfoMsg("Not verilebilecek ödev bulunamadı.");
        return;
    }

    var homeworkList = homeworks.ToList();
    int hwIdx = Helper.AskOption(homeworkList.Select(h => h.Task).ToArray(), "Ödev seç:");
    var selectedHomework = _homeworkRepository.GetById(homeworkList[hwIdx - 1].HomeworkId);

    if (selectedHomework?.Lesson?.Classrooms == null)
    {
        ColoredHelper.ShowErrorMsg("Derse atanmış sınıf bulunamadı.");
        return;
    }

    var lessonStudents = selectedHomework.Lesson.Classrooms
        .SelectMany(c => c.Students ?? new List<Student>())
        .DistinctBy(s => s.StudentId)
        .ToList();

    if (!lessonStudents.Any())
    {
        ColoredHelper.ShowInfoMsg("Bu ödevle ilişkili öğrenci bulunamadı.");
        return;
    }

    var studentOptions = lessonStudents
        .Select(s => $"{s.User?.Name} {s.User?.Surname}")
        .ToArray();

    int selectedIdx = Helper.AskOption(studentOptions, "Not verilecek öğrenciyi seç", "İptal");
    if (selectedIdx == 0)
        return;

    var selectedStudent = lessonStudents[selectedIdx - 1];
    var user = selectedStudent.User!;

    ColoredHelper.ShowInfoMsg($"Seçilen öğrenci: {user.Name} {user.Surname}");

    var input = Helper.Ask("Not gir (0-100)", true);
    var feedback = Helper.Ask("Öğrenciye bir geri dönüş mesajı gir");

    if (int.TryParse(input, out int gradeVal) && gradeVal >= 0 && gradeVal <= 100)
    {
        if(_gradeRepository.AddOrUpdateGrade(selectedHomework.HomeworkId, selectedStudent.StudentId, gradeVal, feedback))
            ColoredHelper.ShowSuccessMsg("Not kaydedildi.");
    }
    else
    {
        ColoredHelper.ShowErrorMsg("Geçersiz not.");
    }
}

    public void ListGradesByTeacher(long teacherTckn)
    {
        var teacher = _teacherRepository.GetByTckn(teacherTckn);
        if (teacher == null)
        {
            ColoredHelper.ShowErrorMsg("Öğretmen bulunamadı.");
            return;
        }

        var grades = _gradeRepository.GetGradesByTeacher(teacher.TeacherId);
        if (!grades.Any())
        {
            ColoredHelper.ShowInfoMsg("Henüz not verilmemiş.");
            return;
        }

        foreach (var grade in grades)
        {
            var studentName = $"{grade.Student.User?.Name} {grade.Student.User?.Surname}";
            var lessonName = grade.Homework.Lesson.LessonName;
            var hwTask = grade.Homework.Task;
            ColoredHelper.ShowListMsg($"{lessonName} - {hwTask} | {studentName} | Not: {grade.Score}");
        }
    }
    public void ListGradesByStudent(long studentTckn)
    {
        var student = _studentRepository.GetByTckn(studentTckn);
        if (student == null)
        {
            ColoredHelper.ShowErrorMsg("Öğrenci bulunamadı.");
            return;
        }

        var grades = _gradeRepository.GetGradesByStudentId(student.StudentId);
        if (!grades.Any())
        {
            ColoredHelper.ShowInfoMsg("Henüz notunuz yok.");
            return;
        }

        foreach (var g in grades)
        {
            ColoredHelper.ShowListMsg($"Ders: {g.Homework.Lesson.LessonName}");
            ColoredHelper.ShowListMsg($"Ödev: {g.Homework.Task}");
            ColoredHelper.ShowListMsg($"Öğretmen Notu: {g.Feedback}");
            ColoredHelper.ShowListMsg($"Not: {g.Score}");
            ColoredHelper.ShowListMsg("------------------------------");
        }
    }
    public void UpdateGrade(long teacherTckn)
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
            ColoredHelper.ShowInfoMsg("Size ait ödev bulunamadı.");
            return;
        }

        var hwList = homeworks.ToList();
        int hwIdx = Helper.AskOption(hwList.Select(h => h.Task).ToArray(), "Güncellenecek ödev:");
        var selectedHw = hwList[hwIdx - 1];

        var grades = _gradeRepository.GetGradesByHomework(selectedHw.HomeworkId);
        if (!grades.Any())
        {
            ColoredHelper.ShowInfoMsg("Bu ödeve verilmiş not yok.");
            return;
        }

        int gradeIdx = Helper.AskOption(grades.Select(g =>
                $"{g.Student.User?.Name} {g.Student.User?.Surname} → Not: {g.Score}").ToArray(),
            "Kimin notu güncellensin?");
    
        var selectedGrade = grades[gradeIdx - 1];

        string newVal = Helper.Ask("Yeni not (0-100)", true);
        string newFeedback = Helper.Ask("Yeni bir not geri bildirimi girin");
        if (int.TryParse(newVal, out int parsed) && parsed >= 0 && parsed <= 100)
        {
            if (_gradeRepository.AddOrUpdateGrade(selectedHw.HomeworkId, selectedGrade.StudentId, parsed, newFeedback))
                ColoredHelper.ShowSuccessMsg("Not güncellendi.");
        }
        else
        {
            ColoredHelper.ShowErrorMsg("Geçersiz not.");
        }
    }
    public void DeleteGrade(long teacherTckn)
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
            ColoredHelper.ShowInfoMsg("Size ait ödev yok.");
            return;
        }

        var hwList = homeworks.ToList();
        int hwIdx = Helper.AskOption(hwList.Select(h => h.Task).ToArray(), "Silinecek notun ödevi:");
        var selectedHw = hwList[hwIdx - 1];

        var grades = _gradeRepository.GetGradesByHomework(selectedHw.HomeworkId);
        if (!grades.Any())
        {
            ColoredHelper.ShowInfoMsg("Bu ödeve ait not bulunamadı.");
            return;
        }

        int gradeIdx = Helper.AskOption(grades.Select(g =>
                $"{g.Student.User?.Name} {g.Student.User?.Surname} → Not: {g.Score}").ToArray(),
            "Hangi not silinsin?");

        var selected = grades[gradeIdx - 1];

        _gradeRepository.DeleteGrade(selectedHw.HomeworkId, selected.StudentId);
        ColoredHelper.ShowSuccessMsg("Not silindi.");
    }
    public void ListSubmittedGradesByStudent(long studentTckn)
    {
        var student = _studentRepository.GetByTckn(studentTckn);
        if (student == null)
        {
            ColoredHelper.ShowErrorMsg("Öğrenci bulunamadı.");
            return;
        }

        var grades = _gradeRepository.GetSubmittedGradesByStudentId(student.StudentId);
        if (!grades.Any())
        {
            ColoredHelper.ShowInfoMsg("Teslim ettiğiniz ödev bulunmamaktadır.");
            return;
        }

        foreach (var g in grades.OrderBy(g => g.Homework.EndDate))
        {
            ColoredHelper.ShowListMsg($"Ders: {g.Homework.Lesson.LessonName}");
            ColoredHelper.ShowListMsg($"Ödev: {g.Homework.Task}");
            ColoredHelper.ShowListMsg($"Teslim Tarihi: {g.SubmittedAt?.ToString("dd.MM.yyyy") ?? "?"}");

            if (g.Score is not null)
                ColoredHelper.ShowListMsg($"Not: {g.Score}");
            else
                ColoredHelper.ShowListMsg("Not: Henüz değerlendirilmedi");
            ColoredHelper.ShowListMsg(new string('-', 30));
        }
    }

}
