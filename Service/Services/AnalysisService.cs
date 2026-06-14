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
        public delegate void WindHandler(object sender, WindArgument e);

        private double anorm = 0;
        private double previousAnorm = 0;
        private double asum = 0;
        private double amount = 0;
        private double aMean = 0;
        private double windEffect = 0;

        private double aThreshold;
        private double wThreshold;
        private double dThreshold;

        public event AccelerationHandler AccelerationSpike;
        public event DeviationHandler OutOfBandWarning;
        public event WindHandler WindSpike;


        public AnalysisService() 
        {

            aThreshold = double.Parse(ConfigurationManager.AppSettings["A_Threshold"],CultureInfo.InvariantCulture);
            wThreshold = double.Parse(ConfigurationManager.AppSettings["W_Threshold"], CultureInfo.InvariantCulture);
            dThreshold = double.Parse(ConfigurationManager.AppSettings["D_Threshold"], CultureInfo.InvariantCulture);
        }

        public void AnalyzeSample(DroneSample sample)
        {
            amount++;
            AnalyzeAcceleration(sample.LinearAccelerationX, sample.LinearAccelerationY, sample.LinearAccelerationZ);
            AnalyzeDeviation(asum, amount);
            AnalyzeWind(sample.WindSpeed, sample.WindAngle);
        }

        private void AnalyzeAcceleration(double linearAccelerationX, double linearAccelerationY, double linearAccelerationZ)
        {
            previousAnorm = anorm;
            anorm = Math.Sqrt(Math.Pow(linearAccelerationX, 2) + Math.Pow(linearAccelerationY, 2) + Math.Pow(linearAccelerationZ, 2));
            if (amount > 1)
            {
                asum += anorm;
                double aDifference = anorm - previousAnorm;

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
            }
        }

        private void AnalyzeDeviation(double sum, double count)
        {
            if (count > 1)
            {
                aMean = sum / count;
                DeviationArgument e = new DeviationArgument()
                {
                    Message = "Out of band warrning detected.",
                    AnalysisStatus = AnalysisStatusType.ABOVE_THRESHOLD,
                    Anorm = anorm,
                    Amean = aMean
                };
                if (anorm > (aMean * (1 + dThreshold)))
                {
                    OutOfBandWarning?.Invoke(this, e);
                }
                else if (anorm < (aMean * (1 - dThreshold)))
                {
                    e.AnalysisStatus = AnalysisStatusType.BELOW_THRESHOLD;
                    OutOfBandWarning?.Invoke(this, e);
                }
            }
        }

        private void AnalyzeWind(double windSpeed, double windAngle)
        {
            double wind = windSpeed * Math.Sin(windAngle);
            windEffect = Math.Abs(wind);
            if (windEffect > wThreshold)
            {
                WindArgument e = new WindArgument()
                {
                    Message = "Wind spike detected.",
                    AnalysisStatus = AnalysisStatusType.ABOVE_THRESHOLD,
                    WindEffect = windEffect
                };
                if (wind > 0)
                {
                    WindSpike?.Invoke(this, e);
                }
                else
                {
                    e.AnalysisStatus = AnalysisStatusType.BELOW_THRESHOLD;
                    WindSpike?.Invoke(this, e);
                }
            }
        }
    }
}
