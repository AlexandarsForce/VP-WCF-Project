using Common;
using Common.Enumerations;
using Common.Exceptions;
using System;
using System.ServiceModel;
using Service.Services;

namespace Service.Services
{
    public class DroneService : IDroneService
    {
        private SessionStatusType sessionStatus;
        public ResponseData StartSession(SessionData meta)
        {
            try
            {
                ValidationService.ValidateSession(meta);
                sessionStatus = SessionStatusType.IN_PROGRESS;
                return new ResponseData("Session started successfully.", ResponseStatusType.ACK, sessionStatus);
            }
            catch (FaultException<DataFormatFault> exDff)
            {
                Console.WriteLine($"Data format error: {exDff.Detail.Message}");
            }
            catch (FaultException<ValidationFault> exVf)
            {
                Console.WriteLine($"Validation error: {exVf.Detail.Message}");  
            }
            catch (Exception ex)
            { 
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
            sessionStatus = SessionStatusType.COMPLETED;
            return new ResponseData($"Failed to start session.", ResponseStatusType.NACK, sessionStatus);
        }

        public ResponseData PushSample(DroneSample sample)
        {
            try
            {
                ValidationService.ValidateSample(sample);
                return new ResponseData("Sample pushed successfully.", ResponseStatusType.ACK,sessionStatus);
            }
            catch(FaultException<DataFormatFault> exDff) 
            {
                Console.WriteLine($"Data format error: {exDff.Detail.Message}");
            }
            catch(FaultException<ValidationFault> exVf) 
            { 
                Console.WriteLine($"Validation error: {exVf.Detail.Message}");
            }
            catch (Exception ex)
            { 
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            return new ResponseData($"Failed to push sample", ResponseStatusType.NACK, sessionStatus);
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
