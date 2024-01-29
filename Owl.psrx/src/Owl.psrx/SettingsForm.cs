using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Owl.psrx;

public readonly struct Settings {
  public readonly string OutputFile { get; init; }
  public readonly bool ScreenCaptureEnabled { get; init; }
  public readonly int NumberOfRecentScreenCapturesToStore { get; init; }

  public override string ToString() {
    return $"OutputFile= {OutputFile} / ScreenCaptureEnabled= {ScreenCaptureEnabled} / NumberOfRecentScreenCapturesToStore= {NumberOfRecentScreenCapturesToStore}";
  }
}
public partial class SettingsForm : Form {
  public SettingsForm() {
    InitializeComponent();
  }

  public Settings Settings => new() {
    OutputFile = outputFilePathTbox.Text,
    ScreenCaptureEnabled = captureEnabledBtn.Checked,
    NumberOfRecentScreenCapturesToStore = (int)storeCaptureCountBox.Value
  };

  public void Set(Settings settings) {
    outputFilePathTbox.Text = settings.OutputFile;
    captureEnabledBtn.Checked = settings.ScreenCaptureEnabled;
    captureDisabledBtn.Checked = !settings.ScreenCaptureEnabled;
    storeCaptureCountBox.Value = settings.NumberOfRecentScreenCapturesToStore;
    storeCaptureCountBox.Enabled = captureEnabledBtn.Checked;
  }

  private void okBtn_Click(object sender, EventArgs e) {
    this.DialogResult = DialogResult.OK;
    this.Close();
  }

  private void cancelBtn_Click(object sender, EventArgs e) {
    this.DialogResult = DialogResult.Cancel;
    this.Close();
  }

  private void captureEnabledBtn_CheckedChanged(object sender, EventArgs e) {
    storeCaptureCountBox.Enabled = true;
  }

  private void captureDisabledBtn_CheckedChanged(object sender, EventArgs e) {
    storeCaptureCountBox.Enabled = false;
  }
}
