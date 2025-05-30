using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories;

namespace StudentManagementSystem.Helpers;

public class ClassroomHelper
{
    private readonly ClassroomRepository _classroomRepository;

    public ClassroomHelper(AppDbContext context)
    {
        _classroomRepository = new ClassroomRepository(context);
    }

    public Classroom AskClassroom(string registerType)
    {
        var classrooms = _classroomRepository.GetClassroomList();

        if (registerType == "Teacher")
        {
            if (classrooms == null || !classrooms.Any())
            {
                ColoredHelper.ShowInfoMsg("Hiç sınıf bulunamadı. Yeni bir sınıf ekleyin.");
                AddClassroom();
                classrooms = _classroomRepository.GetClassroomList();

                if (classrooms == null || !classrooms.Any())
                    return null!;
            }

            var classNames = classrooms.Select(c => c.Name).ToArray();
            int selectClass = Helper.AskOption(classNames, "Hangi sınıfa eklensin?", "Yeni Sınıf Ekle");

            if (selectClass == 0)
            {
                AddClassroom();
                classrooms = _classroomRepository.GetClassroomList();
                classNames = classrooms.Select(c => c.Name).ToArray();
                selectClass = Helper.AskOption(classNames, "Hangi sınıfa eklensin?");
            }

            return classrooms[selectClass - 1];
        }
        
        if (classrooms == null || !classrooms.Any())
        {
            ColoredHelper.ShowErrorMsg("Sistemde tanımlı sınıf yok. Lütfen önce sınıf ekleyin.");
            return null!;
        }

        var classNamesForStudent = classrooms.Select(c => c.Name).ToArray();
        int selectedClassroom = Helper.AskOption(classNamesForStudent, "Hangi sınıfa eklensin?");
        return classrooms[selectedClassroom - 1];
    }


    public void UpdateClassroomName()
    {
        var classrooms = _classroomRepository.GetClassroomList();
        if (!classrooms.Any())
        {
            ColoredHelper.ShowErrorMsg("Güncellenecek sınıf bulunamadı.");
            return;
        }

        var classNames = classrooms.Select(c => c.Name).ToArray();
        int selectedIdx = Helper.AskOption(classNames, "Adını değiştirmek istediğiniz sınıfı seçin", "İptal");
        if (selectedIdx == 0)
            return;

        var selectedClassroom = classrooms[selectedIdx - 1];
        var newName = Helper.Ask("Yeni sınıf adını girin", true);
        
        if (classrooms.Any(c => c.Name != null && c.Name == newName))
        {
            ColoredHelper.ShowErrorMsg("Bu isimde başka bir sınıf zaten mevcut.");
            return;
        }

        selectedClassroom.Name = newName;
        bool success = _classroomRepository.UpdateClassroom(selectedClassroom);

        if (success)
            ColoredHelper.ShowSuccessMsg("Sınıf adı başarıyla güncellendi.");
        else
            ColoredHelper.ShowErrorMsg("Sınıf adı güncellenemedi.");
    }
    public void DeleteClassroom()
    {
        var classrooms = _classroomRepository.GetClassroomList();
        if (!classrooms.Any())
        {
            ColoredHelper.ShowErrorMsg("Silinecek sınıf bulunamadı.");
            return;
        }

        var classNames = classrooms.Select(c => c.Name).ToArray();
        int selectedIdx = Helper.AskOption(classNames, "Silmek istediğiniz sınıfı seçin", "İptal");
        if (selectedIdx == 0)
            return;

        var classroomToDelete = classrooms[selectedIdx - 1];

        bool success = _classroomRepository.DeleteClassroom(classroomToDelete.ClassroomId);
        if (success)
            ColoredHelper.ShowSuccessMsg("Sınıf başarıyla silindi.");
        else
            ColoredHelper.ShowErrorMsg("Sınıf silinemedi.");
    }


    public void ListClassrooms()
    {
        var classrooms = _classroomRepository.GetClassroomList();
        if (classrooms == null || !classrooms.Any())
        {
            ColoredHelper.ShowErrorMsg("Sistemde tanımlı sınıf bulunamadı.");
            return;
        }

        foreach (var classroom in classrooms)
        {
            ColoredHelper.ShowListMsg($"{classroom.ClassroomId} - {classroom.Name}");
        }
    }


    public List<Classroom> UpdateClassRoom(string updateType, List<int>? excludeClassroomIds = null)
    {
        var classrooms = _classroomRepository.GetClassroomList();

        if (!classrooms.Any())
        {
            ColoredHelper.ShowErrorMsg("Sistemde sınıf bulunamadı.");
            return new List<Classroom>();
        }

        excludeClassroomIds ??= new List<int>();

        var selectedClassrooms = new List<Classroom>();

        while (true)
        {
            var available = classrooms
                .Where(c => !selectedClassrooms.Contains(c) && !excludeClassroomIds.Contains(c.ClassroomId))
                .ToList();

            if (!available.Any())
                break;

            var classNames = available.Select(c => c.Name).ToArray();
            var idx = Helper.AskOption(classNames, $"{updateType}mek için sınıf seçin", "İşlemi bitir");
            if (idx == 0)
                break;

            selectedClassrooms.Add(available[idx - 1]);
            ColoredHelper.ShowInfoMsg($"{available[idx - 1].Name} sınıfı eklendi.");
        }

        return selectedClassrooms;
    }
    
    public List<Classroom> UpdateClassRoomsForRemove(List<Classroom> currentClassrooms)
    {
        if (currentClassrooms == null || !currentClassrooms.Any())
        {
            ColoredHelper.ShowErrorMsg("Silinecek sınıf bulunamadı.");
            return new List<Classroom>();
        }

        var selectedClassrooms = new List<Classroom>();

        while (true)
        {
            var available = currentClassrooms.Where(c => !selectedClassrooms.Contains(c)).ToList();

            if (!available.Any())
                break;

            var classNames = available.Select(c => c.Name).ToArray();

            var idx = Helper.AskOption(classNames, "Silmek için sınıf seçin", "İşlemi bitir");
            if (idx == 0)
                break;

            selectedClassrooms.Add(available[idx - 1]);
            ColoredHelper.ShowInfoMsg($"{available[idx - 1].Name} sınıfı silme listesine eklendi.");
        }

        return selectedClassrooms;
    }
    
    public void AddClassroom()
    {
        var inputClassName = Helper.Ask("Eklenecek olan sınıf adını giriniz", true);
        var classname = new Classroom
        {
            Name = inputClassName,
        };
        if (_classroomRepository.AddClassroom(classname))
        {
            ColoredHelper.ShowSuccessMsg("Sınıf başarıyla oluşturuldu.");
            return;
        }
        ColoredHelper.ShowErrorMsg("Sınıf oluşturulamadı!");
    }
    public Classroom? PickClassroomFrom(ICollection<Classroom> list)
    {
        if (list == null || !list.Any())
        {
            ColoredHelper.ShowInfoMsg("Bu derse atanmış sınıf yok.");
            return null;
        }

        var classNames = list.Select(c => c.Name).ToArray();
        int selectedIdx = Helper.AskOption(classNames, "Çıkarılacak sınıfı seçin:", "İptal");
        if (selectedIdx == 0)
            return null;

        return list.ElementAt(selectedIdx - 1);
    }
}