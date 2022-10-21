// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2AboutBoxPropertyDescriptor : Com2PropertyDescriptor
    {
        private TypeConverter _converter;
        private UITypeEditor _editor;

        public Com2AboutBoxPropertyDescriptor()
            : base(Ole32.DispatchID.ABOUTBOX, "About",
                  new Attribute[]
                  {
                      new DispIdAttribute((int)Ole32.DispatchID.ABOUTBOX),
                      DesignerSerializationVisibilityAttribute.Hidden,
                      new DescriptionAttribute(SR.AboutBoxDesc),
                      new ParenthesizePropertyNameAttribute(true)
                  },
                  true, typeof(string), null, false)
        {
        }

        public override Type ComponentType => typeof(Oleaut32.IDispatch);

        public override TypeConverter Converter
        {
            [RequiresUnreferencedCode(TrimmingConstants.PropertyDescriptorPropertyTypeMessage)]
            get => _converter ??= new TypeConverter();
        }

        public override bool IsReadOnly => true;

        public override Type PropertyType => typeof(string);

        public override bool CanResetValue(object component) => false;

        [RequiresUnreferencedCode($"{TrimmingConstants.EditorRequiresUnreferencedCode} {TrimmingConstants.PropertyDescriptorPropertyTypeMessage}")]
        public override object GetEditor(Type editorBaseType)
        {
            if (editorBaseType == typeof(UITypeEditor))
            {
                _editor ??= new AboutBoxUITypeEditor();
            }

            return _editor;
        }

        public override object GetValue(object component) => string.Empty;

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value) => throw new ArgumentException();

        public override bool ShouldSerializeValue(object component) => false;

        public class AboutBoxUITypeEditor : UITypeEditor
        {
            public override unsafe object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                object component = context.Instance;

                if (Marshal.IsComObject(component) && component is Oleaut32.IDispatch pDisp)
                {
                    EXCEPINFO pExcepInfo = new();
                    DISPPARAMS dispParams = new();
                    Guid g = Guid.Empty;
                    HRESULT hr = pDisp.Invoke(
                        Ole32.DispatchID.ABOUTBOX,
                        &g,
                        PInvoke.GetThreadLocale(),
                        DISPATCH_FLAGS.DISPATCH_METHOD,
                        &dispParams,
                        null,
                        &pExcepInfo,
                        null);
                    Debug.Assert(hr.Succeeded, "Failed to launch about box.");
                }

                return value;
            }

            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }
        }
    }
}
