// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace System.Resources;

public partial class ResXFileRef
{
    public class Converter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            => sourceType == typeof(string);

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
            => destinationType == typeof(string);

        public override object? ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object? value,
            Type destinationType)
        {
            object? created = null;
            if (destinationType == typeof(string) && value is ResXFileRef fileRef)
            {
                created = fileRef.ToString();
            }

            return created;
        }

        // "value" is the parameter name of ConvertFrom, which calls this method.
        [return: NotNullIfNotNull(nameof(stringValue))]
        internal static string[]? ParseResxFileRefString(string stringValue)
        {
            if (stringValue is null)
            {
                return null;
            }

            string[]? result;

            stringValue = stringValue.Trim();
            string fileName;
            string remainingString;
            if (stringValue.StartsWith('"'))
            {
                int lastIndexOfQuote = stringValue.LastIndexOf('"');
                if (lastIndexOfQuote - 1 < 0)
                {
                    throw new ArgumentException(null, nameof(stringValue));
                }

                // Remove the quotes in " ..... "
                fileName = stringValue[1..lastIndexOfQuote];
                if (lastIndexOfQuote + 2 > stringValue.Length)
                {
                    throw new ArgumentException(null, nameof(stringValue));
                }

                remainingString = stringValue[(lastIndexOfQuote + 2)..];
            }
            else
            {
                int nextSemiColumn = stringValue.IndexOf(';');
                if (nextSemiColumn == -1)
                {
                    throw new ArgumentException(null, nameof(stringValue));
                }

                fileName = stringValue[..nextSemiColumn];
                if (nextSemiColumn + 1 > stringValue.Length)
                {
                    throw new ArgumentException(null, nameof(stringValue));
                }

                remainingString = stringValue[(nextSemiColumn + 1)..];
            }

            string[] parts = remainingString.Split(';');
            result = parts.Length > 1
                ? ([fileName, parts[0], parts[1]])
                : parts.Length > 0 ? ([fileName, parts[0]]) : ([fileName]);

            return result;
        }

        public override object? ConvertFrom(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object value)
        {
            if (value is not string stringValue)
            {
                return null;
            }

            string[] parts = ParseResxFileRefString(stringValue);
            string fileName = parts[0];
            Type? toCreate = Type.GetType(parts[1], true);

            // Special case string and byte[].
            if (toCreate == typeof(string))
            {
                // We have a string, now we need to check the encoding.
                Encoding textFileEncoding = parts.Length > 2 ? Encoding.GetEncoding(parts[2]) : Encoding.Default;
                using StreamReader sr = new(fileName, textFileEncoding);
                return sr.ReadToEnd();
            }

            // This is a regular file, we call its constructor with a stream as a parameter
            // or if it's a byte array we just return that.
            byte[]? temp = null;

            using (FileStream fileStream = new(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                temp = new byte[fileStream.Length];
                fileStream.ReadExactly(temp, 0, (int)fileStream.Length);
            }

            if (toCreate == typeof(byte[]))
            {
                return temp;
            }

            MemoryStream memoryStream = new(temp);
            if (toCreate == typeof(MemoryStream))
            {
                return memoryStream;
            }

            if (toCreate == typeof(Bitmap) && fileName.EndsWith(".ico", StringComparison.Ordinal))
            {
                // We special case the .ico bitmaps because GDI+ destroy the alpha channel component and
                // we don't want that to happen.
                Icon ico = new(memoryStream);
                return ico.ToBitmap();
            }

            return toCreate is null
                ? null
                : Activator.CreateInstance(
                    toCreate,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance,
                    null,
                    [memoryStream],
                    null);
        }
    }
}
