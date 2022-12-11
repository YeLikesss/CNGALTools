using Extractor.GUI;
using System;
using System.Windows.Forms;

namespace ExtractorGui
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