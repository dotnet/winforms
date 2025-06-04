// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

public class ComboBoxTests : ControlTestBase
{
    public ComboBoxTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task ComboBoxTest_ChangeAutoCompleteSource_DoesNotThrowAsync()
    {
        await RunSingleControlTestAsync<ComboBox>((form, comboBox) =>
        {
            // Test case captured from here.
            // https://github.com/dotnet/winforms/issues/6953
            comboBox.AutoCompleteCustomSource.AddRange(
            [
                "_sss",
                "_sss"
            ]);
            comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            comboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            comboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            comboBox.AutoCompleteMode = AutoCompleteMode.Suggest;

            return Task.CompletedTask;
        });
    }
}
