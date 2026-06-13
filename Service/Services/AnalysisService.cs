using Common.Contracts;
using Common.Enumerations;
using Service.Events.Arguments;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AnalysisService
    {
        public delegate void AccelerationHandler(object sender, AccelerationArgument e);
        public delegate void DeviationHandler(object sender, DeviationArgument e);

        private double anorm = 0;
        private double previousAnorm = 0;
        private double asum = 0;
        private double amount = 0;

        private double aThreshold;
        private double wThreshold;
        private double dThreshold;

        public event AccelerationHandler AccelerationSpike;
        public event DeviationHandler OutOfBandWarning;


        public AnalysisService() 
        {

            aThreshold = double.Parse(ConfigurationManager.AppSettings["A_Threshold"],CultureInfo.InvariantCulture);
            wThreshold = double.Parse(ConfigurationManager.AppSettings["W_Threshold"], CultureInfo.InvariantCulture);
            dThreshold = double.Parse(ConfigurationManager.AppSettings["D_Threshold"], CultureInfo.InvariantCulture);
        }

        public void AnalyzeSample(DroneSample sample)
        {
            double aDifference;
            double aMean;

            amount++;
            previousAnorm = anorm;
            anorm = Math.Sqrt(Math.Pow(sample.LinearAccelerationX, 2) + Math.Pow(sample.LinearAccelerationY, 2) + Math.Pow(sample.LinearAccelerationZ, 2));
            asum += anorm;
            if (amount > 1)
            {
                aMean = asum / amount;
                aDifference = anorm - previousAnorm;

                if (Math.Abs(aDifference) > aThreshold)
                {
                    AccelerationArgument e = new AccelerationArgument()
                    {
                        Message = "Acceleration spike detected.",
                        AnalysisStatus = AnalysisStatusType.ABOVE_THRESHOLD,
                        Anorm = anorm,
                        Difference = aDifference,
                        Aprevious = previousAnorm
                    };

                    if (aDifference > 0)
                    {
                        AccelerationSpike?.Invoke(this, e);
                    }
                    else
                    {
                        e.AnalysisStatus = AnalysisStatusType.BELOW_THRESHOLD;
                        AccelerationSpike?.Invoke(this, e);
                    }
                }

                if (anorm > (aMean * (1 + dThreshold)))
                {
                    OutOfBandWarning?.Invoke(this, new DeviationArgument { Message = "Out of band warrning detected.", AnalysisStatus = AnalysisStatusType.ABOVE_THRESHOLD, Anorm = anorm, Amean = aMean });
                }
                else if (anorm < (aMean * (1 - dThreshold)))
                {
                    OutOfBandWarning?.Invoke(this, new DeviationArgument { Message = "Out of band warrning detected.", AnalysisStatus = AnalysisStatusType.BELOW_THRESHOLD, Anorm = anorm, Amean = aMean });
                }
            }

        }


    }
}
