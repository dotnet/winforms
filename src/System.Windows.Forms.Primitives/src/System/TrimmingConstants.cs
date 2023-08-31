// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static class TrimmingConstants
{
    internal const string AttributesRequiresUnreferencedCodeMessage = "Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.";

    internal const string BindingListViewFilterMessage = "Members of types used in the filter expression might be trimmed.";

    internal const string EditorRequiresUnreferencedCode = "Editors registered in TypeDescriptor.AddEditorTable may be trimmed.";

    internal const string EventDescriptorRequiresUnreferencedCodeMessage = "The built-in EventDescriptor implementation uses Reflection which requires unreferenced code.";

    internal const string FilterRequiresUnreferencedCodeMessage = "The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.";

    internal const string PropertyDescriptorPropertyTypeMessage = "PropertyDescriptor's PropertyType cannot be statically discovered.";

    internal const string SiteNameMessage = "The Type of components in the container cannot be statically discovered to validate the name.";

    internal const string TypeConverterGetPropertiesMessage = "The Type of value cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.";

    internal const string TypeOrValueNotDiscoverableMessage = "The Type of value cannot be statically discovered";
}
