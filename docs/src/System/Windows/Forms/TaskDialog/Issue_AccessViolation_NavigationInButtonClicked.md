# Windows Task Dialog Issue

## Access Violation when receiving [`TDN_NAVIGATED`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdn-navigated) within [`TDN_BUTTON_CLICKED`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdn-button-clicked)

An Access violation can occur when receiving a navigation notification ([`TDN_NAVIGATED`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdn-navigated))
within a [`TDN_BUTTON_CLICKED`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdn-button-clicked)
notification (e.g. due to running the message loop) and then returning `S_OK`.
 
* Run the code.
* Click on the "My Custom Button" button.
* Notice that the dialog navigates and an inner dialog shows.
* Close the inner dialog by clicking the OK button.
* **Issue:** In most cases, an access violation occurs now (if not, try again a
  few times).
* Notice the problem also occurs when you (after the navigation) first close
  the original dialog, and then close the inner dialog.
* The problem can be avoided by returning `S_FALSE` from the `TDN_BUTTON_CLICKED`
  notification.

```cpp
#include "stdafx.h"
#include "CommCtrl.h"

HRESULT WINAPI TaskDialogCallbackProc(_In_ HWND hwnd, _In_ UINT msg, _In_ WPARAM wParam, _In_ LPARAM lParam, _In_ LONG_PTR lpRefData)
{
    switch (msg) {
    case TDN_BUTTON_CLICKED:
        if (wParam == 100) {
            TASKDIALOGCONFIG* navigationConfig = new TASKDIALOGCONFIG;
            *navigationConfig = { 0 };
            navigationConfig->cbSize = sizeof(TASKDIALOGCONFIG);

            navigationConfig->pszMainInstruction = L"After navigation !!!";
            navigationConfig->pszContent = L"Text";
            navigationConfig->pszMainIcon = MAKEINTRESOURCEW(0xFFF7);

            // Navigate the dialog.
            SendMessageW(hwnd, TDM_NAVIGATE_PAGE, 0, (LPARAM)navigationConfig);
            delete navigationConfig;

            // After that, run the event loop by show an inner dialog.
            TaskDialog(nullptr, nullptr, L"Inner Dialog", L"Inner Dialog", nullptr, 0, 0, nullptr);
        }

        break;
    }

    return S_OK;
}

void ShowTaskDialogNavigationInButtonClickedNotification()
{
    TASKDIALOGCONFIG* tConfig = new TASKDIALOGCONFIG;
    *tConfig = { 0 };
    tConfig->cbSize = sizeof(TASKDIALOGCONFIG);

    tConfig->nDefaultRadioButton = 100;
    tConfig->pszMainInstruction = L"Before navigation";
    tConfig->pfCallback = TaskDialogCallbackProc;

    // Create 50 radio buttons.
    int radioButtonCount = 50;
    TASKDIALOG_BUTTON* radioButtons = new TASKDIALOG_BUTTON[radioButtonCount];
    for (int i = 0; i < radioButtonCount; i++) {
        radioButtons[i].nButtonID = 100 + i;
        radioButtons[i].pszButtonText = L"My Radio Button";
    }
    tConfig->pRadioButtons = radioButtons;
    tConfig->cRadioButtons = radioButtonCount;

    // Create a custom button.
    TASKDIALOG_BUTTON* customButton = new TASKDIALOG_BUTTON;
    customButton->nButtonID = 100;
    customButton->pszButtonText = L"My Custom Button";
    tConfig->pButtons = customButton;
    tConfig->cButtons = 1;

    int nButton, nRadioButton;
    BOOL checkboxSelected;
    TaskDialogIndirect(tConfig, &nButton, &nRadioButton, &checkboxSelected);

    delete customButton;
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

    ShowTaskDialogNavigationInButtonClickedNotification();

    return 0;
}
```