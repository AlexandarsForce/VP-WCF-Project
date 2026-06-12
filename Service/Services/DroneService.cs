using Common;
using Common.Enumerations;
using Common.Exceptions;
using System;
using System.ServiceModel;
using Service.Services;
using Common.Contracts;

namespace Service.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,ConcurrencyMode = ConcurrencyMode.Single)]
    public class DroneService : IDroneService, IDisposable
    {
        private WriterService writerService;
        private LoggingService loggingService;

        private static SessionStatusType sessionStatus = SessionStatusType.COMPLETED;
        private bool disposed = false;

        public DroneService()
        {
            writerService = new WriterService();
            loggingService = new LoggingService("log_session.csv");
        }
        ~DroneService()
        {
            Dispose(false);
        }

        public ResponseData StartSession(SessionData meta)
        {
            string responseMessage = "Session started successfully.";
            try
            {
                ValidationService.ValidateSession(meta);
                sessionStatus = SessionStatusType.IN_PROGRESS;
                writerService.StartSession("measurements_session.csv", "rejects.csv");
                Console.WriteLine(responseMessage);
                Console.WriteLine("Transfer in progress...");
                return new ResponseData(responseMessage, ResponseStatusType.ACK, sessionStatus);
            }
            catch (FaultException<DataFormatFault> exDff)
            {
                responseMessage = $"Data format error: {exDff.Detail.Message}";
            }
            catch (FaultException<ValidationFault> exVf)
            {
                responseMessage = $"Validation error: {exVf.Detail.Message}";
            }
            catch (Exception ex)
            {
                responseMessage = $"Unexpected error: {ex.Message}";
            }
            sessionStatus = SessionStatusType.COMPLETED;
            return new ResponseData($"Failed to start session : {responseMessage}", ResponseStatusType.NACK, sessionStatus);
        }

        public ResponseData PushSample(DroneSample sample)
        {
            string responseMessage = "Session is not in progress.";
            if (sessionStatus == SessionStatusType.IN_PROGRESS)
            {
                try
                {
                    ValidationService.ValidateSample(sample);
                    responseMessage = "Sample pushed successfully.";
                    writerService.WriteValidSample(sample);
                    return new ResponseData(responseMessage, ResponseStatusType.ACK, sessionStatus);
                }
                catch (FaultException<DataFormatFault> exDff)
                {
                    responseMessage = $"Data format error: {exDff.Detail.Message}";
                }
                catch (FaultException<ValidationFault> exVf)
                {
                    responseMessage = $"Validation error: {exVf.Detail.Message}";
                }
                catch (Exception ex)
                {
                    responseMessage = $"Unexpected error: {ex.Message}";
                }
            }
            writerService.WriteInvalidSample(sample, responseMessage);
            return new ResponseData($"Failed to push sample : {responseMessage}", ResponseStatusType.NACK, sessionStatus);
        }

        public ResponseData EndSession()
        {
            try
            {
                writerService.EndSession();
                sessionStatus = SessionStatusType.COMPLETED;
                Console.WriteLine("Transfer completed.");
                Console.WriteLine("Session ended successfully.");
                return new ResponseData("Session ended successfully.", ResponseStatusType.ACK, sessionStatus);
            }
            catch (Exception ex)
            {
                return new ResponseData($"Failed to end session: {ex.Message}", ResponseStatusType.NACK, sessionStatus);
            }
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
                    EndSession();
                    writerService?.Dispose();
                    loggingService?.Dispose();
                }
                disposed = true;
            }
        }
    }
}
