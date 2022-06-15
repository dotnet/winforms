// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Drawing.Design
{
    /// <summary>
    ///  Provides an editor that provides a way to visually edit the values of a COM2 type.
    /// </summary>
    internal class Com2ExtendedUITypeEditor : UITypeEditor
    {
        private readonly UITypeEditor? _innerEditor;

        public Com2ExtendedUITypeEditor(UITypeEditor baseTypeEditor)
        {
            _innerEditor = baseTypeEditor;
        }

        public Com2ExtendedUITypeEditor(Type baseType)
        {
            _innerEditor = (UITypeEditor?)TypeDescriptor.GetEditor(baseType, typeof(UITypeEditor));
        }

        public UITypeEditor? InnerEditor
        {
            get
            {
                return _innerEditor;
            }
        }

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            if (_innerEditor is not null)
            {
                return _innerEditor.EditValue(context, provider, value);
            }
            else
            {
                return base.EditValue(context, provider, value);
            }
        }

        /// <summary>
        ///  Determines if this editor supports the painting of a representation
        ///  of an object's value.
        /// </summary>
        public override bool GetPaintValueSupported(ITypeDescriptorContext? context)
        {
            if (_innerEditor is not null)
            {
                return _innerEditor.GetPaintValueSupported(context);
            }

            return base.GetPaintValueSupported(context);
        }

        /// <summary>
        ///  Retrieves the editing style of the Edit method.  If the method
        ///  is not supported, this will return None.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            if (_innerEditor is not null)
            {
                return _innerEditor.GetEditStyle(context);
            }

            return base.GetEditStyle(context);
        }

        /// <summary>
        ///  Paints a representative value of the given object to the provided
        ///  canvas.  Painting should be done within the boundaries of the
        ///  provided rectangle.
        /// </summary>
        public override void PaintValue(PaintValueEventArgs e)
        {
            if (_innerEditor is not null)
            {
                _innerEditor.PaintValue(e);
            }

            base.PaintValue(e);
        }
    }
}
