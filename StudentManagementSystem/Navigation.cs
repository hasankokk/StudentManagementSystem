using StudentManagementSystem.Data;
using StudentManagementSystem.Helpers;
using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories; // Rol enum vs için

namespace StudentManagementSystem;

public class Navigation
{
    private readonly AppDbContext _context;
    private readonly ClassroomHelper _classroomHelper;
    private readonly StudentHelper _studentHelper;
    private readonly TeacherHelper _teacherHelper;
    private readonly UserRepository _userRepository;
    private readonly HomeworkHelper _homeworkHelper;
    private readonly LessonHelper _lessonHelper;
    private readonly GradeHelper _gradeHelper;
    private readonly Auth _auth;

    private User? _loggedInUser;

    public Navigation(AppDbContext context)
    {
        _context = context;
        _classroomHelper = new ClassroomHelper(_context);
        _studentHelper = new StudentHelper(_context);
        _teacherHelper = new TeacherHelper(_context);
        _userRepository = new UserRepository(_context);
        _homeworkHelper = new HomeworkHelper(_context);
        _lessonHelper = new LessonHelper(_context);
        _gradeHelper = new GradeHelper(_context);
        _auth = new Auth(_context);
    }

    public void StartApp()
    {
        var user = new User
        {
            Name = "admin",
            Surname = "admin",
            Password = PasswordHelper.HashPassword("admin"),
            Tckn = 12345678912,
            Role = Role.Admin,
        };
        if (_userRepository.GetByTckn(user.Tckn) == null)
            _userRepository.AddUser(user);
            
        Console.Clear();
        ColoredHelper.ShowInfoMsg("ÖĞRENCİ YÖNETİM SİSTEMİ");

        if (!Login())
        {
            ColoredHelper.ShowErrorMsg("Giriş başarısız. Program kapatılıyor.");
            return;
        }

        // İlk girişte şifre değiştirme zorunluluğu
        if (_loggedInUser!.IsTempPassword)
        {
            ColoredHelper.ShowErrorMsg("İlk girişte lütfen yeni bir şifre belirleyiniz.");
            ChangePassword(_loggedInUser);
        }

        ShowMenuByRole(_loggedInUser.Role);
    }

    private bool Login()
    {
        for (int attempt = 0; attempt < 3; attempt++)
        {
            var tcknStr = Helper.Ask("TCKN (11 haneli)", true);
            if (!long.TryParse(tcknStr, out long tckn))
            {
                Console.WriteLine("Geçersiz TCKN formatı.");
                continue;
            }

            var password = Helper.AskPassword("Şifre");

            var (status, user) = _auth.Login(tckn, password);

            switch (status)
            {
                case Auth.LoginStatus.Success:
                    _loggedInUser = user!;
                    ColoredHelper.ShowSuccessMsg($"Hoşgeldiniz, {_loggedInUser.Name} {_loggedInUser.Surname}!");
                    return true;

                case Auth.LoginStatus.MustChangeTempPassword:
                    _loggedInUser = user!;
                    ColoredHelper.ShowInfoMsg("İlk girişte lütfen yeni bir şifre belirleyiniz.");
                    ChangePassword(_loggedInUser);
                    return true;

                case Auth.LoginStatus.UserNotFound:
                case Auth.LoginStatus.WrongPassword:
                    ColoredHelper.ShowErrorMsg("TCKN veya şifre yanlış. Tekrar deneyiniz.");
                    break;
            }
        }

        return false;
    }

    private void ChangePassword(User user)
    {
        while (true)
        {
            var newPassword = Helper.AskPassword("Yeni şifre (en az 6 karakter)");
            if (newPassword.Length < 6)
            {
                ColoredHelper.ShowErrorMsg("Şifre en az 6 karakter olmalı.");
                continue;
            }

            var confirmPassword = Helper.AskPassword("Yeni şifre tekrar");
            if (newPassword != confirmPassword)
            {
                ColoredHelper.ShowErrorMsg("Şifreler uyuşmuyor, tekrar deneyin.");
                continue;
            }

            _auth.ChangePassword(user, newPassword);
            ColoredHelper.ShowSuccessMsg("Şifre başarıyla değiştirildi.");
            break;
        }
    }

    private void ShowMenuByRole(Role role)
    {
        switch (role)
        {
            case Role.Admin:
                ShowAdminMenu();
                break;
            case Role.Teacher:
                ShowTeacherMenu();
                break;
            case Role.Student:
                ShowStudentMenu();
                break;
            default:
                Console.WriteLine("Tanımlanmamış rol.");
                break;
        }
    }

    private void ShowAdminMenu()
    {
        var mainMenu = new ConsoleMenu("Yönetici Menüsü");
        mainMenu
            .AddOption("Öğrenci Yönetimi", StudentManagement)
            .AddOption("Öğretmen Yönetimi", TeacherManagement)
            .AddOption("Sınıf Yönetimi", ClassroomManagement)
            .AddOption("Ders Yönetimi", LessonManagement);
        mainMenu.Show(isRoot: true);
    }

