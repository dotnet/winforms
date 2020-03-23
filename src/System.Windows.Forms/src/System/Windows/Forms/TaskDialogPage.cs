// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents a page of content of a task dialog.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   It is possible to navigate a task dialog while it is shown by invoking the
    ///   <see cref="Navigate(TaskDialogPage)"/> method with a target <see cref="TaskDialogPage"/>
    ///   instance.
    /// </para>
    /// </remarks>
    public class TaskDialogPage
    {
        /// <summary>
        ///   The start ID for custom buttons.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   We need to ensure we don't use a ID that is already used for a
        ///   standard button (TaskDialogResult), so we start with 100 to be safe
        ///   (100 is also used as first ID in MSDN examples for the task dialog).
        /// </para>
        /// </remarks>
        private const int CustomButtonStartID = 100;

        /// <summary>
        ///   The start ID for radio buttons.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This must be at least 1 because 0 already stands for "no button".
        /// </para>
        /// </remarks>
        private const int RadioButtonStartID = 1;

        private TaskDialogButtonCollection _buttons;
        private TaskDialogRadioButtonCollection _radioButtons;
        private TaskDialogCheckBox? _checkBox;
        private TaskDialogExpander? _expander;
        private TaskDialogFooter? _footer;
        private TaskDialogProgressBar? _progressBar;

        private ComCtl32.TDF _flags;
        private TaskDialogIcon? _icon;
        private string? _caption;
        private string? _mainInstruction;
        private string? _text;
        private int _width;
        private bool _boundIconIsFromHandle;

        private TaskDialogButton[]? _boundCustomButtons;
        private Dictionary<int, TaskDialogButton>? _boundStandardButtonsByID;

        private bool _appliedInitialization;
        private bool _updateMainInstructionOnInitialization;
        private bool _updateTextOnInitialization;

        /// <summary>
        ///   Occurs after this instance is bound to a task dialog and the task dialog
        ///   has created the GUI elements represented by this <see cref="TaskDialogPage"/> instance.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This will happen after showing or navigating the dialog.
        /// </para>
        /// <para>
        ///   When this event occurs, the <see cref="BoundDialog"/> property will return
        ///   the <see cref="TaskDialog"/> instance which this page is bound to.
        /// </para>
        /// </remarks>
        public event EventHandler? Created;

        /// <summary>
        ///   Occurs when the task dialog is about to destroy the GUI elements represented
        ///   by this <see cref="TaskDialogPage"/> instance and it is about to be
        ///   unbound from the task dialog.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This will happen when closing or navigating the dialog.
        /// </para>
        /// <para>
        ///   After this event occurs, the <see cref="BoundDialog"/> property will return
        ///   <see langword="null"/>.
        /// </para>
        /// </remarks>
        public event EventHandler? Destroyed;

        /// <summary>
        ///   Occurs when the user presses F1 while the task dialog has focus, or when the
        ///   user clicks the <see cref="TaskDialogButton.Help"/> button.
        /// </summary>
        public event EventHandler? HelpRequest;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogPage"/> class.
        /// </summary>
        public TaskDialogPage()
        {
            _buttons = new TaskDialogButtonCollection();
            _radioButtons = new TaskDialogRadioButtonCollection();

            // Create empty (hidden) controls.
            _checkBox = new TaskDialogCheckBox();
            _expander = new TaskDialogExpander();
            _footer = new TaskDialogFooter();
            _progressBar = new TaskDialogProgressBar(TaskDialogProgressBarState.None);
        }

        /// <summary>
        ///   Gets or sets the collection of push buttons
        ///   to be shown in this page.
        /// </summary>
        /// <value>
        ///   The collection of custom buttons to be shown in this page.
        /// </value>
        public TaskDialogButtonCollection Buttons
        {
            get => _buttons;

            set
            {
                // We must deny this if we are bound because we need to be able to
                // access the controls from the task dialog's callback.
                DenyIfBound();

                _buttons = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        ///   Gets or sets the default button in the task dialog.
        /// </summary>
        /// <value>
        ///   The default button in the task dialog.
        /// </value>
        public TaskDialogButton? DefaultButton { get; set; }

        /// <summary>
        ///   Gets or sets the collection of radio buttons
        ///   to be shown in this page.
        /// </summary>
        /// <value>
        ///   The collection of radio buttons to be shown in this page.
        /// </value>
        public TaskDialogRadioButtonCollection RadioButtons
        {
            get => _radioButtons;

            set
            {
                // We must deny this if we are bound because we need to be able to
                // access the controls from the task dialog's callback.
                DenyIfBound();

                _radioButtons = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        ///   Gets or sets the check box to be shown in this page.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   The check box will only be shown if its <see cref="TaskDialogCheckBox.Text"/> property
        ///   is not <see langword="null"/> or an empty string.
        /// </para>
        /// </remarks>
        public TaskDialogCheckBox? CheckBox
        {
            get => _checkBox;

            set
            {
                // We must deny this if we are bound because we need to be able to
                // access the control from the task dialog's callback.
                DenyIfBound();

                _checkBox = value;
            }
        }

        /// <summary>
        ///   Gets or sets the dialog expander to be shown in this page.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   The expander button (and the expanded area) will only be shown if its
        ///   <see cref="TaskDialogExpander.Text"/> property is not <see langword="null"/> or an empty string.
        /// </para>
        /// </remarks>
        public TaskDialogExpander? Expander
        {
            get => _expander;

            set
            {
                // We must deny this if we are bound because we need to be able to
                // access the control from the task dialog's callback.
                DenyIfBound();

                _expander = value;
            }
        }

        /// <summary>
        ///   Gets or sets the footer to be shown in this page.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   The footer will only be shown if its <see cref="TaskDialogFooter.Text"/> property
        ///   is not <see langword="null"/> or an empty string.
        /// </para>
        /// </remarks>
        public TaskDialogFooter? Footer
        {
            get => _footer;

            set
            {
                // We must deny this if we are bound because we need to be able to
                // access the control from the task dialog's callback.
                DenyIfBound();

                _footer = value;
            }
        }

        /// <summary>
        ///   Gets or sets the progress bar to be shown in this page.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   The progress bar will only be shown if its <see cref="TaskDialogProgressBar.State"/>
        ///   property is not <see cref="TaskDialogProgressBarState.None"/>.
        /// </para>
        /// </remarks>
        public TaskDialogProgressBar? ProgressBar
        {
            get => _progressBar;

            set
            {
                // We must deny this if we are bound because we need to be able to
                // access the control from the task dialog's callback.
                DenyIfBound();

                _progressBar = value;
            }
        }

        /// <summary>
        ///   Gets or sets the text to display in the title bar of the task dialog.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This property can be set while the dialog is shown.
        /// </para>
        /// </remarks>
        public string? Caption
        {
            get => _caption;

            set
            {
                DenyIfWaitingForInitialization();

                // Note: We set the field values after calling the method to ensure
                // it still has the previous value it the method throws.
                BoundDialog?.UpdateCaption(value);

                _caption = value;
            }
        }

        /// <summary>
        ///   Gets or sets the main instruction text.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This property can be set while the dialog is shown.
        /// </para>
        /// </remarks>
        public string? MainInstruction
        {
            get => _mainInstruction;

            set
            {
                if (BoundDialog != null)
                {
                    // If we are bound but waiting for initialization (e.g. immediately after
                    // starting a navigation), we buffer the change until we apply the
                    // initialization (when navigation is finished).
                    if (WaitingForInitialization)
                    {
                        _updateMainInstructionOnInitialization = true;
                    }
                    else
                    {
                        BoundDialog.UpdateTextElement(ComCtl32.TDE.MAIN_INSTRUCTION, value);
                    }
                }

                _mainInstruction = value;
            }
        }

        /// <summary>
        ///   Gets or sets the dialog's primary text content.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This property can be set while the dialog is shown.
        /// </para>
        /// </remarks>
        public string? Text
        {
            get => _text;

            set
            {
                if (BoundDialog != null)
                {
                    if (WaitingForInitialization)
                    {
                        _updateTextOnInitialization = true;
                    }
                    else
                    {
                        BoundDialog.UpdateTextElement(ComCtl32.TDE.CONTENT, value);
                    }
                }

                _text = value;
            }
        }

        /// <summary>
        ///   Gets or sets the main icon.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This property can be set while the dialog is shown, however, it
        ///   cannot be switched between instances created from an
        ///   <see cref="System.Drawing.Icon"/> (or from a handle pointer)
        ///   and standard icon instances.
        /// </para>
        /// </remarks>
        public unsafe TaskDialogIcon? Icon
        {
            get => _icon;

            set
            {
                // We currently don't support to buffer changes while waiting for the
                // initialization like it is done for string properties (Text, MainInstruction),
                // because for handle icons, this would mean that the previous icon cannot
                // yet be disposed (until initialization is applied) even though the property
                // has been updated to a different icon.
                // It would be possible to initially specify a NULL icon in the TASKDIALOFCONFIG
                // and then update to the actual icon in ApplyInitialization() to avoid this, but
                // that seems like an overkill.
                DenyIfWaitingForInitialization();

                if (BoundDialog != null)
                {
                    (ComCtl32.TASKDIALOGCONFIG.IconUnion icon, bool? iconIsFromHandle) =
                        GetIconValue(value);

                    // The native task dialog icon cannot be updated from a handle
                    // type to a non-handle type and vice versa, so we need to throw
                    // throw in such a case.
                    if (iconIsFromHandle != null && iconIsFromHandle != _boundIconIsFromHandle)
                    {
                        throw new InvalidOperationException(SR.TaskDialogCannotUpdateIconType);
                    }

                    BoundDialog.UpdateIconElement(
                        ComCtl32.TDIE.ICON_MAIN,
                        _boundIconIsFromHandle ? icon.hIcon : (IntPtr)icon.pszIcon);
                }

                _icon = value;
            }
        }

        /// <summary>
        ///   Gets or sets the width in dialog units that the dialog's client area will get
        ///   when the dialog is is created or navigated.
        ///   If <c>0</c>, the width will be automatically calculated by the system.
        /// </summary>
        /// <value>
        ///   The width in dialog units that the dialog's client area will get. The default is
        ///   <c>0</c> which means the width is calculated by the system.
        /// </value>
        public int Width
        {
            get => _width;

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                DenyIfBound();

                _width = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that indicates whether the task dialog can be closed with
        ///   <see cref="TaskDialogButton.Cancel"/> as resulting button by pressing ESC or Alt+F4
        ///   or by clicking the title bar's close button, even if a <see cref="TaskDialogButton.Cancel"/>
        ///   button isn't added to the <see cref="Buttons"/> collection.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> to allow to close the dialog by pressing ESC or Alt+F4 or by clicking
        ///   the title bar's close button; otherwise, <see langword="false"/>. The default value is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   You can intercept cancellation of the dialog without displaying a "Cancel"
        ///   button by adding the <see cref="TaskDialogButton.Cancel"/> button that has its
        ///   <see cref="TaskDialogButton.Visible"/> property set to <see langword="false"/>.
        /// </para>
        /// </remarks>
        public bool AllowCancel
        {
            get => GetFlag(ComCtl32.TDF.ALLOW_DIALOG_CANCELLATION);
            set => SetFlag(ComCtl32.TDF.ALLOW_DIALOG_CANCELLATION, value);
        }

        /// <summary>
        ///   Gets or sets a value that indicates whether text and controls are displayed
        ///   reading right to left.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> to display text and controls reading right to left; <see langword="false"/>
        ///   to display controls reading left to right. The default value is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   Note that once a task dialog has been opened with or has navigated to a
        ///   <see cref="TaskDialogPage"/> where this flag is set, it will keep on
        ///   subsequent navigations to a new <see cref="TaskDialogPage"/> even when
        ///   it doesn't have this flag set.
        /// </para>
        /// </remarks>
        public bool RightToLeftLayout
        {
            get => GetFlag(ComCtl32.TDF.RTL_LAYOUT);
            set => SetFlag(ComCtl32.TDF.RTL_LAYOUT, value);
        }

        /// <summary>
        ///   Gets or sets a value that indicates whether the task dialog can be minimized
        ///   when it is shown modeless.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> to specify that the task dialog can be minimized; otherwise, <see langword="false"/>.
        ///   The default value is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   When setting this property to <see langword="true"/>, <see cref="AllowCancel"/> is
        ///   automatically implied.
        /// </para>
        /// </remarks>
        public bool AllowMinimize
        {
            get => GetFlag(ComCtl32.TDF.CAN_BE_MINIMIZED);
            set => SetFlag(ComCtl32.TDF.CAN_BE_MINIMIZED, value);
        }

        /// <summary>
        ///   Indicates that the width of the task dialog is determined by the width
        ///   of its content area (similar to Message Box sizing behavior).
        /// </summary>
        /// <value>
        ///   <see langword="true"/> to determine the width of the task dialog by the width of
        ///   its content area; otherwise, <see langword="false"/>. The default value is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   This flag is ignored if <see cref="Width"/> is not set to <c>0</c>.
        /// </para>
        /// </remarks>
        public bool SizeToContent
        {
            get => GetFlag(ComCtl32.TDF.SIZE_TO_CONTENT);
            set => SetFlag(ComCtl32.TDF.SIZE_TO_CONTENT, value);
        }

        /// <summary>
        ///   Gets the <see cref="TaskDialog"/> instance which this page
        ///   is currently bound to.
        /// </summary>
        /// <value>
        ///   The <see cref="TaskDialog"/> instance which this page is bound to, or <see langword="null"/>
        ///   if this page is not currently bound.
        /// </value>
        /// <remarks>
        /// <para>
        ///   A page will be bound while it is being displayed, which is indicated by the events
        ///   <see cref="Created"/> and <see cref="Destroyed"/>.
        /// </para>
        /// <para>
        ///   While a page is bound to a task dialog, you cannot show that page instance using a
        ///   different <see cref="TaskDialog"/> instance at the same time.
        /// </para>
        /// </remarks>
        public TaskDialog? BoundDialog { get; private set; }

        /// <summary>
        ///   Gets a value that indicates if the <see cref="BoundDialog"/>
        ///   started navigation to this page but navigation did not yet complete
        ///   (in which case we cannot modify the dialog even though we are bound).
        /// </summary>
        internal bool WaitingForInitialization => BoundDialog != null && !_appliedInitialization;

        /// <summary>
        ///  Show the new content in the dialog.
        /// <paramref name="page"/>.
        /// </summary>
        /// <param name="page">
        ///   The page instance that contains the contents which this task dialog will display.
        /// </param>
        /// <remarks>
        /// <para>
        ///   During the navigation the dialog will recreate the dialog from the specified
        ///   <paramref name="page"/> and its controls, and unbind and destroy the currently shown page.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="page"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The page instance is not bound to a dialog, <see cref="BoundDialog"/> is <see langword="null"/>.
        /// </exception>
        public void Navigate(TaskDialogPage page)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (BoundDialog == null)
            {
                throw new InvalidOperationException(SR.TaskDialogCannotNavigateWithoutDialog);
            }

            BoundDialog.Navigate(page);
        }

        internal static bool IsNativeStringNullOrEmpty(string? str)
        {
            // From a native point of view, the string is empty if its first
            // character is a NUL char.
            return string.IsNullOrEmpty(str) || str[0] == '\0';
        }

        internal static unsafe (ComCtl32.TASKDIALOGCONFIG.IconUnion iconUnion, bool? iconIsFromHandle)
            GetIconValue(TaskDialogIcon? icon)
        {
            ComCtl32.TASKDIALOGCONFIG.IconUnion iconUnion = default;
            bool? iconIsFromHandle = null;

            // If no icon is specified (icon is null), the iconIsFromHandle variable will be
            // null, which allows to clear the dialog's icon while it is shown, regardless
            // of whether the bound icon is a handle icon or a non-handle icon.
            if (icon?.IsHandleIcon == true)
            {
                iconUnion.hIcon = icon.IconHandle;
                iconIsFromHandle = true;
            }
            else if (icon?.IsStandardIcon == true)
            {
                // Convert the value to an ushort before converting it to a pointer,
                // which corresponds to using the MAKEINTRESOURCEW macro in native code.
#pragma warning disable IDE0004 // Remove Unnecessary Cast (false positive, see https://github.com/dotnet/roslyn/issues/20617)
                iconUnion.pszIcon = (char*)checked((ushort)icon.StandardIcon);
#pragma warning restore IDE0004 // Remove Unnecessary Cast
                iconIsFromHandle = false;
            }

            return (iconUnion, iconIsFromHandle);
        }

        internal void DenyIfBound()
        {
            if (BoundDialog != null)
            {
                throw new InvalidOperationException(SR.TaskDialogCannotSetPropertyOfBoundPage);
            }
        }

        internal void DenyIfWaitingForInitialization()
        {
            if (WaitingForInitialization)
            {
                throw new InvalidOperationException(string.Format(
                    SR.TaskDialogNavigationNotCompleted,
                    $"{nameof(TaskDialogPage)}.{nameof(Created)}"));
            }
        }

        internal TaskDialogButton? GetBoundButtonByID(int buttonID)
        {
            if (BoundDialog == null)
            {
                throw new InvalidOperationException();
            }

            if (buttonID == 0)
            {
                return null;
            }

            // Check if the button is part of the custom buttons.
            if (buttonID >= CustomButtonStartID)
            {
                return _boundCustomButtons![buttonID - CustomButtonStartID];
            }
            else
            {
                // Note: We deliberately return null instead of throwing when
                // the common button ID is not part of the collection, because
                // the caller might not know if such a button exists.
                _boundStandardButtonsByID!.TryGetValue(buttonID, out TaskDialogButton? button);
                return button;
            }
        }

        internal TaskDialogRadioButton? GetBoundRadioButtonByID(int buttonID)
        {
            if (BoundDialog == null)
            {
                throw new InvalidOperationException();
            }

            return buttonID == 0 ? null : _radioButtons[buttonID - RadioButtonStartID];
        }

        internal void Validate()
        {
            // Check that this page instance is not already bound to a TaskDialog instance.
            if (BoundDialog != null)
            {
                throw new InvalidOperationException(string.Format(
                    SR.TaskDialogPageIsAlreadyBound,
                    nameof(TaskDialogPage),
                    nameof(TaskDialog)));
            }

            // We also need to check the controls (and collections) since they could also be
            // bound to a TaskDialogPage at the same time.
            if (_buttons.BoundPage != null ||
                _radioButtons.BoundPage != null)
            {
                throw new InvalidOperationException(string.Format(
                    SR.TaskDialogCollectionAlreadyBound,
                    nameof(TaskDialogPage),
                    nameof(TaskDialog)));
            }

            if (_buttons.Concat<TaskDialogControl>(_radioButtons)
                .Append(_checkBox)
                .Append(_expander)
                .Append(_footer)
                .Append(_progressBar)
                .Any(c => c?.BoundPage != null))
            {
                throw new InvalidOperationException(string.Format(
                    SR.TaskDialogControlAlreadyBound,
                    nameof(TaskDialogPage),
                    nameof(TaskDialog)));
            }

            // Note: Theoretically we would need to exclude the standard buttons from the
            // "_buttons" collection when retrieving the Count (because they already have
            // a fixed button ID), but since not excluding them won't allow more buttons
            // than possible, we don't need to do it.
            if (_buttons.Count > int.MaxValue - CustomButtonStartID + 1 ||
                _radioButtons.Count > int.MaxValue - RadioButtonStartID + 1)
            {
                throw new InvalidOperationException(SR.TaskDialogTooManyButtonsAdded);
            }

            if (_radioButtons.Count(e => e.Checked) > 1)
            {
                throw new InvalidOperationException(SR.TaskDialogOnlySingleRadioButtonCanBeChecked);
            }

            // Check that we don't have custom buttons and command links at the same time.
            bool foundCustomButton = false;
            bool foundCommandLink = false;
            foreach (TaskDialogButton button in _buttons)
            {
                if (button is TaskDialogCommandLinkButton commandLink)
                {
                    foundCommandLink = true;
                }
                else if (!button.IsStandardButton)
                {
                    foundCustomButton = true;
                }
            }

            if (foundCustomButton && foundCommandLink)
            {
                throw new InvalidOperationException(/* TODO */ "Cannot show both custom buttons and command links at the same time.");
            }

            // For custom and radio buttons, we need to ensure the strings are not null or empty
            // (except for invisible buttons), as otherwise an error would occur when
            // showing/navigating the dialog.
            if (_buttons.Any(e => e.IsCreatable && !e.IsStandardButton && IsNativeStringNullOrEmpty(e.Text)))
            {
                throw new InvalidOperationException(SR.TaskDialogButtonTextMustNotBeNull);
            }

            if (_radioButtons.Any(e => e.IsCreatable && IsNativeStringNullOrEmpty(e.Text)))
            {
                throw new InvalidOperationException(SR.TaskDialogRadioButtonTextMustNotBeNull);
            }

            // Ensure the default button is part of the button collection.
            if (DefaultButton != null && !_buttons.Contains(DefaultButton))
            {
                throw new InvalidOperationException(SR.TaskDialogDefaultButtonMustExistInCollection);
            }
        }

        internal void Bind(
            TaskDialog owner,
            out ComCtl32.TDF flags,
            out ComCtl32.TDCBF buttonFlags,
            out IEnumerable<(int buttonID, string text)> customButtonElements,
            out IEnumerable<(int buttonID, string text)> radioButtonElements,
            out ComCtl32.TASKDIALOGCONFIG.IconUnion mainIcon,
            out ComCtl32.TASKDIALOGCONFIG.IconUnion footerIcon,
            out int defaultButtonID,
            out int defaultRadioButtonID)
        {
            if (BoundDialog != null)
            {
                throw new InvalidOperationException();
            }

            // This method assumes Validate() has already been called.

            BoundDialog = owner;
            flags = _flags;

            _updateTextOnInitialization = false;
            _updateMainInstructionOnInitialization = false;

            (ComCtl32.TASKDIALOGCONFIG.IconUnion localIconValue, bool? iconIsFromHandle) = GetIconValue(_icon);
            (mainIcon, _boundIconIsFromHandle) = (localIconValue, iconIsFromHandle ?? false);

            if (_boundIconIsFromHandle)
            {
                flags |= ComCtl32.TDF.USE_HICON_MAIN;
            }

            TaskDialogButtonCollection buttons = _buttons;
            TaskDialogRadioButtonCollection radioButtons = _radioButtons;

            buttons.BoundPage = this;
            radioButtons.BoundPage = this;

            // Sort the buttons.
            _boundCustomButtons = buttons.Where(e => !e.IsStandardButton).ToArray();
            _boundStandardButtonsByID = new Dictionary<int, TaskDialogButton>(
                buttons.Where(e => e.IsStandardButton)
                .Select(e => new KeyValuePair<int, TaskDialogButton>(e.ButtonID, e)));

            // Assign IDs to the buttons based on their index.
            defaultButtonID = 0;
            buttonFlags = default;
            foreach (TaskDialogButton standardButton in _boundStandardButtonsByID.Values)
            {
                flags |= standardButton.Bind(this);

                if (standardButton.IsCreated)
                {
                    buttonFlags |= standardButton.GetStandardButtonFlag();
                }
            }

            for (int i = 0; i < _boundCustomButtons.Length; i++)
            {
                TaskDialogButton customButton = _boundCustomButtons[i];
                flags |= customButton.Bind(this, CustomButtonStartID + i);
            }

            if (DefaultButton != null)
            {
                // Retrieve the button from the collection, to handle the case for standard buttons
                // when the user set an equal (but not same) instance as default button.
                var defaultButton = buttons[buttons.IndexOf(DefaultButton)];
                if (defaultButton.IsCreated)
                {
                    defaultButtonID = defaultButton.ButtonID;
                }
            }

            defaultRadioButtonID = 0;
            for (int i = 0; i < radioButtons.Count; i++)
            {
                TaskDialogRadioButton radioButton = radioButtons[i];
                flags |= radioButton.Bind(this, RadioButtonStartID + i);

                if (radioButton.IsCreated)
                {
                    if (radioButton.Checked && defaultRadioButtonID == 0)
                    {
                        defaultRadioButtonID = radioButton.RadioButtonID;
                    }
                    else if (radioButton.Checked)
                    {
                        radioButton.Checked = false;
                    }
                }
            }

            if (defaultRadioButtonID == 0)
            {
                flags |= ComCtl32.TDF.NO_DEFAULT_RADIO_BUTTON;
            }

            customButtonElements = _boundCustomButtons.Where(e => e.IsCreated).Select(e => (e.ButtonID, e.GetResultingText()!));
            radioButtonElements = radioButtons.Where(e => e.IsCreated).Select(e => (e.RadioButtonID, e.Text!));

            // If we have command links, specify the TDF_USE_COMMAND_LINKS flag.
            // Note: The USE_COMMAND_LINKS_NO_ICON is currently not used.
            if (_boundCustomButtons.Any(e => e.IsCreated && e is TaskDialogCommandLinkButton))
                flags |= ComCtl32.TDF.USE_COMMAND_LINKS;

            if (_checkBox != null)
            {
                flags |= _checkBox.Bind(this);
            }

            if (_expander != null)
            {
                flags |= _expander.Bind(this);
            }

            if (_footer != null)
            {
                flags |= _footer.Bind(this, out footerIcon);
            }
            else
            {
                footerIcon = default;
            }

            if (_progressBar != null)
            {
                flags |= _progressBar.Bind(this);
            }
        }

        internal void Unbind()
        {
            if (BoundDialog == null)
            {
                throw new InvalidOperationException();
            }

            _boundCustomButtons = null;
            _boundStandardButtonsByID = null;

            TaskDialogButtonCollection buttons = _buttons;
            TaskDialogRadioButtonCollection radioButtons = _radioButtons;

            foreach (TaskDialogButton button in buttons)
            {
                button.Unbind();
            }

            foreach (TaskDialogRadioButton radioButton in radioButtons)
            {
                radioButton.Unbind();
            }

            buttons.BoundPage = null;
            radioButtons.BoundPage = null;

            _checkBox?.Unbind();
            _expander?.Unbind();
            _footer?.Unbind();
            _progressBar?.Unbind();

            BoundDialog = null;
            _appliedInitialization = false;
        }

        internal void ApplyInitialization()
        {
            if (_appliedInitialization)
            {
                throw new InvalidOperationException();
            }

            _appliedInitialization = true;

            // Check if we need to update some of our elements (if they have been modified
            // after starting navigation, but before navigation was finished).
            if (_updateMainInstructionOnInitialization)
            {
                MainInstruction = _mainInstruction;
                _updateMainInstructionOnInitialization = false;
            }

            if (_updateTextOnInitialization)
            {
                Text = _text;
                _updateTextOnInitialization = false;
            }

            TaskDialogButtonCollection buttons = _buttons;
            TaskDialogRadioButtonCollection radioButtons = _radioButtons;

            foreach (TaskDialogButton button in buttons)
            {
                button.ApplyInitialization();
            }

            foreach (TaskDialogRadioButton button in radioButtons)
            {
                button.ApplyInitialization();
            }

            _checkBox?.ApplyInitialization();
            _expander?.ApplyInitialization();
            _footer?.ApplyInitialization();
            _progressBar?.ApplyInitialization();
        }

        /// <summary>
        ///   Raises the <see cref="Created"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        internal protected void OnCreated(EventArgs e) => Created?.Invoke(this, e);

        /// <summary>
        ///   Raises the <see cref="Destroyed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        internal protected void OnDestroyed(EventArgs e) => Destroyed?.Invoke(this, e);

        /// <summary>
        ///   Raises the <see cref="HelpRequest"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        internal protected void OnHelpRequest(EventArgs e) => HelpRequest?.Invoke(this, e);

        private bool GetFlag(ComCtl32.TDF flag) => (_flags & flag) == flag;

        private void SetFlag(ComCtl32.TDF flag, bool value)
        {
            DenyIfBound();

            if (value)
            {
                _flags |= flag;
            }
            else
            {
                _flags &= ~flag;
            }
        }
    }
}
