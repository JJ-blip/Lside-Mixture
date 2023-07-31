namespace Lside_Mixture.Utils
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Serilog;

    public class TaskUtil
    {
        /// <summary>
        /// Blocks until condition is true or timeout occurs.
        /// </summary>
        /// <param name="condition">The break condition.</param>
        /// <param name="frequency">The frequency at which the condition will be checked.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <param name="minDelaymsec">minimum CYCLE delay in msec</param>
        /// <returns>How long it waited in seconds</returns>
        public int WaitUntil(double timeSpan, ConditionCallback condition, int minDelaymsec = 500)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            bool _waitUntilTrue = false;
            System.Timers.Timer timer = new System.Timers.Timer(timeSpan);
            timer.Elapsed += delegate { _waitUntilTrue = true; };
            timer.AutoReset = false;
            timer.Start();

            while (!(_waitUntilTrue || condition()))
            {
                Thread.Sleep(minDelaymsec);
            }
            stopwatch.Stop();

            return Convert.ToInt32(stopwatch.Elapsed.TotalSeconds);
        }

        public delegate bool ConditionCallback();
    }
}
