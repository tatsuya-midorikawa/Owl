namespace Owl.psrx;

// Ref
// https://learn.microsoft.com/ja-jp/windows/win32/gdi/positioning-objects-on-multiple-display-monitors

using Owl.psrx.core;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

public partial class MainForm : Form {

  private enum State {
    Stop,
    Doing,
  }

  // Mouse Hook Handler
  private IntPtr hookHandle = IntPtr.Zero;
  // Callback Function Instance
  private User32.HookProc? hookProc = null;
  private readonly List<Task> tasks = new();
  private Task? saveTask;
  private int stepCount = -1;
  private readonly string id;
  private State state = State.Stop;

  private readonly SettingsForm settingsForm = new();
  private Settings settings = new();
  private readonly Rectangle origin = Window.max_range;
  private readonly double system_dpi = User32.GetDpiForSystem();
  private readonly string pname = Process.GetCurrentProcess().ProcessName;

  private string SavePath => Path.GetFullPath($"./PSRx_{id}.zip");
  private readonly string _tempPath = Path.GetTempPath();
  private string TempPath => Path.Combine(_tempPath, "psrx", id);
  private string TempSaveImagePath(int steps) => Path.Combine(TempPath, "img", $"step_{steps}.jpg");
  private string TempSaveHtmlPath => Path.Combine(TempPath, "index.html");
  private readonly Pen pen = new Pen(Color.Lime, 6);

  private readonly StringBuilder body = new StringBuilder(4096);

  private Task HookTask(nint hwnd, DateTime now, Bitmap img) {
    return Task.Run(() => {
      try {

        if (!settings.ScreenCaptureEnabled) {
          return;
        }

        if (!(0 < User32.GetWindowThreadProcessId(hwnd, out uint pid)
            && User32.GetWindowInfo(hwnd, out User32.WindowInfo pwi))) {
          return;
        }

        var dpi = User32.GetDpiForWindow(hwnd);
        var scale = system_dpi / dpi;

        using var p = Process.GetProcessById((int)pid);
        if (p.ProcessName == pname) {
          // PSRX operations are not captured.
          return;
        }

        //using var img = Window.capture_all_screen();
        var title = p.MainWindowTitle;
        var name = p.ProcessName;
        Debug.WriteLine($"#   {p.MainWindowTitle} ({p.ProcessName})");
        Debug.WriteLine($"    scale= {scale} (system dpi= {system_dpi} / dpi= {dpi})");

        // Surround the operated application with Lime color.
        using var g = Graphics.FromImage(img);
        var r = new User32.Rect {
          top = (int)(pwi.rcWindow.top / scale),
          left = (int)(pwi.rcWindow.left / scale),
          bottom = (int)(pwi.rcWindow.bottom / scale),
          right = (int)(pwi.rcWindow.right / scale)
        };
        var top = Math.Abs(origin.Top - r.top);
        var left = Math.Abs(origin.Left - r.left);
        var right = Math.Abs(left + (r.right - r.left));
        var bottom = Math.Abs(top + (r.bottom - r.top));

        Debug.WriteLine($"    r .top= {r.top}, r .right= {r.right}, r .bottom= {r.bottom}, r .left= {r.left}");
        Debug.WriteLine($"    top= {top}, right= {right}, bottom= {bottom}, left= {left}");

        Point[] points = {
          new Point(left, top),
          new Point(right, top),
          new Point(right, bottom),
          new Point(left, bottom),
        };

        g.DrawPolygon(pen, points);

        if (state == State.Doing) {
          Jpg.save_as(TempSaveImagePath(stepCount), img, 80);
          body.Append(Html.MakeBodyContent(stepCount, title, name, now));

          // If the maximum number of captures is exceeded,
          // the oldest jpg file are deleted first.
          if (settings.NumberOfRecentScreenCapturesToStore < stepCount) {
            var target = TempSaveImagePath(stepCount - settings.NumberOfRecentScreenCapturesToStore);
            try { File.Delete(target); } catch (Exception) { /* ignore exception */ }
          }

          ++stepCount;
        }
      } finally {
        img.Dispose();
      }
    });
  }

