﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Makes the DesignerOptions queryable through the IDesignerOption service.
/// </summary>
public class WindowsFormsDesignerOptionService : DesignerOptionService
{
    private DesignerOptions? _options;

    public virtual DesignerOptions CompatibilityOptions => _options ??= new DesignerOptions();

    /// <summary>
    ///  This method is called on demand the first time a user asks for child options or
    ///  properties of an options collection.
    /// </summary>
    protected override void PopulateOptionCollection(DesignerOptionCollection options)
    {
        if (options is null || options.Parent is not null)
        {
            return;
        }

        if (CompatibilityOptions is not null)
        {
            CreateOptionCollection(options, "DesignerOptions", CompatibilityOptions);
        }
    }
}
