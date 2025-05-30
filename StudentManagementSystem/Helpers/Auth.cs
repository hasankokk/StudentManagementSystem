using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;
using StudentManagementSystem.Helpers;

namespace StudentManagementSystem.Helpers
{
    public class Auth
    {
        private readonly UserRepository _userRepository;
        private readonly StudentRepository _studentRepository;
        private readonly TeacherRepository _teacherRepository;
        private readonly ClassroomHelper _classroomHelper;

        public Auth(AppDbContext context)
        {
            _userRepository = new UserRepository(context);
            _studentRepository = new StudentRepository(context);
            _teacherRepository = new TeacherRepository(context);
            _classroomHelper = new ClassroomHelper(context);
        }
        public enum RegisterStatus
        {
            Success,
            InvalidTckn
        }
        public enum LoginStatus
        {
            Success,
            UserNotFound,
            WrongPassword,
            MustChangeTempPassword
        }
        public static RegisterStatus Register(long tckn)
        {
            if (!Validation.IsValidTckn(tckn))
                return RegisterStatus.InvalidTckn;

            return RegisterStatus.Success;
        }
        public void RegisterUser(string registerType)
        {
            var inputTckn = long.Parse(Helper.Ask("Tckn", true));
            var isValidRegister = Register(inputTckn);

            switch (isValidRegister)
            {
                case RegisterStatus.Success:
                    if (_userRepository.GetByTckn(inputTckn) != null)
                    {
                        ColoredHelper.ShowErrorMsg("TCKN sistemde kayıtlı!");
                        return;
                    }

                    var inputName = Helper.Ask("Ad", true);
                    var inputSurname = Helper.Ask("Soyad", true);
                    var tempPassword = PasswordHelper.GenerateTemporaryPassword();
                    var hashedTempPassword = PasswordHelper.HashPassword(tempPassword);

                    var role = registerType == "Student" ? Role.Student :
                               registerType == "Teacher" ? Role.Teacher :
                               Role.Student;

                    var user = _userRepository.AddUser(new User
                    {
                        Name = inputName,
                        Surname = inputSurname,
                        Tckn = inputTckn,
                        Role = role,
                        Password = hashedTempPassword,
                        IsTempPassword = true
                    });

                    ColoredHelper.ShowSuccessMsg($"Geçici şifreniz: {tempPassword}");

                    if (registerType == "Student")
                    {
                        var selectedClassroom = _classroomHelper.AskClassroom("Student");
                        var student = _studentRepository.AddStudent(user.UserId);
                        if (selectedClassroom != null)
                            _studentRepository.AddClassroom(student.StudentId, selectedClassroom.ClassroomId);

                        ColoredHelper.ShowSuccessMsg("Öğrenci kaydı başarılı!");
                    }
                    else if (registerType == "Teacher")
                    {
                        var selectedClassroom = _classroomHelper.AskClassroom("Teacher");
                        var teacher = _teacherRepository.AddTeacher(user.UserId);
                        if (selectedClassroom != null)
                            _teacherRepository.AddClass(teacher.TeacherId, selectedClassroom.ClassroomId);

                        ColoredHelper.ShowSuccessMsg("Öğretmen kaydı başarılı!");
                    }
                    break;

                case RegisterStatus.InvalidTckn:
                    ColoredHelper.ShowErrorMsg("Geçersiz Tckn!");
                    break;
            }
        }
        public (LoginStatus status, User? user) Login(long tckn, string password)
        {
            var user = _userRepository.GetByTckn(tckn);

            if (user == null)
                return (LoginStatus.UserNotFound, null);

            if (!PasswordHelper.VerifyPassword(password, user.Password))
                return (LoginStatus.WrongPassword, null);

            if (user.IsTempPassword)
                return (LoginStatus.MustChangeTempPassword, user);

            return (LoginStatus.Success, user);
        }
        public bool ChangePassword(User user, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                ColoredHelper.ShowErrorMsg("Şifre en az 6 karakter olmalı.");
                return false;
            }

            var hashedPassword = PasswordHelper.HashPassword(newPassword);
            user.Password = hashedPassword;
            user.IsTempPassword = false;

            _userRepository.Update(user);

            ColoredHelper.ShowSuccessMsg("Şifreniz başarıyla değiştirildi.");
            return true;
        }
    }
}
