// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

internal class WebBrowserUriTypeConverter : UriTypeConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, Globalization.CultureInfo? culture, object value)
    {
        // The UriTypeConverter gives back a relative Uri for things like "www.Microsoft.com". If
        // the Uri is relative, we'll try sticking "http://" on the front to see whether that fixes it up.
        Uri? uri = base.ConvertFrom(context, culture, value) as Uri;
        if (uri is not null && !string.IsNullOrEmpty(uri.OriginalString) && !uri.IsAbsoluteUri)
        {
            try
            {
                uri = new Uri($"http://{uri.OriginalString.AsSpan().Trim()}");
            }
            catch (UriFormatException)
            {
                // We can't throw "http://" on the front: just return the original (relative) Uri,
                // which will throw an exception with reasonable text later.
            }
        }

        return uri;
    }
}
