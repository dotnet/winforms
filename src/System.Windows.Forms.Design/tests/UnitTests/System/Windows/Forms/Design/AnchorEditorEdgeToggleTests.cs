// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace System.Windows.Forms.Design.Tests;

/// <summary>
///  Drives the real <see cref="AnchorEditor"/> drop-down through a fake editor service to verify how
///  individual edge toggles compose <see cref="AnchorStyles"/> flags and how commit/cancel behave.
/// </summary>
public class AnchorEditorEdgeToggleTests
{
    private const AnchorStyles AllEdges = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

    private static readonly string[] s_edgeFields = ["_left", "_top", "_right", "_bottom"];

    private static readonly AnchorStyles[] s_edges =
        [AnchorStyles.Left, AnchorStyles.Top, AnchorStyles.Right, AnchorStyles.Bottom];

    public static TheoryData<AnchorStyles, int> InitialValueAndEdge_TestData()
    {
        TheoryData<AnchorStyles, int> data = [];
        for (int initial = 0; initial <= (int)AllEdges; initial++)
        {
            for (int edge = 0; edge < s_edges.Length; edge++)
            {
                data.Add((AnchorStyles)initial, edge);
            }
        }

        return data;
    }

    // Scenario: Clicking one edge toggles only that edge; the other three edges keep the state they were opened with,
    // for every one of the 16 possible starting anchors.
    [WinFormsTheory]
    [MemberData(nameof(InitialValueAndEdge_TestData))]
    public void AnchorEditor_ClickEdge_TogglesOnlyThatEdge(AnchorStyles initial, int edgeIndex)
    {
        AnchorStyles edge = s_edges[edgeIndex];

        (object? result, _) = Edit(initial, ui =>
        {
            Click(GetEdge(ui, edgeIndex));
            Press(ui, edgeIndex, Keys.Enter);
        });

        Assert.Equal(initial ^ edge, Assert.IsType<AnchorStyles>(result));
    }

    // Scenario: Toggling several edges one after another accumulates, so earlier toggles are not lost when a later edge changes.
    [WinFormsFact]
    public void AnchorEditor_ClickSeveralEdges_ComposesFlags()
    {
        (object? result, _) = Edit(AnchorStyles.Top | AnchorStyles.Left, ui =>
        {
            Click(GetEdge(ui, 2)); // Right on.
            Click(GetEdge(ui, 3)); // Bottom on.
            Click(GetEdge(ui, 1)); // Top off.
            Press(ui, 3, Keys.Enter);
        });

        Assert.Equal(AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom, result);
    }

    // Scenario: Turning every edge off yields AnchorStyles.None and turning them all back on yields all four edges.
    [WinFormsFact]
    public void AnchorEditor_ToggleAllEdgesOffAndOn_ReachesNoneAndAll()
    {
        (object? none, _) = Edit(AllEdges, ui =>
        {
            for (int i = 0; i < 4; i++)
            {
                Click(GetEdge(ui, i));
            }
        });

        Assert.Equal(AnchorStyles.None, none);

        (object? all, _) = Edit(AnchorStyles.None, ui =>
        {
            for (int i = 0; i < 4; i++)
            {
                Click(GetEdge(ui, i));
            }
        });

        Assert.Equal(AllEdges, all);
    }

    // Scenario: Clicking the same edge twice returns to the original state.
    [WinFormsFact]
    public void AnchorEditor_ClickEdgeTwice_RestoresOriginalValue()
    {
        (object? result, _) = Edit(AnchorStyles.Top | AnchorStyles.Right, ui =>
        {
            Click(GetEdge(ui, 3));
            Click(GetEdge(ui, 3));
        });

        Assert.Equal(AnchorStyles.Top | AnchorStyles.Right, result);
    }

    // Scenario: Pressing Space on a focused edge toggles it, while any other character is ignored.
    [WinFormsFact]
    public void AnchorEditor_SpaceKey_TogglesEdgeAndOtherCharsAreIgnored()
    {
        (object? result, _) = Edit(AnchorStyles.Top | AnchorStyles.Left, ui =>
        {
            Control right = GetEdge(ui, 2);

            Assert.False(ProcessDialogChar(right, 'x'));
            Assert.False(IsSolid(right));

            Assert.True(ProcessDialogChar(right, ' '));
            Assert.True(IsSolid(right));
        });

        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, result);

