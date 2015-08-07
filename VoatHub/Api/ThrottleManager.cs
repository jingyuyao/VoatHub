using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Api
{
    /// <summary>
    /// Simple manager for api throttling. Very relaxed throttle.
    /// </summary>
    public class ThrottleManager
    {
        // This rate limit covers all non burst calls.
        // TODO: More completed throttle policy to allow burst calls
        private static readonly TimeSpan RATE_LIMIT = TimeSpan.FromSeconds(3);

        private DateTime lastCall;

        public ThrottleManager()
        {
            // Allow a call to be made immediately.
            // Not really the best way to go about this but generally
            // an user should be able to close and open the application within
            // the RATE_LIMIT so any additional logic is not necessary.
            lastCall = DateTime.Now.Subtract(RATE_LIMIT);
        }

        public ThrottleManager(DateTime lastCall)
        {
            this.lastCall = lastCall;
        }

        /// <summary>
        /// Return the <see cref="TimeSpan"/> a call has to wait.
        /// <para>Greater or equal to <see cref="TimeSpan.Zero"/> only.</para>
        /// </summary>
        public TimeSpan WaitTime
        {
            get
            {
                var dt = DateTime.Now.Subtract(lastCall);
                var ddt = RATE_LIMIT - dt;

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
