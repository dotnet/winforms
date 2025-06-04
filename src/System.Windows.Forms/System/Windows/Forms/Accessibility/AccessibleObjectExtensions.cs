// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

internal static unsafe class AccessibleObjectExtensions
{
    public static ComScope<IAccessible> TryGetIAccessible(this AgileComPointer<IAccessible>? agile, out HRESULT result)
    {
        if (agile is not null)
        {
            return agile.TryGetInterface(out result);
        }

        result = HRESULT.E_NOINTERFACE;
        return default;
    }

    public static Rectangle TryGetLocation(this AgileComPointer<IAccessible>? agile, int child)
        => agile.TryGetLocation((VARIANT)child);

    public static Rectangle TryGetLocation(this AgileComPointer<IAccessible>? agile, VARIANT child)
    {
        // Use the system provided bounds
        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return default;
        }

        result = accessible.Value->accLocation(
            out int pxLeft,
            out int pyTop,
            out int pcxWidth,
            out int pcyHeight,
            child);

        Debug.Assert(
            result.Succeeded || result == HRESULT.DISP_E_MEMBERNOTFOUND,
            $"{nameof(TryGetLocation)}: accLocation call for id {(int)child} failed with {result}");
        return new Rectangle(pxLeft, pyTop, pcxWidth, pcyHeight);
    }

    public static BSTR TryGetDefaultAction(this AgileComPointer<IAccessible>? agile, int child)
        => agile.TryGetDefaultAction((VARIANT)child);

    public static BSTR TryGetDefaultAction(this AgileComPointer<IAccessible>? agile, VARIANT child)
    {
        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return default;
        }

        BSTR bstr = default;
        result = accessible.Value->get_accDefaultAction(child, &bstr);

        Debug.Assert(
            result.Succeeded || result == HRESULT.DISP_E_MEMBERNOTFOUND,
            $"{nameof(TryGetDefaultAction)}: get_accDefaultAction call for id {(int)child} failed with {result}");
        return bstr;
    }

    public static void TryDoDefaultAction(this AgileComPointer<IAccessible>? agile, int child)
        => agile.TryDoDefaultAction((VARIANT)child);

    public static void TryDoDefaultAction(this AgileComPointer<IAccessible>? agile, VARIANT child)
    {
        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return;
        }

        result = accessible.Value->accDoDefaultAction(child);
    }

    public static BSTR TryGetDescription(this AgileComPointer<IAccessible>? agile, int child) =>
        agile.TryGetDescription((VARIANT)child);

    public static BSTR TryGetDescription(this AgileComPointer<IAccessible>? agile, VARIANT child)
    {
        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return default;
        }

        BSTR description = default;
        accessible.Value->get_accDescription(child, &description);

        Debug.Assert(
            result.Succeeded || result == HRESULT.DISP_E_MEMBERNOTFOUND,
            $"{nameof(TryGetDescription)}: get_accDescription call for {(int)child} failed with {result}");
        return description;
    }

    public static BSTR TryGetHelp(this AgileComPointer<IAccessible>? agile, int child)
        => agile.TryGetHelp((VARIANT)child);

    public static BSTR TryGetHelp(this AgileComPointer<IAccessible>? agile, VARIANT child)
    {
        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return default;
        }

        BSTR bstr = default;
        result = accessible.Value->get_accHelp(child, &bstr);

        Debug.Assert(
            result.Succeeded || result == HRESULT.DISP_E_MEMBERNOTFOUND,
            $"{nameof(TryGetHelp)}: get_accHelp call for id {(int)child} failed with {result}");
        return bstr;
    }

    public static BSTR TryGetKeyboardShortcut(this AgileComPointer<IAccessible>? agile, int child)
        => agile.TryGetKeyboardShortcut((VARIANT)child);

    public static BSTR TryGetKeyboardShortcut(this AgileComPointer<IAccessible>? agile, VARIANT child)
    {
        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return default;
        }

        BSTR bstr = default;
        result = accessible.Value->get_accKeyboardShortcut(child, &bstr);

        Debug.Assert(
            result.Succeeded || result == HRESULT.DISP_E_MEMBERNOTFOUND,
            $"{nameof(TryGetKeyboardShortcut)}: get_accKeyboardShortcut call for id {(int)child} failed with {result}");
        return bstr;
    }

    public static BSTR TryGetName(this AgileComPointer<IAccessible>? agile, int child)
        => agile.TryGetName((VARIANT)child);

    public static BSTR TryGetName(this AgileComPointer<IAccessible>? agile, VARIANT child)
    {
        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return default;
        }

        BSTR bstr = default;
        result = accessible.Value->get_accName(child, &bstr);

        Debug.Assert(result.Succeeded, $"{nameof(TryGetName)}: get_accName call failed with {result}");
        return bstr;
    }

    public static void TrySetName(this AgileComPointer<IAccessible>? agile, VARIANT child, BSTR name)
    {
        // This is not technically supported any more, unclear if this ever actually does anything.
        // https://learn.microsoft.com/windows/win32/api/oleacc/nf-oleacc-iaccessible-put_accname

        if (name.IsNull)
        {
            return;
        }

        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return;
        }

        result = accessible.Value->put_accName(child, name);

        Debug.WriteLineIf(result.Failed, $"{nameof(TrySetName)}: put_accName call failed with {result}");
    }

    public static AccessibleRole TryGetRole(this AgileComPointer<IAccessible>? agile, int child)
        => agile.TryGetRole((VARIANT)child);

    public static AccessibleRole TryGetRole(this AgileComPointer<IAccessible>? agile, VARIANT child)
    {
        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return AccessibleRole.None;
        }

        using VARIANT role = default;
        result = accessible.Value->get_accRole(child, &role);

        Debug.Assert(result.Succeeded, $"{nameof(TryGetRole)}: get_accRole call failed with {result}");

        return role.vt is VARENUM.VT_I4 or VARENUM.VT_INT ? (AccessibleRole)(int)role : AccessibleRole.None;
    }

    public static AccessibleStates TryGetState(this AgileComPointer<IAccessible>? agile, int child)
        => agile.TryGetState((VARIANT)child);

    public static AccessibleStates TryGetState(this AgileComPointer<IAccessible>? agile, VARIANT child)
    {
        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return AccessibleStates.None;
        }

        using VARIANT state = default;
        result = accessible.Value->get_accState(child, &state);

        Debug.Assert(
            result.Succeeded,
            $"{nameof(TryGetState)}: get_accState call for id {(int)child} failed with {result}");

        return state.vt is VARENUM.VT_I4 or VARENUM.VT_INT ? (AccessibleStates)(int)state : AccessibleStates.None;
    }

    public static BSTR TryGetValue(this AgileComPointer<IAccessible>? agile, VARIANT child)
    {
        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return default;
        }

        BSTR bstr = default;
        result = accessible.Value->get_accValue(child, &bstr);

        Debug.Assert(
            result.Succeeded || result == HRESULT.DISP_E_MEMBERNOTFOUND,
            $"{nameof(TryGetValue)}: get_accValue call failed with {result}");
        return bstr;
    }

    public static void TrySetValue(this AgileComPointer<IAccessible>? agile, VARIANT child, BSTR value)
    {
        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return;
        }

        result = accessible.Value->put_accValue(child, value);

        if (result.Failed && result != HRESULT.DISP_E_MEMBERNOTFOUND)
        {
            result.ThrowOnFailure();
        }

        Debug.Assert(
            result.Succeeded || result == HRESULT.DISP_E_MEMBERNOTFOUND,
            $"{nameof(TrySetValue)}: put_accValue call failed with {result}");
    }

    public static (int topic, BSTR helpFile) TryGetHelpTopic(this AgileComPointer<IAccessible>? agile, VARIANT child)
    {
        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return (-1, default);
        }

        BSTR file = default;
        result = accessible.Value->get_accHelpTopic(&file, child, out int topicId);
        Debug.Assert(
            result.Succeeded || result == HRESULT.DISP_E_MEMBERNOTFOUND,
            $"{nameof(TryGetHelpTopic)}: put_accValue call failed with {result}");

        return result.Failed ? (-1, file) : (topicId, file);
    }

    public static void TrySelect(this AgileComPointer<IAccessible>? agile, AccessibleSelection flags, int child)
        => agile.TrySelect(flags, (VARIANT)child);

    public static void TrySelect(this AgileComPointer<IAccessible>? agile, AccessibleSelection flags, VARIANT child)
    {
        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return;
        }

        result = accessible.Value->accSelect((int)flags, child);
    }

    public static int TryGetChildCount(this AgileComPointer<IAccessible>? agile)
    {
        using var accessible = agile.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return default;
        }

        result = accessible.Value->get_accChildCount(out int count);
        return count;
    }

    /// <summary>
    ///  Gets the accessible object's owner control's accessible role.
    /// </summary>
    public static AccessibleRole GetOwnerAccessibleRole<TAccessible>(
        this TAccessible accessibleObject,
        AccessibleRole defaultRole = AccessibleRole.Default)
        where TAccessible : AccessibleObject, IOwnedObject<Control>
    {
        AccessibleRole role = accessibleObject.Owner?.AccessibleRole ?? AccessibleRole.Default;
        return role == AccessibleRole.Default ? defaultRole : role;
    }

    /// <summary>
    ///  Gets the accessible object's owner control's accessible name.
    /// </summary>
    [return: NotNullIfNotNull(nameof(defaultName))]
    public static string? GetOwnerAccessibleName<TAccessible>(
        this TAccessible accessibleObject,
        string? defaultName = null) where TAccessible : AccessibleObject, IOwnedObject<Control>
        => accessibleObject.Owner?.AccessibleName ?? defaultName;

    /// <summary>
    ///  Gets the accessible object's owner control's accessible text.
    /// </summary>
    public static string GetOwnerText<TAccessible>(this TAccessible accessibleObject, string defaultText = "")
        where TAccessible : AccessibleObject, IOwnedObject<Control>
        => accessibleObject.Owner?.Text ?? defaultText;
}
