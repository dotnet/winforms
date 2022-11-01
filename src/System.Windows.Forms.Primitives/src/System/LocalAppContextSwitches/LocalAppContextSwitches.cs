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
        // Switch names declared internal below are used in unit/integration tests. Refer to
        // https://github.com/microsoft/winforms/blob/tree/main/docs/design/anchor_layout_changes_in_net80.md
        // for more details on how to enable these switches in the application.
        private const string ScaleTopLevelFormMinMaxSizeForDpiSwitchName = "System.Windows.Forms.ScaleTopLevelFormMinMaxSizeForDpi";
        internal const string AnchorLayoutV2SwitchName = "System.Windows.Forms.AnchorLayoutV2";

        private static int s_scaleTopLevelFormMinMaxSizeForDpi;
        private static int s_AnchorLayoutV2;

        public static bool ScaleTopLevelFormMinMaxSizeForDpi
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetCachedSwitchValue(ScaleTopLevelFormMinMaxSizeForDpiSwitchName, ref s_scaleTopLevelFormMinMaxSizeForDpi);
        }

        /// <summary>
        ///  Indicates whether AnchorLayoutV2 feature is enabled.
        /// </summary>
        /// <devdoc>
        ///  Returns AnchorLayoutV2 switch value from runtimeconfig.json. Defaults to true if application is targeting .NET 8.0 and beyond.
        ///  Refer to
        ///  https://github.com/microsoft/winforms/blob/tree/main/docs/design/anchor_layout_changes_in_net80.md for more details.
        /// </devdoc>
        public static bool AnchorLayoutV2
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetCachedSwitchValue(AnchorLayoutV2SwitchName, ref s_AnchorLayoutV2);
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
            {
                return false;
            }

            if (cachedSwitchValue > 0)
            {
                return true;
            }

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

                // We are introducing switch defaults in .NET 8.0+ and support matrix for this product is
                // limited to Windows 10 and above versions.
                if (OsVersion.IsWindows10_1703OrGreater())
                {
                    if (s_targetFrameworkName!.Version.CompareTo(new Version("8.0")) >= 0)
                    {
                        if (switchName == ScaleTopLevelFormMinMaxSizeForDpiSwitchName)
                        {
                            return true;
                        }

                        if (switchName == AnchorLayoutV2SwitchName)
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
