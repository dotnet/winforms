// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.ObjectModel;
using System.Drawing;
#if NETCORE
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
#endif

namespace WinFormsControlsTest.UserControls;

[DesignerCategory("Default")]
internal partial class CollectionEditors : Form
{
    private readonly string[] _stringArray = ["Lorem ipsum dolor sit amet", "id quo accusamus definitionem", "graeco salutandi sed te", "mei in solum primis definitionem."];
    private readonly List<string> _stringList = ["Lorem ipsum dolor sit amet", "id quo accusamus definitionem", "graeco salutandi sed te", "mei in solum primis definitionem."];
    private readonly Collection<string> _stringCollection = ["Lorem ipsum dolor sit amet", "id quo accusamus definitionem", "graeco salutandi sed te", "mei in solum primis definitionem."];

    public CollectionEditors()
    {
        InitializeComponent();

        ImageList imageList = new();
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

    private void control_Enter(object sender, EventArgs e)
    {
        label1.Text = sender.GetType().FullName;
        propertyGrid1.SelectedObject = sender;
    }
}
