// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class ListViewInsertionMarkVisualStylesOffTests : ControlTestBase
    {
        public ListViewInsertionMarkVisualStylesOffTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper, enableVisualStyles: false)
        {
        }
    }
}
