using System;
using System.IO;
using System.Windows.Forms;
using EngineCoreStatic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace SeparateHeartsExtractorV1
{
    internal class Program
    {
        private enum ExtractorMode : uint
        {
            /// <summary>
            /// 无封包模式
            /// </summary>
            NonPacked,
            /// <summary>
            /// Hac封包模式
            /// </summary>
            HacPacked,
        }

        private static string GetExtractorModeString(ExtractorMode mode)
        {
            return mode switch
            {
                ExtractorMode.NonPacked => "未打包模式",
                ExtractorMode.HacPacked => "Hac封包模式",
                _ => string.Empty,
            };
        }

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("请选择提取模式:");
            ExtractorMode[] modes = Enum.GetValues<ExtractorMode>();
            for(int i = 0; i < modes.Length; ++i)
            {
                ExtractorMode v = modes[i];
                Console.WriteLine($"{v:d}:{GetExtractorModeString(v)}");
            }

            if(Enum.TryParse(Console.ReadLine(), out ExtractorMode mode))
            {
                switch (mode)
                {
                    case ExtractorMode.NonPacked:
                    {
                        using FolderBrowserDialog fbd = new()
                        {
                            Description = "SeparateHearts V1 - 选择资源路径",
                            ShowNewFolderButton = false,
                            AutoUpgradeEnabled = true,
                            UseDescriptionForTitle = true,
                        };
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            HACDirectFile directFile = new(fbd.SelectedPath);
                            directFile.Extract();

                            Console.WriteLine("===== SeparateHearts V1 - 提取完成 =====");
                            Console.Read();
                        }
                        break;
                    }
                    case ExtractorMode.HacPacked:
                    {
                        using OpenFileDialog ofd = new()
                        {
                            AddExtension = true,
                            AutoUpgradeEnabled = true,
                            CheckFileExists = true,
                            CheckPathExists = true,
                            DefaultExt = ".hac",
                            Filter = "HAC封包(*.hac)|*.hac|所有文件(*.*)|*.*",
                            Multiselect = true,
                            RestoreDirectory = true,
                            ShowHelp = false,
                            Title = "SeparateHearts V1 - 选择封包",
                        };
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            foreach (string file in ofd.FileNames)
                            {
                                string outDir = Path.Combine(Path.GetDirectoryName(file)!, "Static_Extract");
                                using HACPackage pkg = new(file);
                                if (pkg.IsVaild)
                                {
                                    pkg.Extract(outDir);
                                }
                            }

                            Console.WriteLine("===== SeparateHearts V1 - 提取完成 =====");
                            Console.Read();
                        }
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
        }
    }
}