// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Imaging;

namespace System.Drawing.Design;

/// <summary>
///  Provides a base class for editors that may provide users with a user interface to visually edit the values
///  of the supported type or types.
/// </summary>
public class UITypeEditor
{
    // Feature switch, when set to false, UITypeEditor is not supported in trimmed applications.
    [FeatureSwitchDefinition("System.Drawing.Design.UITypeEditor.IsSupported")]
#pragma warning disable IDE0075 // Simplify conditional expression - the simpler expression is hard to read
    private static bool IsSupported { get; } =
        AppContext.TryGetSwitch("System.Drawing.Design.UITypeEditor.IsSupported", out bool isSupported)
            ? isSupported
            : true;
#pragma warning restore IDE0075

    static UITypeEditor()
    {
        // Trimming doesn't support UITypeEditor
        if (!IsSupported)
        {
            return;
        }

        // Our set of intrinsic editors.
        Hashtable intrinsicEditors = new Hashtable
        {
            // System.ComponentModel type Editors
            [typeof(DateTime)] = $"System.ComponentModel.Design.DateTimeEditor, {AssemblyRef.SystemDesign}",
            [typeof(Array)] = $"System.ComponentModel.Design.ArrayEditor, {AssemblyRef.SystemDesign}",
            [typeof(IList)] = $"System.ComponentModel.Design.CollectionEditor, {AssemblyRef.SystemDesign}",
            [typeof(ICollection)] = $"System.ComponentModel.Design.CollectionEditor, {AssemblyRef.SystemDesign}",
            [typeof(byte[])] = $"System.ComponentModel.Design.BinaryEditor, {AssemblyRef.SystemDesign}",
            [typeof(Stream)] = $"System.ComponentModel.Design.BinaryEditor, {AssemblyRef.SystemDesign}",

            // System.Windows.Forms type Editors
            [typeof(string[])] = $"System.Windows.Forms.Design.StringArrayEditor, {AssemblyRef.SystemDesign}",
            [typeof(Collection<string>)] = $"System.Windows.Forms.Design.StringCollectionEditor, {AssemblyRef.SystemDesign}",
            [typeof(StringCollection)] = $"System.Windows.Forms.Design.StringCollectionEditor, {AssemblyRef.SystemDesign}",

            // System.Drawing.Design type Editors
            [typeof(Bitmap)] = $"System.Drawing.Design.BitmapEditor, {AssemblyRef.SystemDrawingDesign}",
            [typeof(Color)] = $"System.Drawing.Design.ColorEditor, {AssemblyRef.SystemDrawingDesign}",
            [typeof(ContentAlignment)] = $"System.Drawing.Design.ContentAlignmentEditor, {AssemblyRef.SystemDrawingDesign}",
            [typeof(Font)] = $"System.Drawing.Design.FontEditor, {AssemblyRef.SystemDrawingDesign}",
            // No way to add Font.Name and associate it with FontNameEditor.
            [typeof(Icon)] = $"System.Drawing.Design.IconEditor, {AssemblyRef.SystemDrawingDesign}",
            [typeof(Image)] = $"System.Drawing.Design.ImageEditor, {AssemblyRef.SystemDrawingDesign}",
            [typeof(Metafile)] = $"System.Drawing.Design.MetafileEditor, {AssemblyRef.SystemDrawingDesign}",
        };

        // Add our intrinsic editors to TypeDescriptor.
        TypeDescriptor.AddEditorTable(typeof(UITypeEditor), intrinsicEditors);
    }

    public UITypeEditor()
    {
        if (!IsSupported)
        {
            throw new NotSupportedException(string.Format(SR.ControlNotSupportedInTrimming, nameof(UITypeEditor)));
        }
    }

    /// <summary>
    ///  Determines if drop-down editors should be resizable by the user.
    /// </summary>
    public virtual bool IsDropDownResizable => false;

    /// <summary>
    ///  Edits the specified value using the editor style provided by <see cref="GetEditStyle()"/>.
    /// </summary>
    /// <param name="provider">An <see cref="IServiceProvider" /> that this editor can use to obtain services.</param>
    /// <param name="value">The object to edit.</param>
    public object? EditValue(IServiceProvider provider, object? value) => EditValue(null, provider, value);

    /// <summary>
    ///  Edits the specified value using the editor style provided by <see cref="GetEditStyle()"/>.
    /// </summary>
    /// <param name="context">
    ///  The <see cref="ITypeDescriptorContext" /> that can be used to gain additional context information.
    /// </param>
    /// <param name="provider">The <see cref="IServiceProvider" /> that this editor can use to obtain services.</param>
    /// <param name="value">The object to edit.</param>
    public virtual object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value) => value;

    /// <summary>
    ///  Gets the <see cref="UITypeEditorEditStyle"/> of the Edit method.
    /// </summary>
    public UITypeEditorEditStyle GetEditStyle() => GetEditStyle(null);

    /// <summary>
    ///  Gets a value indicating whether this editor supports painting a representation of an object's value.
    /// </summary>
    public bool GetPaintValueSupported() => GetPaintValueSupported(null);

    /// <summary>
    ///  Gets a value indicating whether this editor supports painting a representation of an object's value.
    /// </summary>
    /// <param name="context">
    ///  The <see cref="ITypeDescriptorContext" /> that can be used to gain additional context information.
    /// </param>
    public virtual bool GetPaintValueSupported(ITypeDescriptorContext? context) => false;

    /// <summary>
    ///  Gets the editing style of the Edit method.
    /// </summary>
    /// <param name="context">
    ///  The <see cref="ITypeDescriptorContext" /> that can be used to gain additional context information.
    /// </param>
    public virtual UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.None;

    /// <summary>
    ///  Paints a representative value of the specified object to the specified canvas.
    /// </summary>
    /// <param name="value">The object whose value this type editor will display. </param>
    /// <param name="canvas">A drawing canvas on which to paint the representation of the object's value. </param>
    /// <param name="rectangle">A <see cref="Rectangle" /> within whose boundaries to paint the value. </param>
    public void PaintValue(object? value, Graphics canvas, Rectangle rectangle)
        => PaintValue(new PaintValueEventArgs(null, value, canvas, rectangle));

    /// <summary>
    ///  Paints a representative value of the specified object to the specified canvas.
    /// </summary>
    /// <param name="e">A <see cref="PaintValueEventArgs" /> that indicates what to paint and where to paint it. </param>
    public virtual void PaintValue(PaintValueEventArgs e) { }
}
