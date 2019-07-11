// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
#if NETCORE
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
#endif

namespace WinformsControlsTest.UserControls
{
    internal class UserControlWithCollectionEditors : UserControl
    {
        private const string Category = "!CollectionEditors";
        private string[] _stringArray = new[] { "Lorem ipsum dolor sit amet", "id quo accusamus definitionem", "graeco salutandi sed te", "mei in solum primis definitionem." };
        private List<string> _stringList = new List<string> { "Lorem ipsum dolor sit amet", "id quo accusamus definitionem", "graeco salutandi sed te", "mei in solum primis definitionem." };
        private Collection<string> _stringCollection = new Collection<string> { "Lorem ipsum dolor sit amet", "id quo accusamus definitionem", "graeco salutandi sed te", "mei in solum primis definitionem." };

        public UserControlWithCollectionEditors()
        {
            AutoScaleMode = AutoScaleMode.Font;

            ListView = new ListView
            {
                LargeImageList = Images,
                SmallImageList = new ImageList(),
            };

            Images.Images.Add("SmallA", Bitmap.FromFile("Images\\SmallA.bmp"));
            Images.Images.Add(Bitmap.FromFile("Images\\SmallABlue.bmp"));
            Images.Images.Add("LargeA", Bitmap.FromFile("Images\\LargeA.bmp"));
            Images.Images.Add(Bitmap.FromFile("Images\\LargeABlue.bmp"));
        }

#if NETCORE
        [Editor(typeof(ArrayEditor), typeof(UITypeEditor))]
#endif
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public string[] StringArray
        {
            get => _stringArray;
            set => _stringArray = value;
        }

#if NETCORE
        [Editor(typeof(StringCollectionEditor), typeof(UITypeEditor))]
#endif
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public List<string> StringList
        {
            get => _stringList;
            set => _stringList = value;
        }

#if NETCORE
        [Editor(typeof(StringCollectionEditor), typeof(UITypeEditor))]
#endif
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public Collection<string> StringCollection
        {
            get => _stringCollection;
            set => _stringCollection = value;
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public ImageList Images { get; set; } = new ImageList();

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public ListView ListView { get; set; }
    }
}
