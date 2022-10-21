// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Drawing;
using System.Windows.Forms.Primitives;

namespace System.Windows.Forms.Layout.Tests
{
    public class AnchorLayoutTests : IClassFixture<ThreadExceptionFixture>
    {
        private static (Form, Button) GetFormWithAnchoredButton()
        {
            Form form = new ();
            form.Size = new Size(200, 300);
            Button button = new ();
            button.Location = new Point(20, 30);
            button.Size = new Size(20, 30);
            button.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            return (form, button);
        }

        private static void Dispose(Form form, Button button)
        {
            button?.Dispose();
            form?.Dispose();
        }

        [WinFormsFact]
        public void ControlNotParented_NoAnchorsComputed()
        {
            var (form, button) = GetFormWithAnchoredButton();
            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);
            _ = button.Handle; // Force handle creation;
            Assert.Null(anchorInfo);
            Dispose(form, button);
        }

        [WinFormsFact]
        public void ControlHandleNotCreated_NoAnchorsComputed()
        {
            var (form, button) = GetFormWithAnchoredButton();
            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);
            form.Controls.Add(button);
            Assert.Null(anchorInfo);
            Dispose(form, button);
        }

        [WinFormsFact]
        public void ControlParentHandleNotCreated_NoAnchorsComputed()
        {
            var (form, button) = GetFormWithAnchoredButton();
            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);
            _ = button.Handle; // Force handle creation;
            form.Controls.Add(button);
            Assert.Null(anchorInfo);
            Dispose(form, button);
        }

        [WinFormsFact]
        public void ConfigSwitchDisabled_NoHanldeCreated_AnchorsComputed()
        {
            AppContext.SetSwitch(LocalAppContextSwitches.EnableAnchorLayoutV2SwitchName, false);
            var (form, button) = GetFormWithAnchoredButton();
            form.Controls.Add(button);
            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.NotNull(anchorInfo);
            Dispose(form, button);
        }
    }
}
