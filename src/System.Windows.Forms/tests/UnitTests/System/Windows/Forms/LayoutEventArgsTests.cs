// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms.Tests;

public class LayoutEventArgsTests
{
    public static IEnumerable<object[]> Ctor_IComponent_String_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { new Control(), "" };
        yield return new object[] { new SubComponent(), "affectedProperty" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_IComponent_String_TestData))]
    public void Ctor_IComponent_String(IComponent affectedComponent, string affectedProperty)
    {
        LayoutEventArgs e = new(affectedComponent, affectedProperty);
        Assert.Equal(affectedComponent, e.AffectedComponent);
        Assert.Equal(affectedComponent as Control, e.AffectedControl);
        Assert.Equal(affectedProperty, e.AffectedProperty);
    }

    public static IEnumerable<object[]> Ctor_Control_String_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { new Control(), "" };
        yield return new object[] { new Control(), "affectedProperty" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Control_String_TestData))]
    public void Ctor_Control_String(Control affectedControl, string affectedProperty)
    {
        LayoutEventArgs e = new(affectedControl, affectedProperty);
        Assert.Equal(affectedControl, e.AffectedComponent);
        Assert.Equal(affectedControl, e.AffectedControl);
        Assert.Equal(affectedProperty, e.AffectedProperty);
    }

    [WinFormsFact]
    public void LayoutEventArgs_DoesNotRootControl()
    {
        LayoutEventArgs layoutEventArgs = CreateAndDisposeControl();
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
        Assert.Null(layoutEventArgs.AffectedComponent);

        [MethodImpl(MethodImplOptions.NoInlining)]
        static LayoutEventArgs CreateAndDisposeControl()
        {
            // Setup TableLayoutPanel and Panel
            using TableLayoutPanel tableLayoutPanel = new()
            {
                ColumnCount = 2,
                RowCount = 2,
                AutoScroll = true,
            };

            tableLayoutPanel.Controls.Add(new Panel()
            {
                Name = "Panel",
                Dock = DockStyle.Fill
            }, 0, 0); // Add the panel to the first cell

            // Force layout update
            tableLayoutPanel.PerformLayout();

            // Suspend layout
            tableLayoutPanel.SuspendLayout();
            using (Panel panel = (Panel)tableLayoutPanel.Controls.Find("Panel", false).First())
            {
                tableLayoutPanel.Controls.Remove(panel);
                panel.Dispose();
            }

            // Check the cachedLayoutEventArgs
            ITestAccessor tableLayoutPanelTestAccessor = tableLayoutPanel.TestAccessor();
            LayoutEventArgs layoutEventArgs = (LayoutEventArgs)tableLayoutPanelTestAccessor.Dynamic._cachedLayoutEventArgs;

            tableLayoutPanel.ResumeLayout();
            return layoutEventArgs;
        }
    }

    private class SubComponent : IComponent
    {
        public ISite Site { get; set; }

#pragma warning disable 0067
        public event EventHandler Disposed;
#pragma warning restore 0067

        public void Dispose() { }
    }
}
