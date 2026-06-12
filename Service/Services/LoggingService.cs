using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class LoggingService : IDisposable
    {
        private StreamWriter logWriter;
        private bool disposed = false;
        public LoggingService(string logFileName)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Dataset", logFileName);
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }
            logWriter = new StreamWriter(filePath, false, Encoding.UTF8);
        }
        ~LoggingService()
        {
            Dispose(false);
        }
        public void Log(string message)
        {
            if (disposed || logWriter == null)
            {
                throw new ObjectDisposedException(nameof(LoggingService));
            }
            try
            {
                logWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {message}");
                logWriter.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write log message: {ex.Message}");
            }

        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    logWriter?.Close();
                    logWriter = null;
                }
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
