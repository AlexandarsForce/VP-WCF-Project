using Common;
using Common.Enumerations;
using Common.Exceptions;
using System;
using System.ServiceModel;
using Service.Services;
using Common.Contracts;
using Service.Events.Arguments;

namespace Service.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,ConcurrencyMode = ConcurrencyMode.Single)]
    public class DroneService : IDroneService, IDisposable
    {
        private WriterService writerService;
        private LoggingService loggingService;

        private static SessionStatusType sessionStatus = SessionStatusType.COMPLETED;
        private bool disposed = false;

        public delegate void TransferHandler(object sender, TransferArgument e);

        private event TransferHandler TransferStarted;
        private event TransferHandler TransferSample;
        private event TransferHandler TransferCompleted;
        private event TransferHandler TransferWarning;

        public DroneService()
        {
            writerService = new WriterService();
            loggingService = new LoggingService("log_session.csv");

                TransferStarted += OnTransferStarted;
                TransferSample += OnSampleReceived;
                TransferCompleted += OnTransferCompleted;
                TransferWarning += OnWarningRaised;
        }
        ~DroneService()
        {
            Dispose(false);
        }

        // --- IDroneService Implementation ---

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
                TransferStarted?.Invoke(this, new TransferArgument { Message = responseMessage, LogStatus = LogStatusType.INFO });
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
            TransferStarted?.Invoke(this, new TransferArgument { Message = responseMessage, LogStatus = LogStatusType.ERROR });
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
                    TransferSample?.Invoke(this, new TransferArgument { Message = responseMessage, LogStatus = LogStatusType.INFO });
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
            TransferSample?.Invoke(this, new TransferArgument { Message = responseMessage, LogStatus = LogStatusType.ERROR });
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
                TransferCompleted?.Invoke(this, new TransferArgument { Message = "Session ended successfully.", LogStatus = LogStatusType.INFO });
                return new ResponseData("Session ended successfully.", ResponseStatusType.ACK, sessionStatus);
            }
            catch (Exception ex)
            {
                TransferCompleted?.Invoke(this, new TransferArgument { Message = $"Failed to end session: {ex.Message}", LogStatus = LogStatusType.ERROR });
                return new ResponseData($"Failed to end session: {ex.Message}", ResponseStatusType.NACK, sessionStatus);
            }
        }

        // --- IDisposable Implementation ---

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

        // --- Events Implementation ---

        public void OnTransferStarted(object sender, TransferArgument e)
        {
            loggingService.Log($"{e.LogStatus}: Transfer started: {e.Message}");
        }

        public void OnSampleReceived(object sender, TransferArgument e)
        {
            loggingService.Log($"{e.LogStatus}: Sample received: {e.Message}");
        }

        public void OnTransferCompleted(object sender, TransferArgument e)
        {
            loggingService.Log($"{e.LogStatus}: Transfer completed: {e.Message}");
        }

        public void OnWarningRaised(object sender, TransferArgument e)
        {
            loggingService.Log($"{e.LogStatus}: Warning raised: {e.Message}");
        }
    }
}
