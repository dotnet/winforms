// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Configuration;

namespace System.Windows.Forms.Tests
{
    public class UserConfigDisposableFixture : ThreadExceptionFixture, IDisposable
    {
        public UserConfigDisposableFixture()
        {
            DeleteUserConfig();
        }

        public override void Dispose()
        {
            DeleteUserConfig();
            base.Dispose();
        }

        private static void DeleteUserConfig()
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            if (File.Exists(configuration.FilePath))
            {
                File.Delete(configuration.FilePath);
            }
        }
    }
}
