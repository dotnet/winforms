// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="ACLineStatus"]/*' />
    public enum PowerLineStatus
    {
        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerLineStatus.Offline"]/*' />
        Offline = 0,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerLineStatus.Online"]/*' />
        Online = 1,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerLineStatus.Unknown"]/*' />
        Unknown = 255
    }

    /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="BatteryChargeStatus"]/*' />
    [Flags]
    public enum BatteryChargeStatus
    {
        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="BatteryChargeStatus.High"]/*' />
        High = 1,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="BatteryChargeStatus.Low"]/*' />
        Low = 2,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="BatteryChargeStatus.Critical"]/*' />
        Critical = 4,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="BatteryChargeStatus.Charging"]/*' />
        Charging = 8,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="BatteryChargeStatus.NoSystemBattery"]/*' />
        NoSystemBattery = 128,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="BatteryChargeStatus.Unknown"]/*' />
        Unknown = 255
    }

    /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerState"]/*' />
    public enum PowerState
    {
        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerState.Suspend"]/*' />
        Suspend = 0,

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerState.Hibernate"]/*' />
        Hibernate = 1
    }

    /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerStatus"]/*' />
    public class PowerStatus
    {
        private NativeMethods.SYSTEM_POWER_STATUS systemPowerStatus;

        internal PowerStatus() {
        }
        
        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerStatus.ACLineStatus"]/*' />
        public PowerLineStatus PowerLineStatus
        {
            get
            {
                UpdateSystemPowerStatus();
                return (PowerLineStatus)systemPowerStatus.ACLineStatus;
            }
        }

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerStatus.BatteryChargeStatus"]/*' />
        public BatteryChargeStatus BatteryChargeStatus
        {
            get
            {
                UpdateSystemPowerStatus();
                return (BatteryChargeStatus)systemPowerStatus.BatteryFlag;
            }
        }

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerStatus.BatteryFullLifetime"]/*' />
        public int BatteryFullLifetime
        {
            get
            {
                UpdateSystemPowerStatus();
                return systemPowerStatus.BatteryFullLifeTime;
            }
        }

        /// <include file='doc\PowerStatus.uex' path='docs/doc[@for="PowerStatus.BatteryLifePercent"]/*' />
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
