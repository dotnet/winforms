// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
#if NETCORE
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
#endif

namespace WinformsControlsTest.UserControls
{
    internal partial class CollectionEditors : Form
    {
        private int[] _intArray = Enumerable.Range(1, 10).ToArray();
        private string[] _stringArray = new[] { "Lorem ipsum dolor sit amet", "id quo accusamus definitionem", "graeco salutandi sed te", "mei in solum primis definitionem." };
        private List<string> _stringList = new List<string> { "Lorem ipsum dolor sit amet", "id quo accusamus definitionem", "graeco salutandi sed te", "mei in solum primis definitionem." };
        private Collection<string> _stringCollection = new Collection<string> { "Lorem ipsum dolor sit amet", "id quo accusamus definitionem", "graeco salutandi sed te", "mei in solum primis definitionem." };

        public CollectionEditors()
        {
            InitializeComponent();


            var imageList = new ImageList();
            imageList.Images.Add("SmallA", Image.FromFile("Images\\SmallA.bmp"));
            imageList.Images.Add(Image.FromFile("Images\\SmallABlue.bmp"));
            imageList.Images.Add("LargeA", Image.FromFile("Images\\LargeA.bmp"));
            imageList.Images.Add(Image.FromFile("Images\\LargeABlue.bmp"));

            textBox1.Lines = _stringArray;
            domainUpDown1.Items.AddRange(_stringCollection);
            listView1.LargeImageList = imageList;
            _stringList.ForEach(s => listView1.Items.Add(s));
            _stringList.ForEach(s => comboBox1.Items.Add(s));
        }

        private void control_Enter(object sender, System.EventArgs e)
        {
            label1.Text = sender.GetType().FullName;
            propertyGrid1.SelectedObject = sender;
        }
    }
}
