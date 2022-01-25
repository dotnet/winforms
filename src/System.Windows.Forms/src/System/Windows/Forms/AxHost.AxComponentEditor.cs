// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms.Design;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        public class AxComponentEditor : WindowsFormsComponentEditor
        {
#pragma warning disable CA1725 // Parameter names should match base declaration - "obj" and "parent" is how this is documented
            public override bool EditComponent(ITypeDescriptorContext context, object obj, IWin32Window parent)
#pragma warning restore CA1725
            {
                if (obj is AxHost host)
                {
                    try
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in AxComponentEditor.EditComponent");
                        ((Ole32.IOleControlSite)host._oleSite).ShowPropertyFrame();
                        return true;
                    }
                    catch (Exception t)
                    {
                        Debug.Fail(t.ToString());
                        throw;
                    }
                }

                return false;
            }
        }
    }
}
