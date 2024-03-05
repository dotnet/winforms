// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API
[Obsolete("MainMenu has been deprecated. Use MenuStrip instead.")]
public class MainMenu : Menu
{
#nullable disable
    internal Form form;
    internal Form ownerForm;  // this is the form that created this menu, and is the only form allowed to dispose it.

    /// <summary>
    ///  Creates a new MainMenu control.
    /// </summary>
    public MainMenu()
        : base(null)
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='MainMenu'/> class with the specified container.
    /// </summary>
    public MainMenu(IContainer container) : this()
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Creates a new MainMenu control with the given items to start
    ///  with.
    /// </summary>
    public MainMenu(MenuItem[] items)
        : base(items)
    {
        throw new PlatformNotSupportedException();
    }

    [SRDescription("Occurs when the main menu collapses.")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler Collapse
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  This is used for international applications where the language
    ///  is written from RightToLeft. When this property is true,
    ///  text alignment and reading order will be from right to left.
    /// </summary>
    // Add an AmbientValue attribute so that the Reset context menu becomes available in the Property Grid.
    [Localizable(true), AmbientValue(RightToLeft.Inherit),
    SRDescription("Indicates if this menu should display right to left"),
    Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public virtual RightToLeft RightToLeft
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    /// <summary>
    ///  Creates a new MainMenu object which is a dupliate of this one.
    /// </summary>
    public virtual MainMenu CloneMenu()
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Indicates which form in which we are currently residing [if any]
    /// </summary>
    public Form GetForm()
    {
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString()
    {
        throw new PlatformNotSupportedException();
    }
}
