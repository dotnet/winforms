// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.Shell32;

namespace System.Windows.Forms
{
    public partial class FileDialog
    {
        private class VistaDialogEvents : IFileDialogEvents
        {
            private readonly FileDialog _ownerDialog;

            public VistaDialogEvents(FileDialog dialog)
            {
                _ownerDialog = dialog;
            }

            public HRESULT OnFileOk(IFileDialog pfd)
            {
                return _ownerDialog.HandleVistaFileOk((Interop.WinFormsComWrappers.FileDialogWrapper)pfd) ? HRESULT.S_OK : HRESULT.S_FALSE;
            }

            public HRESULT OnFolderChanging(IFileDialog pfd, IShellItem psiFolder)
            {
                return HRESULT.S_OK;
            }

            public HRESULT OnFolderChange(IFileDialog pfd)
            {
                return HRESULT.S_OK;
            }

            public HRESULT OnSelectionChange(IFileDialog pfd)
            {
                return HRESULT.S_OK;
            }

            public unsafe HRESULT OnShareViolation(IFileDialog pfd, IShellItem psi, FDESVR* pResponse)
            {
                if (pResponse is null)
                {
                    return HRESULT.E_POINTER;
                }

                *pResponse = FDESVR.DEFAULT;
                return HRESULT.S_OK;
            }

            public HRESULT OnTypeChange(IFileDialog pfd)
            {
                return HRESULT.S_OK;
            }

            public unsafe HRESULT OnOverwrite(IFileDialog pfd, IShellItem psi, FDEOR* pResponse)
            {
                if (pResponse is null)
                {
                    return HRESULT.E_POINTER;
                }

                *pResponse = FDEOR.DEFAULT;
                return HRESULT.S_OK;
            }
        }
    }
}
