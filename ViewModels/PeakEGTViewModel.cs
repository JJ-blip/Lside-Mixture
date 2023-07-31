namespace Lside_Mixture.ViewModels
{
    using System;
    using System.Threading;
    using Lside_Mixture.Services;
    using Lside_Mixture.Utils;
    using Lside_Mixture.Views;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    public class PeakEGTViewModel : BindableBase
    {
        private readonly ISimService simservice = App.Current.Services.GetService<ISimService>();

        private readonly int INITIAL_STEP_PERCENTAGE = -10;
        private readonly int DELAY_MSEC = 10000;
        private readonly double CONVERGENCE_EPSILON = 2.0;

        public PeakEGTViewModel(PeakEGTWindow peakEGTWindow)
        {
            this.PeakEGTWindow = peakEGTWindow;

           var peak = this.PeakEGT();

            Log.Information(String.Format("Peak at {0:0} F", peak));
        }

        public PeakEGTWindow PeakEGTWindow { get; }


        private double PeakEGT()
        {
            var samples = new BoundedQueue<(double temp, double mixture)>(3);

            var nowEGT = simservice.MostRecentSample.EGT;
            var nowMixture = simservice.MostRecentSample.Mixture;
            samples.Enqueue((nowEGT, nowMixture));

            /* Preload samples with 2 values to enable SA to make decesion about next
             */

            // 1st: lean by step%
            int step = INITIAL_STEP_PERCENTAGE;
            var nextMixture = nowMixture + step;
            simservice.SetMixture(nextMixture);
            var nextEGT = NextEGT();
            samples.Enqueue((nextEGT, nextMixture));

            // 2nd: lean by step% once more 
            nextMixture = samples.ElementAt(1).mixture + step;
            simservice.SetMixture(nextMixture);
            nextEGT = NextEGT();
            samples.Enqueue((nextEGT, nextMixture));

            {
                // t0 is the 1st change in EGT arising from a m0 change in mixture
                var t0 = samples.ElementAt(1).temp - samples.ElementAt(0).temp;
                var m0 = samples.ElementAt(1).mixture - samples.ElementAt(0).mixture;
                Log.Information(String.Format("1st Mixture change {0:####.#}% (to {3,5:0.0}%) temperature change {1:##0.0} F (to {2:####} F)", m0, t0, samples.ElementAt(1).temp, samples.ElementAt(1).mixture));

                // t1 is the next change in EGT arising from a m1 change in mixture
                var t1 = samples.ElementAt(2).temp - samples.ElementAt(1).temp;
                var m1 = samples.ElementAt(2).mixture - samples.ElementAt(1).mixture;
                Log.Information(String.Format("Next Mixture change {0:####.#}% (to {3,5:0.0}%) temperature change {1:##0.0} F (to {2:####} F)", m1, t1, samples.ElementAt(2).temp, samples.ElementAt(2).mixture));
            }

            // define the convergence threshold
            double epsilon = CONVERGENCE_EPSILON;

            return PeakEGTSucessiveApproxAlgorithm(samples, epsilon, step);
        }

        private double PeakEGTSucessiveApproxAlgorithm(BoundedQueue<(double temp, double mixture)> samples, double epsilon, double step)
        {
            for (int i = 0; i < 15; i++)
            {
                var diff = difference(samples);
                if (-epsilon < diff && diff < epsilon)
                {
                    Log.Information(String.Format("Convergence at {0,4:00.0} F at {1:##} %", samples.ElementAt(2).temp, samples.ElementAt(2).mixture));
                    return extractResult(samples);
                }
                else
                {
                    (step, samples) = EstimateEGT(samples, step);
                }
            }

            Log.Information(String.Format("Exceeded iteration count at {0,4:00.0} F at {1:##} %", samples.ElementAt(2).temp, samples.ElementAt(2).mixture));
            return extractResult(samples);
        }

        private (double step, BoundedQueue<(double temp, double mixture)>) EstimateEGT(BoundedQueue<(double temp, double mixture)> samples, double step)
        {
            // work with last 3 samples.

            // t1 is the most recent change in EGT arising from a m1 change in mixture 
            var t1 = samples.ElementAt(2).temp - samples.ElementAt(1).temp;
            // var m1 = samples.ElementAt(1).mixture - samples.ElementAt(0).mixture;


            // t0 is the earlier recent change in EGT arising from a m0 change in mixture
            var t0 = samples.ElementAt(1).temp - samples.ElementAt(0).temp;
            // var m0 = samples.ElementAt(1).mixture - samples.ElementAt(0).mixture;


            string change = string.Empty;

            double nextEGT, nextMixture; //, nextStep;
            if (t1 >0)
            {
                // most recent change rising - good
                if (t1 > t0)
                { 
                    // rising faster & thus moving in right direction
                  //  nextStep = - Math.Abs(samples.ElementAt(2).mixture - samples.ElementAt(1).mixture) * 1.0;
                    change = String.Format ("Rising, but faster {0:00.0}", t1 - t0);
                }
                else
                {
                    // rising slower, convergence, slow down mixture updates
                  //  nextStep = - (Math.Abs((samples.ElementAt(2).mixture - samples.ElementAt(1).mixture))) * 0.5;
                    change = String.Format("Rising, but slower {0:00.0}", t1 - t0);
                }

                // using the computed step
                // step = nextStep;
                nextMixture = samples.ElementAt(2).mixture + step;
                simservice.SetMixture(nextMixture);
                nextEGT = NextEGT();
                samples.Enqueue((nextEGT, nextMixture));              
            }
            else
            {
                // falling, not good
                // step back 2 samples & half step size
                step = Math.Abs(samples.ElementAt(1).mixture - samples.ElementAt(0).mixture) * 0.5;
                nextMixture = samples.ElementAt(0).mixture;
                simservice.SetMixture(nextMixture);
                nextEGT = NextEGT();
                samples.Enqueue((nextEGT, nextMixture));
            }

            {
                // tn is the next change in EGT arising from a m1 change in mixture
                var tn = samples.ElementAt(2).temp - samples.ElementAt(1).temp;
                var mn = samples.ElementAt(2).mixture - samples.ElementAt(1).mixture;
                Log.Information(String.Format("{4,25}, Next Mixture change {0,4:00.0}% (to {3,5:0.0}%) temperature change {1:##0.0} F (to {2:####} F)", mn, tn, samples.ElementAt(2).temp, samples.ElementAt(2).mixture, change));
            }

            return (step, samples);
        }

        private double extractResult(BoundedQueue<(double temp, double mixture)> samples)
        {
            return samples.ElementAt(2).temp;
        }

        private double difference(BoundedQueue<(double temp, double mixture)> samples)
        {
            return samples.ElementAt(2).temp - samples.ElementAt(1).temp;
        }

        private double NextEGT(double last = 0)
        {
            // need better algorithim that a delay
            Thread.Sleep(DELAY_MSEC);
            var nextEGT = simservice.MostRecentSample.EGT;
            return nextEGT;
        }
    }
}
