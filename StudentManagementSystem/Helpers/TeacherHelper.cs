using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;

namespace StudentManagementSystem.Helpers;

public class TeacherHelper
{
    private readonly TeacherRepository _teacherRepository;
    private readonly StudentHelper _studentHelper;

    public TeacherHelper(AppDbContext context)
    {
        _teacherRepository = new TeacherRepository(context);
        _studentHelper = new StudentHelper(context);

    }

    public void RemoveTeacher()
    {
        var teachers = _teacherRepository.All();
        if (!teachers.Any())
        {
            ColoredHelper.ShowErrorMsg("Listelenecek öğretmen bulunamadı!");
            return;
        }
        foreach (var t in teachers)
            ColoredHelper.ShowListMsg($"{t.User?.Tckn} - {t.User?.Name} {t.User?.Surname}");

        long tckn = long.Parse(Helper.Ask("Silmek istediğiniz öğretmenin TCKN", true));
        var teacher = _teacherRepository.GetByTckn(tckn);
        if (teacher == null)
        {
            ColoredHelper.ShowErrorMsg("Öğretmen bulunamadı!");
            return;
        }
        _teacherRepository.RemoveTeacher(teacher);
        ColoredHelper.ShowSuccessMsg("Öğretmen silindi!");
    }
    public void UpdateTeacher()
    {
        var teachers = _teacherRepository.All();
        if (!teachers.Any())
        {
            ColoredHelper.ShowErrorMsg("Listelenecek öğretmen bulunamadı!");
            return;
        }

        foreach (var t in teachers)
            ColoredHelper.ShowListMsg($"{t.User?.Tckn} - {t.User?.Name} {t.User?.Surname}");

        long tckn = long.Parse(Helper.Ask("Güncellenecek öğretmenin TCKN", true)!);
        var teacher = _teacherRepository.GetByTckn(tckn);
        if (teacher == null)
        {
            ColoredHelper.ShowErrorMsg("Öğretmen bulunamadı!");
            return;
        }

        string newName = Helper.Ask("Yeni isim (boş = aynı)");
        string newSurname = Helper.Ask("Yeni soyisim (boş = aynı)");
        _teacherRepository.UpdateTeacher(teacher, newName, newSurname);

        _studentHelper.HandleClassroomUpdate(teacher.TeacherId, teacher.Classrooms?.ToList() ?? new(), false);
        ColoredHelper.ShowSuccessMsg("Öğretmen başarıyla güncellendi!");
    }


    public void ListTeachers()
    {
        var teachers = _teacherRepository.All();
        if (!teachers.Any())
        {
            ColoredHelper.ShowErrorMsg("Kayıtlı öğretmen bulunamadı!");
            return;
        }
        foreach (var t in teachers)
        {
            string classes = t.Classrooms.Any() ? string.Join(", ", t.Classrooms.Select(c => c.Name)) : "Sınıf yok";
            string lessons = t.Lessons.Any() ? string.Join(", ", t.Lessons.Select(l => l.LessonName)) : "Ders yok";
            ColoredHelper.ShowListMsg($"{t.User.Tckn} - {t.User.Name} {t.User.Surname} | Sınıflar: {classes} | Dersler: {lessons}");
        }
    }
    
    public void ShowTeacherDetails(long teacherTckn)
    {
        var teacher = _teacherRepository.GetByTckn(teacherTckn);
        if (teacher == null)
        {
            ColoredHelper.ShowErrorMsg("Öğretmen bulunamadı.");
            return;
        }

        var user = teacher.User!;
        ColoredHelper.ShowListMsg($"Ad Soyad: {user.Name} {user.Surname}");
        ColoredHelper.ShowListMsg($"TCKN: {user.Tckn}");
        ColoredHelper.ShowListMsg($"Rol: {user.Role}");

        var classroomNames = _teacherRepository
            .GetClassroomsByTeacherId(teacher.TeacherId)
            .Select(c => c.Name)
            .ToList();

        if (!classroomNames.Any())
            ColoredHelper.ShowListMsg("Sınıflar: Kayıtlı sınıf yok.");
        else
            ColoredHelper.ShowListMsg($"Sınıflar: {string.Join(", ", classroomNames)}");

        if (teacher.Lessons == null || !teacher.Lessons.Any())
        {
            ColoredHelper.ShowListMsg("Dersler: Kayıtlı ders yok.");
        }
        else
        {
            var lessonDetails = teacher.Lessons.Select(lesson =>
            {
                var classList = lesson.Classrooms?.Any() == true
                    ? string.Join(", ", lesson.Classrooms.Select(c => c.Name))
                    : "Sınıf yok";
                return $"- {lesson.LessonName} (Sınıflar: {classList})";
            });

            ColoredHelper.ShowListMsg("Dersler:");
            foreach (var line in lessonDetails)
                ColoredHelper.ShowListMsg(line);
        }
    }

    public Teacher? PickTeacherFrom(ICollection<Teacher> list)
    {
        if (list == null || !list.Any())
        {
            ColoredHelper.ShowInfoMsg("Bu derse atanmış öğretmen yok.");
            return null;
        }

        var teacherNames = list.Select(t => $"{t.User?.Name} {t.User?.Surname}").ToArray();
        int selectedIdx = Helper.AskOption(teacherNames, "Çıkarılacak öğretmeni seçin:", "İptal");
        if (selectedIdx == 0)
            return null;

        return list.ElementAt(selectedIdx - 1);
    }
}