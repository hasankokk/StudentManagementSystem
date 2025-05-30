using StudentManagementSystem.Data;

namespace StudentManagementSystem;

class Program
{
    static void Main(string[] args)
    {
        using var context = new AppDbContext();
        var navigation = new Navigation(context);
        navigation.StartApp();
    }
}