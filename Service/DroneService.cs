using Common;
using Common.Enumerations;
using Common.Exceptions;
using System;
using System.ServiceModel;
using Service.Services;

namespace Service
{
    public class DroneService : IDroneService
    {
        public ResponseData StartSession(SessionData meta)
        {
            try
            {
                ValidationService.ValidateSession(meta);
                return new ResponseData("Session started successfully.", ResponseStatusType.ACK, SessionStatusType.IN_PROGRESS);
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
            return new ResponseData($"Failed to start session.", ResponseStatusType.NACK, SessionStatusType.COMPLETED);
        }

        public ResponseData PushSample(DroneSample sample)
        {
            try
            {
                ValidationService.ValidateSample(sample);
                return new ResponseData("Sample pushed successfully.", ResponseStatusType.ACK, SessionStatusType.IN_PROGRESS);
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

            return new ResponseData($"Failed to push sample", ResponseStatusType.NACK, SessionStatusType.IN_PROGRESS);
        }

        public ResponseData EndSession()
        {
            try
            {
                return new ResponseData("Session ended successfully.", ResponseStatusType.ACK, SessionStatusType.COMPLETED);
            }
            catch (Exception ex)
            {
                return new ResponseData($"Failed to end session: {ex.Message}", ResponseStatusType.NACK, SessionStatusType.IN_PROGRESS);
            }
        }
    }
}
