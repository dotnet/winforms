// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class PictureBoxes : Form
{
    private const string Heading = "PictureBox tests";
    private bool _isLoading;

    public PictureBoxes()
    {
        InitializeComponent();
        Text = Heading;
    }

    private void btnloadImage_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(imageUri.Text))
        {
            pictureBox1.Image = null;
            if (_isLoading)
            {
                pictureBox1.CancelAsync();
            }

            return;
        }

        try
        {
            _isLoading = true;
            pictureBox1.WaitOnLoad = false;
            pictureBox1.LoadAsync(imageUri.Text);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void pictureBox1_LoadCompleted(object sender, AsyncCompletedEventArgs e)
    {
        _isLoading = false;
        Text = Heading;

        if (e.Cancelled)
        {
            MessageBox.Show("Image loading cancelled");
        }

        if (e.Error is not null)
        {
            MessageBox.Show(e.Error.Message, $"{e.Error.GetType().FullName} occurred");
        }
    }

    private void pictureBox1_LoadProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        Text = $"{Heading}: loading image progress: {e.ProgressPercentage}%";
    }
}
