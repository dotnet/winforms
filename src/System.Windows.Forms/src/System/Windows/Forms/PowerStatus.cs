// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public class PowerStatus
    {
        private NativeMethods.SYSTEM_POWER_STATUS systemPowerStatus;

        internal PowerStatus()
        {
        }

        public PowerLineStatus PowerLineStatus
        {
            get
            {
                UpdateSystemPowerStatus();
                return (PowerLineStatus)systemPowerStatus.ACLineStatus;
            }
        }

        public BatteryChargeStatus BatteryChargeStatus
        {
            get
            {
                UpdateSystemPowerStatus();
                return (BatteryChargeStatus)systemPowerStatus.BatteryFlag;
            }
        }

        public int BatteryFullLifetime
        {
            get
            {
                UpdateSystemPowerStatus();
                return systemPowerStatus.BatteryFullLifeTime;
            }
        }

        public float BatteryLifePercent
        {
            get
            {
                UpdateSystemPowerStatus();
                float lifePercent = systemPowerStatus.BatteryLifePercent / 100f;
                return lifePercent > 1f ? 1f : lifePercent;
            }
        }

        public int BatteryLifeRemaining
        {
            get
            {
                UpdateSystemPowerStatus();
                return systemPowerStatus.BatteryLifeTime;
            }
        }

        private void UpdateSystemPowerStatus()
        {
            UnsafeNativeMethods.GetSystemPowerStatus(ref systemPowerStatus);
        }
    }
}
