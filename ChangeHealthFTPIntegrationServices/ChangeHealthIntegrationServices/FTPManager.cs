using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ChangeHealthIntegrationServices
{
    public static class FTPManager
    {
        public static void UploadFile(FTPInfo ftpInfo, string filePath, string destinationPath)
        {
            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader(filePath))
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ftpInfo.Server}:{ftpInfo.Port}/{destinationPath}/{Path.GetFileName(filePath)}");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(ftpInfo.User, ftpInfo.Password);
            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

        }

        public static void DownloadAllFilesInDirectory(FTPInfo ftpInfo, string directory, string destinationDirectory)
        {
            var fileNames = GetAllFileNames(ftpInfo, directory);

            foreach (var fileName in fileNames)
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ftpInfo.Server}:{ftpInfo.Port}/{directory}/{fileName}");
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(ftpInfo.User, ftpInfo.Password);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                using (FileStream fs = File.Create($"{destinationDirectory}{fileName}"))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(reader.ReadToEnd());
                    fs.Write(info);
                }

                reader.Close();
                response.Close();
            }

        }


        private static List<string> GetAllFileNames(FTPInfo ftpInfo, string originDirectory)
        {
            List<string> fileNames = new List<string>();

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ftpInfo.Server}:{ftpInfo.Port}/{originDirectory}");
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = new NetworkCredential(ftpInfo.User, ftpInfo.Password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            string fileName = reader.ReadLine();

            while (!string.IsNullOrEmpty(fileName))
            {
                var array = fileName.Split(" ");
                fileNames.Add(array[array.Length - 1]);
                fileName = reader.ReadLine();
            }

            reader.Close();
            response.Close();

            return fileNames;
        }
    }
}
