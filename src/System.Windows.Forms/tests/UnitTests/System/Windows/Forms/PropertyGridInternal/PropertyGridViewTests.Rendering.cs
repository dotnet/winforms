// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Metafiles;
using System.Windows.Forms.PropertyGridInternal;

namespace System.Windows.Forms.Tests;

public partial class PropertyGridViewTests
{
    [WinFormsFact]
    public void PropertyGridView_Render_Labels_Values_Correctly()
    {
        Point pt = default;
        using PropertyGrid propertyGrid = new()
        {
            SelectedObject = pt,
            Size = new(300, 200),
            Visible = true
        };

        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;

        // For us to be able to render PropertyGridView and its values
        // PropertyGrid must be visible and have a valid handle.
        // To be Visible is must either be visible (we don't want this in tests) or
        // have no parent - so we can't add it to another control (such as a form).
        propertyGrid.CreateControl();

        using EmfScope emf = new();
        DeviceContextState state = new(emf);

        propertyGridView.PrintToMetafile(emf);

        // Only care about text: labels and values
        emf.Validate(
            state,

            // Category: Misc
            Validate.SkipTo(
                Validate.TextOut("Misc", new(25, 4, 25, 14), stateValidators: State.FontFace(Control.DefaultFont.Name))),

            // Value for "X"
            Validate.SkipTo(
                Validate.TextOut(" ", new(145, 22, 5, 14))), // blank??
            Validate.SkipTo(
                Validate.TextOut(pt.X.ToString(), new(145, 22, 5, 14), stateValidators: State.FontFace(Control.DefaultFont.Name))),
            // Label for "X"
            Validate.SkipTo(
                Validate.TextOut(nameof(Point.X), new(25, 23, 6, 14), stateValidators: State.FontFace(Control.DefaultFont.Name))),

            // Value for "Y"
            Validate.SkipTo(
                Validate.TextOut(" ", new(145, 41, 5, 14))), // blank??
            Validate.SkipTo(
                Validate.TextOut(pt.Y.ToString(), new(145, 41, 5, 14), stateValidators: State.FontFace(Control.DefaultFont.Name))),
            // Label for "Y"
            Validate.SkipTo(
                Validate.TextOut(nameof(Point.Y), new(25, 42, 6, 14), stateValidators: State.FontFace(Control.DefaultFont.Name))),

           Validate.SkipAll());
    }
}
