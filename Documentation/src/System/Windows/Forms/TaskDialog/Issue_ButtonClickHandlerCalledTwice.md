# Windows Task Dialog Issue

## [`TDN_BUTTON_CLICKED`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdn-button-clicked) notification sent twice when pressing a button with its access key

When you "click" a button by pressing its access key (mnemonic) and the dialog
is still open when the message loop continues, the [`TDN_BUTTON_CLICKED`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdn-button-clicked)
notification is sent twice instead of once.
 
* Run the code.
* Click one of the buttons with the mouse, or press space or enter, and notice
  that the counter increments by 1.
* **Issue:** Press <kbd>Alt</kbd>+<kbd>M</kbd> or <kbd>Alt</kbd>+<kbd>N</kbd> and 
  notice the counter increments by 2.

```cpp
#include "stdafx.h"
#include "CommCtrl.h"

int counter = 0;
wchar_t* counterTextBuffer;
HRESULT WINAPI ShowTaskDialogButtonClickHandlerCalledTwice_Callback(_In_ HWND hwnd, _In_ UINT msg, _In_ WPARAM wParam, _In_ LPARAM lParam, _In_ LONG_PTR lpRefData) {
    switch (msg) {
    case TDN_BUTTON_CLICKED:
        // Increment the counter.
        counter++;
        swprintf_s(counterTextBuffer, 100, L"Counter: %d", counter);
        SendMessageW(hwnd, TDM_SET_ELEMENT_TEXT, TDE_MAIN_INSTRUCTION, (LPARAM)counterTextBuffer);

        // When the user clicks the custom button, return false so that the dialog
        // stays open.
        if (wParam == 100) {
            return S_FALSE;
        }
        else if (wParam == IDNO) {
            // Otherwise, when the user clicks the common button, run the message loop.
            MSG msg;
            int bRet;
            while ((bRet = GetMessageW(&msg, nullptr, 0, 0)) != 0) {
                if (bRet == -1) {
                    // Error
                }
                else {
                    TranslateMessage(&msg);
                    DispatchMessageW(&msg);
                }
            }
        }

        break;
    }

    return S_OK;
}

void ShowTaskDialogButtonClickHandlerCalledTwice() {
    counterTextBuffer = new wchar_t[100];

    TASKDIALOGCONFIG* tConfig = new TASKDIALOGCONFIG;
    *tConfig = { 0 };
    tConfig->cbSize = sizeof(TASKDIALOGCONFIG);

    tConfig->dwFlags = TDF_USE_COMMAND_LINKS;
    tConfig->pszMainInstruction = L"Text...";
    tConfig->pfCallback = ShowTaskDialogButtonClickHandlerCalledTwice_Callback;

    // Create a custom button and a common ("No") button.
    tConfig->dwCommonButtons = TDCBF_NO_BUTTON;    

    TASKDIALOG_BUTTON customButton = { 0 };
    customButton.nButtonID = 100;
    customButton.pszButtonText = L"&My Button";
    tConfig->pButtons = &customButton;
    tConfig->cButtons = 1;

    int nButton, nRadioButton;
    BOOL checkboxSelected;
    TaskDialogIndirect(tConfig, &nButton, &nRadioButton, &checkboxSelected);

    delete tConfig;
    delete[] counterTextBuffer;
}

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
    _In_opt_ HINSTANCE hPrevInstance,
    _In_ LPWSTR    lpCmdLine,
    _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    ShowTaskDialogButtonClickHandlerCalledTwice();

    return 0;
}
```