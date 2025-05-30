namespace StudentManagementSystem.Helpers;

public class ColoredHelper
{
    public static void ShowListMsg(string msg)
    {
        ShowColoredMsg(msg, ConsoleColor.Green);
    }
    public static void ShowMenuMsg(string msg)
    {
        ShowColoredMsg(msg, ConsoleColor.DarkRed);
    }
    public static void ShowQuestionMsgLine(string msg)
    {
        ShowColoredMsg(msg, ConsoleColor.Blue);
    }
    public static void ShowQuestionMsg(string msg)
    {
        ShowColoredMsgNoLine(msg, ConsoleColor.Blue);
    }
    public static void ShowSuccessMsg(string msg)
    {
        ShowColoredMsg(msg, ConsoleColor.Green);
    }

    public static void ShowErrorMsg(string msg)
    {
        ShowColoredMsg(msg, ConsoleColor.Red);
    }

    public static void ShowInfoMsg(string msg)
    {
        ShowColoredMsg(msg, ConsoleColor.Yellow);
    }
    
    private static void ShowColoredMsg(string msg, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ResetColor();
    }
    private static void ShowColoredMsgNoLine(string msg, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(msg);
        Console.ResetColor();
    }
}