using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace HashDecoder
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}