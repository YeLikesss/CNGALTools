
using System;
using OpaiStatic;
using OpaiStatic.Extractor;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string root = "E:\\Archenemy Lunafall v1.0.0";
            IExtractor extractor = new ArchenemyLunafall();
            extractor.Extract(root);
            Console.Read();
        }
    }
}