//------------------------------------------------------------------------------
// <copyright file="FontNameEditor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

/*
 */
namespace System.Drawing.Design
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.IO;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using System.Windows.Forms.ComponentModel;
    using System.Windows.Forms.Design;

    /// <internalonly/>
    /// <include file='doc\FontNameEditor.uex' path='docs/doc[@for="FontNameEditor"]/*' />
    /// <devdoc>
    ///    <para>Provides an editor that paints a glyph for the font name.</para>
    /// </devdoc>
    [System.Security.SecurityCritical]
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public class FontNameEditor : UITypeEditor
    {

        /// <include file='doc\FontNameEditor.uex' path='docs/doc[@for="FontNameEditor.GetPaintValueSupported"]/*' />
        /// <devdoc>
        ///      Determines if this editor supports the painting of a representation
        ///      of an object's value.
        /// </devdoc>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <include file='doc\FontNameEditor.uex' path='docs/doc[@for="FontNameEditor.PaintValue"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Paints a representative value of the given object to the provided
        ///       canvas. Painting should be done within the boundaries of the
        ///       provided rectangle.
        ///    </para>
        /// </devdoc>

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public override void PaintValue(PaintValueEventArgs e)
        {
            string fontName = e.Value as string;
            if (fontName != null)
            {
                if (fontName == "")
                {
                    // don't draw anything if we don't have a value.
                    return;
                }

                e.Graphics.FillRectangle(SystemBrushes.ActiveCaption, e.Bounds);

                FontFamily family = null;

                try
                {
                    family = new FontFamily(fontName);
                }
                catch
                {
                    // Ignore the exception if the fontName does not exist or is invalid...
                    // we just won't render a preview of the font at all
                }

                if (family != null)
                {
                    // Believe it or not, not all font families have a "normal" face.  Try normal, then italic, 
                    // then bold, then bold italic, then give up.
                    try
                    {
                        DrawFontSample(e, family, FontStyle.Regular);
                    }
                    catch
                    {
                        try
                        {
                            DrawFontSample(e, family, FontStyle.Italic);
                        }
                        catch
                        {
                            try
                            {
                                DrawFontSample(e, family, FontStyle.Bold);
                            }
                            catch
                            {
                                try
                                {
                                    DrawFontSample(e, family, FontStyle.Bold | FontStyle.Italic);
                                }
                                catch
                                {
                                    // No font style we can think of is supported
                                }
                            }
                        }
                    }
                    finally
                    {
                        family.Dispose();
                    }
                }

                e.Graphics.DrawLine(SystemPens.WindowFrame, e.Bounds.Right, e.Bounds.Y, e.Bounds.Right, e.Bounds.Bottom);
            }
        }

        // Tries to render sample of text in specified font and style,
        // throwing exception if specified font does not support that style...
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        static private void DrawFontSample(PaintValueEventArgs e, FontFamily fontFamily, FontStyle fontStyle)
        {
            float fontSize = (float)(e.Bounds.Height / 1.2);

            Font font = new Font(fontFamily, fontSize, fontStyle, GraphicsUnit.Pixel);

            if (font == null)
            {
                return;
            }

            try
            {
                e.Graphics.DrawString("abcd", font, SystemBrushes.ActiveCaptionText, e.Bounds);
            }
            finally
            {
                font.Dispose();
            }
        }

    }
}

