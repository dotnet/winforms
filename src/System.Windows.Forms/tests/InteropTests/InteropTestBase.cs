// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit.Sdk;

namespace System.Windows.Forms.InteropTests
{
    public abstract class InteropTestBase
    {
        public const string NativeTests = "NativeTests";
        public const string Success = "Success";

        protected static void AssertSuccess(string result)
        {
            if (result != Success)
            {
                throw new XunitException(result);
            }
        }
    }
}
