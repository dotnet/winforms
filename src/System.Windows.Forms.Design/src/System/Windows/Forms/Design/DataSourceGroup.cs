// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design;

internal abstract class DataSourceGroup
{
    public abstract string Name { get; }

    public abstract Bitmap Image { get; }

    public abstract DataSourceDescriptorCollection DataSources { get; }

    public abstract bool IsDefault { get; }
}
