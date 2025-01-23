// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualBasic.ApplicationServices.Tests;

public class UserTests
{
    [Fact]
    public void Properties()
    {
        User user = new();
        Assert.Equal(Thread.CurrentPrincipal, user.CurrentPrincipal);
        if (user.CurrentPrincipal is not null)
        {
            Assert.Equal(Thread.CurrentPrincipal.Identity.Name, user.Name);
            Assert.Equal(Thread.CurrentPrincipal.Identity.IsAuthenticated, user.IsAuthenticated);
            Assert.Equal(Thread.CurrentPrincipal.IsInRole("Guest"), user.IsInRole("Guest"));
        }
    }
}
