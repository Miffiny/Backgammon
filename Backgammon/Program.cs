using System.Reflection;
using Backgammon.UI;

namespace Backgammon;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Console.WriteLine("Assembly is " +Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
        Application.Run(new Window());
    }
}