        (result, _) = Edit(AnchorStyles.Top | AnchorStyles.Left, ui =>
        {
            Control left = GetEdge(ui, 0);
            Assert.True(ProcessDialogChar(left, ' '));
            Assert.False(IsSolid(left));
        });

        Assert.Equal(AnchorStyles.Top, result);
    }

    // Scenario: Pressing Enter commits the toggled edges and closes the drop-down exactly once.
    [WinFormsFact]
    public void AnchorEditor_EnterKey_CommitsAndClosesDropDown()
    {
        AnchorEditor editor = new();
        FakeEditorService service = new();
        service.OnDropDown = ui =>
        {
            Click(GetEdge(ui, 3));
            Assert.Equal(0, service.CloseCount);
            Assert.True(Press(ui, 3, Keys.Enter));
        };

        object? result = editor.EditValue(null, new FakeServiceProvider(service), AnchorStyles.Top | AnchorStyles.Left);
        service.LastControl?.Dispose();
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom, result);
        Assert.Equal(1, service.CloseCount);
        Assert.Equal(1, service.DropDownCount);
    }

    // Scenario: Pressing Enter without toggling anything keeps the original value.
    [WinFormsFact]
    public void AnchorEditor_EnterKeyWithoutToggle_KeepsOriginalValue()
    {
        (object? result, FakeEditorService service) = Edit(AnchorStyles.Bottom | AnchorStyles.Right, ui => Press(ui, 0, Keys.Enter));

        Assert.Equal(AnchorStyles.Bottom | AnchorStyles.Right, result);
        Assert.Equal(1, service.CloseCount);
    }

    // Scenario: Pressing Escape after toggling edges discards the changes, restores the value the editor was opened with,
    // and closes the drop-down.
    [WinFormsFact]
    public void AnchorEditor_EscapeKey_CancelsAndRestoresOriginalValue()
    {
        (object? result, FakeEditorService service) = Edit(AnchorStyles.Top | AnchorStyles.Left, ui =>
        {
            Click(GetEdge(ui, 2));
            Click(GetEdge(ui, 0));
            Assert.True(Press(ui, 2, Keys.Escape));
        });

        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, result);
        Assert.Equal(1, service.CloseCount);
    }

    // Scenario: Cancelling an edit that started from AnchorStyles.None returns None rather than any toggled edges.
    [WinFormsFact]
    public void AnchorEditor_EscapeKeyFromNone_ReturnsNone()
    {
        (object? result, _) = Edit(AnchorStyles.None, ui =>
        {
            Click(GetEdge(ui, 1));
            Press(ui, 1, Keys.Escape);
        });

        Assert.Equal(AnchorStyles.None, result);
    }

    // Scenario: When the incoming value is not an AnchorStyles, cancelling falls back to the default Top | Left anchor,
    // and committing without a toggle returns the original (non-anchor) value untouched.
    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("text")]
    [InlineData(42)]
    public void AnchorEditor_NonAnchorValue_CancelUsesDefaultAndCommitKeepsValue(object? value)
    {
        (object? cancelled, _) = Edit(value, ui =>
        {
            Click(GetEdge(ui, 2));
            Press(ui, 2, Keys.Escape);
        });

        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, cancelled);

        (object? committed, _) = Edit(value, ui => Press(ui, 2, Keys.Enter));

        Assert.Equal(value, committed);
    }

    // Scenario: Enter or Escape combined with Ctrl or Alt is not treated as commit/cancel, so the drop-down stays open.
    [WinFormsTheory]
    [InlineData(Keys.Enter | Keys.Control)]
    [InlineData(Keys.Enter | Keys.Alt)]
    [InlineData(Keys.Escape | Keys.Control)]
    [InlineData(Keys.Escape | Keys.Alt)]
    [InlineData(Keys.Tab | Keys.Control)]
    [InlineData(Keys.Tab | Keys.Alt)]
    public void AnchorEditor_ModifiedCommitCancelTabKeys_AreIgnored(Keys keyData)
    {
        (object? result, FakeEditorService service) = Edit(AnchorStyles.Top | AnchorStyles.Left, ui =>
        {
            Click(GetEdge(ui, 2));
            Assert.False(Press(ui, 2, keyData));
        });

        Assert.Equal(0, service.CloseCount);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, result);
    }

    // Scenario: Tab and Shift+Tab walk the edges in left, top, right, bottom order and wrap around at both ends;
    // the Tab key is consumed by the editor, and moving focus alone does not change the anchor value.
    [WinFormsFact]
    public void AnchorEditor_TabAndShiftTab_CycleEdgesInOrderWithoutChangingValue()
    {
        (object? result, _) = Edit(AnchorStyles.Top | AnchorStyles.Left, ui =>
        {
            using Form form = new() { ShowInTaskbar = false };
            form.Controls.Add(ui);
            form.Show();
            Control[] edges = [GetEdge(ui, 0), GetEdge(ui, 1), GetEdge(ui, 2), GetEdge(ui, 3)];

            Assert.Equal([0, 1, 2, 3], edges.Select(e => e.TabIndex));
            Assert.All(edges, e => Assert.True(e.TabStop));

            Assert.True(edges[0].Focus());
            for (int i = 1; i <= 4; i++)
            {
                Assert.True(ProcessDialogKey(edges[(i - 1) % 4], Keys.Tab));
                Assert.True(edges[i % 4].Focused);
            }

            Assert.True(ProcessDialogKey(edges[0], Keys.Tab | Keys.Shift));
            Assert.True(edges[3].Focused);
            Assert.True(ProcessDialogKey(edges[3], Keys.Tab | Keys.Shift));
            Assert.True(edges[2].Focused);

            Assert.True(ProcessDialogKey(edges[2], Keys.Enter));
        });

        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, result);
    }

    // Scenario: The accessible object of each edge reports the Checked state and the Check/Uncheck default action
    // that match whether the edge is currently part of the anchor, and updates after a toggle.
    [WinFormsFact]
    public void AnchorEditor_EdgeAccessibility_ReflectsToggleState()
    {
        Edit(AnchorStyles.Top, ui =>
        {
            for (int i = 0; i < 4; i++)
            {
                Control edge = GetEdge(ui, i);
                Assert.Equal(AccessibleRole.CheckButton, edge.AccessibleRole);
                Assert.False(string.IsNullOrEmpty(edge.AccessibleName));

                bool solid = i == 1;
                Assert.Equal(solid, edge.AccessibilityObject.State.HasFlag(AccessibleStates.Checked));
                Assert.Equal(solid ? SR.AccessibleActionUncheck : SR.AccessibleActionCheck, edge.AccessibilityObject.DefaultAction);

                Click(edge);
                Assert.Equal(!solid, edge.AccessibilityObject.State.HasFlag(AccessibleStates.Checked));
                Assert.Equal(solid ? SR.AccessibleActionCheck : SR.AccessibleActionUncheck, edge.AccessibilityObject.DefaultAction);
            }
        });
    }

    // Scenario: The four edges expose distinct accessible names, and the drop-down itself has an accessible name.
    [WinFormsFact]
    public void AnchorEditor_EdgeAccessibleNames_AreDistinct()
    {
        Edit(AnchorStyles.Top, ui =>
        {
            Assert.False(string.IsNullOrEmpty(ui.AccessibleName));
            string?[] names = [GetEdge(ui, 0).AccessibleName, GetEdge(ui, 1).AccessibleName, GetEdge(ui, 2).AccessibleName, GetEdge(ui, 3).AccessibleName];
            Assert.Equal(4, names.Distinct().Count());
        });
    }

    // Scenario: The same editor instance reuses its drop-down across edits, and each new edit starts from the value
    // passed in rather than from toggles or a cancel left over from the previous edit.
    [WinFormsFact]
    public void AnchorEditor_EditValueTwice_SecondEditIsIndependent()
    {
        AnchorEditor editor = new();
        FakeEditorService service = new()
        {
            OnDropDown = ui =>
            {
                Click(GetEdge(ui, 2));
                Click(GetEdge(ui, 0));
            }
        };

        Assert.Equal(AnchorStyles.Top | AnchorStyles.Right, editor.EditValue(null, new FakeServiceProvider(service), AnchorStyles.Top | AnchorStyles.Left));
        Control? firstUi = service.LastControl;

        service.OnDropDown = ui => Click(GetEdge(ui, 3));
        Assert.Equal(AnchorStyles.Bottom | AnchorStyles.Left, editor.EditValue(null, new FakeServiceProvider(service), AnchorStyles.Left));
        Assert.Same(firstUi, service.LastControl);

        service.OnDropDown = ui => Press(ui, 0, Keys.Escape);
        Assert.Equal(AnchorStyles.Top, editor.EditValue(null, new FakeServiceProvider(service), AnchorStyles.Top));
        Assert.Equal(3, service.DropDownCount);
        firstUi?.Dispose();
    }

    // Scenario: After an earlier edit changed the edges, cancelling a later edit that starts from a non-anchor value
    // returns the default Top | Left anchor rather than the earlier edit's result.
    [WinFormsFact]
    public void AnchorEditor_EscapeAfterPreviousEdit_NonAnchorValueUsesDefaultAnchor()
    {
        AnchorEditor editor = new();
        FakeEditorService service = new() { OnDropDown = ui => Click(GetEdge(ui, 3)) };

        Assert.Equal(AllEdges, editor.EditValue(null, new FakeServiceProvider(service), AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right));

        service.OnDropDown = ui => Press(ui, 0, Keys.Escape);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, editor.EditValue(null, new FakeServiceProvider(service), null));
        service.LastControl?.Dispose();
    }

    // Scenario: If the editor service cannot provide the drop-down host, the value is returned unchanged and no drop-down is shown.
    [WinFormsFact]
    public void AnchorEditor_EditValue_NoEditorService_ReturnsOriginalValue()
    {
        AnchorEditor editor = new();

        Assert.Equal(AnchorStyles.Right, editor.EditValue(null, new FakeServiceProvider(null), AnchorStyles.Right));
    }

    private static (object? Result, FakeEditorService Service) Edit(object? value, Action<Control> drive)
    {
        AnchorEditor editor = new();
        FakeEditorService service = new() { OnDropDown = drive };
        object? result = editor.EditValue(null, new FakeServiceProvider(service), value);
        service.LastControl?.Dispose();
        return (result, service);
    }

    private static Control GetEdge(Control ui, int index)
    {
        FieldInfo field = ui.GetType().GetField(s_edgeFields[index], BindingFlags.NonPublic | BindingFlags.Instance)!;
        return (Control)field.GetValue(ui)!;
    }

    private static bool IsSolid(Control edge) =>
        (bool)edge.GetType().GetProperty("IsSolid", BindingFlags.Public | BindingFlags.Instance)!.GetValue(edge)!;

    private static void Click(Control edge)
    {
        MethodInfo method = typeof(Control).GetMethod("OnMouseDown", BindingFlags.NonPublic | BindingFlags.Instance)!;
        method.Invoke(edge, [new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0)]);
    }

    private static bool Press(Control ui, int edgeIndex, Keys keyData) => ProcessDialogKey(GetEdge(ui, edgeIndex), keyData);

    private static bool ProcessDialogKey(Control edge, Keys keyData) =>
        (bool)typeof(Control).GetMethod("ProcessDialogKey", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(edge, [keyData])!;

    private static bool ProcessDialogChar(Control edge, char charCode) =>
        (bool)typeof(Control).GetMethod("ProcessDialogChar", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(edge, [charCode])!;

    private sealed class FakeServiceProvider(IWindowsFormsEditorService? service) : IServiceProvider
    {
        public object? GetService(Type serviceType) => serviceType == typeof(IWindowsFormsEditorService) ? service : null;
    }

    private sealed class FakeEditorService : IWindowsFormsEditorService
    {
        public Action<Control>? OnDropDown { get; set; }

        public Control? LastControl { get; private set; }

        public int DropDownCount { get; private set; }

        public int CloseCount { get; private set; }

        public void CloseDropDown() => CloseCount++;

        public void DropDownControl(Control control)
        {
            DropDownCount++;
            LastControl = control;
            OnDropDown?.Invoke(control);
        }

        public DialogResult ShowDialog(Form dialog) => throw new NotSupportedException();
    }
}
