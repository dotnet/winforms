// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Layout;
using System.Windows.Forms.Primitives;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

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
    public void Control_ResizeAnchoredControls_ParentHandleCreated_NewAnchorsApplied(AnchorStyles anchors, int expectedX, int expectedY, int expectedWidth, int expectedHeight)
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
    public void Control_AnchorLayoutV2_ResizeAnchoredControls_ParentHandleCreated_NewAnchorsApplied(AnchorStyles anchors, int expectedX, int expectedY, int expectedWidth, int expectedHeight)
    {
        int previousLayoutSwitch = SetAnchorLayoutV2();

        try
        {
            LaunchFormAndVerify(anchors, expectedX, expectedY, expectedWidth, expectedHeight);
        }
        finally
        {
            // Reset switch.
            SetAnchorLayoutV2Switch(previousLayoutSwitch);
        }
    }

    [WinFormsFact]
    public void Control_NotParented_AnchorsNotComputed()
    {
        int previousSwitchValue = SetAnchorLayoutV2();
        (Form form, Button button) = GetFormWithAnchoredButton(anchorAllDirection);

        try
        {
            DefaultLayout.AnchorInfo? anchorInfo = DefaultLayout.GetAnchorInfo(button);
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
        int previousSwitchValue = SetAnchorLayoutV2();
        (Form form, Button button) = GetFormWithAnchoredButton(anchorAllDirection);

        try
        {
            DefaultLayout.AnchorInfo? anchorInfo = DefaultLayout.GetAnchorInfo(button);
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
        int previousSwitchValue = SetAnchorLayoutV2();
        (Form form, Button button) = GetFormWithAnchoredButton(anchorAllDirection);

        try
        {
            DefaultLayout.AnchorInfo? anchorInfo = DefaultLayout.GetAnchorInfo(button);
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
        int previousSwitchValue = SetAnchorLayoutV1();
        (Form form, Button button) = GetFormWithAnchoredButton(anchorAllDirection);

        try
        {
            form.Controls.Add(button);

            DefaultLayout.AnchorInfo? anchorInfo = DefaultLayout.GetAnchorInfo(button);
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
    public void NestedContainer_AnchorsComputed()
    {
        int previousSwitchValue = SetAnchorLayoutV2();
        (Form form, Button button) = GetFormWithAnchoredButton(anchorAllDirection);
        try
        {
            using var container = new ContainerControl();
            container.Dock = DockStyle.Fill;
            container.SuspendLayout();
            container.Controls.Add(button);

            DefaultLayout.AnchorInfo? anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            form.Controls.Add(container);
            Assert.Null(anchorInfo);

            container.ResumeLayout(false);
            form.ResumeLayout(false);
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
    public void ParentChanged_AnchorsUpdated()
    {
        int previousSwitchValue = SetAnchorLayoutV2();
        (Form form, Button button) = GetFormWithAnchoredButton(anchorAllDirection);
        try
        {
            using var container = new ContainerControl();
            container.Dock = DockStyle.Fill;
            container.SuspendLayout();
            container.Controls.Add(button);

            DefaultLayout.AnchorInfo? anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            form.Controls.Add(container);
            Assert.Null(anchorInfo);

            container.ResumeLayout(false);
            form.ResumeLayout(false);

            anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.NotNull(anchorInfo);

            container.Controls.Remove(button);
            Assert.NotNull(anchorInfo);

            var previousDisplayRect = anchorInfo.DisplayRectangle;
            form.Controls.Add(button);
            Assert.NotEqual(previousDisplayRect, anchorInfo.DisplayRectangle);
        }
        finally
        {
            // Reset switch.
            SetAnchorLayoutV2Switch(previousSwitchValue);
            Dispose(form, button);
        }
    }

    [WinFormsFact]
    public void SetBoundsOnAnchoredControl_BoundsChanged()
    {
        int previousSwitchValue = SetAnchorLayoutV2();
        (Form form, Button button) = GetFormWithAnchoredButton(anchorAllDirection);
        try
        {
            form.Controls.Add(button);

            DefaultLayout.AnchorInfo? anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            form.ResumeLayout(false);

            anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.NotNull(anchorInfo);

            var bounds = button.Bounds;
            button.SetBounds(bounds.X + 5, bounds.Y + 5, bounds.Width + 10, bounds.Height + 10, BoundsSpecified.None);
            Assert.Equal(bounds, button.Bounds); // Bounds Specified is None.

            button.SetBounds(bounds.X + 5, bounds.Y + 5, bounds.Width + 10, bounds.Height + 10, BoundsSpecified.All);
            Assert.NotEqual(bounds, button.Bounds); // Bounds Specified is None.

            bounds = button.Bounds;
            button.SetBounds(bounds.X + 5, bounds.Y + 5, bounds.Width + 10, bounds.Height + 10);
            Assert.NotEqual(bounds, button.Bounds);
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
        Rectangle newButtonBounds = button.Bounds;
        try
        {
            form.ResumeLayout(true);
            form.Controls.Add(button);
            form.Shown += OnFormShown;
            form.ShowDialog();

            Assert.Equal(expectedX, newButtonBounds.X);
            Assert.Equal(expectedY, newButtonBounds.Y);
            Assert.Equal(expectedWidth, newButtonBounds.Width);
            Assert.Equal(expectedHeight, newButtonBounds.Height);
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
            newButtonBounds = button.Bounds;
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

        DefaultLayout.AnchorInfo? anchorInfo = DefaultLayout.GetAnchorInfo(button);
        Assert.Null(anchorInfo);

        return (form, button);
    }

    private static int SetAnchorLayoutV2Switch(int value)
    {
        // TargetFramework on the test host.exe is NetCoreApp2.1. AppContext.TargetFrameworkName return this value
        // while running unit tests. To avoid using this invalid target framework for unit tests, we are
        // explicitly setting and unsetting the switch.
        // Switch value has 3 states: 0 - unknown, 1 - true, -1 - false
        dynamic localAppContextSwitches = typeof(LocalAppContextSwitches).TestAccessor().Dynamic;
        int previousSwitchValue = localAppContextSwitches.s_anchorLayoutV2;
        localAppContextSwitches.s_anchorLayoutV2 = value;

        return previousSwitchValue;
    }

    private static int SetAnchorLayoutV2() => SetAnchorLayoutV2Switch(value: 1);

    private static int SetAnchorLayoutV1() => SetAnchorLayoutV2Switch(value: -1);

    private static void Dispose(Form form, Button button)
    {
        button.Dispose();
        form.Dispose();
    }
}
