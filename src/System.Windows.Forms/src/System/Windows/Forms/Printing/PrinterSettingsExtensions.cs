// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Printing;

namespace System.Windows.Forms.Printing;

internal static class PrinterSettingsExtensions
{
    internal ref struct DevModeScope(int sizeInBytes)
    {
        private readonly int _sizeInBytes = sizeInBytes;
        private BufferScope<byte> _buffer = new(sizeInBytes);

        public readonly ref byte GetPinnableReference() => ref _buffer.GetPinnableReference();
        public readonly uint SizeInBytes => (uint)_sizeInBytes;
        public void Dispose() => _buffer.Dispose();
    }

    internal static unsafe DevModeScope GetDevMode(PrinterSettings printerSettings)
    {
        int modeSize;

        fixed (char* n = printerSettings.PrinterName)
        {
            modeSize = PInvoke.DocumentProperties(HWND.Null, HANDLE.Null, n, null, (DEVMODEW*)null, 0);
            if (modeSize < 1)
            {
                throw new InvalidPrinterException(printerSettings);
            }
        }

        DevModeScope devModeScope = new(modeSize);

        fixed (char* n = printerSettings.PrinterName)
        fixed (byte* b = devModeScope)
        {
            modeSize = PInvoke.DocumentProperties(
                HWND.Null,
                HANDLE.Null,
                n,
                (DEVMODEW*)b,
                (DEVMODEW*)null,
                (uint)DEVMODE_FIELD_FLAGS.DM_OUT_BUFFER);

            if (modeSize < 1)
            {
                devModeScope.Dispose();
                throw new InvalidPrinterException(printerSettings);
            }
        }

        return devModeScope;
    }
}
