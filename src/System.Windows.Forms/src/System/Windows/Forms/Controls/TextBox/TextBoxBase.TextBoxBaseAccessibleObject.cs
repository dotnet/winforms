// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ButtonBase;

namespace System.Windows.Forms;

public abstract partial class TextBoxBase
{
    internal unsafe class TextBoxBaseAccessibleObject : ControlAccessibleObject
    {
        private TextBoxBaseUiaTextProvider? _textProvider;

        public TextBoxBaseAccessibleObject(TextBoxBase owner) : base(owner)
        {
            _textProvider = new(owner);
        }

        internal void ClearObjects() => _textProvider = null;

        internal override Rectangle BoundingRectangle => this.IsOwnerHandleCreated(out TextBoxBase? owner) ?
            owner.GetToolNativeScreenRectangle() : Rectangle.Empty;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
        {
            if (this.TryGetOwnerAs(out TextBoxBase? owner))
            {
                if (propertyID == UIA_PROPERTY_ID.UIA_IsPasswordPropertyId)
                {
                    return (VARIANT)owner.PasswordProtect;
                }

                if (propertyID == UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId)
                {
                    return (VARIANT)owner.Focused;
                }
            }

            return base.GetPropertyValue(propertyID);
        }

        internal override bool IsIAccessibleExSupported() => true;

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_TextPatternId => true,
                UIA_PATTERN_ID.UIA_TextPattern2Id => true,
                UIA_PATTERN_ID.UIA_ValuePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };

        internal override bool IsReadOnly => this.TryGetOwnerAs(out TextBoxBase? owner) && owner.ReadOnly;

        public override string? Name
        {
            get
            {
                string? name = base.Name;
                return name is not null ? name : string.Empty;
            }
        }

        internal override bool CanGetValueInternal => false;

        public override string? Value => this.TryGetOwnerAs(out TextBoxBase? owner) && !owner.PasswordProtect ? ValueInternal : SR.AccessDenied;

        protected virtual string ValueInternal
            => this.TryGetOwnerAs(out Control? owner) && owner.Text is { } text ? text : string.Empty;

        internal override void SetFocus()
        {
            if (!this.IsOwnerHandleCreated(out Control? _))
            {
                return;
            }

            base.SetFocus();

            RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }

        internal override void SetValue(string? newValue)
        {
            if (this.TryGetOwnerAs(out Control? owner))
            {
                owner.Text = newValue;
            }

            base.SetValue(newValue);
        }

        internal override ITextRangeProvider* DocumentRangeInternal
            => _textProvider is null ? null : _textProvider.DocumentRange;

        internal override HRESULT GetTextSelection(SAFEARRAY** pRetVal)
            => _textProvider?.GetSelection(pRetVal) ?? HRESULT.COR_E_OBJECTDISPOSED;

        internal override HRESULT GetTextVisibleRanges(SAFEARRAY** pRetVal)
            => _textProvider?.GetVisibleRanges(pRetVal) ?? HRESULT.COR_E_OBJECTDISPOSED;

        internal override HRESULT GetTextRangeFromChild(IRawElementProviderSimple* childElement, ITextRangeProvider** pRetVal)
            => _textProvider?.RangeFromChild(childElement, pRetVal) ?? HRESULT.COR_E_OBJECTDISPOSED;

        internal override HRESULT GetTextRangeFromPoint(UiaPoint screenLocation, ITextRangeProvider** pRetVal)
            => _textProvider?.RangeFromPoint(screenLocation, pRetVal) ?? HRESULT.COR_E_OBJECTDISPOSED;

        internal override SupportedTextSelection SupportedTextSelectionInternal
            => _textProvider?.SupportedTextSelection ?? SupportedTextSelection.SupportedTextSelection_None;

        internal override HRESULT GetTextCaretRange(BOOL* isActive, ITextRangeProvider** pRetVal)
            => _textProvider?.GetCaretRange(isActive, pRetVal) ?? HRESULT.COR_E_OBJECTDISPOSED;

        internal override HRESULT GetRangeFromAnnotation(IRawElementProviderSimple* annotationElement, ITextRangeProvider** pRetVal)
            => _textProvider?.RangeFromAnnotation(annotationElement, pRetVal) ?? HRESULT.COR_E_OBJECTDISPOSED;

        public override string? KeyboardShortcut => this.TryGetOwnerAs(out TextBoxBase? owner)
            ? ButtonBaseAccessibleObject.GetKeyboardShortcut(owner, useMnemonic: false, PreviousLabel)
            : null;

        private protected override bool IsInternal => true;
    }
}
