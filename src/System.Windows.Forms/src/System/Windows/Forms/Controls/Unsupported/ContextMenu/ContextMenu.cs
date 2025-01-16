// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

/// <summary>
///  This type is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code.
/// </summary>
[DefaultEvent(nameof(Popup))]
[Obsolete(
    Obsoletions.ContextMenuMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
public class ContextMenu : Menu
{
    public ContextMenu() : base(items: default) => throw new PlatformNotSupportedException();

    public ContextMenu(MenuItem[] menuItems) : base(items: menuItems) => throw new PlatformNotSupportedException();

    [Localizable(true)]
    [DefaultValue(RightToLeft.No)]
    public virtual RightToLeft RightToLeft
    {
        get => throw null;
        set { }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Control SourceControl => throw null;

    public event EventHandler Popup
    {
        add { }
        remove { }
    }

    public event EventHandler Collapse
    {
        add { }
        remove { }
    }

    protected internal virtual void OnCollapse(EventArgs e) { }

    protected internal virtual void OnPopup(EventArgs e) { }

    protected internal virtual bool ProcessCmdKey(ref Message msg, Keys keyData, Control control) => throw null;

    public void Show(Control control, Point pos) { }

    public void Show(Control control, Point pos, LeftRightAlignment alignment) { }
}
