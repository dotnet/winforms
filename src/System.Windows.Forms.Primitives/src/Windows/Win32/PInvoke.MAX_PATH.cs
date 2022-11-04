// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        private const string WINDOWS_FILE_SYSTEM_REGISTRY_KEY = @"SYSTEM\CurrentControlSet\Control\FileSystem";
        private const string WINDOWS_LONG_PATHS_ENABLED_VALUE_NAME = "LongPathsEnabled";

        /// <summary>
        /// Cached value for MaxPath.
        /// </summary>
        private static int _maxPath;
        private static bool IsMaxPathSet { get; set; }
        private static readonly object MaxPathLock = new object();
        internal static bool HasMaxPath => MaxPath == MAX_PATH;
        public const int MAX_PATH = 260;

        /// <summary>
        /// Gets the windows max path limit.
        /// </summary>
        internal static int MaxPath
        {
            get
            {
                if (!IsMaxPathSet)
                {
                    SetMaxPath();
                }

                return _maxPath;
            }
        }

        private static void SetMaxPath()
        {
            lock (MaxPathLock)
            {
                if (!IsMaxPathSet)
                {
                    bool isMaxPathRestricted = IsLongPathsEnabledRegistry();
                    _maxPath = isMaxPathRestricted ? MAX_PATH : int.MaxValue;
                    IsMaxPathSet = true;
                }
            }
        }

        private static bool IsLongPathsEnabledRegistry()
        {
            using (RegistryKey? fileSystemKey = Registry.LocalMachine.OpenSubKey(WINDOWS_FILE_SYSTEM_REGISTRY_KEY))
            {
                if (fileSystemKey is null)
                {
                    return false;
                }

                object? longPathsEnabledValue = fileSystemKey?.GetValue(WINDOWS_LONG_PATHS_ENABLED_VALUE_NAME, 0);
                return longPathsEnabledValue is not null && Convert.ToInt32(longPathsEnabledValue) == 1;
            }
        }
    }
}
