# Windows Task Dialog Issue

## Weird behavior with radio buttons

Weird behavior occurs when running the message loop within an
[`TDN_RADIO_BUTTON_CLICKED`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdn-radio-button-clicked)
notification.

* Run the code.
* Notice that within the
  [`TDN_TIMER`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdn-timer) notification, a
  [`TDM_CLICK_RADIO_BUTTON`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdm-click-radio-button) is
  sent to select the second radio button which causes an inner dialog to be shown within
  the [`TDN_RADIO_BUTTON_CLICKED`](https://docs.microsoft.com/en-us/windows/desktop/Controls/tdn-radio-button-clicked)
  notification, but the radio button is only selected after 
  the inner dialog is closed (and therefore the notification handler returns).<br>
  While that seems like a strange/undesired behavior, it is actually the documented behavior:
  > "The specified radio button ID is sent to the TaskDialogCallbackProc callback function
  as part of a TDN_RADIO_BUTTON_CLICKED notification code. After the callback function
  returns, the radio button will be selected."

  (For the .NET implementation that has the `TaskDialogRadioButton.CheckedChanged` event
  that is expected to show already the new state, this can be worked-around by ignoring
  `TDN_RADIO_BUTTON_CLICKED` notifications caused by the code itself, and instead raising
  the events after sending the `TDM_CLICK_RADIO_BUTTON` message.)
  However, this special behavior might be related to the following issue (and the other
  code samples regarding radio buttons):  
* When you click one of the radio buttons and then immediately close the inner dialog,
  everything works.
* Now try the following: Click the first radio button (but do NOT close the inner dialog),
  then click the second radio button.
* Now close the most inner dialog (showing call count 2), and then the other inner dialog.
* **Issue:** Notice that the dialog changed the selection of the radio button and shows the inner
  dialog again. When you close it, even more inner dialogs open.
* During that time, when you focus the original dialog, it will switch the selected radio
  button again.

```cpp
#include "stdafx.h"
#include "CommCtrl.h"

int callCount = 0;
int timerCounter = 0;
HRESULT WINAPI TaskDialogCallbackProc(_In_ HWND hwnd, _In_ UINT msg, _In_ WPARAM wParam, _In_ LPARAM lParam, _In_ LONG_PTR lpRefData)
{
    switch (msg) {
    case TDN_TIMER:
        if (timerCounter < 4) {
            timerCounter++;
        }
        else if (timerCounter == 4) {
            timerCounter++;

            // Select the second radio button.
            SendMessageW(hwnd, TDM_CLICK_RADIO_BUTTON, 101, 0);
        }

        break;

    case TDN_RADIO_BUTTON_CLICKED:
        callCount++;
        wchar_t* instruction = new wchar_t[200];
        swprintf_s(instruction, 100, L"InnerDialog - CallCount: %d", callCount);
        TaskDialog(nullptr, nullptr, L"Inner Dialog", instruction, nullptr, 0, 0, nullptr);
        delete instruction;
        callCount--;

        break;
    }

    return S_OK;
}

void ShowTaskDialogRadioButtonsBehavior()
{
    TASKDIALOGCONFIG* tConfig = new TASKDIALOGCONFIG;
    *tConfig = { 0 };
    tConfig->cbSize = sizeof(TASKDIALOGCONFIG);

    tConfig->dwFlags = TDF_NO_DEFAULT_RADIO_BUTTON | TDF_CALLBACK_TIMER;
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