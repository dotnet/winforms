// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms.UITests;

public class AnchorLayoutTests : ControlTestBase
{
    private const AnchorStyles AnchorAllDirection = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

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
        using AnchorLayoutV2Scope scope = new(enable: true);
        LaunchFormAndVerify(anchors, expectedX, expectedY, expectedWidth, expectedHeight);
    }

    [WinFormsFact]
    public void Control_NotParented_AnchorsNotComputed()
    {
        using AnchorLayoutV2Scope scope = new(enable: true);
        (Form form, Button button) = GetFormWithAnchoredButton(AnchorAllDirection);

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
            Dispose(form, button);
        }
    }

    [WinFormsFact]
    public void Control_SuspendedLayout_AnchorsNotComputed()
    {
        using AnchorLayoutV2Scope scope = new(enable: true);
        (Form form, Button button) = GetFormWithAnchoredButton(AnchorAllDirection);

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
            Dispose(form, button);
        }
    }

    [WinFormsFact]
    public void Control_ResumedLayout_AnchorsComputed()
    {
        using AnchorLayoutV2Scope scope = new(enable: true);
        (Form form, Button button) = GetFormWithAnchoredButton(AnchorAllDirection);

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
            Dispose(form, button);
        }
    }

    [WinFormsFact]
    public void ConfigSwitch_Disabled_SuspendedLayout_AnchorsComputed()
    {
        using AnchorLayoutV2Scope scope = new(enable: false);
        (Form form, Button button) = GetFormWithAnchoredButton(AnchorAllDirection);

        try
        {
            form.Controls.Add(button);

            DefaultLayout.AnchorInfo? anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.NotNull(anchorInfo);
        }
        finally
        {
            Dispose(form, button);
        }
    }

    [WinFormsFact]
    public void NestedContainer_AnchorsComputed()
    {
        using AnchorLayoutV2Scope scope = new(enable: true);
        (Form form, Button button) = GetFormWithAnchoredButton(AnchorAllDirection);
        try
        {
            using ContainerControl container = new();
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
            Dispose(form, button);
        }
    }

    [WinFormsFact]
    public void ParentChanged_AnchorsUpdated()
    {
        using AnchorLayoutV2Scope scope = new(enable: true);
        (Form form, Button button) = GetFormWithAnchoredButton(AnchorAllDirection);
        try
        {
            using ContainerControl container = new();
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
            Dispose(form, button);
        }
    }

    [WinFormsFact]
    public void AnchorLayoutV2_ReplaceParentWhileAncestorLayoutSuspended_AnchorsRemainConsistent()
    {
        // Regression test for https://github.com/dotnet/winforms/issues/14500
        using AnchorLayoutV2Scope scope = new(enable: true);
        using Form form = new() { ClientSize = new Size(400, 300) };
        using Panel originalPanel = new()
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Location = new Point(20, 30),
            Size = new Size(100, 120)
        };
        using Panel replacementPanel = new();

        form.Controls.Add(originalPanel);
        form.ResumeLayout(performLayout: true);

        form.ClientSize = new Size(500, 350);
        Assert.Equal(new Rectangle(20, 30, 200, 170), originalPanel.Bounds);

        ReplaceControl(originalPanel, replacementPanel);
        Assert.Equal(originalPanel.Anchor, replacementPanel.Anchor);
        Assert.Equal(new Rectangle(20, 30, 200, 170), replacementPanel.Bounds);

        form.ClientSize = new Size(600, 400);
        Assert.Equal(new Rectangle(20, 30, 300, 220), replacementPanel.Bounds);

        static void ReplaceControl(Control controlToRemove, Control controlToAdd)
        {
            controlToRemove.SuspendLayout();
            controlToAdd.SuspendLayout();
            controlToAdd.Anchor = controlToRemove.Anchor;
            controlToAdd.Dock = controlToRemove.Dock;
            controlToAdd.Location = controlToRemove.Location;
            controlToAdd.Size = controlToRemove.Size;
            controlToRemove.ResumeLayout(true);
            controlToAdd.ResumeLayout(true);

            Control parentOfControlToRemove = controlToRemove.Parent!;
            Control? parentOfControlToAdd = controlToAdd.Parent;
            int index = parentOfControlToRemove.Controls.IndexOf(controlToRemove);
            parentOfControlToRemove.SuspendLayout();
            if (parentOfControlToAdd is { } parentToSuspend)
            {
                parentToSuspend.SuspendLayout();
            }

            parentOfControlToRemove.Controls.Remove(controlToRemove);
            parentOfControlToRemove.Controls.Add(controlToAdd);
            parentOfControlToRemove.Controls.SetChildIndex(controlToAdd, index);
            parentOfControlToRemove.ResumeLayout(true);

            if (parentOfControlToAdd is { } parentToResume)
            {
                parentToResume.ResumeLayout(true);
            }
        }
    }

    [WinFormsFact]
    public void SetBoundsOnAnchoredControl_BoundsChanged()
    {
        using AnchorLayoutV2Scope scope = new(enable: true);
        (Form form, Button button) = GetFormWithAnchoredButton(AnchorAllDirection);
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

    private static void Dispose(Form form, Button button)
    {
        button.Dispose();
        form.Dispose();
    }
}
