// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;

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
        get => throw new PlatformNotSupportedException();
    }

    protected override ImeMode DefaultImeMode
    {
        get => throw new PlatformNotSupportedException();
    }

    protected override Size DefaultSize
    {
        get => throw new PlatformNotSupportedException();
    }

    protected override bool DoubleBuffered
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
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
        => throw new PlatformNotSupportedException();

    protected override void Dispose(bool disposing)
        => throw new PlatformNotSupportedException();

    protected override void OnHandleCreated(EventArgs e)
        => throw new PlatformNotSupportedException();

    protected override void OnHandleDestroyed(EventArgs e)
        => throw new PlatformNotSupportedException();

    protected override void OnMouseDown(MouseEventArgs e)
        => throw new PlatformNotSupportedException();

    protected virtual void OnPanelClick(StatusBarPanelClickEventArgs e)
        => throw new PlatformNotSupportedException();

    protected override void OnLayout(LayoutEventArgs levent)
        => throw new PlatformNotSupportedException();

    protected virtual void OnDrawItem(StatusBarDrawItemEventArgs sbdievent)
        => throw new PlatformNotSupportedException();

    protected override void OnResize(EventArgs e)
        => throw new PlatformNotSupportedException();

    public override string ToString()
        => throw new PlatformNotSupportedException();

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
            get => throw new PlatformNotSupportedException();
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
            get => throw new PlatformNotSupportedException();
        }

        bool ICollection.IsSynchronized
        {
            get => throw new PlatformNotSupportedException();
        }

        bool IList.IsFixedSize
        {
            get => throw new PlatformNotSupportedException();
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
            => throw new PlatformNotSupportedException();

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
            => throw new PlatformNotSupportedException();

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
