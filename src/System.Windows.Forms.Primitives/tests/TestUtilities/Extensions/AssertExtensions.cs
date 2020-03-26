// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System
{
    public static class AssertExtensions
    {
        public static T Throws<T>(string paramName, Func<object> testCode)
            where T : ArgumentException
        {
            T exception = Assert.Throws<T>(testCode);

            if (!RuntimeInformation.FrameworkDescription.StartsWith(".NET Native"))
            {
                Assert.Equal(paramName, exception.ParamName);
            }

            return exception;
        }
    }
}