  // Mouse Hook events Callback Function
  private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam) {
    var now = DateTime.Now;
    var img = Window.capture_all_screen();
    // When mouse event is fired.
    if (state == State.Doing
        && 0 <= nCode
        && Marshal.PtrToStructure(lParam, typeof(User32.MsllHookStruct)) is User32.MsllHookStruct mhook) {
      switch ((int)wParam) {
        case User32.WM_LBUTTONUP: // On left mouse button clicked.
        case User32.WM_RBUTTONUP: // On right mouse button clicked.
          Debug.WriteLine($"MouseHookProc ({now:yyyy/MM/dd HH:mm:ss.fffff}): x= {mhook.pt.X}, y= {mhook.pt.Y}");
          // Heavy processing during Hook event will degrade overall system performance,
          // so processing is performed as a separate task.
          var hwnd = User32.GetForegroundWindow();
          tasks.Add(HookTask(hwnd, now, img));
          break;
        default:
          break;
      }
    }

    // Call Next Hook event
    return User32.CallNextHookEx(hookHandle, nCode, wParam, lParam);
  }

  public MainForm() {
    InitializeComponent();

    Shcore.SetProcessDpiAwareness(Shcore.PROCESS_PER_MONITOR_DPI_AWARE);
    User32.SetThreadDpiAwarenessContext(User32.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE);

    using var current = Process.GetCurrentProcess();
    id = $"{DateTime.Now:yyyyMMddHHmmss}_{current.Id}";

    settings = settingsForm.Settings;
    Debug.WriteLine(settings);

    hookProc = new User32.HookProc(MouseHookProc);
    // Set Global Mouse Hook
    hookHandle = User32.SetWindowsHookEx(User32.WH_MOUSE_LL, hookProc, IntPtr.Zero, 0);

    // If the mouse event hook fails, this app terminates.
    if (hookHandle == IntPtr.Zero) {
      Debug.WriteLine("Mouse event hook failed.");
      Application.Exit();
    }

    this.Disposed += MainForm_Disposed;
  }

  private void MainForm_Disposed(object? sender, EventArgs e) {
    Task.WaitAll(tasks.ToArray());
    if (saveTask != null) {
      Task.WaitAll(saveTask);
    }
    settingsForm.Dispose();
  }

  private void startRecBtn_Click(object sender, EventArgs e) {

    var tmpDir = Path.GetDirectoryName(TempSaveImagePath(-1)) ?? "";
    if (!Directory.Exists(tmpDir)) {
      Directory.CreateDirectory(tmpDir);
    }

    state = State.Doing;
    startRecBtn.BackColor = SystemColors.ControlDark;
    stopRecBtn.BackColor = SystemColors.Control;
    settingsBtn.BackColor = SystemColors.ControlDark;
    stepCount = 1;

    startRecBtn.Enabled = false;
    stopRecBtn.Enabled = true;
    settingsBtn.Enabled = false;

    var saveDir = Path.GetDirectoryName(SavePath) ?? "";
    if (!Directory.Exists(saveDir)) {
      Directory.CreateDirectory(saveDir);
    }
  }

  private void stopRecBtn_Click(object sender, EventArgs e) {
    state = State.Stop;
    startRecBtn.BackColor = SystemColors.Control;
    stopRecBtn.BackColor = SystemColors.ControlDark;
    settingsBtn.BackColor = SystemColors.Control;

    startRecBtn.Enabled = true;
    stopRecBtn.Enabled = false;
    settingsBtn.Enabled = true;

    saveTask = Task.Run(() => {
      Task.WaitAll(tasks.ToArray());
      // Create .html file
      var content = Html.Build(body);
      using (var writer = File.CreateText(TempSaveHtmlPath)) {
        writer.Write(content);
      }
      body.Clear();
      // Output ZIP file
      System.IO.Compression.ZipFile.CreateFromDirectory(TempPath, SavePath);
      Directory.Delete(TempPath, true);
      // Open exprorer.exe
      Process.Start("explorer.exe", Path.GetDirectoryName(SavePath)!);
    });
  }

  private void settingsBtn_Click(object sender, EventArgs e) {
    var cache = settingsForm.Settings;
    var r = settingsForm.ShowDialog();
    if (r != DialogResult.OK) {
      settingsForm.Set(cache);
    }
    settings = settingsForm.Settings;
  }
}
