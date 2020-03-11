// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using ApprovalTests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class TypeMembersAttributeDecorationTests : IClassFixture<ThreadExceptionFixture>
    {
#if !DEBUG
       [Fact]
        public void VerifyAttributes()
        {
            Dictionary<string, string> dic = TypeMembersDecoratior.EnumerateAttributes(typeof(AnchorEditor).Assembly);
            Approvals.VerifyAll(dic);
        }
#endif
    }
}
