namespace StudentManagementSystem.Helpers;

public static class Helper
{
    public static string? Ask(string question, bool isRequired = false, string validationMsg = "Bu alanı boş bırakamazsın.")
    {
        string? response;
        do
        {
            ColoredHelper.ShowQuestionMsg($"{question}: ");
            response = Console.ReadLine();

            if (isRequired && string.IsNullOrWhiteSpace(response))
            {
                ColoredHelper.ShowErrorMsg(validationMsg);
            }
            
        } while (isRequired && string.IsNullOrWhiteSpace(response));
        
        return response?.Trim();
    }
    
    public static DateTime AskDate(string question, string validationMsg = "Geçerli bir tarih giriniz (örn: 31.12.2025)")
    {
        while (true)
        {
            var response = Ask(question, true);
            if (DateTime.TryParse(response, out var result))
            {
                return result;
            }
            ColoredHelper.ShowErrorMsg(validationMsg);
        }
    }

    public static int AskNumber(string question, string validationMsg = "Bir sayı girmelisin.")
    {
        while (true)
        {
            var response = Ask(question, true);
            if (int.TryParse(response, out var result))
            {
                return result;
            }
            ColoredHelper.ShowErrorMsg(validationMsg);
        }
    }

    public static int AskOption(string[] options, string? question = null, string? cancelOption = null)
    {
        if (options.Length == 0)
        {
            throw new ArgumentException($"{nameof(options)} içinde seçenekler olmalı.", nameof(options));
        }

        if (question != null)
        {
            ColoredHelper.ShowQuestionMsgLine(question);
        }
        
        for (int i = 0; i < options.Length; i++)
        {
            ColoredHelper.ShowQuestionMsgLine($"{i + 1}. {options[i]}");
        }
        
        var optionsStartFrom = 1;
        
        if (cancelOption != null)
        {
            ColoredHelper.ShowQuestionMsgLine($"0. {cancelOption}");
            optionsStartFrom--;
        }
        
        while (true)
        {
            var inputResponse = AskNumber($"Seçimin ({optionsStartFrom}-{options.Length})");
            if (inputResponse >= optionsStartFrom && inputResponse <= options.Length)
            {
                return inputResponse;
            }
    
            ColoredHelper.ShowErrorMsg("Hatalı seçim yaptın.");
        }
    }

    public static string AskPassword(string question, string validationMsg = "Bu alanı boş bırakamazsın.")
    {
        string password;
        do
        {
            ColoredHelper.ShowQuestionMsg($"{question}: ");
            password = ReadSecretLine();
            
            if (string.IsNullOrWhiteSpace(password))
            {
                ColoredHelper.ShowErrorMsg(validationMsg);
            }
            
        } while (string.IsNullOrWhiteSpace(password));
        
        return password;
    }
    
    
    private static string ReadSecretLine()
    {
        var line = "";
        ConsoleKeyInfo key;
        do
        {
            key =  Console.ReadKey(true);

            if (key.Key == ConsoleKey.Backspace && line.Length > 0)
            {
                line = line.Substring(0, line.Length - 1);
                Console.Write("\b \b");
                continue;
            }

            if (!IsSecurePassChar(key.KeyChar) || char.IsControl(key.KeyChar))
            {
                continue;
            }
            
            line += key.KeyChar;

            Console.Write(key.KeyChar);
            Thread.Sleep(75);
            Console.Write("\b*");

        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine("\n");
        return line;
    }

    private static bool IsSecurePassChar(char c)
    {
        return char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c) || char.IsWhiteSpace(c);
    }
}

public class ConsoleMenu(string title)
{
    public string Title { get; } = title;
    private readonly List<MenuOption> _options = [];
    public ConsoleMenu AddOption(string title, Action action)
    {
        _options.Add(new MenuOption(title, action));
        return this;
    }

    public ConsoleMenu AddMenu(string title, Action action)
    {
        _options.Add(new MenuOption(title, action, true));
        return this;
    }

    public void Show(bool isRoot = false)
    {
        while (true)
        {
            Console.Clear(); // menüyü göstermek için ekranı temizle
            ColoredHelper.ShowMenuMsg(Title.ToUpper());
            Console.WriteLine();
            var inputMenuNumber = Helper.AskOption(_options.Select(x => x.Title).ToArray(), cancelOption: isRoot ? "Çıkış" : "Üst menü");
            Console.Clear(); // menü aksiyonunu çalıştırmadan önce ekranı temizle

            if (inputMenuNumber == 0)
            {
                if (isRoot)
                {
                    ColoredHelper.ShowInfoMsg("Hoşçakalın...");
                    Thread.Sleep(1000);
                }
                
                return;
            }
            
            var selectedOption = _options[inputMenuNumber - 1];
            selectedOption.Action.Invoke();
            if (!selectedOption.SkipWait)
            {
                ColoredHelper.ShowInfoMsg("\nMenüye dönmek için bir tuşa basın.");
                Console.ReadKey(true); 
            }
        }
    }
}

public class MenuOption(string title, Action action, bool skipWait = false)
{
    public string Title { get; } = title;
    public Action Action { get; } = action;
    public bool SkipWait { get; } = skipWait;
}