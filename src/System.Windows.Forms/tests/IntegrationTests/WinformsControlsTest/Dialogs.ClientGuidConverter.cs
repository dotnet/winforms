// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Globalization;

namespace WinFormsControlsTest;

public partial class Dialogs
{
    /// <summary>
    /// Provides a predefined set of GUIDs to chose from in order to configure <see cref="ExposedClientGuidMetadata.ClientGuid"/>.
    /// </summary>
    private sealed class ClientGuidConverter : GuidConverter
    {
        private StandardValuesCollection? _values;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => true;

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            // base class is for plain Guid, but ClientGuid is nullable Guid, which we need to implement separately
            if ((value is null) || (value is string str && str.Length == 0))
                return null;

            return base.ConvertFrom(context, culture, value);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
        {
            // chose from two GUIDs we pregenerated for testing
            _values ??= new StandardValuesCollection(new Guid?[]
            {
                    null,
                    new Guid("38EA9AE9-13BE-4992-9482-DAD370894BBD"),
                    new Guid("46DFEE70-A89E-4D9A-8842-6D46DBC1F195"),
            });

            return _values;
        }
    }
}
