// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design;

/// <summary>
///  Generic editor for editing binary data. This presents a hex editing window to the user.
/// </summary>
public sealed partial class BinaryEditor : UITypeEditor
{
    private const string HELP_KEYWORD = "System.ComponentModel.Design.BinaryEditor";
    private ITypeDescriptorContext? _context;
    private BinaryUI? _binaryUI;

    internal object? GetService(Type serviceType)
    {
        if (_context is null)
        {
            return null;
        }

        return _context.TryGetService(out IDesignerHost? designerhost)
            ? designerhost.GetService(serviceType)
            : _context.GetService(serviceType);
    }

    private bool TryGetService<T>([NotNullWhen(true)] out T? service) where T : class
    {
        service = GetService(typeof(T)) as T;
        return service is not null;
    }

    /// <summary>
    ///  Converts the given object to an array of bytes to be manipulated by the editor. The default implementation
    ///  of this supports byte[] and <see cref="Stream"/> objects.
    /// </summary>
    internal static byte[] ConvertToBytes(object value)
    {
        if (value is Stream stream)
        {
            stream.Position = 0;
            int byteCount = (int)(stream.Length - stream.Position);
            byte[] bytes = new byte[byteCount];
            stream.ReadExactly(bytes, 0, byteCount);
            return bytes;
        }

        if (value is byte[] byteArray)
        {
            return byteArray;
        }

        if (value is string stringValue)
        {
            int size = stringValue.Length * 2;
            byte[] buffer = new byte[size];
            Encoding.Unicode.GetBytes(stringValue.AsSpan(), buffer.AsSpan());
            return buffer;
        }

        Debug.Fail($"No conversion from {value?.GetType().FullName ?? "null"} to byte[]");
        return null;
    }

    /// <summary>
    ///  Converts the given byte array back into a native object. If the object itself needs to be replaced (as is
    ///  the case for arrays), then a new object may be assigned out through <paramref name="value"/>.
    /// </summary>
    internal static void ConvertToValue(byte[] bytes, ref object value)
    {
        if (value is Stream stream)
        {
            stream.Position = 0;
            stream.Write(bytes, 0, bytes.Length);
        }
        else if (value is byte[])
        {
            value = bytes;
        }
        else if (value is string)
        {
            value = BitConverter.ToString(bytes);
        }
        else
        {
            Debug.Fail($"No conversion from byte[] to {value?.GetType().FullName ?? "null"}");
        }
    }

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (provider is null)
        {
            return value;
        }

        _context = context;

        if (!provider.TryGetService(out IWindowsFormsEditorService? editorService))
        {
            return value;
        }

        // Child modal dialog- launching in SystemAware mode.
        _binaryUI ??= ScaleHelper.InvokeInSystemAwareContext(() => new BinaryUI(this));

        _binaryUI.Value = value;
        if (editorService.ShowDialog(_binaryUI) == DialogResult.OK)
        {
            value = _binaryUI.Value;
        }

        _binaryUI.Value = null;

        return value;
    }

    /// <inheritdoc />
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;

    internal void ShowHelp()
    {
        if (TryGetService(out IHelpService? helpService))
        {
            helpService.ShowHelpFromKeyword(HELP_KEYWORD);
        }
        else
        {
            Debug.Fail("Unable to get IHelpService.");
        }
    }
}
