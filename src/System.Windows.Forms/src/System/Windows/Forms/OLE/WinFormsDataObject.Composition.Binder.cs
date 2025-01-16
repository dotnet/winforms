// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Private.Windows.Core.OLE;
using System.Reflection.Metadata;

namespace System.Windows.Forms;

internal partial class WinFormsDataObject
{
    internal unsafe partial class Composition
    {
        internal sealed class Binder : DesktopDataObject.Composition.Binder
        {
            public Binder(Type type, Func<TypeName, Type>? resolver, bool legacyMode) : base(type, resolver, legacyMode)
            {
            }

            public override Type[] AdditionalSupportedTypes() => [typeof(ImageListStreamer), typeof(Bitmap)];
        }
    }
}
