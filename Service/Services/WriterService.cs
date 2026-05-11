using Common.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class WriterService : IDisposable
    {
        private StreamWriter measurementsWriter;
        private StreamWriter rejectsWriter;
        private bool disposed = false;
        public WriterService() { }

        ~WriterService()
        {
            Dispose(false);
        }

        public void StartSession(string sampleFileName, string rejectsFileName)
        {            
            string sampleFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Dataset", sampleFileName);
            if(!File.Exists(sampleFilePath))
            {
                File.Create(sampleFilePath).Dispose();
            }

            string rejectedDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Errors");
            if (!Directory.Exists(rejectedDirectoryPath))
            {
                Directory.CreateDirectory(rejectedDirectoryPath);
            }
            rejectsWriter = new StreamWriter(Path.Combine(rejectedDirectoryPath, rejectsFileName), false, Encoding.UTF8);
            measurementsWriter = new StreamWriter(sampleFilePath, false, Encoding.UTF8);
            measurementsWriter.WriteLine("Time,WindSpeed,WindAngle,LinearAccelerationX,LinearAccelerationY,LinearAccelerationZ");
        }

        public void WriteValidSample(DroneSample sample)
        {
            if (disposed || measurementsWriter == null)
            {
                throw new ObjectDisposedException(nameof(WriterService));
            }
            try
            {
                measurementsWriter.WriteLine(
                    $"{sample.Time.ToString(CultureInfo.InvariantCulture)}," +
                    $"{sample.WindSpeed.ToString(CultureInfo.InvariantCulture)}," +
                    $"{sample.WindAngle.ToString(CultureInfo.InvariantCulture)}," +
                    $"{sample.LinearAccelerationX.ToString(CultureInfo.InvariantCulture)}," +
                    $"{sample.LinearAccelerationY.ToString(CultureInfo.InvariantCulture)}," +
                    $"{sample.LinearAccelerationZ.ToString(CultureInfo.InvariantCulture)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write rejected sample: {ex.Message}");
            }
        }

        public void WriteInvalidSample(DroneSample sample, string errorMessage)
        {

            if (disposed || rejectsWriter == null)
            {
                throw new ObjectDisposedException(nameof(WriterService));
            }
            try 
            {
                rejectsWriter.WriteLine(
                    $"{sample.Time.ToString(CultureInfo.InvariantCulture)}," +
                    $"{sample.WindSpeed.ToString(CultureInfo.InvariantCulture)}," +
                    $"{sample.WindAngle.ToString(CultureInfo.InvariantCulture)}," +
                    $"{sample.LinearAccelerationX.ToString(CultureInfo.InvariantCulture)}," +
                    $"{sample.LinearAccelerationY.ToString(CultureInfo.InvariantCulture)}," +
                    $"{sample.LinearAccelerationZ.ToString(CultureInfo.InvariantCulture)}, " +
                    $"{errorMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write rejected sample: {ex.Message}");
            }
        }

        public void EndSession()
        {
            if (disposed)
            {
                return;
            }
            measurementsWriter?.Flush();
            measurementsWriter?.Dispose();
            measurementsWriter = null;
            rejectsWriter?.Flush();
            rejectsWriter?.Dispose();
            rejectsWriter = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    measurementsWriter?.Dispose();
                    rejectsWriter?.Dispose();
                }
                disposed = true;
            }
        }
    }
}
