using Common;
using Common.Enumerations;
using Common.Exceptions;
using System;
using System.ServiceModel;
using Service.Services;
using Common.Contracts;

namespace Service.Services
{
    public class DroneService : IDroneService
    {
        private static SessionStatusType sessionStatus = SessionStatusType.COMPLETED;

        public ResponseData StartSession(SessionData meta)
        {
            string responseMessage = "Session started successfully.";
            try
            {
                ValidationService.ValidateSession(meta);
                sessionStatus = SessionStatusType.IN_PROGRESS;
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
            return new ResponseData($"Failed to push sample : {responseMessage}", ResponseStatusType.NACK, sessionStatus);
        }

        public ResponseData EndSession()
        {
            try
            {
                sessionStatus = SessionStatusType.COMPLETED;
                return new ResponseData("Session ended successfully.", ResponseStatusType.ACK, sessionStatus);
            }
            catch (Exception ex)
            {
                return new ResponseData($"Failed to end session: {ex.Message}", ResponseStatusType.NACK, sessionStatus);
            }
        }
    }
}
