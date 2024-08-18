using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Environment statistics class
    /// </summary>
    public class WorldEnvironment
    {
        /// <summary>
        /// 
        /// </summary>
        protected static int envMinute = 0;

        /// <summary>
        /// 
        /// </summary>
        protected static int envHour = 0;

        /// <summary>
        /// 
        /// </summary>
        protected static int envDay = 0;

        /// <summary>
        /// 
        /// </summary>
        protected static int envSeason = 0;

        /// <summary>
        /// 
        /// </summary>
        protected static int envTimeFactor = 0;

        /// <summary>
        /// Get the current minute.
        /// </summary>
        public static int Minute { get { return envMinute; } }

        /// <summary>
        /// Get the current hour.
        /// </summary>
        public static int Hour { get { return envHour; } }

        /// <summary>
        /// Get the current day.
        /// </summary>
        public static int Day { get { return envDay; } }

        /// <summary>
        /// Get the Time Factor.
        /// </summary>
        public static int TimeFactor { get { return envTimeFactor; } }
    }
}
