// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Component, which sends a notification when it has not been triggered within a certain time.
    /// </summary>
    public class IdleSensor : Component
    {
        private const int TimerResolution = 25;
        private const int DefaultKeepAliveTime = 300;

        private readonly Timer _timer = new Timer()
        {
            Interval = TimerResolution,
            Enabled = true,
        };

        private int _keepAliveSpan = DefaultKeepAliveTime;

        public event CancelEventHandler Idle;

        public IdleSensor()
        {
            _timer.Tick += TimerEllapsed;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _timer.Tick -= TimerEllapsed;
            }
        }

        private void TimerEllapsed(object sender, EventArgs e)
        {
            _keepAliveSpan -= TimerResolution;

            if (_keepAliveSpan < 0)
            {
                // Prevent underflow in years...
                _keepAliveSpan += TimerResolution;

                var cancelEventArgs = new CancelEventArgs();
                Idle?.Invoke(this, cancelEventArgs);
            }
        }

        /// <summary>
        /// Keep-Alive trigger time in milliseconds.
        /// </summary>
        [DefaultValue(DefaultKeepAliveTime)]
        public int KeepAliveTime { get; set; } = DefaultKeepAliveTime;

        /// <summary>
        /// Starts the KeepAlive timer countdown from the beginning.
        /// </summary>
        public void KeepAlive()
        {
            _keepAliveSpan = KeepAliveTime;
        }

        public void Pause()
        {
            _timer.Enabled = false;
        }

        public void Resume()
        {
            _timer.Enabled = true;
        }
    }
}
