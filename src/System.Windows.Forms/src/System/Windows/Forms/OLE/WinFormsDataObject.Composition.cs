// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.OLE;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms;

internal partial class WinFormsDataObject
{
    internal unsafe partial class Composition : DesktopDataObject.Composition
    {
        private static BinaryFormatUtilities? s_formatUtilities;

        public Composition() { }

        private static BinaryFormatUtilities BinaryFormatUtilitiesInstance => s_formatUtilities ??= new BinaryFormatUtilities();

        public override DesktopDataObject.Composition PopulateFromDesktopDataObject(IDataObjectDesktop desktopDataObject)
        {
            DesktopToNativeAdapter winFormsToNative = new(desktopDataObject);
            NativeToRuntimeAdapter nativeToRuntime = new(ComHelpers.GetComPointer<Com.IDataObject>(winFormsToNative));
            return Populate(desktopDataObject, winFormsToNative, nativeToRuntime);
        }

        public override unsafe DesktopDataObject.Composition PopulateFromNativeDataObject(Com.IDataObject* nativeDataObject)
        {
            // Add ref so each adapter can take ownership of the native data object.
            nativeDataObject->AddRef();
            nativeDataObject->AddRef();
            NativeToDesktopAdapter nativeToWinForms = new(nativeDataObject);
            NativeToRuntimeAdapter nativeToRuntime = new(nativeDataObject);
            return Populate(nativeToWinForms, nativeToWinForms, nativeToRuntime);
        }

        public override DesktopDataObject.Composition PopulateFromRuntimeDataObject(Runtime.InteropServices.ComTypes.IDataObject runtimeDataObject)
        {
            RuntimeToNativeAdapter runtimeToNative = new(runtimeDataObject);
            NativeToDesktopAdapter nativeToWinForms = new(ComHelpers.GetComPointer<Com.IDataObject>(runtimeToNative));
            return Populate(nativeToWinForms, runtimeToNative, runtimeDataObject);
        }
    }
}
