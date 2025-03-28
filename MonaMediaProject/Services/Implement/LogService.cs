using Microsoft.IdentityModel.Logging;
using MonaMediaProject.DataTier;
using MonaMediaProject.Services.Interface;
using System.Diagnostics;
using static Azure.Core.HttpHeader;

namespace MonaMediaProject.Services.Implement
{
    public class LogService : ILogService
    {
        private static readonly string LogDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs"); // đường dẫn đến thư mục Logs
        private static readonly string LogFilePath = Path.Combine(LogDirectory, $"LogReview-{DateTime.Now:yyyyMMdd}.txt"); // file LogReview theo ngày
        //Ghi log vào file LogReview trong thư mục Logs
        public void WriteLogError(string message, Exception ex)
        {
            // Nếu thư mục logs chưa tồn tại thì tạo mới
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            // Ghi log vào file
            using (StreamWriter writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine($"{message} - {ex}");
            }
        }
    }
}
