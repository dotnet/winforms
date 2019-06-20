# Windows Task Dialog Issue

## Access Violation when receiving [`TDN_NAVIGATED`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdn-navigated) within [`TDN_RADIO_BUTTON_CLICKED`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdn-radio-button-clicked)

An Access Violation occurs when receiving a navigation notification
([`TDN_NAVIGATED`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdn-navigated))
within a [`TDN_RADIO_BUTTON_CLICKED`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdn-radio-button-clicked)
notification (e.g. due to running the message loop).
 
* Run the code and select one of the radio buttons.
* Notice the dialog has navigated and an inner dialog is opened.
* Close the inner dialog.
* **Issue:** Notice the application crashes with an access violation.

```cpp
#include "stdafx.h"
#include "CommCtrl.h"

HRESULT WINAPI TaskDialogCallbackProc(_In_ HWND hwnd, _In_ UINT msg, _In_ WPARAM wParam, _In_ LPARAM lParam, _In_ LONG_PTR lpRefData)
{
    switch (msg) {
    case TDN_RADIO_BUTTON_CLICKED:
        // When the user clicked the second radio button, navigate the dialog, then show an inner dialog.

        // Navigate
        TASKDIALOGCONFIG* navigationConfig = new TASKDIALOGCONFIG;
        *navigationConfig = { 0 };
        navigationConfig->cbSize = sizeof(TASKDIALOGCONFIG);

        navigationConfig->pszMainInstruction = L"After navigation !!";

        SendMessageW(hwnd, TDM_NAVIGATE_PAGE, 0, (LPARAM)navigationConfig);
        delete navigationConfig;

        // After navigating, run the event loop by showing an inner dialog.
        TaskDialog(nullptr, nullptr, L"Inner Dialog", L"Inner Dialog", nullptr, 0, 0, nullptr);

        break;
    }

    return S_OK;
}

void ShowTaskDialogRadioButtonsBehavior()
{
    TASKDIALOGCONFIG* tConfig = new TASKDIALOGCONFIG;
    *tConfig = { 0 };
    tConfig->cbSize = sizeof(TASKDIALOGCONFIG);

    tConfig->dwFlags = TDF_NO_DEFAULT_RADIO_BUTTON;
    tConfig->pszMainInstruction = L"Before Navigation";
    tConfig->pfCallback = TaskDialogCallbackProc;

    // Create 2 radio buttons.
    int radioButtonCount = 2;

    TASKDIALOG_BUTTON* radioButtons = new TASKDIALOG_BUTTON[radioButtonCount];
    for (int i = 0; i < radioButtonCount; i++) {
        radioButtons[i].nButtonID = 100 + i;
        radioButtons[i].pszButtonText = i == 0 ? L"Radio Button 1" : L"Radio Button 2";
    }
    tConfig->pRadioButtons = radioButtons;
    tConfig->cRadioButtons = radioButtonCount;

    int nButton, nRadioButton;
    BOOL checkboxSelected;
    TaskDialogIndirect(tConfig, &nButton, &nRadioButton, &checkboxSelected);

    delete radioButtons;
    delete tConfig;
}

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
    _In_opt_ HINSTANCE hPrevInstance,
    _In_ LPWSTR    lpCmdLine,
    _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    ShowTaskDialogRadioButtonsBehavior();

    return 0;
}
```