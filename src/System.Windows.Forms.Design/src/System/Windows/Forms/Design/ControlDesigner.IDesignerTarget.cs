// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

public partial class ControlDesigner
{
    private interface IDesignerTarget : IDisposable
    {
        void DefWndProc(ref Message m);
    }
}
