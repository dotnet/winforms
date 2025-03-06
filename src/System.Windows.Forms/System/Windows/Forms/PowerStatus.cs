// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Power;

namespace System.Windows.Forms;

public class PowerStatus
{
    private SYSTEM_POWER_STATUS _systemPowerStatus;

    internal PowerStatus()
    {
    }

    public PowerLineStatus PowerLineStatus
    {
        get
        {
            UpdateSystemPowerStatus();
            return (PowerLineStatus)_systemPowerStatus.ACLineStatus;
        }
    }

    public BatteryChargeStatus BatteryChargeStatus
    {
        get
        {
            UpdateSystemPowerStatus();
            return (BatteryChargeStatus)_systemPowerStatus.BatteryFlag;
        }
    }

    public int BatteryFullLifetime
    {
        get
        {
            UpdateSystemPowerStatus();
            return (int)_systemPowerStatus.BatteryFullLifeTime;
        }
    }

    public float BatteryLifePercent
    {
        get
        {
            UpdateSystemPowerStatus();
            float lifePercent = _systemPowerStatus.BatteryLifePercent / 100f;
            return lifePercent > 1f ? 1f : lifePercent;
        }
    }

    public int BatteryLifeRemaining
    {
        get
        {
            UpdateSystemPowerStatus();
            return (int)_systemPowerStatus.BatteryLifeTime;
        }
    }

    private void UpdateSystemPowerStatus() => PInvoke.GetSystemPowerStatus(out _systemPowerStatus);
}
