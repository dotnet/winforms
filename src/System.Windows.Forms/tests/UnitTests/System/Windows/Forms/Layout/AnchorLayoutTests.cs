﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Drawing;
using System.Windows.Forms.Primitives;

namespace System.Windows.Forms.Layout.Tests
{
    public class AnchorLayoutTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void Control_NotParented_AnchorsNotComputed()
        {
            (Form form, Button button) = GetFormWithAnchoredButton();

            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            Assert.NotEqual(IntPtr.Zero, button.Handle);
            Assert.Null(anchorInfo);

            Dispose(form, button);
        }

        [WinFormsFact]
        public void Control_HandleNotCreated_AnchorsNotComputed()
        {
            (Form form, Button button) = GetFormWithAnchoredButton();

            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            form.Controls.Add(button);
            Assert.Null(anchorInfo);

            Dispose(form, button);
        }

        [WinFormsFact]
        public void Control_ParentHandleNotCreated_AnchorsNotComputed()
        {
            (Form form, Button button) = GetFormWithAnchoredButton();

            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            Assert.NotEqual(IntPtr.Zero, button.Handle);

            form.Controls.Add(button);
            Assert.Null(anchorInfo);

            Dispose(form, button);
        }

        [WinFormsFact]
        public void ConfigSwitch_Disabled_HanldeNotCreated_AnchorsComputed()
        {
            AppContext.SetSwitch(LocalAppContextSwitches.AnchorLayoutV2SwitchName, false);

            (Form form, Button button) = GetFormWithAnchoredButton();
            form.Controls.Add(button);

            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.NotNull(anchorInfo);

            Dispose(form, button);
        }

        private static (Form, Button) GetFormWithAnchoredButton()
        {
            Form form = new()
            {
                Size = new Size(200, 300)
            };

            Button button = new()
            {
                Location = new Point(20, 30),
                Size = new Size(20, 30),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
            };

            return (form, button);
        }

        private static void Dispose(Form form, Button button)
        {
            button?.Dispose();
            form?.Dispose();
        }
    }
}
