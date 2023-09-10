using System;
using System.Windows.Forms;
namespace MainFrom
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainFrom());
        }
    }
}