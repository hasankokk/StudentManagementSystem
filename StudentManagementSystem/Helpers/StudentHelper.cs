using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;

namespace StudentManagementSystem.Helpers;

public class StudentHelper
{
    private readonly StudentRepository _studentRepository;
    private readonly TeacherRepository _teacherRepository;
    private readonly ClassroomHelper _classroomHelper;

    public StudentHelper(AppDbContext context)
    {
        _studentRepository = new StudentRepository(context);
        _classroomHelper = new ClassroomHelper(context);
        _teacherRepository = new TeacherRepository(context);
    }

    public void RemoveStudent()
    {
        var allStudents = _studentRepository.GetAll();
        if (!allStudents.Any())
        {
            ColoredHelper.ShowErrorMsg("Listelenecek öğrenci bulunamadı!");
            return;
        }

        foreach (var student in allStudents)
        {
            ColoredHelper.ShowListMsg($"{student.User.Tckn} - {student.User.Name} {student.User.Surname}");
        }

        var tckn = long.Parse(Helper.Ask("Silmek istediğiniz öğrencinin TCKN giriniz", true));
        var studentToRemove = _studentRepository.GetByTckn(tckn);

        if (studentToRemove != null)
        {
            _studentRepository.RemoveStudent(studentToRemove);
            ColoredHelper.ShowSuccessMsg("Öğrenci başarıyla silindi!");
        }
        else
        {
            ColoredHelper.ShowErrorMsg("Öğrenci bulunamadı!");
        }
    }

    public void UpdateStudent()
    {
        var students = _studentRepository.GetAll();
        if (!students.Any())
        {
            ColoredHelper.ShowErrorMsg("Listelenecek öğrenci bulunamadı!");
            return;
        }

        foreach (var s in students)
            ColoredHelper.ShowListMsg($"{s.User.Tckn} - {s.User.Name} {s.User.Surname}");

        var tckn = long.Parse(Helper.Ask("Güncellemek istediğiniz öğrencinin TCKN giriniz", true));
        var student = _studentRepository.GetByTckn(tckn);
        if (student == null)
        {
            ColoredHelper.ShowErrorMsg("Öğrenci bulunamadı!");
            return;
        }

        var inputName = Helper.Ask("Yeni bir isim giriniz");
        var inputSurname = Helper.Ask("Yeni bir soyisim giriniz");
        _studentRepository.UpdateStudent(student, inputName, inputSurname);

        HandleClassroomUpdate(student.StudentId, student.Classrooms?.ToList() ?? new(), true);
        ColoredHelper.ShowSuccessMsg("Öğrenci başarıyla güncellendi!");
    }

    public void HandleClassroomUpdate(int entityId, List<Classroom> existingClassrooms, bool isStudent)
    {
        var updateClassroom = Helper.AskOption(["Ekle", "Sil"], "Sınıf güncellemesi yapmak istiyor musun?", "Vazgeç");
        var excludeIds = existingClassrooms.Select(c => c.ClassroomId).ToList();

        if (updateClassroom == 1)
        {
            var newClassrooms = _classroomHelper.UpdateClassRoom("Ekle", excludeIds);
            foreach (var c in newClassrooms)
            {
                if (isStudent)
                    _studentRepository.AddClassroom(entityId, c.ClassroomId);
                else 
                    _teacherRepository.AddClass(entityId, c.ClassroomId);
            }
        }
        else if (updateClassroom == 2)
        {
            var toRemove = _classroomHelper.UpdateClassRoomsForRemove(existingClassrooms);
            foreach (var c in toRemove)
            {
                if (isStudent) 
                    _studentRepository.RemoveClassroom(entityId, c.ClassroomId);
                else 
                    _teacherRepository.RemoveClass(entityId, c.ClassroomId);
            }
        }
    }


    public void ListStudent()
    {
        var students = _studentRepository.GetAll();

        if (!students.Any())
        {
            ColoredHelper.ShowErrorMsg("Kayıtlı öğrenci bulunamadı!");
            return;
        }

        foreach (var student in students)
        {
            var classrooms = student.Classrooms != null && student.Classrooms.Any()
                ? string.Join(", ", student.Classrooms.Select(c => c.Name))
                : "Sınıf yok";

            ColoredHelper.ShowListMsg($"{student.User.Tckn} - {student.User.Name} {student.User.Surname} | Sınıflar: {classrooms}");
        }
    }
    public void ShowStudentDetails(long studentTckn)
    {
        var student = _studentRepository.GetByTckn(studentTckn);
        var user = student.User;

        if (student == null)
        {
            ColoredHelper.ShowErrorMsg("Öğrenci bulunamadı.");
            return;
        }
        
        ColoredHelper.ShowListMsg($"Ad Soyad: {user.Name} {user.Surname}");
        ColoredHelper.ShowListMsg($"TCKN: {user.Tckn}");
        ColoredHelper.ShowListMsg($"Rol: {user.Role}");
        var classroomNames = _studentRepository.GetClassroomsByStudentId(student.StudentId)
            .Select(c => c.Name);
        if (classroomNames == null || classroomNames.Count() == 0)
        {
            ColoredHelper.ShowListMsg("Sınıflar: Kayıtlı sınıf yok.");
        }
        else
        {
            ColoredHelper.ShowListMsg($"Sınıflar: {string.Join(", ", classroomNames)}");
        }
    }
}
