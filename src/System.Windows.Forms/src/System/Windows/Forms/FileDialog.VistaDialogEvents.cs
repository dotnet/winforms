// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace System.Windows.Forms;

public partial class FileDialog
{
    private unsafe class VistaDialogEvents : IFileDialogEvents.Interface, IManagedWrapper<IFileDialogEvents>
    {
        private readonly FileDialog _ownerDialog;

        public VistaDialogEvents(FileDialog dialog)
        {
            _ownerDialog = dialog;
        }

        public HRESULT OnFileOk(IFileDialog* pfd)
        {
            return _ownerDialog.HandleVistaFileOk(pfd) ? HRESULT.S_OK : HRESULT.S_FALSE;
        }

        public HRESULT OnFolderChanging(IFileDialog* pfd, IShellItem* psiFolder)
        {
            return HRESULT.S_OK;
        }

        public HRESULT OnFolderChange(IFileDialog* pfd)
        {
            return HRESULT.S_OK;
        }

        public HRESULT OnSelectionChange(IFileDialog* pfd)
        {
            return HRESULT.S_OK;
        }

        public unsafe HRESULT OnShareViolation(IFileDialog* pfd, IShellItem* psi, FDE_SHAREVIOLATION_RESPONSE* pResponse)
        {
            if (pResponse is null)
            {
                return HRESULT.E_POINTER;
            }

            *pResponse = FDE_SHAREVIOLATION_RESPONSE.FDESVR_DEFAULT;
            return HRESULT.S_OK;
        }

        public HRESULT OnTypeChange(IFileDialog* pfd)
        {
            return HRESULT.S_OK;
        }

        public unsafe HRESULT OnOverwrite(IFileDialog* pfd, IShellItem* psi, FDE_OVERWRITE_RESPONSE* pResponse)
        {
            if (pResponse is null)
            {
                return HRESULT.E_POINTER;
            }

            *pResponse = FDE_OVERWRITE_RESPONSE.FDEOR_DEFAULT;
            return HRESULT.S_OK;
        }
    }
}
