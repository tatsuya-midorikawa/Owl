namespace Owl.psrx;

partial class SettingsForm {
  /// <summary>
  /// Required designer variable.
  /// </summary>
  private System.ComponentModel.IContainer components = null;

  /// <summary>
  /// Clean up any resources being used.
  /// </summary>
  /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
  protected override void Dispose(bool disposing) {
    if (disposing && (components != null)) {
      components.Dispose();
    }
    base.Dispose(disposing);
  }

  #region Windows Form Designer generated code

  /// <summary>
  /// Required method for Designer support - do not modify
  /// the contents of this method with the code editor.
  /// </summary>
  private void InitializeComponent() {
    outputFilePathTbox = new TextBox();
    label1 = new Label();
    browseBtn = new Button();
    groupBox1 = new GroupBox();
    storeCaptureCountBox = new NumericUpDown();
    label3 = new Label();
    captureDisabledBtn = new RadioButton();
    captureEnabledBtn = new RadioButton();
    label2 = new Label();
    okBtn = new Button();
    cancelBtn = new Button();
    groupBox1.SuspendLayout();
    ((System.ComponentModel.ISupportInitialize)storeCaptureCountBox).BeginInit();
    SuspendLayout();
    // 
    // outputFilePathTbox
    // 
    outputFilePathTbox.Enabled = false;
    outputFilePathTbox.Location = new Point(12, 34);
    outputFilePathTbox.Name = "outputFilePathTbox";
    outputFilePathTbox.Size = new Size(314, 23);
    outputFilePathTbox.TabIndex = 0;
    // 
    // label1
    // 
    label1.AutoSize = true;
    label1.Location = new Point(13, 17);
    label1.Name = "label1";
    label1.Size = new Size(69, 15);
    label1.TabIndex = 1;
    label1.Text = "Output File:";
    // 
    // browseBtn
    // 
    browseBtn.Location = new Point(332, 33);
    browseBtn.Name = "browseBtn";
    browseBtn.Size = new Size(99, 23);
    browseBtn.TabIndex = 2;
    browseBtn.Text = "Browse...";
    browseBtn.UseVisualStyleBackColor = true;
    // 
    // groupBox1
    // 
    groupBox1.Controls.Add(storeCaptureCountBox);
    groupBox1.Controls.Add(label3);
    groupBox1.Controls.Add(captureDisabledBtn);
    groupBox1.Controls.Add(captureEnabledBtn);
    groupBox1.Controls.Add(label2);
    groupBox1.Location = new Point(13, 63);
    groupBox1.Name = "groupBox1";
    groupBox1.Size = new Size(418, 95);
    groupBox1.TabIndex = 3;
    groupBox1.TabStop = false;
    groupBox1.Text = "Screen Capture";
    // 
    // storeCaptureCountBox
    // 
    storeCaptureCountBox.Location = new Point(262, 62);
    storeCaptureCountBox.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
    storeCaptureCountBox.Name = "storeCaptureCountBox";
    storeCaptureCountBox.Size = new Size(51, 23);
    storeCaptureCountBox.TabIndex = 4;
    storeCaptureCountBox.TextAlign = HorizontalAlignment.Center;
    storeCaptureCountBox.Value = new decimal(new int[] { 25, 0, 0, 0 });
    // 
    // label3
    // 
    label3.AutoSize = true;
    label3.Location = new Point(25, 64);
    label3.Name = "label3";
    label3.Size = new Size(231, 15);
    label3.TabIndex = 3;
    label3.Text = "Number of recent screen captures to store:";
    // 
    // captureDisabledBtn
    // 
    captureDisabledBtn.AutoSize = true;
    captureDisabledBtn.Location = new Point(152, 41);
    captureDisabledBtn.Name = "captureDisabledBtn";
    captureDisabledBtn.Size = new Size(41, 19);
    captureDisabledBtn.TabIndex = 2;
    captureDisabledBtn.Text = "No";
    captureDisabledBtn.UseVisualStyleBackColor = true;
    captureDisabledBtn.CheckedChanged += captureDisabledBtn_CheckedChanged;
    // 
    // captureEnabledBtn
    // 
    captureEnabledBtn.AutoSize = true;
    captureEnabledBtn.Checked = true;
    captureEnabledBtn.Location = new Point(70, 41);
    captureEnabledBtn.Name = "captureEnabledBtn";
    captureEnabledBtn.Size = new Size(42, 19);
    captureEnabledBtn.TabIndex = 1;
    captureEnabledBtn.TabStop = true;
    captureEnabledBtn.Text = "Yes";
    captureEnabledBtn.UseVisualStyleBackColor = true;
    captureEnabledBtn.CheckedChanged += captureEnabledBtn_CheckedChanged;
    // 
    // label2
    // 
    label2.AutoSize = true;
    label2.Location = new Point(25, 22);
    label2.Name = "label2";
    label2.Size = new Size(125, 15);
    label2.TabIndex = 0;
    label2.Text = "Enable screen capture:";
    // 
    // okBtn
    // 
    okBtn.Location = new Point(275, 164);
    okBtn.Name = "okBtn";
    okBtn.Size = new Size(75, 23);
    okBtn.TabIndex = 4;
    okBtn.Text = "OK";
    okBtn.UseVisualStyleBackColor = true;
    okBtn.Click += okBtn_Click;
    // 
    // cancelBtn
    // 
    cancelBtn.Location = new Point(356, 164);
    cancelBtn.Name = "cancelBtn";
    cancelBtn.Size = new Size(75, 23);
    cancelBtn.TabIndex = 5;
    cancelBtn.Text = "Cancel";
    cancelBtn.UseVisualStyleBackColor = true;
    cancelBtn.Click += cancelBtn_Click;
    // 
    // SettingsForm
    // 
    AutoScaleDimensions = new SizeF(7F, 15F);
    AutoScaleMode = AutoScaleMode.Font;
    ClientSize = new Size(443, 195);
    Controls.Add(cancelBtn);
    Controls.Add(okBtn);
    Controls.Add(groupBox1);
    Controls.Add(browseBtn);
    Controls.Add(label1);
    Controls.Add(outputFilePathTbox);
    FormBorderStyle = FormBorderStyle.FixedDialog;
    Name = "SettingsForm";
    Text = "Problem Steps Recorder - NEXT / Settings";
    groupBox1.ResumeLayout(false);
    groupBox1.PerformLayout();
    ((System.ComponentModel.ISupportInitialize)storeCaptureCountBox).EndInit();
    ResumeLayout(false);
    PerformLayout();
  }

  #endregion

  private TextBox outputFilePathTbox;
  private Label label1;
  private Button browseBtn;
  private GroupBox groupBox1;
  private Label label3;
  private RadioButton captureDisabledBtn;
  private RadioButton captureEnabledBtn;
  private Label label2;
  private NumericUpDown storeCaptureCountBox;
  private Button okBtn;
  private Button cancelBtn;
}