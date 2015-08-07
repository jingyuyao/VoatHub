using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Api
{
    /// <summary>
    /// Simple manager for api throttling. Very relaxed throttle.
    /// <para>TODO: More completed throttle policy to allow burst calls</para>
    /// </summary>
    public class ThrottleManager
    {
        private DateTime lastCall;

        public ThrottleManager()
        {
            RateLimit = TimeSpan.FromSeconds(3);

            // Allow a call to be made immediately.
            // Not really the best way to go about this but generally
            // an user should be able to close and open the application within
            // the RATE_LIMIT so any additional logic is not necessary.
            lastCall = DateTime.Now.Subtract(RateLimit);
        }

        public ThrottleManager(DateTime lastCall)
        {
            this.lastCall = lastCall;
        }

        public TimeSpan RateLimit { get; set; }

        /// <summary>
        /// Return the <see cref="TimeSpan"/> a call has to wait.
        /// <para>Greater or equal to <see cref="TimeSpan.Zero"/> only.</para>
        /// </summary>
        public TimeSpan WaitTime
        {
            get
            {
                var dt = DateTime.Now.Subtract(lastCall);
                var ddt = RateLimit - dt;

                if (ddt > TimeSpan.Zero) return ddt;
                else return TimeSpan.Zero;
            }
        }

        public void MadeCall()
        {
            lastCall = DateTime.Now;
        }
    }
}
