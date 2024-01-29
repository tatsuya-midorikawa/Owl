namespace Owl.psrx;

partial class MainForm {
  /// <summary>
  ///  Required designer variable.
  /// </summary>
  private System.ComponentModel.IContainer components = null;

  /// <summary>
  ///  Clean up any resources being used.
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
  ///  Required method for Designer support - do not modify
  ///  the contents of this method with the code editor.
  /// </summary>
  private void InitializeComponent() {
    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
    toolStrip1 = new ToolStrip();
    startRecBtn = new ToolStripButton();
    stopRecBtn = new ToolStripButton();
    settingsBtn = new ToolStripButton();
    toolStrip1.SuspendLayout();
    SuspendLayout();
    // 
    // toolStrip1
    // 
    toolStrip1.Items.AddRange(new ToolStripItem[] { startRecBtn, stopRecBtn, settingsBtn });
    toolStrip1.Location = new Point(0, 0);
    toolStrip1.Name = "toolStrip1";
    toolStrip1.Size = new Size(278, 25);
    toolStrip1.TabIndex = 1;
    toolStrip1.Text = "toolStrip1";
    // 
    // startRecBtn
    // 
    startRecBtn.DisplayStyle = ToolStripItemDisplayStyle.Text;
    startRecBtn.Image = (Image)resources.GetObject("startRecBtn.Image");
    startRecBtn.ImageTransparentColor = Color.Magenta;
    startRecBtn.Name = "startRecBtn";
    startRecBtn.Size = new Size(90, 22);
    startRecBtn.Text = "◉ Start Record";
    startRecBtn.Click += startRecBtn_Click;
    // 
    // stopRecBtn
    // 
    stopRecBtn.BackColor = SystemColors.ControlDark;
    stopRecBtn.DisplayStyle = ToolStripItemDisplayStyle.Text;
    stopRecBtn.Enabled = false;
    stopRecBtn.Image = (Image)resources.GetObject("stopRecBtn.Image");
    stopRecBtn.ImageTransparentColor = Color.Magenta;
    stopRecBtn.Name = "stopRecBtn";
    stopRecBtn.Size = new Size(90, 22);
    stopRecBtn.Text = "◼︎ Stop Record";
    stopRecBtn.Click += stopRecBtn_Click;
    // 
    // settingsBtn
    // 
    settingsBtn.DisplayStyle = ToolStripItemDisplayStyle.Text;
    settingsBtn.Image = (Image)resources.GetObject("settingsBtn.Image");
    settingsBtn.ImageTransparentColor = Color.Magenta;
    settingsBtn.Name = "settingsBtn";
    settingsBtn.Size = new Size(68, 22);
    settingsBtn.Text = "⚙ Settings";
    settingsBtn.Click += settingsBtn_Click;
    // 
    // MainForm
    // 
    AutoScaleDimensions = new SizeF(7F, 15F);
    AutoScaleMode = AutoScaleMode.Font;
    ClientSize = new Size(278, 25);
    Controls.Add(toolStrip1);
    FormBorderStyle = FormBorderStyle.FixedToolWindow;
    Name = "MainForm";
    Text = "Problem Steps Recorder - NEXT";
    TopMost = true;
    toolStrip1.ResumeLayout(false);
    toolStrip1.PerformLayout();
    ResumeLayout(false);
    PerformLayout();
  }

  #endregion
  private ToolStrip toolStrip1;
  private ToolStripButton startRecBtn;
  private ToolStripButton stopRecBtn;
  private ToolStripButton settingsBtn;
}
