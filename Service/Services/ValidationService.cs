using Common;
using Common.Exceptions;
using System;
using System.Linq;
using System.ServiceModel;

namespace Service.Services
{
    public static class ValidationService
    {
        private static readonly double[] maxLinearAccelerations = { 1500, 1500, 1500 };
        private static readonly string[] validSampleHeader = { "LinearAccelerationX", "LinearAccelerationY", "LinearAccelerationZ", "WindSpeed", "WindAngle", "Time" };

        public static void ValidateSession(SessionData meta)
        {
            if (meta == null)
            {
                throw new FaultException<DataFormatFault>(new DataFormatFault("Session metadata cannot be null!"));
            }
            if (meta.SampleHeader == null || meta.SampleHeader.Length == 0)
            {
                throw new FaultException<DataFormatFault>(new DataFormatFault("Sample header cannot be null or empty!"));
            }
            if (!meta.SampleHeader.SequenceEqual(validSampleHeader))
            {
                throw new FaultException<ValidationFault>(new ValidationFault("Sample header is not valid!"));
            }
        }


        public static void ValidateSample(DroneSample sample)
        {
            if (sample == null)
            {
                throw new FaultException<DataFormatFault>(new DataFormatFault("Drone sample cannot be null!"));
            }

            if (Math.Abs(sample.LinearAccelerationX) > maxLinearAccelerations[0])
            {
                throw new FaultException<ValidationFault>(new ValidationFault("Linear acceleration X value exceeds the maximum limit!"));
            }
            if (Math.Abs(sample.LinearAccelerationY) > maxLinearAccelerations[1])
            {
                throw new FaultException<ValidationFault>(new ValidationFault("Linear acceleration Y value exceeds the maximum limit!"));
            }
            if (Math.Abs(sample.LinearAccelerationZ) > maxLinearAccelerations[2])
            {
                throw new FaultException<ValidationFault>(new ValidationFault("Linear acceleration Z value exceeds the maximum limit!"));
            }

            if (sample.WindSpeed <= 0)
            {
                throw new FaultException<ValidationFault>(new ValidationFault("Wind speed value must be above 0!"));
            }

            if(sample.WindAngle < 0 || sample.WindAngle > 360)
            {
                throw new FaultException<ValidationFault>(new ValidationFault("Wind angle value must be between 0 and 360!"));
            }

            if (sample.Time <= 0)
            {
                throw new FaultException<ValidationFault>(new ValidationFault("Drone flight time is not within valid range!"));
            }
        }
    }
}
