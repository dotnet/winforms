// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Windows.Forms.Primitives
{
    // Barrowed from https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/LocalAppContextSwitches.Common.cs
    internal static partial class LocalAppContextSwitches
    {
        private static int s_scaleTopLevelFormMinMaxSize;
        public static bool ScaleTopLevelFormMinMaxSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetCachedSwitchValue("Switch.System.Windows.Forms.ScaleTopLevelFormMinMaxSizeForDpi", ref s_scaleTopLevelFormMinMaxSize);
        }

        private static bool GetCachedSwitchValue(string switchName, ref int cachedSwitchValue)
        {
            // The cached switch value has 3 states: 0 - unknown, 1 - true, -1 - false
            if (cachedSwitchValue < 0)
                return false;
            if (cachedSwitchValue > 0)
                return true;

            return GetCachedSwitchValueInternal(switchName, ref cachedSwitchValue);
        }

        private static bool GetCachedSwitchValueInternal(string switchName, ref int cachedSwitchValue)
        {
            bool hasSwitch = AppContext.TryGetSwitch(switchName, out bool isSwitchEnabled);
            if (!hasSwitch)
            {
                isSwitchEnabled = GetSwitchDefaultValue(switchName);
            }

            // Is caching switches disabled?.
            AppContext.TryGetSwitch("TestSwitch.LocalAppContext.DisableCaching", out bool disableCaching);
            if (!disableCaching)
            {
                cachedSwitchValue = isSwitchEnabled ? 1 /*true*/ : -1 /*false*/;
            }

            return isSwitchEnabled;
        }

        // Provides default values for switches based on OS and Targetframework versions.
        private static bool GetSwitchDefaultValue(string switchName)
        {
            if (OsVersion.IsWindows10_1703OrGreater)
            {
                var tfm = new TargetFramework(AppContext.TargetFrameworkName);
                if (tfm.Name == ".NETCoreApp" && string.Compare(tfm.Version, "v8.0", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (switchName == "Switch.System.Windows.Forms.ScaleTopLevelFormMinMaxSizeForDpi")
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
