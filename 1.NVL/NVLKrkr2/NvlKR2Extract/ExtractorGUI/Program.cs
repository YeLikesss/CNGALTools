using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ExtractorGUI
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //NvlKr2Extract.V2.Hasher.GetFileNameHash(".avi");

            AllocConsole();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            FreeConsole();
        }
        [DllImport("kernel32.dll", EntryPoint = "AllocConsole")]
        extern static bool AllocConsole();

        [DllImport("kernel32.dll", EntryPoint = "FreeConsole")]
        extern static bool FreeConsole();
    }
}
