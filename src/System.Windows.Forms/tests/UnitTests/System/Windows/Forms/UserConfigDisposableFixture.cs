// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Configuration;

namespace System.Windows.Forms.Tests;

public class UserConfigDisposableFixture : IDisposable
{
    public UserConfigDisposableFixture()
    {
        DeleteUserConfig();
    }

    public void Dispose()
    {
        DeleteUserConfig();
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
