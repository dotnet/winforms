// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;
using Windows.Win32.System.Ole;
using ComIDataObject = Windows.Win32.System.Com.IDataObject;
using RECTL = Windows.Win32.Foundation.RECTL;

namespace System.Windows.Forms;

public unsafe partial class Control :
    IOleControl.Interface,
    IOleObject.Interface,
    IOleInPlaceObject.Interface,
    IOleInPlaceActiveObject.Interface,
    IOleWindow.Interface,
    IViewObject.Interface,
    IViewObject2.Interface,
    IPersist.Interface,
    IPersistStreamInit.Interface,
    IPersistPropertyBag.Interface,
    IPersistStorage.Interface,
    IQuickActivate.Interface
{
    /// <inheritdoc cref="IOleControl.GetControlInfo(CONTROLINFO*)"/>
    unsafe HRESULT IOleControl.Interface.GetControlInfo(CONTROLINFO* pCI)
    {
        if (pCI is null)
        {
            return HRESULT.E_POINTER;
        }

        pCI->cb = (uint)sizeof(CONTROLINFO);
        pCI->hAccel = HACCEL.Null;
        pCI->cAccel = 0;
        pCI->dwFlags = 0;

        if (IsInputKey(Keys.Return))
        {
            pCI->dwFlags |= CTRLINFO.CTRLINFO_EATS_RETURN;
        }

        if (IsInputKey(Keys.Escape))
        {
            pCI->dwFlags |= CTRLINFO.CTRLINFO_EATS_ESCAPE;
        }

        return ActiveXInstance.GetControlInfo(pCI);
    }

    /// <inheritdoc cref="IOleControl.OnMnemonic(MSG*)"/>
    unsafe HRESULT IOleControl.Interface.OnMnemonic(MSG* pMsg)
    {
        if (pMsg is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        // If we got a mnemonic here, then the appropriate control will focus itself which
        // will cause us to become UI active.
        bool processed = ProcessMnemonic((char)(nuint)pMsg->wParam);
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleControl.OnAmbientPropertyChange(int)"/>
    HRESULT IOleControl.Interface.OnAmbientPropertyChange(int dispID)
    {
        ActiveXInstance.OnAmbientPropertyChange(dispID);
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleControl.FreezeEvents(BOOL)"/>
    HRESULT IOleControl.Interface.FreezeEvents(BOOL bFreeze)
    {
        ActiveXInstance.EventsFrozen = bFreeze;
        Debug.Assert(ActiveXInstance.EventsFrozen == bFreeze, "Failed to set EventsFrozen correctly");
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleWindow.GetWindow(HWND*)"/>
    HRESULT IOleInPlaceActiveObject.Interface.GetWindow(HWND* phwnd)
        => ((IOleInPlaceObject.Interface)this).GetWindow(phwnd);

    /// <inheritdoc cref="IOleWindow.ContextSensitiveHelp(BOOL)"/>
    HRESULT IOleInPlaceActiveObject.Interface.ContextSensitiveHelp(BOOL fEnterMode)
        => ((IOleInPlaceObject.Interface)this).ContextSensitiveHelp(fEnterMode);

    /// <inheritdoc cref="IOleInPlaceActiveObject.TranslateAccelerator(MSG*)"/>
    HRESULT IOleInPlaceActiveObject.Interface.TranslateAccelerator(MSG* lpmsg)
        => ActiveXInstance.TranslateAccelerator(lpmsg);

    /// <inheritdoc cref="IOleInPlaceActiveObject.OnFrameWindowActivate(BOOL)"/>
    HRESULT IOleInPlaceActiveObject.Interface.OnFrameWindowActivate(BOOL fActivate)
    {
        OnFrameWindowActivate(fActivate);
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleInPlaceActiveObject.OnDocWindowActivate(BOOL)"/>
    HRESULT IOleInPlaceActiveObject.Interface.OnDocWindowActivate(BOOL fActivate)
    {
        ActiveXInstance.OnDocWindowActivate(fActivate);
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleInPlaceActiveObject.ResizeBorder(RECT*, IOleInPlaceUIWindow*, BOOL)"/>
    HRESULT IOleInPlaceActiveObject.Interface.ResizeBorder(RECT* prcBorder, IOleInPlaceUIWindow* pUIWindow, BOOL fFrameWindow) =>
        HRESULT.S_OK;

    /// <inheritdoc cref="IOleInPlaceActiveObject.EnableModeless(BOOL)"/>
    HRESULT IOleInPlaceActiveObject.Interface.EnableModeless(BOOL fEnable) => HRESULT.E_NOTIMPL;

    /// <inheritdoc cref="IOleWindow.GetWindow(HWND*)"/>
    HRESULT IOleInPlaceObject.Interface.GetWindow(HWND* phwnd) => ActiveXInstance.GetWindow(phwnd);

    /// <inheritdoc cref="IOleWindow.ContextSensitiveHelp(BOOL)"/>
    HRESULT IOleInPlaceObject.Interface.ContextSensitiveHelp(BOOL fEnterMode)
    {
        if (fEnterMode)
        {
            OnHelpRequested(new HelpEventArgs(MousePosition));
        }

        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleInPlaceObject.InPlaceDeactivate"/>
    HRESULT IOleInPlaceObject.Interface.InPlaceDeactivate() => ActiveXInstance.InPlaceDeactivate();

    /// <inheritdoc cref="IOleInPlaceObject.UIDeactivate"/>
    HRESULT IOleInPlaceObject.Interface.UIDeactivate() => ActiveXInstance.UIDeactivate();

    /// <inheritdoc cref="IOleInPlaceObject.SetObjectRects(RECT*, RECT*)"/>
    HRESULT IOleInPlaceObject.Interface.SetObjectRects(RECT* lprcPosRect, RECT* lprcClipRect) =>
        ActiveXInstance.SetObjectRects(lprcPosRect, lprcClipRect);

    /// <inheritdoc cref="IOleInPlaceObject.ReactivateAndUndo"/>
    HRESULT IOleInPlaceObject.Interface.ReactivateAndUndo() => HRESULT.S_OK;

    /// <inheritdoc cref="IOleObject.SetClientSite(IOleClientSite*)"/>
    HRESULT IOleObject.Interface.SetClientSite(IOleClientSite* pClientSite)
    {
        ActiveXInstance.SetClientSite(pClientSite);
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleObject.GetClientSite(IOleClientSite**)"/>
    HRESULT IOleObject.Interface.GetClientSite(IOleClientSite** ppClientSite)
    {
        if (ppClientSite is null)
        {
            return HRESULT.E_POINTER;
        }

        *ppClientSite = ActiveXInstance.GetClientSite().Value;
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleObject.SetHostNames(PCWSTR, PCWSTR)"/>
    HRESULT IOleObject.Interface.SetHostNames(PCWSTR szContainerApp, PCWSTR szContainerObj) =>
        // Since ActiveX controls never "open" for editing, we shouldn't need to store these.
        HRESULT.S_OK;

    /// <inheritdoc cref="IOleObject.Close(uint)"/>
    HRESULT IOleObject.Interface.Close(uint dwSaveOption)
    {
        ActiveXInstance.Close((OLECLOSE)dwSaveOption);
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleObject.SetMoniker(uint, IMoniker*)"/>
    HRESULT IOleObject.Interface.SetMoniker(uint dwWhichMoniker, IMoniker* pmk) => HRESULT.E_NOTIMPL;

    /// <inheritdoc cref="IOleObject.GetMoniker(uint, uint, IMoniker**)"/>
    HRESULT IOleObject.Interface.GetMoniker(uint dwAssign, uint dwWhichMoniker, IMoniker** ppmk)
    {
        if (ppmk is null)
        {
            return HRESULT.E_POINTER;
        }

        *ppmk = null;
        return HRESULT.E_NOTIMPL;
    }

    /// <inheritdoc cref="IOleObject.InitFromData(ComIDataObject*, BOOL, uint)"/>
    HRESULT IOleObject.Interface.InitFromData(ComIDataObject* pDataObject, BOOL fCreation, uint dwReserved) =>
        HRESULT.E_NOTIMPL;

    /// <inheritdoc cref="IOleObject.GetClipboardData(uint, ComIDataObject**)"/>
    HRESULT IOleObject.Interface.GetClipboardData(uint dwReserved, ComIDataObject** ppDataObject)
    {
        if (ppDataObject is null)
        {
            return HRESULT.E_POINTER;
        }

        *ppDataObject = null;
        return HRESULT.E_NOTIMPL;
    }

    /// <inheritdoc cref="IOleObject.DoVerb(int, MSG*, IOleClientSite*, int, HWND, RECT*)"/>
    HRESULT IOleObject.Interface.DoVerb(
        int iVerb,
        MSG* lpmsg,
        IOleClientSite* pActiveSite,
        int lindex,
        HWND hwndParent,
        RECT* lprcPosRect)
    {
        // In Office they are internally casting an iVerb to a short and not doing the proper sign extension.
        short sVerb = unchecked((short)iVerb);
        iVerb = sVerb;

        return ActiveXInstance.DoVerb((OLEIVERB)iVerb, lpmsg, pActiveSite, lindex, hwndParent, lprcPosRect);
    }

    /// <inheritdoc cref="IOleObject.EnumVerbs(IEnumOLEVERB**)"/>
    HRESULT IOleObject.Interface.EnumVerbs(IEnumOLEVERB** ppEnumOleVerb)
    {
        if (ppEnumOleVerb is null)
        {
            return HRESULT.E_POINTER;
        }

        *ppEnumOleVerb = ActiveXImpl.EnumVerbs();
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleObject.Update()"/>
    HRESULT IOleObject.Interface.Update() => HRESULT.S_OK;

    /// <inheritdoc cref="IOleObject.IsUpToDate"/>
    HRESULT IOleObject.Interface.IsUpToDate() => HRESULT.S_OK;

    /// <inheritdoc cref="IOleObject.GetUserClassID(Guid*)"/>
    HRESULT IOleObject.Interface.GetUserClassID(Guid* pClsid)
    {
        if (pClsid is null)
        {
            return HRESULT.E_POINTER;
        }

        *pClsid = GetType().GUID;
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleObject.GetUserType(uint, PWSTR*)"/>
    HRESULT IOleObject.Interface.GetUserType(uint dwFormOfType, PWSTR* pszUserType)
    {
        if (pszUserType is null)
        {
            return HRESULT.E_POINTER;
        }

        *pszUserType = (char*)Marshal.StringToCoTaskMemUni(
            (USERCLASSTYPE)dwFormOfType == USERCLASSTYPE.USERCLASSTYPE_FULL ? GetType().FullName : GetType().Name);

        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleObject.SetExtent(DVASPECT, SIZE*)"/>
    HRESULT IOleObject.Interface.SetExtent(DVASPECT dwDrawAspect, SIZE* psizel)
    {
        if (psizel is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        ActiveXInstance.SetExtent(dwDrawAspect, (Size*)psizel);
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleObject.GetExtent(DVASPECT, SIZE*)"/>
    HRESULT IOleObject.Interface.GetExtent(DVASPECT dwDrawAspect, SIZE* psizel)
    {
        if (psizel is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        ActiveXInstance.GetExtent(dwDrawAspect, (Size*)psizel);
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleObject.Advise(IAdviseSink*, uint*)"/>
    HRESULT IOleObject.Interface.Advise(IAdviseSink* pAdvSink, uint* pdwConnection)
    {
        if (pdwConnection is null)
        {
            return HRESULT.E_POINTER;
        }

        return ActiveXInstance.Advise(pAdvSink, pdwConnection);
    }

    /// <inheritdoc cref="IOleObject.Unadvise(uint)"/>
    HRESULT IOleObject.Interface.Unadvise(uint dwConnection) => ActiveXInstance.Unadvise(dwConnection);

    /// <inheritdoc cref="IOleObject.EnumAdvise(IEnumSTATDATA**)"/>
    HRESULT IOleObject.Interface.EnumAdvise(IEnumSTATDATA** ppenumAdvise)
    {
        if (ppenumAdvise is null)
        {
            return HRESULT.E_POINTER;
        }

        *ppenumAdvise = null;
        return HRESULT.E_NOTIMPL;
    }

    /// <inheritdoc cref="IOleObject.GetMiscStatus(DVASPECT, OLEMISC*)"/>
    HRESULT IOleObject.Interface.GetMiscStatus(DVASPECT dwAspect, OLEMISC* pdwStatus)
    {
        if (pdwStatus is null)
        {
            return HRESULT.E_POINTER;
        }

        if (!dwAspect.HasFlag(DVASPECT.DVASPECT_CONTENT))
        {
            *pdwStatus = 0;
            return HRESULT.DV_E_DVASPECT;
        }

        OLEMISC status = OLEMISC.OLEMISC_ACTIVATEWHENVISIBLE | OLEMISC.OLEMISC_INSIDEOUT | OLEMISC.OLEMISC_SETCLIENTSITEFIRST;
        if (GetStyle(ControlStyles.ResizeRedraw))
        {
            status |= OLEMISC.OLEMISC_RECOMPOSEONRESIZE;
        }

        if (this is IButtonControl)
        {
            status |= OLEMISC.OLEMISC_ACTSLIKEBUTTON;
        }

        *pdwStatus = status;
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IOleObject.SetColorScheme(LOGPALETTE*)"/>
    HRESULT IOleObject.Interface.SetColorScheme(LOGPALETTE* pLogpal) => HRESULT.S_OK;

    /// <inheritdoc cref="IOleWindow.GetWindow(HWND*)"/>
    HRESULT IOleWindow.Interface.GetWindow(HWND* phwnd) => ((IOleInPlaceObject.Interface)this).GetWindow(phwnd);

    /// <inheritdoc cref="IOleWindow.ContextSensitiveHelp(BOOL)"/>
    HRESULT IOleWindow.Interface.ContextSensitiveHelp(BOOL fEnterMode)
        => ((IOleInPlaceObject.Interface)this).ContextSensitiveHelp(fEnterMode);

    unsafe HRESULT IPersist.Interface.GetClassID(Guid* pClassID)
    {
        if (pClassID is null)
        {
            return HRESULT.E_POINTER;
        }

        *pClassID = GetType().GUID;
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IPersistPropertyBag.InitNew"/>
    HRESULT IPersistPropertyBag.Interface.InitNew() => HRESULT.S_OK;

    /// <inheritdoc cref="IPersist.GetClassID(Guid*)"/>
    HRESULT IPersistPropertyBag.Interface.GetClassID(Guid* pClassID)
    {
        if (pClassID is null)
        {
            return HRESULT.E_POINTER;
        }

        *pClassID = GetType().GUID;
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IPersistPropertyBag.Load(IPropertyBag*, IErrorLog*)"/>
    HRESULT IPersistPropertyBag.Interface.Load(IPropertyBag* pPropBag, IErrorLog* pErrorLog)
    {
        ActiveXInstance.Load(pPropBag, pErrorLog);
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IPersistPropertyBag.Save(IPropertyBag*, BOOL, BOOL)"/>
    HRESULT IPersistPropertyBag.Interface.Save(IPropertyBag* pPropBag, BOOL fClearDirty, BOOL fSaveAllProperties)
    {
        ActiveXInstance.Save(pPropBag, fClearDirty, fSaveAllProperties);
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IPersist.GetClassID(Guid*)"/>
    HRESULT IPersistStorage.Interface.GetClassID(Guid* pClassID)
    {
        if (pClassID is null)
        {
            return HRESULT.E_POINTER;
        }

        *pClassID = GetType().GUID;
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IPersistStorage.IsDirty"/>
    HRESULT IPersistStorage.Interface.IsDirty() => ActiveXInstance.IsDirty();

    /// <inheritdoc cref="IPersistStorage.InitNew(IStorage*)"/>
    HRESULT IPersistStorage.Interface.InitNew(IStorage* pStg) => HRESULT.S_OK;

    /// <inheritdoc cref="IPersistStorage.Load(IStorage*)"/>
    HRESULT IPersistStorage.Interface.Load(IStorage* pStg)
    {
        if (pStg is null)
        {
            return HRESULT.E_POINTER;
        }

        return ActiveXInstance.Load(pStg);
    }

    /// <inheritdoc cref="IPersistStorage.Save(IStorage*, BOOL)"/>
    HRESULT IPersistStorage.Interface.Save(IStorage* pStgSave, BOOL fSameAsLoad)
    {
        if (pStgSave is null)
        {
            return HRESULT.E_POINTER;
        }

        return ActiveXInstance.Save(pStgSave, fSameAsLoad);
    }

    /// <inheritdoc cref="IPersistStorage.SaveCompleted(IStorage*)"/>
    HRESULT IPersistStorage.Interface.SaveCompleted(IStorage* pStgNew) => HRESULT.S_OK;

    /// <inheritdoc cref="IPersistStorage.HandsOffStorage"/>
    HRESULT IPersistStorage.Interface.HandsOffStorage() => HRESULT.S_OK;

    /// <inheritdoc cref="IPersist.GetClassID(Guid*)"/>
    HRESULT IPersistStreamInit.Interface.GetClassID(Guid* pClassID)
    {
        if (pClassID is null)
        {
            return HRESULT.E_POINTER;
        }

        *pClassID = GetType().GUID;
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IPersistStorage.IsDirty"/>
    HRESULT IPersistStreamInit.Interface.IsDirty() => ActiveXInstance.IsDirty();

    /// <inheritdoc cref="IPersistStreamInit.Load(IStream*)"/>
    HRESULT IPersistStreamInit.Interface.Load(IStream* pStm)
    {
        if (pStm is null)
        {
            return HRESULT.E_POINTER;
        }

        ActiveXInstance.Load(pStm);
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IPersistStreamInit.Save(IStream*, BOOL)"/>
    HRESULT IPersistStreamInit.Interface.Save(IStream* pStm, BOOL fClearDirty)
    {
        if (pStm is null)
        {
            return HRESULT.E_POINTER;
        }

        ActiveXInstance.Save(pStm, fClearDirty);
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IPersistStreamInit.GetSizeMax(ulong*)"/>
    HRESULT IPersistStreamInit.Interface.GetSizeMax(ulong* pCbSize) => HRESULT.S_OK;

    /// <inheritdoc cref="IPersistStreamInit.InitNew"/>
    HRESULT IPersistStreamInit.Interface.InitNew() => HRESULT.S_OK;

    /// <inheritdoc cref="IQuickActivate.QuickActivate(QACONTAINER*, QACONTROL*)"/>
    HRESULT IQuickActivate.Interface.QuickActivate(QACONTAINER* pQaContainer, QACONTROL* pQaControl) =>
        ActiveXInstance.QuickActivate(pQaContainer, pQaControl);

    /// <inheritdoc cref="IQuickActivate.SetContentExtent(SIZE*)"/>
    HRESULT IQuickActivate.Interface.SetContentExtent(SIZE* pSizel)
    {
        if (pSizel is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        ActiveXInstance.SetExtent(DVASPECT.DVASPECT_CONTENT, (Size*)pSizel);
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IQuickActivate.GetContentExtent(SIZE*)"/>
    HRESULT IQuickActivate.Interface.GetContentExtent(SIZE* pSizel)
    {
        if (pSizel is null)
        {
            return HRESULT.E_INVALIDARG;
        }

        ActiveXInstance.GetExtent(DVASPECT.DVASPECT_CONTENT, (Size*)pSizel);
        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IViewObject.Draw(DVASPECT, int, void*, DVTARGETDEVICE*, HDC, HDC, RECTL*, RECTL*, nint, nuint)"/>
    HRESULT IViewObject.Interface.Draw(
        DVASPECT dwDrawAspect,
        int lindex,
        void* pvAspect,
        DVTARGETDEVICE* ptd,
        HDC hdcTargetDev,
        HDC hdcDraw,
        RECTL* lprcBounds,
        RECTL* lprcWBounds,
        nint pfnContinue,
        nuint dwContinue)
    {
        HRESULT hr = ActiveXInstance.Draw(
            dwDrawAspect,
            lindex,
            pvAspect,
            ptd,
            hdcTargetDev,
            hdcDraw,
            (RECT*)lprcBounds,
            (RECT*)lprcWBounds,
            pfnContinue,
            dwContinue);

        Debug.Assert(hr.Succeeded);

        return HRESULT.S_OK;
    }

    /// <inheritdoc cref="IViewObject.GetColorSet(DVASPECT, int, void*, DVTARGETDEVICE*, HDC, LOGPALETTE**)"/>
    HRESULT IViewObject.Interface.GetColorSet(
        DVASPECT dwDrawAspect,
        int lindex,
        void* pvAspect,
        DVTARGETDEVICE* ptd,
        HDC hicTargetDev,
        LOGPALETTE** ppColorSet) =>
        // GDI+ doesn't do palettes.
        HRESULT.E_NOTIMPL;

    /// <inheritdoc cref="IViewObject.Freeze(DVASPECT, int, void*, uint*)"/>
    HRESULT IViewObject.Interface.Freeze(DVASPECT dwDrawAspect, int lindex, void* pvAspect, uint* pdwFreeze) =>
        HRESULT.E_NOTIMPL;

    /// <inheritdoc cref="IViewObject.Unfreeze(uint)"/>
    HRESULT IViewObject.Interface.Unfreeze(uint dwFreeze) => HRESULT.E_NOTIMPL;

    /// <inheritdoc cref="IViewObject.SetAdvise(DVASPECT, uint, IAdviseSink*)"/>
    HRESULT IViewObject.Interface.SetAdvise(DVASPECT aspects, uint advf, IAdviseSink* pAdvSink) =>
        ActiveXInstance.SetAdvise(aspects, (ADVF)advf, pAdvSink);

    /// <inheritdoc cref="IViewObject.GetAdvise(uint*, uint*, IAdviseSink**)"/>
    HRESULT IViewObject.Interface.GetAdvise(uint* pAspects, uint* pAdvf, IAdviseSink** ppAdvSink) =>
        ActiveXInstance.GetAdvise((DVASPECT*)pAspects, (ADVF*)pAdvf, ppAdvSink);

    /// <inheritdoc cref="IViewObject.Draw(DVASPECT, int, void*, DVTARGETDEVICE*, HDC, HDC, RECTL*, RECTL*, nint, nuint)"/>
    HRESULT IViewObject2.Interface.Draw(
        DVASPECT dwDrawAspect,
        int lindex,
        void* pvAspect,
        DVTARGETDEVICE* ptd,
        HDC hdcTargetDev,
        HDC hdcDraw,
        RECTL* lprcBounds,
        RECTL* lprcWBounds,
        nint pfnContinue,
        nuint dwContinue)
        => ((IViewObject.Interface)this).Draw(
            dwDrawAspect,
            lindex,
            pvAspect,
            ptd,
            hdcTargetDev,
            hdcDraw,
            lprcBounds,
            lprcWBounds,
            pfnContinue,
            dwContinue);

    /// <inheritdoc cref="IViewObject.GetColorSet(DVASPECT, int, void*, DVTARGETDEVICE*, HDC, LOGPALETTE**)"/>
    HRESULT IViewObject2.Interface.GetColorSet(
        DVASPECT dwDrawAspect,
        int lindex,
        void* pvAspect,
        DVTARGETDEVICE* ptd,
        HDC hdcTargetDev,
        LOGPALETTE** ppColorSet)
        => ((IViewObject.Interface)this).GetColorSet(dwDrawAspect, lindex, pvAspect, ptd, hdcTargetDev, ppColorSet);

    /// <inheritdoc cref="IViewObject.Freeze(DVASPECT, int, void*, uint*)"/>
    HRESULT IViewObject2.Interface.Freeze(DVASPECT dwDrawAspect, int lindex, void* pvAspect, uint* pdwFreeze)
        => ((IViewObject.Interface)this).Freeze(dwDrawAspect, lindex, pvAspect, pdwFreeze);

    /// <inheritdoc cref="IViewObject.Unfreeze(uint)"/>
    HRESULT IViewObject2.Interface.Unfreeze(uint dwFreeze)
        => ((IViewObject.Interface)this).Unfreeze(dwFreeze);

    /// <inheritdoc cref="IViewObject.SetAdvise(DVASPECT, uint, IAdviseSink*)"/>
    HRESULT IViewObject2.Interface.SetAdvise(DVASPECT aspects, uint advf, IAdviseSink* pAdvSink)
        => ((IViewObject.Interface)this).SetAdvise(aspects, advf, pAdvSink);

    /// <inheritdoc cref="IViewObject.GetAdvise(uint*, uint*, IAdviseSink**)"/>
    HRESULT IViewObject2.Interface.GetAdvise(uint* pAspects, uint* pAdvf, IAdviseSink** ppAdvSink)
        => ((IViewObject.Interface)this).GetAdvise(pAspects, pAdvf, ppAdvSink);

    /// <inheritdoc cref="IOleObject.GetExtent(DVASPECT, SIZE*)"/>
    HRESULT IViewObject2.Interface.GetExtent(DVASPECT dwDrawAspect, int lindex, DVTARGETDEVICE* ptd, SIZE* lpsizel)
        => ((IOleObject.Interface)this).GetExtent(dwDrawAspect, lpsizel);
}
