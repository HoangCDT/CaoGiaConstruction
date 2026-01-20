using System.Text;

namespace CaoGiaConstruction.Utilities
{
    public static class FileUtility
    {
        public static string ReadFile(string url, ref bool isSuccess, ref Exception error)
        {
            try
            {
                string result = File.ReadAllText(url);

                isSuccess = true;

                return result;
            }
            catch (Exception ex)
            {
                error = ex;

                isSuccess = false;

                return string.Empty;
            }
        }

        public static bool WriteFile(string value, string url, ref Exception error)
        {
            try
            {
                string addText = File.ReadAllText(url);
                File.WriteAllText(url, addText + value, Encoding.UTF8);

                return true;
            }
            catch (Exception ex)
            {
                error = ex;
                return false;
            }
        }

        public class UploadFiles
        {
            public UploadFiles(long _size, List<string> _listFileName, bool _success, string _message)
            {
                size = _size;
                listFileName = _listFileName;
                success = _success;
                message = _message;
            }

            public long size { get; set; }
            public List<string> listFileName { get; set; }
            public bool success { get; set; }
            public string message { get; set; }
        }

        public class DownloadFile
        {
            public DownloadFile(System.IO.MemoryStream _content, string _contentType, string _original_fileName)
            {
                content = _content;
                contentType = _contentType;
                original_fileName = _original_fileName;
            }

            public System.IO.MemoryStream content { get; set; }
            public string contentType { get; set; }
            public string original_fileName { get; set; }
        }
    }
}