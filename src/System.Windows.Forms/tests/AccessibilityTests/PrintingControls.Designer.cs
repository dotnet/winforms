namespace Accessibility_Core_App;

partial class PrintingControls
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
        System.ComponentModel.ComponentResourceManager resources = new(typeof(PrintingControls));
        this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
        this.printDocument1 = new System.Drawing.Printing.PrintDocument();
        this.printDialog1 = new System.Windows.Forms.PrintDialog();
        this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
        this.label1 = new System.Windows.Forms.Label();
        this.txtPrint = new System.Windows.Forms.TextBox();
        this.btnSetting = new System.Windows.Forms.Button();
        this.btnPreView = new System.Windows.Forms.Button();
        this.btnPrint = new System.Windows.Forms.Button();
        this.button1 = new System.Windows.Forms.Button();
        this.label2 = new System.Windows.Forms.Label();
        this.printPreviewControl1 = new System.Windows.Forms.PrintPreviewControl();
        this.SuspendLayout();
        // 
        // pageSetupDialog1
        // 
        this.pageSetupDialog1.Document = this.printDocument1;
        // 
        // printDocument1
        // 
        this.printDocument1.PrintPage += this.printDocument1_PrintPage;
        // 
        // printDialog1
        // 
        this.printDialog1.Document = this.printDocument1;
        this.printDialog1.UseEXDialog = true;
        // 
        // printPreviewDialog1
        // 
        this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
        this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
        this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
        this.printPreviewDialog1.Document = this.printDocument1;
        this.printPreviewDialog1.Enabled = true;
        this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
        this.printPreviewDialog1.Name = "printPreviewDialog1";
        this.printPreviewDialog1.Visible = false;
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(14, 10);
        this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(165, 15);
        this.label1.TabIndex = 0;
        this.label1.Text = "Please &input text you want to print";
        // 
        // txtPrint
        // 
        this.txtPrint.Location = new System.Drawing.Point(191, 7);
        this.txtPrint.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.txtPrint.Name = "txtPrint";
        this.txtPrint.Size = new System.Drawing.Size(213, 23);
        this.txtPrint.TabIndex = 1;
        this.txtPrint.Text = "This is the test message.";
        // 
        // btnSetting
        // 
        this.btnSetting.Location = new System.Drawing.Point(18, 43);
        this.btnSetting.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.btnSetting.Name = "btnSetting";
        this.btnSetting.Size = new System.Drawing.Size(88, 27);
        this.btnSetting.TabIndex = 2;
        this.btnSetting.Text = "&Setting";
        this.btnSetting.UseVisualStyleBackColor = true;
        this.btnSetting.Click += this.BtnSetting_Click;
        // 
        // btnPreView
        // 
        this.btnPreView.Location = new System.Drawing.Point(112, 43);
        this.btnPreView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.btnPreView.Name = "btnPreView";
        this.btnPreView.Size = new System.Drawing.Size(88, 27);
        this.btnPreView.TabIndex = 3;
        this.btnPreView.Text = "P&review";
        this.btnPreView.UseVisualStyleBackColor = true;
        this.btnPreView.Click += this.BtnPreView_Click;
        // 
        // btnPrint
        // 
        this.btnPrint.Location = new System.Drawing.Point(206, 43);
        this.btnPrint.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.btnPrint.Name = "btnPrint";
        this.btnPrint.Size = new System.Drawing.Size(88, 27);
        this.btnPrint.TabIndex = 4;
        this.btnPrint.Text = "&Print";
        this.btnPrint.UseVisualStyleBackColor = true;
        this.btnPrint.Click += this.BtnPrint_Click;
        // 
        // button1
        // 
        this.button1.Location = new System.Drawing.Point(301, 43);
        this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(104, 27);
        this.button1.TabIndex = 6;
        this.button1.Text = "Preview &Control";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.Click += this.Button1_Click;
        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(18, 86);
        this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(91, 15);
        this.label2.TabIndex = 7;
        this.label2.Text = "View your page:";
        // 
        // printPreviewControl1
        //
        this.printPreviewControl1.AccessibleName = "Image layout ready for printing";
        this.printPreviewControl1.AutoZoom = false;
        this.printPreviewControl1.Document = this.printDocument1;
        this.printPreviewControl1.Location = new System.Drawing.Point(18, 104);
        this.printPreviewControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.printPreviewControl1.Name = "printPreviewControl1";
        this.printPreviewControl1.Rows = 24;
        this.printPreviewControl1.Size = new System.Drawing.Size(387, 408);
        this.printPreviewControl1.TabIndex = 8;
        this.printPreviewControl1.TabStop = true;
        this.printPreviewControl1.Zoom = 0.36454545454545456D;
        // 
        // PrintingControls
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(426, 522);
        this.Controls.Add(this.printPreviewControl1);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.button1);
        this.Controls.Add(this.btnPrint);
        this.Controls.Add(this.btnPreView);
        this.Controls.Add(this.btnSetting);
        this.Controls.Add(this.txtPrint);
        this.Controls.Add(this.label1);
        this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.Name = "PrintingControls";
        this.Text = "PrintingTesting";
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
    private System.Windows.Forms.PrintDialog printDialog1;
    private System.Drawing.Printing.PrintDocument printDocument1;
    private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtPrint;
    private System.Windows.Forms.Button btnSetting;
    private System.Windows.Forms.Button btnPreView;
    private System.Windows.Forms.Button btnPrint;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.PrintPreviewControl printPreviewControl1;
}
