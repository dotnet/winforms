﻿using System.Collections;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class PrintDialogDesigner : ComponentDesigner
{
    /// <summary>
    ///  This method is called when a component is first initialized, typically after being first added
    ///  to a design surface.  The defaultValues property contains a name/value dictionary of default
    ///  values that should be applied to properties.  This dictionary may be null if no default values
    ///  are specified.  You may perform any initialization of this component that you like, and you
    ///  may even ignore the defaultValues dictionary altogether if you wish.
    ///  The default implementation of this method does nothing.
    /// </summary>
    public override void InitializeNewComponent(IDictionary defaultValues)
    {
        PrintDialog? pd = Component as PrintDialog;
        if (pd is not null)
        {
            pd.UseEXDialog = true;
        }
    }
}
