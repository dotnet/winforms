// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents an icon that can be shown in the main area of a task dialog
    ///   (by setting the <see cref="TaskDialogPage.Icon"/> property) or in the
    ///   footer of a task dialog (by setting the <see cref="TaskDialogFooter.Icon"/>
    ///   property).
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The task dialog currently supports two icon types: Standard icons that are
    ///   stored in static fields of class <see cref="TaskDialogIcon"/>, and custom icons
    ///   created from an <see cref="Icon"/> instance (or an icon handle).
    /// </para>
    /// <para>
    ///   Some standard icons play a typical system sound when used as the main icon of
    ///   the task dialog.
    /// </para>
    /// <para>
    ///   Note that while a task dialog is shown, you can only update an icon if the
    ///   new icon is of the same type (standard icon or custom icon) as the previous
    ///   one.
    /// </para>
    /// </remarks>
    public class TaskDialogIcon : IDisposable
    {
#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        ///   Gets a standard <see cref="TaskDialogIcon"/> instance where the task dialog
        ///   does not display an icon.
        /// </summary>
        public static readonly TaskDialogIcon None = new TaskDialogIcon(TaskDialogStandardIcon.None);

        /// <summary>
        ///   Gets a standard <see cref="TaskDialogIcon"/> instance where the task dialog
        ///   contains a symbol consisting of a lowercase letter i in a circle.
        /// </summary>
        public static readonly TaskDialogIcon Information = new TaskDialogIcon(TaskDialogStandardIcon.Information);

        /// <summary>
        ///   Gets a standard <see cref="TaskDialogIcon"/> instance where the task dialog
        ///   contains an icon consisting of an exclamation point in a triangle with a yellow background.
        /// </summary>
        public static readonly TaskDialogIcon Warning = new TaskDialogIcon(TaskDialogStandardIcon.Warning);

        /// <summary>
        ///   Gets a standard <see cref="TaskDialogIcon"/> instance where the task dialog
        ///   contains an icon consisting of white X in a circle with a red background.
        /// </summary>
        public static readonly TaskDialogIcon Error = new TaskDialogIcon(TaskDialogStandardIcon.Error);

        /// <summary>
        ///   Gets a standard <see cref="TaskDialogIcon"/> instance where the task dialog
        ///   contains an icon consisting of an user account control (UAC) shield.
        /// </summary>
        public static readonly TaskDialogIcon Shield = new TaskDialogIcon(TaskDialogStandardIcon.Shield);

        /// <summary>
        ///   Gets a standard <see cref="TaskDialogIcon"/> instance where the task dialog
        ///   contains an icon consisting of an user account control (UAC) shield and shows a blue bar around the icon.
        /// </summary>
        public static readonly TaskDialogIcon ShieldBlueBar = new TaskDialogIcon(TaskDialogStandardIcon.ShieldBlueBar);

        /// <summary>
        ///   Gets a standard <see cref="TaskDialogIcon"/> instance where the task dialog
        ///   contains an icon consisting of an user account control (UAC) shield and shows a gray bar around the icon.
        /// </summary>
        public static readonly TaskDialogIcon ShieldGrayBar = new TaskDialogIcon(TaskDialogStandardIcon.ShieldGrayBar);

        /// <summary>
        ///   Gets a standard <see cref="TaskDialogIcon"/> instance where the task dialog
        ///   contains an icon consisting of an exclamation point in a yellow shield and shows a yellow bar around the icon.
        /// </summary>
        public static readonly TaskDialogIcon ShieldWarningYellowBar = new TaskDialogIcon(TaskDialogStandardIcon.ShieldWarningYellowBar);

        /// <summary>
        ///   Gets a standard <see cref="TaskDialogIcon"/> instance where the task dialog
        ///   contains an icon consisting of white X in a red shield and shows a red bar around the icon.
        /// </summary>
        public static readonly TaskDialogIcon ShieldErrorRedBar = new TaskDialogIcon(TaskDialogStandardIcon.ShieldErrorRedBar);

        /// <summary>
        ///   Gets a standard <see cref="TaskDialogIcon"/> instance where the task dialog
        ///   contains an icon consisting of white tick in a green shield and shows a green bar around the icon.
        /// </summary>
        public static readonly TaskDialogIcon ShieldSuccessGreenBar = new TaskDialogIcon(TaskDialogStandardIcon.ShieldSuccessGreenBar);
#pragma warning restore IDE1006 // Naming Styles

        private readonly TaskDialogStandardIcon? _standardIcon;
        private readonly Icon? _ownedIcon;
        private readonly IntPtr? _iconHandle;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogIcon"/> class from an
        ///   <see cref="Bitmap"/> instance.
        /// </summary>
        /// <param name="image">The <see cref="Bitmap"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> is <see langword="null"/>.</exception>
        /// <remarks>
        ///   <para>
        ///     The <see cref="Icon"/> instance from which this <see cref="TaskDialogIcon"/>
        ///     instance is created must not be disposed while the icon is shown in the task
        ///     dialog.
        ///   </para>
        /// </remarks>
        public TaskDialogIcon(Bitmap image)
            : this(BitmapToIcon(image ?? throw new ArgumentNullException(nameof(image))), true)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogIcon"/> class from an
        ///   <see cref="Icon"/> instance.
        /// </summary>
        /// <param name="icon">The <see cref="Icon"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="icon"/> is <see langword="null"/>.</exception>
        /// <remarks>
        ///   <para>
        ///     The <see cref="Icon"/> instance from which this <see cref="TaskDialogIcon"/>
        ///     instance is created must not be disposed while the icon is shown in the task
        ///     dialog.
        ///   </para>
        /// </remarks>
        public TaskDialogIcon(Icon icon)
            : this((icon ?? throw new ArgumentNullException(nameof(icon))), false)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogIcon"/> class from an
        ///   icon handle.
        /// </summary>
        /// <param name="iconHandle">A handle to an instance of an icon.</param>
        ///   <remarks>
        ///   <para>
        ///     The specified icon handle must not be released while the icon is shown in the
        ///     task dialog.
        ///   </para>
        /// </remarks>
        public TaskDialogIcon(IntPtr iconHandle)
        {
            _iconHandle = iconHandle;
        }

        private TaskDialogIcon(Icon icon, bool takeOwnership)
            : this(icon.Handle)
        {
            if (takeOwnership)
            {
                _ownedIcon = icon;
            }
        }

        private TaskDialogIcon(TaskDialogStandardIcon standardIcon)
        {
            _standardIcon = standardIcon;
        }

        /// <summary>
        ///   Releases all resources used by this <see cref="TaskDialogIcon"/>.
        /// </summary>
        public void Dispose()
        {
            _ownedIcon?.Dispose();
        }

        /// <summary>
        ///   The icon handle (<c>HICON</c>) that is represented by this
        ///   <see cref="TaskDialogIcon"/> instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///   This <see cref="TaskDialogIcon"/> instance was not created using a
        ///   constructor that takes an icon or icon handle.
        /// </exception>
        public IntPtr IconHandle => _iconHandle ?? throw new InvalidOperationException();

        internal TaskDialogStandardIcon StandardIcon => _standardIcon ?? throw new InvalidOperationException();

        internal bool IsStandardIcon => _standardIcon != null;

        internal bool IsHandleIcon => _iconHandle != null;

        private static Icon BitmapToIcon(Bitmap bitmap)
        {
            IntPtr handle = IntPtr.Zero;
            try
            {
                handle = bitmap.GetHicon();
                var icon = Icon.FromHandle(handle);

                return (Icon)icon.Clone();
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    Interop.User32.DestroyIcon(handle);
                }
            }
        }
    }
}
