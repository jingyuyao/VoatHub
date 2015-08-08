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
        private static readonly TimeSpan DEFAULT_RATE_LIMIT = TimeSpan.FromSeconds(3);

        private DateTime lastCall;
        private TimeSpan rateLimit;

        /// <summary>
        /// Creates a <see cref="ThrottleManager"/> with rate limit equal to <see cref="DEFAULT_RATE_LIMIT"/>
        /// that allows call to be made immediately.
        /// </summary>
        public ThrottleManager() : this(DEFAULT_RATE_LIMIT) { }

        /// <summary>
        /// Creates a <see cref="ThrottleManager"/> with the given rate limit that
        /// allows call to be made immediately.
        /// </summary>
        /// <param name="rateLimit"></param>
        public ThrottleManager(TimeSpan rateLimit)
        {
            this.rateLimit = rateLimit;
            // Allow a call to be made immediately.
            lastCall = DateTime.Now.Subtract(rateLimit);
        }

        /// <summary>
        /// Creates a <see cref="ThrottleManager"/> with the given rate limit and
        /// date which the last call is made.
        /// </summary>
        /// <param name="rateLimit"></param>
        /// <param name="lastCall"></param>
        public ThrottleManager(TimeSpan rateLimit, DateTime lastCall)
        {
            this.rateLimit = rateLimit;
            this.lastCall = lastCall;
        }

        /// <summary>
        /// Wait until throttle rate is cleared.
        /// </summary>
        public async Task Wait()
        {
            var dt = DateTime.Now.Subtract(lastCall);
            var ddt = rateLimit - dt;

            if (ddt > TimeSpan.Zero)  await Task.Delay(ddt);
        }

        public void MadeCall()
        {
            lastCall = DateTime.Now;
        }
    }
}
