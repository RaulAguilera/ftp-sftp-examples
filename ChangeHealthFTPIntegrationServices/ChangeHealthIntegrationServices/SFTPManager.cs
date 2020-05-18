using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.IO;

namespace ChangeHealthIntegrationServices
{
    public static class SFTPManager
    {
        public static void UploadFile(FTPInfo ftpInfo, string filePath, string destinationDirectory)
        {
            using (var client = new SftpClient(ftpInfo.Server, ftpInfo.User, ftpInfo.Password))
            {
                client.Connect();

                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    client.UploadFile(fileStream, $"{destinationDirectory}/{Path.GetFileName(filePath)}");
                }
            }
        }

        public static void DownloadAllFilesInDirectory(FTPInfo ftpInfo, string directory, string destinationDirectory)
        {
            using (var client = new SftpClient(ftpInfo.Server, ftpInfo.User, ftpInfo.Password))
            { 
                client.Connect();

                var filesInfo = client.ListDirectory(directory);

                foreach (var fileInfo in filesInfo)
                {
                    if (!fileInfo.IsDirectory)
                    {
                        using (FileStream fs = File.Create($"{destinationDirectory}{fileInfo.Name}"))
                        {
                            client.DownloadFile(fileInfo.FullName, fs);
                        }
                    }
                }
            }
        }
    }
}
