// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace WinformsControlsTest.UserControls
{
    internal class UserControlWithFontNameEditor : UserControl
    {
        private const string Category = "!Fonts";

        public UserControlWithFontNameEditor()
        {
            AutoScaleMode = AutoScaleMode.Font;
        }

        [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public string FontNameBold
        {
            get { return "Arial Black"; }
            set { }
        }

        [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public string FontNameBoldItalic
        {
            get { return "Forte"; }
            set { }
        }

        [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public string FontNameEmpty
        {
            get { return ""; }
            set { }
        }

        [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public int FontNameInt
        {
            get { return -1345; }
            set { }
        }

        [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public string FontNameInvalid
        {
            get { return "some invalid font family"; }
            set { }
        }

        [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public string FontNameNull
        {
            get { return null; }
            set { }
        }

        [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public string FontNameItalics
        {
            get { return "Lucida Handwriting"; }
            set { }
        }

        [Editor(typeof(FontNameEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public string FontNameRegular
        {
            get { return "Arial"; }
            set { }
        }
    }
}
