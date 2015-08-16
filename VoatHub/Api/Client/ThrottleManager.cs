using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace VoatHub.Api.Client
{
    /// <summary>
    /// Simple manager for api throttling. Very relaxed throttle.
    /// <para>TODO: More completed throttle policy to allow burst calls</para>
    /// </summary>
    public class ThrottleManager
    {
        private static readonly TimeSpan DEFAULT_RATE_LIMIT = TimeSpan.FromSeconds(3);

        private bool makingCall;
        private DateTime lastCall;
        private TimeSpan rateLimit;
        private ApplicationDataContainer roamingSettings;
        private string lastCallStorageKey;

        /// <summary>
        /// Creates a <see cref="ThrottleManager"/> with rate limit equal to <see cref="DEFAULT_RATE_LIMIT"/>
        /// that allows call to be made immediately.
        /// </summary>
        public ThrottleManager(string clientName) : this(clientName, DEFAULT_RATE_LIMIT) { }

        /// <summary>
        /// Creates a <see cref="ThrottleManager"/> with the given rate limit that
        /// allows call to be made immediately.
        /// </summary>
        /// <param name="rateLimit"></param>
        public ThrottleManager(string clientName, TimeSpan rateLimit)
        {
            this.rateLimit = rateLimit;
            roamingSettings = ApplicationData.Current.RoamingSettings;
            lastCallStorageKey = clientName + "lastCall";

            var savedLastCall = roamingSettings.Values[lastCallStorageKey];

            if (savedLastCall != null)
            {
                lastCall = DateTime.FromBinary((long)savedLastCall);
            }
            else
            {
                lastCall = new DateTime();
            }
        }

        ~ThrottleManager()
        {
            roamingSettings.Values[lastCallStorageKey] = lastCall.ToBinary();
        }

        /// <summary>
        /// Wait until throttle rate is cleared.
        /// </summary>
        public async Task Wait()
        {
            while (makingCall)
            {
                await Task.Delay(100);
            }

            makingCall = true;

            var dt = DateTime.Now.Subtract(lastCall);
            var ddt = rateLimit - dt;

            if (ddt > TimeSpan.Zero)  await Task.Delay(ddt);
        }

        public void MadeCall()
        {
            makingCall = false;
            lastCall = DateTime.Now;
        }
    }
}
