using System.Windows.Forms;
namespace Accessibility_Core_App;

partial class Main
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.button1 = new System.Windows.Forms.Button();
        this.button2 = new System.Windows.Forms.Button();
        this.button3 = new System.Windows.Forms.Button();
        this.button4 = new System.Windows.Forms.Button();
        this.button5 = new System.Windows.Forms.Button();
        this.button6 = new System.Windows.Forms.Button();
        this.button7 = new System.Windows.Forms.Button();
        this.button8 = new System.Windows.Forms.Button();
        this.button9 = new System.Windows.Forms.Button();
        this.button10 = new System.Windows.Forms.Button();
        this.button12 = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // button1
        // 
        this.button1.Location = new System.Drawing.Point(14, 14);
        this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(531, 27);
        this.button1.TabIndex = 0;
        this.button1.Text = "Common_Controls1: Testing the controls under the Common_Controls Tab";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.Click += this.Button1_Click;
        // 
        // button2
        // 
        this.button2.Location = new System.Drawing.Point(14, 60);
        this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.button2.Name = "button2";
        this.button2.Size = new System.Drawing.Size(531, 27);
        this.button2.TabIndex = 1;
        this.button2.Text = "Common_Controls2: Testing the controls under the Common_Controls Tab";
        this.button2.UseVisualStyleBackColor = true;
        this.button2.Click += this.Button2_Click;
        // 
        // button3
        // 
        this.button3.Location = new System.Drawing.Point(13, 105);
        this.button3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.button3.Name = "button3";
        this.button3.Size = new System.Drawing.Size(531, 27);
        this.button3.TabIndex = 2;
        this.button3.Text = "Data_Controls: Testing the controls under the Data Tab";
        this.button3.UseVisualStyleBackColor = true;
        this.button3.Click += this.Button3_Click;
        // 
        // button4
        // 
        this.button4.Location = new System.Drawing.Point(13, 152);
        this.button4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.button4.Name = "button4";
        this.button4.Size = new System.Drawing.Size(531, 27);
        this.button4.TabIndex = 3;
        this.button4.Text = "Dialogs_Controls: Testing the controls under the Dialogs Tab";
        this.button4.UseVisualStyleBackColor = true;
        this.button4.Click += this.Button4_Click;
        // 
        // button5
        // 
        this.button5.Location = new System.Drawing.Point(13, 199);
        this.button5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.button5.Name = "button5";
        this.button5.Size = new System.Drawing.Size(531, 27);
        this.button5.TabIndex = 4;
        this.button5.Text = "MenuToolbars_Controls: Testing the controls under the Menu & Toolbars Tab";
        this.button5.UseVisualStyleBackColor = true;
        this.button5.Click += this.Button5_Click;
        // 
        // button6
        // 
        this.button6.Location = new System.Drawing.Point(14, 247);
        this.button6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.button6.Name = "button6";
        this.button6.Size = new System.Drawing.Size(531, 27);
        this.button6.TabIndex = 5;
        this.button6.Text = "Printing_Controls: Testing the controls under the Printing Tab";
        this.button6.UseVisualStyleBackColor = true;
        this.button6.Click += this.Button6_Click;
        // 
        // button7
        // 
        this.button7.Location = new System.Drawing.Point(13, 292);
        this.button7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.button7.Name = "button7";
        this.button7.Size = new System.Drawing.Size(531, 27);
        this.button7.TabIndex = 6;
        this.button7.Text = "Remaining_Controls_Testing";
        this.button7.UseVisualStyleBackColor = true;
        this.button7.Click += this.Button7_Click;
        // 
        // button8
        // 
        this.button8.Location = new System.Drawing.Point(14, 336);
        this.button8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.button8.Name = "button8";
        this.button8.Size = new System.Drawing.Size(531, 27);
        this.button8.TabIndex = 7;
        this.button8.Text = "Containers_Controls: Testing the controls under the Containers Tab";
        this.button8.UseVisualStyleBackColor = true;
        this.button8.Click += this.Button8_Click;
        // 
        // button9
        // 
        this.button9.Location = new System.Drawing.Point(13, 380);
        this.button9.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.button9.Name = "button9";
        this.button9.Size = new System.Drawing.Size(531, 27);
        this.button9.TabIndex = 8;
        this.button9.Text = "Containers_Controls_2: Testing the controls under the Containers Tab";
        this.button9.UseVisualStyleBackColor = true;
        this.button9.Click += this.button9_Click;
        // 
        // button10
        // 
        this.button10.Location = new System.Drawing.Point(14, 425);
        this.button10.Name = "button10";
        this.button10.Size = new System.Drawing.Size(530, 27);
        this.button10.TabIndex = 9;
        this.button10.Text = "TaskDialog: Testing the task dialog";
        this.button10.UseVisualStyleBackColor = true;
        this.button10.Click += this.button10_Click;
        // 
        // button12
        // 
        this.button12.Location = new System.Drawing.Point(15, 470);
        this.button12.Name = "button12";
        this.button12.Size = new System.Drawing.Size(530, 23);
        this.button12.TabIndex = 11;
        this.button12.Text = "DataBinding Example";
        this.button12.UseVisualStyleBackColor = true;
        this.button12.Click += this.button12_Click;
        // 
        // Main
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(571, 504);
        this.Controls.Add(this.button12);
        this.Controls.Add(this.button10);
        this.Controls.Add(this.button9);
        this.Controls.Add(this.button8);
        this.Controls.Add(this.button7);
        this.Controls.Add(this.button6);
        this.Controls.Add(this.button5);
        this.Controls.Add(this.button4);
        this.Controls.Add(this.button3);
        this.Controls.Add(this.button2);
        this.Controls.Add(this.button1);
        this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.Name = "Main";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "MainForm";
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Button button3;
    private System.Windows.Forms.Button button4;
    private System.Windows.Forms.Button button5;
    private System.Windows.Forms.Button button6;
    private System.Windows.Forms.Button button7;
    private System.Windows.Forms.Button button8;
    private Button button9;
    private Button button10;
    private Button button12;
}

