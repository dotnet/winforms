// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms;

#nullable disable
#pragma warning disable RS0016 // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.StatusBarMessage,
    error: false,
    DiagnosticId = Obsoletions.StatusBarDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public class StatusBar : Control
{
    public StatusBar() : base()
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color BackColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler BackColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Image BackgroundImage
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler BackgroundImageChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler BackgroundImageLayoutChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected override CreateParams CreateParams
    {
        get => base.CreateParams;
    }

    protected override ImeMode DefaultImeMode
    {
        get => ImeMode.Disable;
    }

    protected override Size DefaultSize
    {
        get => new Size(100, 22);
    }

    protected override bool DoubleBuffered
    {
        get => base.DoubleBuffered;
        set => base.DoubleBuffered = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override DockStyle Dock
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Font Font
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler ForeColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    new public ImeMode ImeMode
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler ImeModeChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBarPanelCollection Panels
    {
        get => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string Text
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShowPanels
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool SizingGrip
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    new public bool TabStop
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event StatusBarDrawItemEventHandler DrawItem
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event StatusBarPanelClickEventHandler PanelClick
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler Paint
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    protected override void CreateHandle()
        => base.CreateHandle();

    protected override void Dispose(bool disposing)
        => base.Dispose(disposing);

    protected override void OnHandleCreated(EventArgs e)
        => base.OnHandleCreated(e);

    protected override void OnHandleDestroyed(EventArgs e)
        => base.OnHandleDestroyed(e);

    protected override void OnMouseDown(MouseEventArgs e)
        => base.OnMouseDown(e);

    protected virtual void OnPanelClick(StatusBarPanelClickEventArgs e)
        => throw new PlatformNotSupportedException();

    protected override void OnLayout(LayoutEventArgs levent)
        => base.OnLayout(levent);

    protected virtual void OnDrawItem(StatusBarDrawItemEventArgs sbdievent)
        => throw new PlatformNotSupportedException();

    protected override void OnResize(EventArgs e)
        => base.OnResize(e);

    public override string ToString()
    {
        string s = base.ToString();
        if (Panels is not null)
        {
            s += ", Panels.Count: " + Panels.Count.ToString(CultureInfo.CurrentCulture);
            if (Panels.Count > 0)
                s += ", Panels[0]: " + Panels[0].ToString();
        }

        return s;
    }

    protected override void WndProc(ref Message m)
        => throw new PlatformNotSupportedException();

    [Obsolete(
        Obsoletions.StatusBarPanelCollectionMessage,
        error: false,
        DiagnosticId = Obsoletions.StatusBarPanelCollectionDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public class StatusBarPanelCollection : IList
    {
        public StatusBarPanelCollection(StatusBar owner)
            => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual StatusBarPanel this[int index]
        {
            get => throw new PlatformNotSupportedException();
            set => throw new PlatformNotSupportedException();
        }

        object IList.this[int index]
        {
            get => this[index];
            set => throw new PlatformNotSupportedException();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual StatusBarPanel this[string key]
        {
            get => throw new PlatformNotSupportedException();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Count
        {
            get => throw new PlatformNotSupportedException();
        }

        object ICollection.SyncRoot
        {
            get => this;
        }

        bool ICollection.IsSynchronized
        {
            get => false;
        }

        bool IList.IsFixedSize
        {
            get => false;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsReadOnly
        {
            get => throw new PlatformNotSupportedException();
        }

        public virtual StatusBarPanel Add(string text)
            => throw new PlatformNotSupportedException();

        public virtual int Add(StatusBarPanel value)
            => throw new PlatformNotSupportedException();

        int IList.Add(object value)
            => throw new PlatformNotSupportedException();

        public virtual void AddRange(StatusBarPanel[] panels)
            => throw new PlatformNotSupportedException();

        public bool Contains(StatusBarPanel panel)
            => throw new PlatformNotSupportedException();

        bool IList.Contains(object panel)
            => false;

        public virtual bool ContainsKey(string key)
            => throw new PlatformNotSupportedException();

        public int IndexOf(StatusBarPanel panel)
            => throw new PlatformNotSupportedException();

        int IList.IndexOf(object panel)
            => throw new PlatformNotSupportedException();

        public virtual int IndexOfKey(string key)
            => throw new PlatformNotSupportedException();

        public virtual void Insert(int index, StatusBarPanel value)
            => throw new PlatformNotSupportedException();

        void IList.Insert(int index, object value)
            => throw new PlatformNotSupportedException();

        public virtual void Clear()
            => throw new PlatformNotSupportedException();

        public virtual void Remove(StatusBarPanel value)
            => throw new PlatformNotSupportedException();

        void IList.Remove(object value)
            => Remove((StatusBarPanel)value);

        public virtual void RemoveAt(int index)
            => throw new PlatformNotSupportedException();

        public virtual void RemoveByKey(string key)
            => throw new PlatformNotSupportedException();

        void ICollection.CopyTo(Array dest, int index)
            => throw new PlatformNotSupportedException();

        public IEnumerator GetEnumerator()
            => throw new PlatformNotSupportedException();
    }
}
