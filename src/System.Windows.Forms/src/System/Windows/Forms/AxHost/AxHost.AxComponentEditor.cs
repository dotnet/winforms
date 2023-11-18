// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms.Design;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    public class AxComponentEditor : WindowsFormsComponentEditor
    {
#pragma warning disable CA1725 // Parameter names should match base declaration - "obj" and "parent" is how this is documented
        public override bool EditComponent(ITypeDescriptorContext? context, object obj, IWin32Window? parent)
#pragma warning restore CA1725
        {
            if (obj is AxHost host)
            {
                ((IOleControlSite.Interface)host._oleSite).ShowPropertyFrame().ThrowOnFailure();
                return true;
            }

            return false;
        }
    }
}
