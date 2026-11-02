// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms.Tests.TestResources;

namespace System.Windows.Forms.Tests;

public class AxHostVisualBasic6Tests
{
    [WinFormsFact]
    public void AxHost_SimpleControl_Create()
    {
        if (RuntimeInformation.ProcessArchitecture != Architecture.X86)
        {
            return;
        }

        HWND controlWindow = default;
        using Form form = new();
        using DynamicAxHost control = new(ComClasses.VisualBasicSimpleControl!);
        form.Shown += (object? sender, EventArgs e) =>
        {
            // Capture the hosted VB6 control's HWND while it is active, not the Form's HWND or VB6's parking HWND.
            controlWindow = (HWND)control.Handle;
            form.Close();
        };

        ((ISupportInitialize)control).BeginInit();
        form.Controls.Add(control);
        ((ISupportInitialize)control).EndInit();
        form.ShowDialog();

        Assert.False(controlWindow.IsNull);
        control.Dispose();

        // Disposal must destroy the native control window, not just detach the managed host. The factory used to
        // leak the IUnknown returned by CreateInstance, leaving this HWND alive under VB6's parking window.
        // When the test's STA exited, CVBThreadAction::Stop reset its projects before InitTermUIThread called
        // DeskDestroyParkingHwnd. Destroying the parking window sent WM_DESTROY to the surviving control;
        // CommonGizWndProc called hctl->hxmod->lpmdl->pctlproc through invalid state into non-executable heap memory.
        // This could crash the process after xUnit had already reported Passed. Check while the STA is still alive,
        // without reading control.Handle again (which could recreate a handle). The parking HWND itself was not
        // shown to be invalid; the observed fault was an execute AV in the surviving child's control procedure.
        Assert.False(PInvoke.IsWindow(controlWindow));
    }
}
