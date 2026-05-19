// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static class OperatingSystemExtensions
{
    extension(OperatingSystem)
    {
        /// <summary>
        ///  Checks whether the Windows version (returned by <see cref="Environment.OSVersion"/>) is greater than
        ///  or equal to the specified version. This method can be used to guard APIs that were added in the
        ///  specified Windows version.
        /// </summary>
        /// <param name="major">The major OS version number.</param>
        /// <param name="minor">The minor OS version number.</param>
        /// <param name="build">The OS build number.</param>
        /// <param name="revision">The OS revision number.</param>
        /// <returns>
        ///  <see langword="true"/> if the current OS version is greater than or equal to the specified version;
        ///  otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsWindowsVersionAtLeast(int major, int minor = 0, int build = 0, int revision = 0)
        {
            Version current = Environment.OSVersion.Version;

            if (current.Major != major)
            {
                return current.Major > major;
            }

            if (current.Minor != minor)
            {
                return current.Minor > minor;
            }

            // Unspecified build component is to be treated as zero
            int currentBuild = current.Build < 0 ? 0 : current.Build;
            build = build < 0 ? 0 : build;
            if (currentBuild != build)
            {
                return currentBuild > build;
            }

            // Unspecified revision component is to be treated as zero
            int currentRevision = current.Revision < 0 ? 0 : current.Revision;
            revision = revision < 0 ? 0 : revision;

            return currentRevision >= revision;
        }
    }
}
