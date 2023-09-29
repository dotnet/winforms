// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Automation;

internal abstract unsafe class UiaTextProvider2 : UiaTextProvider, ITextProvider2.Interface
{
    public abstract HRESULT RangeFromAnnotation(IRawElementProviderSimple* annotationElement, ITextRangeProvider** pRetVal);

    public abstract HRESULT GetCaretRange(BOOL* isActive, ITextRangeProvider** pRetVal);
}