    private void ShowStudentMenu()
    {
        var menu = new ConsoleMenu("Öğrenci Menüsü");
        menu
            .AddOption("Bilgilerimi Görüntüle", () => _studentHelper.ShowStudentDetails(_loggedInUser!.Tckn))
            .AddOption("Notlarımı Görüntüle", () => _gradeHelper.ListGradesByStudent(_loggedInUser!.Tckn))
            .AddOption("Ödevlerimi Görüntüle", () => _homeworkHelper.ListHomeworksByStudent(_loggedInUser!.Tckn))
            .AddOption("Ödev Teslim Et", () => _homeworkHelper.SubmitHomework(_loggedInUser!.Tckn))
            .AddOption("Teslim Edilen Ödevlerim", () => _gradeHelper.ListSubmittedGradesByStudent(_loggedInUser!.Tckn))
            .AddOption("Şifremi Değiştir", () => ChangePassword(_loggedInUser!));
        menu.Show(isRoot: true);
    }

    private void ShowTeacherMenu()
    {
        var menu = new ConsoleMenu("Öğretmen Menüsü");
        menu
            .AddOption("Bilgilerimi Görüntüle", () => _teacherHelper.ShowTeacherDetails(_loggedInUser!.Tckn))
            .AddOption("Ödev Yönetimi", ShowHomeworkMenu)
            .AddOption("Not Yönetimi",  ShowGradesMenu);
        menu.Show(isRoot: true);
    }

    private void ShowHomeworkMenu()
    {
        var menu = new ConsoleMenu("Ödev İşlemleri");
        menu
            .AddOption("Ödev Oluştur", () => _homeworkHelper.CreateHomework(_loggedInUser!.Tckn))
            .AddOption("Ödev Güncelle", () => _homeworkHelper.UpdateHomework(_loggedInUser!.Tckn))
            .AddOption("Ödevleri Listele", () => _homeworkHelper.ListHomeworksByTeacher(_loggedInUser.Tckn))
            .AddOption("Ödev Sil", () => _homeworkHelper.DeleteHomework(_loggedInUser!.Tckn));
        menu.Show();
    }
   
    private void ShowGradesMenu()
    {
        var menu = new ConsoleMenu("Not İşlemleri");
        menu
            .AddOption("Not Ver", () => _gradeHelper.GiveGrades(_loggedInUser.Tckn))
            .AddOption("Notları Listele", () => _gradeHelper.ListGradesByTeacher(_loggedInUser.Tckn))
            .AddOption("Not Düzenle", () => _gradeHelper.UpdateGrade(_loggedInUser.Tckn))
            .AddOption("Not Sil", () => _gradeHelper.DeleteGrade(_loggedInUser!.Tckn));
        menu.Show();
    }


    public void StudentManagement()
    {
        var studentMenu = new ConsoleMenu("Öğrenci Yönetimi");
        studentMenu
            .AddOption("Öğrenci Ekle", () => _auth.RegisterUser("Student"))
            .AddOption("Öğrenci Düzenle", () => _studentHelper.UpdateStudent())
            .AddOption("Öğrenci Sil", () => _studentHelper.RemoveStudent())
            .AddOption("Öğrencileri Listele", () => _studentHelper.ListStudent());
        studentMenu.Show();
    }
    
    public void TeacherManagement()
    {
        var teacherMenu = new ConsoleMenu("Öğretmen Yönetimi");
        teacherMenu
            .AddOption("Öğretmen Ekle", () => _auth.RegisterUser("Teacher"))
            .AddOption("Öğretmen Düzenle", () => _teacherHelper.UpdateTeacher())
            .AddOption("Öğretmen Sil", () => _teacherHelper.RemoveTeacher())
            .AddOption("Öğretmenleri Listele", () => _teacherHelper.ListTeachers());
        teacherMenu.Show();
    }

    public void ClassroomManagement()
    {
        var classroomMenu = new ConsoleMenu("Sınıf Yönetimi");
        classroomMenu
            .AddOption("Sınıf Ekle", () => _classroomHelper.AddClassroom())
            .AddOption("Sınıf Düzenle", () => _classroomHelper.UpdateClassroomName())
            .AddOption("Sınıf Sil", () => _classroomHelper.DeleteClassroom())
            .AddOption("Sınıfları Listele", () => _classroomHelper.ListClassrooms());
        classroomMenu.Show();
    }

    public void LessonManagement()
    {
        var lessonMenu = new ConsoleMenu("Ders Yönetimi");
        lessonMenu
            .AddOption("Ders Ekle", () => _lessonHelper.AddOrAssignLesson())
            .AddOption("Ders Düzenle", () => _lessonHelper.EditLesson())
            .AddOption("Dersi Sil", () => _lessonHelper.DeleteLesson())
            .AddOption("Dersleri Listele", () => _lessonHelper.PickLesson());
        lessonMenu.Show();
    }
}
