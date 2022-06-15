# Windows Task Dialog Issue

## Infinite loop with radio buttons

An infinite loop occurs when sending a
[`TDM_CLICK_RADIO_BUTTON`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdm-click-radio-button) message within a
[`TDN_RADIO_BUTTON_CLICKED`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdn-radio-button-clicked) notification.
 
* Run the code and select the second radio button.
* **Issue:** Notice that the GUI doesn't respond any more. When debugging, you can see that
  the callback is flooded with `TDN_RADIO_BUTTON_CLICKED` notifications even though we
  don't send any more messages to the dialog.

```cpp
#include "stdafx.h"
#include "CommCtrl.h"

bool done = false;
HRESULT WINAPI TaskDialogCallbackProc(_In_ HWND hwnd, _In_ UINT msg, _In_ WPARAM wParam, _In_ LPARAM lParam, _In_ LONG_PTR lpRefData)
{
    switch (msg) {
    case TDN_RADIO_BUTTON_CLICKED:
        // When the user clicked the second radio button, select the first one.
        // However, do this only once.
        if (wParam == 101 && !done) {
            done = true;
            SendMessageW(hwnd, TDM_CLICK_RADIO_BUTTON, 100, 0);
        }
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
    tConfig->pszMainInstruction = L"Radio Button Example";
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