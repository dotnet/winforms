// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="ACLineStatus"]/*' />
    /// <devdoc>
    ///    <para>
    ///       To be supplied.
    ///    </para>
    /// </devdoc>
    public enum PowerLineStatus
    {
        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerLineStatus.Offline"]/*' />
        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Offline = 0,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerLineStatus.Online"]/*' />
        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Online = 1,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerLineStatus.Unknown"]/*' />
        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Unknown = 255
    }

    /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="BatteryChargeStatus"]/*' />
    /// <devdoc>
    ///    <para>
    ///       To be supplied.
    ///    </para>
    /// </devdoc>
    [Flags]
    public enum BatteryChargeStatus
    {
        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="BatteryChargeStatus.High"]/*' />
        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        High = 1,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="BatteryChargeStatus.Low"]/*' />
        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Low = 2,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="BatteryChargeStatus.Critical"]/*' />
        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Critical = 4,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="BatteryChargeStatus.Charging"]/*' />
        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Charging = 8,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="BatteryChargeStatus.NoSystemBattery"]/*' />
        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        NoSystemBattery = 128,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="BatteryChargeStatus.Unknown"]/*' />
        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Unknown = 255
    }

    /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerState"]/*' />
    /// <devdoc>
    ///    <para>
    ///       To be supplied.
    ///    </para>
    /// </devdoc>
    public enum PowerState
    {
        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerState.Suspend"]/*' />
        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Suspend = 0,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerState.Hibernate"]/*' />
        /// <devdoc>
        ///     To be supplied.
        /// </devdoc>
        Hibernate = 1
    }

    /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerStatus"]/*' />
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
        
        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerStatus.ACLineStatus"]/*' />
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

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerStatus.BatteryChargeStatus"]/*' />
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

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerStatus.BatteryFullLifetime"]/*' />
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

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerStatus.BatteryLifePercent"]/*' />
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

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerStatus.BatteryLifeRemaining"]/*' />
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
