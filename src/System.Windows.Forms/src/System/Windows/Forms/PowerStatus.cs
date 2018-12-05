// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    /// <devdoc>
    ///    <para>
    ///       To be supplied.
    ///    </para>
    /// </devdoc>
    public enum PowerLineStatus
    {
        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Offline = 0,

        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Online = 1,

        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Unknown = 255
    }

    /// <devdoc>
    ///    <para>
    ///       To be supplied.
    ///    </para>
    /// </devdoc>
    [Flags]
    public enum BatteryChargeStatus
    {
        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        High = 1,

        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Low = 2,

        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Critical = 4,

        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Charging = 8,

        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        NoSystemBattery = 128,

        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Unknown = 255
    }

    /// <devdoc>
    ///    <para>
    ///       To be supplied.
    ///    </para>
    /// </devdoc>
    public enum PowerState
    {
        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Suspend = 0,

        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Hibernate = 1
    }

    /// <devdoc>
    ///    <para>
    ///       To be supplied.
    ///    </para>
    /// </devdoc>
    public class PowerStatus
    {
        private NativeMethods.SYSTEM_POWER_STATUS systemPowerStatus;

        internal PowerStatus() {
        }
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public PowerLineStatus PowerLineStatus
        {
            get
            {
                UpdateSystemPowerStatus();
                return (PowerLineStatus)systemPowerStatus.ACLineStatus;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public BatteryChargeStatus BatteryChargeStatus
        {
            get
            {
                UpdateSystemPowerStatus();
                return (BatteryChargeStatus)systemPowerStatus.BatteryFlag;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int BatteryFullLifetime
        {
            get
            {
                UpdateSystemPowerStatus();
                return systemPowerStatus.BatteryFullLifeTime;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public float BatteryLifePercent
        {
            get
            {
                UpdateSystemPowerStatus();
                float lifePercent = systemPowerStatus.BatteryLifePercent / 100f;
                return lifePercent > 1f ? 1f : lifePercent;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int BatteryLifeRemaining
        {
            get
            {
                UpdateSystemPowerStatus();
                return systemPowerStatus.BatteryLifeTime;
            }
        }

        private void UpdateSystemPowerStatus() {                
            UnsafeNativeMethods.GetSystemPowerStatus(ref systemPowerStatus);
        }
    }
}
