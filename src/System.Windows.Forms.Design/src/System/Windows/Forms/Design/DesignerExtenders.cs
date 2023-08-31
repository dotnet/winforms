// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class provides the Modifiers property to components.  It is shared between
///  the document designer and the component document designer.
/// </summary>
internal partial class DesignerExtenders
{
    private IExtenderProvider[] providers;
    private IExtenderProviderService extenderService;

    /// <summary>
    ///  This is called by a root designer to add the correct extender providers.
    /// </summary>
    public DesignerExtenders(IExtenderProviderService ex)
    {
        extenderService = ex;
        providers ??= new IExtenderProvider[]
            {
                new NameExtenderProvider(),
                new NameInheritedExtenderProvider()
            };

        for (int i = 0; i < providers.Length; i++)
        {
            ex.AddExtenderProvider(providers[i]);
        }
    }

    /// <summary>
    ///  This is called at the appropriate time to remove any extra extender
    ///  providers previously added to the designer host.
    /// </summary>
    public void Dispose()
    {
        if (extenderService is not null && providers is not null)
        {
            for (int i = 0; i < providers.Length; i++)
            {
                extenderService.RemoveExtenderProvider(providers[i]);
            }

            providers = null;
            extenderService = null;
        }
    }
}
