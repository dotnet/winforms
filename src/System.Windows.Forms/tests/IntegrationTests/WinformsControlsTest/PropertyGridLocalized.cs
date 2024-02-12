// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace WinformsControlsTest;

public partial class PropertyGridLocalized : Form
{
    // https://github.com/dotnet/runtime/issues/94365
    public PropertyGridLocalized()
    {
        InitializeComponent();
        propertyGrid1.SelectedObject = new MyObjectClass();
        cultureComboBox.DisplayMember = "EnglishName";
        cultureComboBox.ValueMember = "Name";
        LoadCultures();
        cultureComboBox.SelectedItem = CultureInfo.CurrentUICulture;
        cultureComboBox.SelectedIndexChanged += CultureComboBox_SelectedIndexChanged;
    }

    private void CultureComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cultureComboBox.SelectedItem is CultureInfo selectedCulture && selectedCulture != CultureInfo.CurrentUICulture)
        {
            CultureInfo.CurrentUICulture = selectedCulture;
            CultureInfo.CurrentCulture = selectedCulture;
            propertyGrid1.SelectedObject = new MyObjectClass();
        }
    }

    private void LoadCultures()
    {
        foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
        {
            cultureComboBox.Items.Add(lang.Culture);
        }
    }

    private class MyObjectClass : UserControl
    {
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string MiscValue { get; set; }
    }
}
