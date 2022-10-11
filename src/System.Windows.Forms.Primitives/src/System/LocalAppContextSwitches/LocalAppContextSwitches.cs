// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace System.Windows.Forms.Primitives
{
    // Borrowed from https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/LocalAppContextSwitches.Common.cs
    internal static partial class LocalAppContextSwitches
    {
        private const string ScaleTopLevelFormMinMaxSizeForDpiSwitchName = "System.Windows.Forms.ScaleTopLevelFormMinMaxSizeForDpi";
        internal const string EnableImprovedAnchorLayoutSwitchName = "System.Windows.Forms.EnableImprovedAnchorLayout";

        private static int s_scaleTopLevelFormMinMaxSizeForDpi;
        private static int s_enableImprovedAnchorLayout;

        public static bool ScaleTopLevelFormMinMaxSizeForDpi
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetCachedSwitchValue(ScaleTopLevelFormMinMaxSizeForDpiSwitchName, ref s_scaleTopLevelFormMinMaxSizeForDpi);
        }

        public static bool EnableImprovedAnchorLayout
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetCachedSwitchValue(EnableImprovedAnchorLayoutSwitchName, ref s_enableImprovedAnchorLayout);
        }

        private static readonly FrameworkName? s_targetFrameworkName = GetTargetFrameworkName();

        private static readonly bool s_isNetCoreApp = (s_targetFrameworkName?.Identifier) == ".NETCoreApp";

        private static FrameworkName? GetTargetFrameworkName()
        {
            string? targetFrameworkName = AppContext.TargetFrameworkName;
            return targetFrameworkName is null ? null : new FrameworkName(targetFrameworkName);
        }

        private static bool GetCachedSwitchValue(string switchName, ref int cachedSwitchValue)
        {
            // The cached switch value has 3 states: 0 - unknown, 1 - true, -1 - false
            if (cachedSwitchValue < 0)
                return false;
            if (cachedSwitchValue > 0)
                return true;

            return GetSwitchValue(switchName, ref cachedSwitchValue);
        }

        private static bool GetSwitchValue(string switchName, ref int cachedSwitchValue)
        {
            bool hasSwitch = AppContext.TryGetSwitch(switchName, out bool isSwitchEnabled);
            if (!hasSwitch)
            {
                isSwitchEnabled = GetSwitchDefaultValue(switchName);
            }

            // Is caching of the switches disabled?
            AppContext.TryGetSwitch("TestSwitch.LocalAppContext.DisableCaching", out bool disableCaching);
            if (!disableCaching)
            {
                cachedSwitchValue = isSwitchEnabled ? 1 /*true*/ : -1 /*false*/;
            }

            return isSwitchEnabled;

            static bool GetSwitchDefaultValue(string switchName)
            {
                if (!s_isNetCoreApp)
                {
                    return false;
                }

                if (OsVersion.IsWindows10_1703OrGreater())
                {
                    if (s_targetFrameworkName!.Version.CompareTo(new Version("8.0")) >= 0)
                    {
                        if (switchName == ScaleTopLevelFormMinMaxSizeForDpiSwitchName)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}
