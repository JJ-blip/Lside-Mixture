namespace Lside_Mixture.Utils
{
    using System;
    using System.Threading;

    public class TaskUtil
    {
        /// <summary>
        /// Blocks until condition is true or timeout occurs.
        /// </summary>
        /// <param name="condition">The break condition.</param>
        /// <param name="frequency">The frequency at which the condition will be checked.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <returns></returns>
        public void WaitUntil(double timeSpan, ConditionCallback condition)
        {
            bool _waitUntilTrue = false;
            System.Timers.Timer timer = new System.Timers.Timer(timeSpan);
            timer.Elapsed += delegate { _waitUntilTrue = true; };
            timer.AutoReset = false;
            timer.Start();

            while (!(_waitUntilTrue || condition()))
            {
                Thread.Sleep(500);
            }

            Console.WriteLine("Now True");
        }

        public delegate bool ConditionCallback();
    }
}
