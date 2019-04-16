// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WinForms.Common.Tests;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class ProjectTargetFrameworkAttributeTests
    {
        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ProjectTargetFrameworkAttribute_Ctor_String(string targetFrameworkMoniker)
        {
            var attribute = new ProjectTargetFrameworkAttribute(targetFrameworkMoniker);
            Assert.Equal(targetFrameworkMoniker, attribute.TargetFrameworkMoniker);
        }
    }
}
