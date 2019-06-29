// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;

using TaskDialogFlags = Interop.TaskDialog.TASKDIALOG_FLAGS;
using TaskDialogIconElement = Interop.TaskDialog.TASKDIALOG_ICON_ELEMENTS;
using TaskDialogTextElement = Interop.TaskDialog.TASKDIALOG_ELEMENTS;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a page of content of a task dialog.
    /// </summary>
    /// <remarks>
    /// It is possible to navigate a task dialog while it is shown by setting the
    /// <see cref="TaskDialog.Page"/> property to a different <see cref="TaskDialogPage"/>
    /// instance. See the <see cref="TaskDialog.Page"/> property for more information about
    /// navigation.
    /// </remarks>
    public class TaskDialogPage
    {
        /// <summary>
        /// The start ID for custom buttons.
        /// </summary>
        /// <remarks>
        /// We need to ensure we don't use a ID that is already used for a
        /// standard button (TaskDialogResult), so we start with 100 to be safe
        /// (100 is also used as first ID in MSDN examples for the task dialog).
        /// </remarks>
        private const int CustomButtonStartID = 100;

        /// <summary>
        /// The start ID for radio buttons.
        /// </summary>
        /// <remarks>
        /// This must be at least 1 because 0 already stands for "no button".
        /// </remarks>
        private const int RadioButtonStartID = 1;

        private TaskDialogStandardButtonCollection _standardButtons;

        private TaskDialogCustomButtonCollection _customButtons;

        private TaskDialogRadioButtonCollection _radioButtons;

        private TaskDialogCheckBox _checkBox;

        private TaskDialogExpander _expander;

        private TaskDialogFooter _footer;

        private TaskDialogProgressBar _progressBar;

        private TaskDialogFlags _flags;
        private string _title;
        private string _instruction;
        private string _text;
        private TaskDialogIcon _icon;
        private int _width;
        private TaskDialogCustomButtonStyle _customButtonStyle;

        private TaskDialog _boundTaskDialog;

        private bool _boundIconIsFromHandle;

        private bool _appliedInitialization;

        /// <summary>
        /// Occurs after this instance is bound to a task dialog and the task dialog
        /// has created the GUI elements represented by this
        /// <see cref="TaskDialogPage"/> instance.
        /// </summary>
        /// <remarks>
        /// This will happen after showing or navigating the dialog.
        /// </remarks>
        public event EventHandler Created;

        /// <summary>
        /// Occurs when the task dialog is about to destroy the GUI elements represented
        /// by this <see cref="TaskDialogPage"/> instance and it is about to be
        /// unbound from the task dialog.
        /// </summary>
        /// <remarks>
        /// This will happen when closing or navigating the dialog.
        /// </remarks>
        public event EventHandler Destroyed;

        /// <summary>
        /// Occurs when the user presses F1 while the task dialog has focus, or when the
        /// user clicks the <see cref="TaskDialogResult.Help"/> button.
        /// </summary>
        public event EventHandler HelpRequest;

        /// <summary>
        /// Occurs when the user has clicked on a hyperlink.
        /// </summary>
        /// <remarks>
        /// This event will only be raised if <see cref="EnableHyperlinks"/> is set to <c>true</c>.
        /// </remarks>
        public event EventHandler<TaskDialogHyperlinkClickedEventArgs> HyperlinkClicked;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogPage"/> class.
        /// </summary>
        public TaskDialogPage()
        {
            // Create empty (hidden) controls.
            _checkBox = new TaskDialogCheckBox();
            _expander = new TaskDialogExpander();
            _footer = new TaskDialogFooter();
            _progressBar = new TaskDialogProgressBar(TaskDialogProgressBarState.None);
        }

        /// <summary>
        /// Gets or sets the collection of <see cref="TaskDialogStandardButton"/> instances
        /// to be shown in this page.
        /// </summary>
        /// <value>
        /// A <see cref="TaskDialogStandardButtonCollection"/> instance representing the
        /// collection of standard buttons to be shown in this page.
        /// </value>
        public TaskDialogStandardButtonCollection StandardButtons
        {
            get => _standardButtons ??= new TaskDialogStandardButtonCollection();

            set
            {
                // We must deny this if we are bound because we need to be able to
                // access the controls from the task dialog's callback.
                DenyIfBound();

                _standardButtons = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Gets or sets the collection of <see cref="TaskDialogCustomButton"/> instances
        /// to be shown in this page.
        /// </summary>
        /// <value>
        /// A <see cref="TaskDialogCustomButtonCollection"/> instance representing the
        /// collection of custom buttons to be shown in this page.
        /// </value>
        public TaskDialogCustomButtonCollection CustomButtons
        {
            get => _customButtons ??= new TaskDialogCustomButtonCollection();

            set
            {
                // We must deny this if we are bound because we need to be able to
                // access the controls from the task dialog's callback.
                DenyIfBound();

                _customButtons = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Gets or sets the collection of <see cref="TaskDialogRadioButton"/> instances
        /// to be shown in this page.
        /// </summary>
        /// <value>
        /// A <see cref="TaskDialogRadioButtonCollection"/> instance representing the
        /// collection of radio buttons to be shown in this page.
        /// </value>
        public TaskDialogRadioButtonCollection RadioButtons
        {
            get => _radioButtons ??= new TaskDialogRadioButtonCollection();

            set
            {
                // We must deny this if we are bound because we need to be able to
                // access the controls from the task dialog's callback.
                DenyIfBound();

                _radioButtons = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TaskDialogCheckBox"/> to be shown in this page.
        /// </summary>
        /// <remarks>
        /// The checkbox will only be shown if its <see cref="TaskDialogCheckBox.Text"/> property
        /// is not <c>null</c> or an empty string.
        /// </remarks>
        public TaskDialogCheckBox CheckBox
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
        /// Gets or sets the <see cref="TaskDialogExpander"/> to be shown in this page.
        /// </summary>
        /// <remarks>
        /// The expander button (and the expanded area) will only be shown if its
        /// <see cref="TaskDialogExpander.Text"/> property is not <c>null</c> or an empty string.
        /// </remarks>
        public TaskDialogExpander Expander
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
        /// Gets or sets the <see cref="TaskDialogFooter"/> to be shown in this page.
        /// </summary>
        /// <remarks>
        /// The footer will only be shown if its <see cref="TaskDialogFooter.Text"/> property
        /// is not <c>null</c> or an empty string.
        /// </remarks>
        public TaskDialogFooter Footer
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
        /// Gets or sets the <see cref="TaskDialogProgressBar"/> to be shown in this page.
        /// </summary>
        /// <remarks>
        /// The progress bar will only be shown if its <see cref="TaskDialogProgressBar.State"/>
        /// property is not <see cref="TaskDialogProgressBarState.None"/>.
        /// </remarks>
        public TaskDialogProgressBar ProgressBar
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
        /// Gets or sets the title of the task dialog window.
        /// </summary>
        /// <remarks>
        /// This property can be set while the dialog is shown.
        /// </remarks>
        public string Title
        {
            get => _title;

            set
            {
                DenyIfWaitingForInitialization();

                // Note: We set the field values after calling the method to ensure
                // it still has the previous value it the method throws.
                _boundTaskDialog?.UpdateTitle(value);

                _title = value;
            }
        }

        /// <summary>
        /// Gets or sets the main instruction text.
        /// </summary>
        /// <remarks>
        /// This property can be set while the dialog is shown.
        /// </remarks>
        public string Instruction
        {
            get => _instruction;

            set
            {
                DenyIfWaitingForInitialization();

                _boundTaskDialog?.UpdateTextElement(TaskDialogTextElement.TDE_MAIN_INSTRUCTION, value);

                _instruction = value;
            }
        }

        /// <summary>
        /// Gets or sets the dialog's primary text content.
        /// </summary>
        /// <remarks>
        /// This property can be set while the dialog is shown.
        /// </remarks>
        public string Text
        {
            get => _text;

            set
            {
                DenyIfWaitingForInitialization();

                _boundTaskDialog?.UpdateTextElement(TaskDialogTextElement.TDE_CONTENT, value);

                _text = value;
            }
        }

        /// <summary>
        /// Gets or sets the main icon.
        /// </summary>
        /// <remarks>
        /// This property can be set while the dialog is shown (but in that case, it
        /// cannot be switched between instances of <see cref="TaskDialogIconHandle"/>
        /// and instances of other icon types).
        /// </remarks>
        public TaskDialogIcon Icon
        {
            get => _icon;

            set
            {
                DenyIfWaitingForInitialization();

                (IntPtr iconValue, bool? iconIsFromHandle) = GetIconValue(value);

                // The native task dialog icon cannot be updated from a handle
                // type to a non-handle type and vice versa, so we need to throw
                // throw in such a case.
                if (_boundTaskDialog != null && iconIsFromHandle != null &&
                    iconIsFromHandle != _boundIconIsFromHandle)
                {
                    throw new InvalidOperationException(
                        "Cannot update the icon from a handle icon type to a " +
                        "non-handle icon type, and vice versa.");
                }

                _boundTaskDialog?.UpdateIconElement(TaskDialogIconElement.TDIE_ICON_MAIN, iconValue);

                _icon = value;
            }
        }

        /// <summary>
        /// Gets or sets the width in dialog units that the dialog's client area will get
        /// when the dialog is is created or navigated.
        /// If <c>0</c>, the width will be automatically calculated by the system.
        /// </summary>
        /// <value>
        /// The width in dialog units that the dialog's client area will get. The default is
        /// <c>0</c> which means the width is calculated by the system.
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
        /// Gets or sets the <see cref="TaskDialogCustomButtonStyle"/> that specifies how to
        /// display custom buttons.
        /// </summary>
        /// <value>
        /// The <see cref="TaskDialogCustomButtonStyle"/> that specifies how to display custom
        /// buttons. The default value is <see cref="TaskDialogCustomButtonStyle.Default"/>.
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
        /// Gets or sets a value that specifies whether the task dialog should
        /// interpret strings in the form <c>&lt;a href="link"&gt;Hyperlink Text&lt;/a&gt;</c>
        /// as hyperlink when specified in the <see cref="Text"/>,
        /// <see cref="TaskDialogFooter.Text"/>
        /// or <see cref="TaskDialogExpander.Text"/> properties.
        /// When the user clicks on such a link, the <see cref="HyperlinkClicked"/>
        /// event is raised, containing the value of the <c>href</c> attribute.
        /// 
        /// Warning: Enabling hyperlinks when using content from an unsafe source
        /// may cause security vulnerabilities.
        /// </summary>
        /// <value>
        /// <c>true</c> to enable hyperlinks; otherwise, <c>false</c>. The default value
        /// is <c>false</c>.
        /// </value>
        /// <remarks>
        /// Note: The Task Dialog will not actually execute any hyperlinks.
        /// Hyperlink execution must be handled in the <see cref="HyperlinkClicked"/>
        /// event.
        /// 
        /// Note: Enabling this setting causes the <c>"&amp;"</c> character to be
        /// interpreted as prefix for an access key character (mnemonic) if at least
        /// one link is used.
        /// 
        /// Note: When you enable this setting and you want to display a text
        /// without interpreting links, you must replace the strings <c>"&lt;a"</c>
        /// and <c>"&lt;A"</c> with something like <c>"&lt;\u200Ba"</c>.
        /// </remarks>
        public bool EnableHyperlinks
        {
            get => GetFlag(TaskDialogFlags.TDF_ENABLE_HYPERLINKS);
            set => SetFlag(TaskDialogFlags.TDF_ENABLE_HYPERLINKS, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the task dialog can be closed with
        /// a <see cref="TaskDialogResult.Cancel"/> result by pressing ESC or Alt+F4 or by clicking
        /// the title bar's close button even if no <see cref="TaskDialogButtons.Cancel"/> button
        /// is added to the <see cref="StandardButtons"/> collection.
        /// </summary>
        /// <value>
        /// <c>true</c> to allow to close the dialog by pressing ESC or Alt+F4 or by clicking
        /// the title bar's close button; otherwise, <c>false</c>. The default value is <c>false</c>.
        /// </value>
        /// <remarks>
        /// You can intercept cancellation of the dialog without displaying a "Cancel"
        /// button by adding a <see cref="TaskDialogStandardButton"/> with its
        /// <see cref="TaskDialogStandardButton.Visible"/> set to <c>false</c> and specifying
        /// a <see cref="TaskDialogResult.Cancel"/> result.
        /// </remarks>
        public bool AllowCancel
        {
            get => GetFlag(TaskDialogFlags.TDF_ALLOW_DIALOG_CANCELLATION);
            set => SetFlag(TaskDialogFlags.TDF_ALLOW_DIALOG_CANCELLATION, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether text and controls are displayed
        /// reading right to left. 
        /// </summary>
        /// <value>
        /// <c>true</c> to display text and controls reading right to left; <c>false</c>
        /// to display controls reading left to right. The default value is <c>false</c>.
        /// </value>
        /// <remarks>
        /// Note that once a task dialog has been opened with or has navigated to a
        /// <see cref="TaskDialogPage"/> where this flag is set, it will keep on
        /// subsequent navigations to a new <see cref="TaskDialogPage"/> even when
        /// it doesn't have this flag set.
        /// </remarks>
        public bool RightToLeftLayout
        {
            get => GetFlag(TaskDialogFlags.TDF_RTL_LAYOUT);
            set => SetFlag(TaskDialogFlags.TDF_RTL_LAYOUT, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the task dialog can be minimized
        /// when it is shown modeless.
        /// </summary>
        /// <value>
        /// <c>true</c> to specify that the task dialog can be minimized; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        /// <remarks>
        /// When setting this property to <c>true</c>, <see cref="AllowCancel"/> is
        /// automatically implied.
        /// </remarks>
        public bool CanBeMinimized
        {
            get => GetFlag(TaskDialogFlags.TDF_CAN_BE_MINIMIZED);
            set => SetFlag(TaskDialogFlags.TDF_CAN_BE_MINIMIZED, value);
        }

        /// <summary>
        /// Indicates that the width of the task dialog is determined by the width
        /// of its content area (similar to Message Box sizing behavior).
        /// </summary>
        /// <value>
        /// <c>true</c> to determine the width of the task dialog by the width of
        /// its content area; otherwise, <c>false</c>. The default value is <c>false</c>.
        /// </value>
        /// <remarks>
        /// This flag is ignored if <see cref="Width"/> is not set to <c>0</c>.
        /// </remarks>
        public bool SizeToContent
        {
            get => GetFlag(TaskDialogFlags.TDF_SIZE_TO_CONTENT);
            set => SetFlag(TaskDialogFlags.TDF_SIZE_TO_CONTENT, value);
        }

        internal TaskDialog BoundTaskDialog
        {
            get => _boundTaskDialog;
        }

        /// <summary>
        /// Gets a value that indicates if the <see cref="_boundTaskDialog"/>
        /// started navigation to this page but navigation did not yet complete
        /// (in which case we cannot modify the dialog even though we are bound).
        /// </summary>
        internal bool WaitingForInitialization
        {
            get => _boundTaskDialog != null && !_appliedInitialization;
        }

        internal static bool IsNativeStringNullOrEmpty(string str)
        {
            // From a native point of view, the string is empty if its first
            // character is a NUL char.
            return string.IsNullOrEmpty(str) || str[0] == '\0';
        }

        internal static (IntPtr iconValue, bool? iconIsFromHandle) GetIconValue(TaskDialogIcon icon)
        {
            IntPtr iconValue = default;
            bool? iconIsFromHandle = null;

            // If no icon is specified (icon is null), we don't set the
            // "iconIsFromHandle" flag, which means that the icon can be updated
            // to show a Standard icon while the dialog is running.
            if (icon is TaskDialogIconHandle iconHandle)
            {
                iconIsFromHandle = true;
                iconValue = iconHandle.IconHandle;
            }
            else if (icon is TaskDialogStandardIconContainer standardIconContainer)
            {
                iconIsFromHandle = false;
                iconValue = (IntPtr)standardIconContainer.Icon;
            }

            return (iconValue, iconIsFromHandle);
        }

        internal void DenyIfBound()
        {
            if (_boundTaskDialog != null)
            {
                throw new InvalidOperationException(
                    "Cannot set this property or call this method while the " +
                    "page is bound to a task dialog.");
            }
        }

        internal void DenyIfWaitingForInitialization()
        {
            if (WaitingForInitialization)
            {
                throw new InvalidOperationException(
                    $"Navigation of the task dialog did not complete yet. " +
                    $"Please wait for the " +
                    $"{nameof(TaskDialogPage)}.{nameof(Created)} event to occur.");
            }
        }

        internal TaskDialogButton GetBoundButtonByID(int buttonID)
        {
            if (_boundTaskDialog == null)
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

        internal TaskDialogRadioButton GetBoundRadioButtonByID(int buttonID)
        {
            if (_boundTaskDialog == null)
            {
                throw new InvalidOperationException();
            }

            return buttonID == 0 ? null : _radioButtons[buttonID - RadioButtonStartID];
        }

        internal void Validate()
        {
            // Check that this page instance is not already bound to a TaskDialog instance.
            if (_boundTaskDialog != null)
            {
                throw new InvalidOperationException(
                    $"This {nameof(TaskDialogPage)} instance is already bound to " +
                    $"a {nameof(TaskDialog)} instance.");
            }

            // We also need to check the controls (and collections) since they could also be
            // bound to a TaskDialogPage at the same time.
            // Access the collections using the property to ensure they exist.
            if (StandardButtons.BoundPage != null ||
                CustomButtons.BoundPage != null ||
                RadioButtons.BoundPage != null)
            {
                throw new InvalidOperationException(
                    $"One of the collections of this {nameof(TaskDialogPage)} is already bound to a " +
                    $"{nameof(TaskDialog)} instance.");
            }

            if (StandardButtons.Concat<TaskDialogControl>(CustomButtons).Concat(RadioButtons)
                .Append(_checkBox)
                .Append(_expander)
                .Append(_footer)
                .Append(_progressBar)
                .Any(c => c.BoundPage != null))
            {
                throw new InvalidOperationException(
                    $"One of the controls of this {nameof(TaskDialogPage)} is already bound to a " +
                    $"{nameof(TaskDialog)} instance.");
            }

            if (CustomButtons.Count > int.MaxValue - CustomButtonStartID + 1 ||
                RadioButtons.Count > int.MaxValue - RadioButtonStartID + 1)
            {
                throw new InvalidOperationException(
                    "Too many custom buttons or radio buttons have been added.");
            }

            if (StandardButtons.Concat<TaskDialogButton>(CustomButtons).Where(e => e.DefaultButton).Count() > 1)
            {
                throw new InvalidOperationException("Only a single button can be set as default button.");
            }

            if (_radioButtons.Where(e => e.Checked).Count() > 1)
            {
                throw new InvalidOperationException("Only a single radio button can be set as checked.");
            }

            // For custom and radio buttons, we need to ensure the strings are not null or empty,
            // as otherwise an error would occur when showing/navigating the dialog.
            if (_customButtons.Any(e => !e.IsCreatable))
            {
                throw new InvalidOperationException("The text of a custom button must not be null or empty.");
            }

            if (_radioButtons.Any(e => !e.IsCreatable))
            {
                throw new InvalidOperationException("The text of a radio button must not be null or empty.");
            }
        }

        internal void Bind(
            TaskDialog owner,
            out TaskDialogFlags flags,
            out TaskDialogButtons buttonFlags,
            out IntPtr iconValue,
            out IntPtr footerIconValue,
            out int defaultButtonID,
            out int defaultRadioButtonID)
        {
            if (_boundTaskDialog != null)
            {
                throw new InvalidOperationException();
            }

            // This method assumes Validate() has already been called.

            _boundTaskDialog = owner;
            flags = _flags;

            (IntPtr localIconValue, bool? iconIsFromHandle) = GetIconValue(_icon);
            (iconValue, _boundIconIsFromHandle) = (localIconValue, iconIsFromHandle ?? false);

            if (_boundIconIsFromHandle)
            {
                flags |= TaskDialogFlags.TDF_USE_HICON_MAIN;
            }

            // Only specify the command link flags if there actually are custom buttons;
            // otherwise the dialog will not work.
            if (_customButtons.Count > 0)
            {
                if (_customButtonStyle == TaskDialogCustomButtonStyle.CommandLinks)
                {
                    flags |= TaskDialogFlags.TDF_USE_COMMAND_LINKS;
                }
                else if (_customButtonStyle == TaskDialogCustomButtonStyle.CommandLinksNoIcon)
                {
                    flags |= TaskDialogFlags.TDF_USE_COMMAND_LINKS_NO_ICON;
                }
            }

            TaskDialogStandardButtonCollection standardButtons = StandardButtons;
            TaskDialogCustomButtonCollection customButtons = CustomButtons;
            TaskDialogRadioButtonCollection radioButtons = RadioButtons;

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
                flags |= TaskDialogFlags.TDF_NO_DEFAULT_RADIO_BUTTON;
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
            if (_boundTaskDialog == null)
            {
                throw new InvalidOperationException();
            }

            TaskDialogStandardButtonCollection standardButtons = StandardButtons;
            TaskDialogCustomButtonCollection customButtons = CustomButtons;
            TaskDialogRadioButtonCollection radioButtons = RadioButtons;

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

            _boundTaskDialog = null;
            _appliedInitialization = false;
        }

        internal void ApplyInitialization()
        {
            if (_appliedInitialization)
            {
                throw new InvalidOperationException();
            }

            _appliedInitialization = true;

            TaskDialogStandardButtonCollection standardButtons = StandardButtons;
            TaskDialogCustomButtonCollection customButtons = CustomButtons;
            TaskDialogRadioButtonCollection radioButtons = RadioButtons;

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
        /// Raises the <see cref="Created"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        internal protected void OnCreated(EventArgs e)
        {
            Created?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="Destroyed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        internal protected void OnDestroyed(EventArgs e)
        {
            Destroyed?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="HelpRequest"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        internal protected void OnHelpRequest(EventArgs e)
        {
            HelpRequest?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="HyperlinkClicked"/> event.
        /// </summary>
        /// <param name="e">An <see cref="TaskDialogHyperlinkClickedEventArgs"/> that contains the event data.</param>
        internal protected void OnHyperlinkClicked(TaskDialogHyperlinkClickedEventArgs e)
        {
            HyperlinkClicked?.Invoke(this, e);
        }

        private bool GetFlag(TaskDialogFlags flag)
        {
            return (_flags & flag) == flag;
        }

        private void SetFlag(TaskDialogFlags flag, bool value)
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
