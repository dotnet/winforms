// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  An Adorner manages a collection of UI-related Glyphs. Each Adorner
///  can be enabled/disabled. Only Enabled Adorners will receive hit test
///  and paint messages from the BehaviorService. An Adorner can be viewed
///  as a proxy between UI-related elements (all Glyphs) and the BehaviorService.
/// </summary>
public sealed class Adorner
{
    private BehaviorService? _behaviorService; // ptr back to the BehaviorService
    private readonly GlyphCollection _glyphs; // collection of Glyphs that this particular Adorner manages

    /// <summary>
    ///  Standard constructor. Creates a new GlyphCollection and by default is enabled.
    /// </summary>
    public Adorner()
    {
        _glyphs = [];
        EnabledInternal = true;
    }

    /// <summary>
    ///  When an Adorner is added to the BehaviorService's AdornerCollection, the collection
    ///  will set this property so that the Adorner can call back to the BehaviorService.
    /// </summary>
    public BehaviorService? BehaviorService
    {
        get => _behaviorService;
        set => _behaviorService = value;
    }

    /// <summary>
    ///  Determines if the BehaviorService will send HitTest and Paint messages to
    ///  the Adorner. This will invalidate behavior service when changed.
    /// </summary>
    public bool Enabled
    {
        get => EnabledInternal;
        set
        {
            if (value != EnabledInternal)
            {
                EnabledInternal = value;
                Invalidate();
            }
        }
    }

    internal bool EnabledInternal { get; set; }

    /// <summary>
    ///  Returns the stronly-typed Glyph collection.
    /// </summary>
    public GlyphCollection Glyphs
    {
        get => _glyphs;
    }

    /// <summary>
    ///  Forces the BehaviorService to refresh its AdornerWindow.
    /// </summary>
    public void Invalidate()
    {
        _behaviorService?.Invalidate();
    }

    /// <summary>
    ///  Forces the BehaviorService to refresh its AdornerWindow within the given Rectangle.
    /// </summary>
    public void Invalidate(Rectangle rectangle)
    {
        _behaviorService?.Invalidate(rectangle);
    }

    /// <summary>
    ///  Forces the BehaviorService to refresh its AdornerWindow within the given Region.
    /// </summary>
    public void Invalidate(Region region)
    {
        _behaviorService?.Invalidate(region);
    }
}
