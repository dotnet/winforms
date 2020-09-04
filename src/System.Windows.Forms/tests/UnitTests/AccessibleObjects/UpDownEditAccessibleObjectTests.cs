// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class UpDownEditAccessibleObjectTests
    {
        [WinFormsFact]
        public void UpDownEditAccessibleObject_ctor_default()
        {
            using UpDownBase upDown = new SubUpDownBase();
            using UpDownBase.UpDownEdit upDownEdit = new UpDownBase.UpDownEdit(upDown);
            Assert.NotNull(upDownEdit.AccessibilityObject);
            Assert.False(upDown.IsHandleCreated);
        }

        private class SubUpDownBase : UpDownBase
        {
            public override void DownButton() { }

            public override void UpButton() { }

            protected override void UpdateEditText() { }
        }
    }
}
