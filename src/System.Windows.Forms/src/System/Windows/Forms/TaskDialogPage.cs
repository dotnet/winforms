// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Linq;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents a page of content of a task dialog.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   It is possible to navigate a task dialog while it is shown by setting the
    ///   <see cref="TaskDialog.Page"/> property to a different <see cref="TaskDialogPage"/>
    ///   instance. For more information about navigation, see the
    ///   <see cref="TaskDialog.Page"/> property.
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

        private TaskDialogStandardButtonCollection _standardButtons;
        private TaskDialogCustomButtonCollection _customButtons;
        private TaskDialogRadioButtonCollection _radioButtons;
        private TaskDialogCheckBox? _checkBox;
        private TaskDialogExpander? _expander;
        private TaskDialogFooter? _footer;
        private TaskDialogProgressBar? _progressBar;

        private ComCtl32.TDF _flags;
        private TaskDialogCustomButtonStyle _customButtonStyle;
        private TaskDialogIcon? _icon;
        private string? _caption;
        private string? _mainInstruction;
        private string? _text;
        private int _width;
        private bool _boundIconIsFromHandle;

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
        ///   user clicks the <see cref="TaskDialogResult.Help"/> button.
        /// </summary>
        public event EventHandler? HelpRequest;

        /// <summary>
        ///   Occurs when the user has clicked on a hyperlink.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This event will only be raised if <see cref="EnableHyperlinks"/> is set to <see langword="true"/>.
        /// </para>
        /// </remarks>
        public event EventHandler<TaskDialogHyperlinkClickedEventArgs>? HyperlinkClicked;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogPage"/> class.
        /// </summary>
        public TaskDialogPage()
        {
            _customButtons = new TaskDialogCustomButtonCollection();
            _standardButtons = new TaskDialogStandardButtonCollection();
            _radioButtons = new TaskDialogRadioButtonCollection();

            // Create empty (hidden) controls.
            _checkBox = new TaskDialogCheckBox();
            _expander = new TaskDialogExpander();
            _footer = new TaskDialogFooter();
            _progressBar = new TaskDialogProgressBar(TaskDialogProgressBarState.None);
        }

        /// <summary>
        ///   Gets or sets the collection of standard buttons
        ///   to be shown in this page.
        /// </summary>
        /// <value>
        ///   The collection of standard buttons to be shown in this page.
        /// </value>
        public TaskDialogStandardButtonCollection StandardButtons
        {
            get => _standardButtons;

            set
            {
                // We must deny this if we are bound because we need to be able to
                // access the controls from the task dialog's callback.
                DenyIfBound();

                _standardButtons = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        ///   Gets or sets the collection of custom buttons
        ///   to be shown in this page.
        /// </summary>
        /// <value>
        ///   The collection of custom buttons to be shown in this page.
        /// </value>
        public TaskDialogCustomButtonCollection CustomButtons
        {
            get => _customButtons;

            set
            {
                // We must deny this if we are bound because we need to be able to
                // access the controls from the task dialog's callback.
                DenyIfBound();

                _customButtons = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

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
        public TaskDialogIcon? Icon
        {
            get => _icon;

            set
            {
                // We currently don't support to buffer changes while waiting for the
                // initialization like it is done for string properties (Text, MainInstruction),
                // because for handle icons, this would mean that the previous icon cannot
                // yet be disposed (until initialization is applied) even though the property
                // has been updated to a different icon.
                DenyIfWaitingForInitialization();

                (IntPtr iconValue, bool? iconIsFromHandle) = GetIconValue(value);

                // The native task dialog icon cannot be updated from a handle
                // type to a non-handle type and vice versa, so we need to throw
                // throw in such a case.
                if (BoundDialog != null && iconIsFromHandle != null &&
                    iconIsFromHandle != _boundIconIsFromHandle)
                {
                    throw new InvalidOperationException(SR.TaskDialogCannotUpdateIconType);
                }

                BoundDialog?.UpdateIconElement(ComCtl32.TDIE.ICON_MAIN, iconValue);
                
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
        ///   Gets or sets the <see cref="TaskDialogCustomButtonStyle"/> that specifies how to
        ///   display custom buttons.
        /// </summary>
        /// <value>
        ///   The <see cref="TaskDialogCustomButtonStyle"/> that specifies how to display custom
        ///   buttons. The default value is <see cref="TaskDialogCustomButtonStyle.Default"/>.
        /// </value>
        public TaskDialogCustomButtonStyle CustomButtonStyle
        {
            get => _customButtonStyle;

            set
            {
                DenyIfBound();

                _customButtonStyle = value;
            }
        }

        /// <summary>
        /// <para>
        ///   Gets or sets a value that specifies whether the task dialog should
        ///   interpret strings in the form <c>&lt;a href="link"&gt;Hyperlink Text&lt;/a&gt;</c>
        ///   as hyperlink when specified in the <see cref="Text"/>,
        ///   <see cref="TaskDialogFooter.Text"/>
        ///   or <see cref="TaskDialogExpander.Text"/> properties.
        ///   When the user clicks on such a link, the <see cref="HyperlinkClicked"/>
        ///   event is raised, containing the value of the <c>href</c> attribute.
        /// </para>
        /// <para>
        ///   <b>Warning:</b> Enabling hyperlinks when using content from an unsafe source
        ///   may cause security vulnerabilities.
        /// </para>
        /// </summary>
        /// <value>
        ///   <see langword="true"/> to enable hyperlinks; otherwise, <see langword="false"/>. 
        ///   The default value is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   The Task Dialog will not actually execute any hyperlinks.
        ///   Hyperlink execution must be handled in the <see cref="HyperlinkClicked"/> event.
        /// </para>
        /// <para>
        ///   Note: Enabling this setting causes the <c>"&amp;"</c> character to be
        ///   interpreted as prefix for an access key character (mnemonic) if at least
        ///   one link is used.
        /// </para>
        /// <para>
        ///   When you enable this setting and you want to display a text
        ///   without interpreting links, you must replace the strings <c>"&lt;a"</c>
        ///   and <c>"&lt;A"</c> with something like <c>"&lt;\u200Ba"</c>.
        /// </para>
        /// </remarks>
        public bool EnableHyperlinks
        {
            get => GetFlag(ComCtl32.TDF.ENABLE_HYPERLINKS);
            set => SetFlag(ComCtl32.TDF.ENABLE_HYPERLINKS, value);
        }

        /// <summary>
        ///   Gets or sets a value that indicates whether the task dialog can be closed with a
        ///   <see cref="TaskDialogResult.Cancel"/> result by pressing ESC or Alt+F4 or by clicking
        ///   the title bar's close button even if no button with a <see cref="TaskDialogResult.Cancel"/>
        ///   result is added to the <see cref="StandardButtons"/> collection.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> to allow to close the dialog by pressing ESC or Alt+F4 or by clicking
        ///   the title bar's close button; otherwise, <see langword="false"/>. The default value is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   You can intercept cancellation of the dialog without displaying a "Cancel"
        ///   button by adding a <see cref="TaskDialogStandardButton"/> with its
        ///   <see cref="TaskDialogStandardButton.Visible"/> set to <see langword="false"/> and specifying
        ///   a <see cref="TaskDialogResult.Cancel"/> result.
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

        internal static bool IsNativeStringNullOrEmpty(string? str)
        {
            // From a native point of view, the string is empty if its first
            // character is a NUL char.
            return string.IsNullOrEmpty(str) || str[0] == '\0';
        }

        internal static (IntPtr iconValue, bool? iconIsFromHandle) GetIconValue(TaskDialogIcon? icon)
        {
            IntPtr iconValue = default;
            bool? iconIsFromHandle = null;

            // If no icon is specified (icon is null), we don't set the
            // "iconIsFromHandle" flag, which means that the icon can be updated
            // to show a Standard icon while the dialog is running.
            if (icon?.IsHandleIcon == true)
            {
                iconIsFromHandle = true;
                iconValue = icon.IconHandle;
            }
            else if (icon?.IsStandardIcon == true)
            {
                iconIsFromHandle = false;
                iconValue = (IntPtr)icon.StandardIcon;
            }

            return (iconValue, iconIsFromHandle);
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
            var button = null as TaskDialogButton;
            if (buttonID >= CustomButtonStartID)
            {
                button = _customButtons[buttonID - CustomButtonStartID];
            }
            else
            {
                // Note: We deliberately return null instead of throwing when
                // the common button ID is not part of the collection, because
                // the caller might not know if such a button exists.
                var result = (TaskDialogResult)buttonID;
                if (_standardButtons.Contains(result))
                {
                    button = _standardButtons[result];
                }
            }

            return button;
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
            if (_standardButtons.BoundPage != null ||
                _customButtons.BoundPage != null ||
                _radioButtons.BoundPage != null)
            {
                throw new InvalidOperationException(string.Format(
                    SR.TaskDialogCollectionAlreadyBound,
                    nameof(TaskDialogPage),
                    nameof(TaskDialog)));
            }

            if (StandardButtons.Concat<TaskDialogControl>(_customButtons).Concat(_radioButtons)
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

            if (_customButtons.Count > int.MaxValue - CustomButtonStartID + 1 ||
                _radioButtons.Count > int.MaxValue - RadioButtonStartID + 1)
            {
                throw new InvalidOperationException(SR.TaskDialogTooManyButtonsAdded);
            }

            if (_standardButtons.Concat<TaskDialogButton>(_customButtons).Count(e => e.DefaultButton) > 1)
            {
                throw new InvalidOperationException(SR.TaskDialogOnlySingleButtonCanBeDefault);
            }

            if (_radioButtons.Count(e => e.Checked) > 1)
            {
                throw new InvalidOperationException(SR.TaskDialogOnlySingleRadioButtonCanBeChecked);
            }

            // For custom and radio buttons, we need to ensure the strings are not null or empty,
            // as otherwise an error would occur when showing/navigating the dialog.
            if (_customButtons.Any(e => !e.IsCreatable))
            {
                throw new InvalidOperationException(SR.TaskDialogButtonTextMustNotBeNull);
            }

            if (_radioButtons.Any(e => !e.IsCreatable))
            {
                throw new InvalidOperationException(SR.TaskDialogRadioButtonTextMustNotBeNull);
            }
        }

        internal void Bind(
            TaskDialog owner,
            out ComCtl32.TDF flags,
            out TaskDialogButtons buttonFlags,
            out IntPtr iconValue,
            out IntPtr footerIconValue,
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

            (IntPtr localIconValue, bool? iconIsFromHandle) = GetIconValue(_icon);
            (iconValue, _boundIconIsFromHandle) = (localIconValue, iconIsFromHandle ?? false);

            if (_boundIconIsFromHandle)
            {
                flags |= ComCtl32.TDF.USE_HICON_MAIN;
            }

            // Only specify the command link flags if there actually are custom buttons;
            // otherwise the dialog will not work.
            if (_customButtons.Count > 0)
            {
                if (_customButtonStyle == TaskDialogCustomButtonStyle.CommandLinks)
                {
                    flags |= ComCtl32.TDF.USE_COMMAND_LINKS;
                }
                else if (_customButtonStyle == TaskDialogCustomButtonStyle.CommandLinksNoIcon)
                {
                    flags |= ComCtl32.TDF.USE_COMMAND_LINKS_NO_ICON;
                }
            }

            TaskDialogStandardButtonCollection standardButtons = _standardButtons;
            TaskDialogCustomButtonCollection customButtons = _customButtons;
            TaskDialogRadioButtonCollection radioButtons = _radioButtons;

            standardButtons.BoundPage = this;
            customButtons.BoundPage = this;
            radioButtons.BoundPage = this;

            // Assign IDs to the buttons based on their index.
            // Note: The collections will be locked while this page is bound, so we
            // don't need to copy them here.
            defaultButtonID = 0;
            buttonFlags = default;
            foreach (TaskDialogStandardButton standardButton in standardButtons)
            {
                flags |= standardButton.Bind(this);

                if (standardButton.IsCreated)
                {
                    buttonFlags |= standardButton.GetButtonFlag();

                    if (standardButton.DefaultButton && defaultButtonID == 0)
                    {
                        defaultButtonID = standardButton.ButtonID;
                    }
                }
            }

            for (int i = 0; i < customButtons.Count; i++)
            {
                TaskDialogCustomButton customButton = customButtons[i];
                flags |= customButton.Bind(this, CustomButtonStartID + i);

                if (customButton.IsCreated)
                {
                    if (customButton.DefaultButton && defaultButtonID == 0)
                    {
                        defaultButtonID = customButton.ButtonID;
                    }
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
                flags |= _footer.Bind(this, out footerIconValue);
            }
            else
            {
                footerIconValue = default;
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

            TaskDialogStandardButtonCollection standardButtons = _standardButtons;
            TaskDialogCustomButtonCollection customButtons = _customButtons;
            TaskDialogRadioButtonCollection radioButtons = _radioButtons;

            foreach (TaskDialogStandardButton standardButton in standardButtons)
            {
                standardButton.Unbind();
            }

            foreach (TaskDialogCustomButton customButton in customButtons)
            {
                customButton.Unbind();
            }

            foreach (TaskDialogRadioButton radioButton in radioButtons)
            {
                radioButton.Unbind();
            }

            standardButtons.BoundPage = null;
            customButtons.BoundPage = null;
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

            TaskDialogStandardButtonCollection standardButtons = _standardButtons;
            TaskDialogCustomButtonCollection customButtons = _customButtons;
            TaskDialogRadioButtonCollection radioButtons = _radioButtons;

            foreach (TaskDialogStandardButton button in standardButtons)
            {
                button.ApplyInitialization();
            }

            foreach (TaskDialogCustomButton button in customButtons)
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

        /// <summary>
        ///   Raises the <see cref="HyperlinkClicked"/> event.
        /// </summary>
        /// <param name="e">An <see cref="TaskDialogHyperlinkClickedEventArgs"/> that contains the event data.</param>
        internal protected void OnHyperlinkClicked(TaskDialogHyperlinkClickedEventArgs e) => HyperlinkClicked?.Invoke(this, e);

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
