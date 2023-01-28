// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.Layout;
using System.Windows.Forms.Primitives;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class AnchorLayoutTests : ControlTestBase
    {
        static AnchorStyles anchorAllDirection = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

        public AnchorLayoutTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsTheory]
        [InlineData(AnchorStyles.Top, 120, 30, 20, 30)]
        [InlineData(AnchorStyles.Left, 20, 180, 20, 30)]
        [InlineData(AnchorStyles.Right, 220, 180, 20, 30)]
        [InlineData(AnchorStyles.Bottom, 120, 330, 20, 30)]
        [InlineData(AnchorStyles.Top | AnchorStyles.Left, 20, 30, 20, 30)]
        [InlineData(AnchorStyles.Top | AnchorStyles.Right, 220, 30, 20, 30)]
        [InlineData(AnchorStyles.Top | AnchorStyles.Bottom, 120, 30, 20, 330)]
        [InlineData(AnchorStyles.Left | AnchorStyles.Right, 20, 180, 220, 30)]
        [InlineData(AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left, 20, 30, 220, 330)]
        public void Control_ResizeAnchoredControls_ParentHanldeCreated_NewAnchorsApplied(AnchorStyles anchors, int expectedX, int expectedY, int expectedWidth, int expectedHeight)
        {
            LaunchFormAndVerify(anchors, expectedX, expectedY, expectedWidth, expectedHeight);
        }

        [WinFormsTheory]
        [InlineData(AnchorStyles.Top, 120, 30, 20, 30)]
        [InlineData(AnchorStyles.Left, 20, 180, 20, 30)]
        [InlineData(AnchorStyles.Right, 220, 180, 20, 30)]
        [InlineData(AnchorStyles.Bottom, 120, 330, 20, 30)]
        [InlineData(AnchorStyles.Top | AnchorStyles.Left, 20, 30, 20, 30)]
        [InlineData(AnchorStyles.Top | AnchorStyles.Right, 220, 30, 20, 30)]
        [InlineData(AnchorStyles.Top | AnchorStyles.Bottom, 120, 30, 20, 330)]
        [InlineData(AnchorStyles.Left | AnchorStyles.Right, 20, 180, 220, 30)]
        [InlineData(AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left, 20, 30, 220, 330)]
        public void Control_AnchorLayoutV2_ResizeAnchoredControls_ParentHanldeCreated_NewAnchorsApplied(AnchorStyles anchors, int expectedX, int expectedY, int expectedWidth, int expectedHeight)
        {
            int previousLayoutSwtch = SetAncorLayoutV2();

            try
            {
                LaunchFormAndVerify(anchors, expectedX, expectedY, expectedWidth, expectedHeight);
            }
            finally
            {
                // Reset switch.
                SetAnchorLayoutV2Switch(previousLayoutSwtch);
            }
        }

        [WinFormsFact]
        public void Control_NotParented_AnchorsNotComputed()
        {
            int previousSwitchValue = SetAncorLayoutV2();
            (Form form, Button button) = GetFormWithAnchoredButton(anchorAllDirection);

            try
            {
                DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
                Assert.Null(anchorInfo);

                // Unparent button and resume layout.
                form.Controls.Remove(button);
                form.ResumeLayout(performLayout: false);

                anchorInfo = DefaultLayout.GetAnchorInfo(button);
                Assert.Null(anchorInfo);
            }
            finally
            {
                // Reset switch.
                SetAnchorLayoutV2Switch(previousSwitchValue);
                Dispose(form, button);
            }
        }

        [WinFormsFact]
        public void Control_SuspendedLayout_AnchorsNotComputed()
        {
            int previousSwitchValue = SetAncorLayoutV2();
            (Form form, Button button) = GetFormWithAnchoredButton(anchorAllDirection);

            try
            {
                DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
                Assert.Null(anchorInfo);

                form.Controls.Add(button);
                anchorInfo = DefaultLayout.GetAnchorInfo(button);
                Assert.Null(anchorInfo);
            }
            finally
            {
                // Reset switch.
                SetAnchorLayoutV2Switch(previousSwitchValue);
                Dispose(form, button);
            }
        }

        [WinFormsFact]
        public void Control_ResumedLayout_AnchorsComputed()
        {
            int previousSwitchValue = SetAncorLayoutV2();
            (Form form, Button button) = GetFormWithAnchoredButton(anchorAllDirection);

            try
            {
                DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
                Assert.Null(anchorInfo);

                form.Controls.Add(button);
                anchorInfo = DefaultLayout.GetAnchorInfo(button);
                Assert.Null(anchorInfo);

                form.ResumeLayout(performLayout: false);
                anchorInfo = DefaultLayout.GetAnchorInfo(button);
                Assert.NotNull(anchorInfo);
            }
            finally
            {
                // Reset switch.
                SetAnchorLayoutV2Switch(previousSwitchValue);
                Dispose(form, button);
            }
        }

        [WinFormsFact]
        public void ConfigSwitch_Disabled_SuspendedLayout_AnchorsComputed()
        {
            int previousSwitchValue = SetAncorLayoutV1();
            (Form form, Button button) = GetFormWithAnchoredButton(anchorAllDirection);

            try
            {
                form.Controls.Add(button);

                DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
                Assert.NotNull(anchorInfo);
            }
            finally
            {
                // Reset switch.
                SetAnchorLayoutV2Switch(previousSwitchValue);
                Dispose(form, button);
            }
        }

        private static void LaunchFormAndVerify(AnchorStyles anchors, int expectedX, int expectedY, int expectedWidth, int expectedHeight)
        {
            (Form form, Button button) = GetFormWithAnchoredButton(anchors);

            try
            {
                form.ResumeLayout(true);
                form.Controls.Add(button);
                form.Shown += OnFormShown;
                form.ShowDialog();
            }
            finally
            {
                Dispose(form, button);
            }

            return;

            void OnFormShown(object? sender, EventArgs e)
            {
                // Resize the form to compute button anchors.
                form.Size = new Size(400, 600);
                Rectangle buttonBounds = button.Bounds;

                Assert.Equal(expectedX, buttonBounds.X);
                Assert.Equal(expectedY, buttonBounds.Y);
                Assert.Equal(expectedWidth, buttonBounds.Width);
                Assert.Equal(expectedHeight, buttonBounds.Height);

                form.Close();
            }
        }

        private static (Form, Button) GetFormWithAnchoredButton(AnchorStyles buttonAnchors)
        {
            Form form = new()
            {
                Size = new Size(200, 300)
            };

            form.SuspendLayout();
            Button button = new()
            {
                Location = new Point(20, 30),
                Size = new Size(20, 30),
                Anchor = buttonAnchors
            };

            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            return (form, button);
        }

        private static int SetAnchorLayoutV2Switch(int value)
        {
            // TargetFramework on the testhost.exe is NetCoreApp2.1. AppContext.TargetFrameworkName return this value
            // while running unit tests. To avoid using this invalid target framework for unit tests, we are
            // explicitly setting and unsetting the switch.
            // Switch value has 3 states: 0 - unknown, 1 - true, -1 - false
            dynamic localAppContextSwitches = typeof(LocalAppContextSwitches).TestAccessor().Dynamic;
            int previousSwitchValue = localAppContextSwitches.s_AnchorLayoutV2;
            localAppContextSwitches.s_AnchorLayoutV2 = value;

            return previousSwitchValue;
        }

        private static int SetAncorLayoutV2() => SetAnchorLayoutV2Switch(value: 1);

        private static int SetAncorLayoutV1() => SetAnchorLayoutV2Switch(value: -1);

        private static void Dispose(Form form, Button button)
        {
            button.Dispose();
            form.Dispose();
        }
    }
}
