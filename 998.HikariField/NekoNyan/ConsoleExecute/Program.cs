using System;
using NekoNyanStatic.Crypto;

namespace ConsoleExecute
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetBufferSize(1280, 720);

            foreach(var pkgPath in args)
            {

                if (ArchiveCryptoBase.IsVaildPackage(pkgPath))
                {
                    ArchiveCryptoBase filter = ArchiveCryptoBase.Create(pkgPath,CryptoVersion.V10);
                    filter.Extract();
                    filter.Dispose();
                }
            }
        }
    }
}
