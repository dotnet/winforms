// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    internal class HelpNamespaceEditor : FileNameEditor
    {
        /// <summary>
        ///  Initializes the open file dialog when it is created.  This gives you
        ///  an opportunity to configure the dialog as you please.  The default
        ///  implementation provides a generic file filter and title.
        /// </summary>
        protected override void InitializeDialog(OpenFileDialog openFileDialog)
        {
            openFileDialog.Filter = SR.HelpProviderEditorFilter;
            openFileDialog.Title = SR.HelpProviderEditorTitle;
        }
    }
}
