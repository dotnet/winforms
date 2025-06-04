// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.VisualBasic.Devices;

namespace Microsoft.VisualBasic.MyServices.Tests;

public class SpecialDirectoriesProxyTests
{
    [Fact]
    public void Properties()
    {
        SpecialDirectoriesProxy specialDirectories = new ServerComputer().FileSystem.SpecialDirectories;
        VerifySpecialDirectory(() => FileIO.SpecialDirectories.AllUsersApplicationData, () => specialDirectories.AllUsersApplicationData);
        VerifySpecialDirectory(() => FileIO.SpecialDirectories.CurrentUserApplicationData, () => specialDirectories.CurrentUserApplicationData);
        VerifySpecialDirectory(() => FileIO.SpecialDirectories.Desktop, () => specialDirectories.Desktop);
        VerifySpecialDirectory(() => FileIO.SpecialDirectories.MyDocuments, () => specialDirectories.MyDocuments);
        VerifySpecialDirectory(() => FileIO.SpecialDirectories.MyMusic, () => specialDirectories.MyMusic);
        VerifySpecialDirectory(() => FileIO.SpecialDirectories.MyPictures, () => specialDirectories.MyPictures);
        VerifySpecialDirectory(() => FileIO.SpecialDirectories.Programs, () => specialDirectories.Programs);
        VerifySpecialDirectory(() => FileIO.SpecialDirectories.ProgramFiles, () => specialDirectories.ProgramFiles);
        VerifySpecialDirectory(() => FileIO.SpecialDirectories.Temp, () => specialDirectories.Temp);
    }

    private static void VerifySpecialDirectory(Func<string> getExpected, Func<string> getActual)
    {
        string expected;
        try
        {
            expected = getExpected();
        }
        catch (Exception)
        {
            return;
        }

        string actual = getActual();
        Assert.Equal(expected, actual);
    }
}
