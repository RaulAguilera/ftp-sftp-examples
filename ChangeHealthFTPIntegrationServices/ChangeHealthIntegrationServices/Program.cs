using System;
using System.IO;
using System.Text;

namespace ChangeHealthIntegrationServices
{
    class Program
    {
        static void Main(string[] args)
        {
            

            FTPInfo ftpInfo = new FTPInfo { Server = "localhost", Port = "12345" ,User = "raul", Password = "raul_2020" };

            FTPManager.UploadFile(ftpInfo,"testFile.txt","claims");
            FTPManager.DownloadAllFilesInDirectory(ftpInfo, "era", @"\LocalFolder\era\");

            SFTPManager.UploadFile(ftpInfo, "testFile.txt", "claims");
            SFTPManager.DownloadAllFilesInDirectory(ftpInfo, "era", @"\LocalFolder\era\");


            Console.ReadKey();
        }
    }
}
