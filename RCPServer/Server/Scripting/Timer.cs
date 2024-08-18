using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Generic timer class
    /// </summary>
    public class Timer
    {
        uint startTime = 0;
        uint interval = 0;
        bool enabled = false;
        bool autoReset = false;

        /// <summary>
        /// Callback invoked when Interval has elapsed
        /// </summary>
        public event global::Scripting.Forms.EventHandler Tick;

        /// <summary>
        /// Constructor
        /// </summary>
        public Timer()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interval"></param>
        public Timer(uint interval)
        {
            this.interval = interval;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="autoReset"></param>
        public Timer(uint interval, bool autoReset)
        {
            this.interval = interval;
            this.autoReset = autoReset;
        }

        /// <summary>
        /// Gets or sets the interval time in milliseconds.
        /// </summary>
        public uint Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        /// <summary>
        /// Gets or sets whether the timer is running.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    if (value)
                        Start();
                    else
                        Stop();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the timer will reset itself after it has ticked.
        /// </summary>
        public bool AutoReset
        {
            get { return autoReset; }
            set { autoReset = value; }
        }

        /// <summary>
        /// Start or restart this timer.
        /// </summary>
        public void Start()
        {
            if (enabled != true)
                ActiveTimers.AddLast(this);

            startTime = (uint)(DateTime.Now.Ticks / 10000); ;
            enabled = true;
        }

        /// <summary>
        /// Stop the timer.
        /// </summary>
        public void Stop()
        {
            if(enabled)
                ActiveTimers.Remove(this);
            enabled = false;
        }

        /// <summary>
        /// List of active timers
        /// </summary>
        protected static LinkedList<Timer> ActiveTimers = new LinkedList<Timer>();

        /// <summary>
        /// Internal update
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        protected bool Update(uint ticks)
        {
            if (enabled)
            {
                // Tick elapsed!
                if (startTime + interval < ticks)
                {
                    if (Tick != null)
                    {
                        enabled = false;
                        Tick.Invoke(this, new Scripting.Forms.FormEventArgs());
                    }
                    else // If the callback was free'd but we're still updating then the user didn't clean up
                    {
                        enabled = true;
                        Stop();
                        return true;
                    }

                    if (autoReset)
                    {
                        enabled = true;
                        startTime = ticks;
                        return false;
                    }
                    else
                    {
                        // User re-started it during callback
                        if (enabled)
                            return false;
                        enabled = true;
                        Stop();
                        return true;
                    }
                }
            }
            else
            {
                ActiveTimers.Remove(this);
                return true;
            }

            return true;
        }

        /// <summary>
        /// Update all script timers (Called from server application)
        /// </summary>
        public static void Update()
        {
            LinkedListNode<Timer> Node = ActiveTimers.First;
            LinkedListNode<Timer> Delete = null;
            uint Ticks = (uint)(DateTime.Now.Ticks / 10000);

            while (Node != null)
            {
                Delete = null;

                if (Node.Value.Update(Ticks))
                    Delete = Node;

                Node = Node.Next;
                //JB: Don't delete, the node does itself
            }
        }
    }
}
