using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using OpenQA.Selenium.Chrome;

namespace GSM_BYTUTJ2025.DAO
{
    public class Libary
    {
        private static Libary instance;

        public static Libary Instance
        {
            get { if (instance == null) instance = new Libary(); return Libary.instance; }
            private set { Libary.instance = value; }
        }
        public void ClearLog()
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logfile");
            string logFilePath = Path.Combine(logDirectory, "logfile.txt");

            if (File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, string.Empty);
                Console.WriteLine("Đã xóa trắng file log.");
            }
            else
            {
                // Tạo thư mục nếu chưa có
                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);

                // Tạo file rỗng
                File.Create(logFilePath).Close();
                Console.WriteLine("Đã tạo file log mới.");
            }
        }

        public void CreateLog(string message, Exception ex = null, [CallerMemberName] string callerName = "")
        {
            try
            {
                string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logfile");
                string logFilePath = Path.Combine(logDirectory, "logfile.txt");

                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);

                if (!File.Exists(logFilePath))
                    File.Create(logFilePath).Close(); // Tạo file nếu chưa có

                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - [{callerName}] - {message}";

                if (ex != null)
                {
                    logMessage += $"{Environment.NewLine}Exception: {ex.Message}{Environment.NewLine}StackTrace: {ex.StackTrace}";
                }

                Console.WriteLine(logMessage);
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception logEx)
            {
                Console.WriteLine("Error writing log: " + logEx.Message);
            }
        }
        public ChromeOptions Options(string profile)
        {
            ChromeOptions option = new ChromeOptions();
            option.AddArguments("user-data-dir=" + profile);
            option.AddArgument("--disable-infobars");
            option.AddArgument("start-maximized");
            option.AddArgument("--disable-extensions");
            //option.AddArgument("--headless"); //chạy ngầm
            return option;
        }
    }
}
