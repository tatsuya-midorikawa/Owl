namespace Owl.psrx.core

open System.Runtime.InteropServices

module User32 =
  [<Literal>]
  let WH_MOUSE_LL = 14
  [<Literal>]
  let WM_LBUTTONDOWN = 0x0201
  [<Literal>]
  let WM_LBUTTONUP = 0x0202
  [<Literal>]
  let WM_RBUTTONDOWN = 0x0204
  [<Literal>]
  let WM_RBUTTONUP = 0x0205
  [<Literal>]
  let DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = -3
  
  type HookProc = delegate of nCode: int * wParam: nativeint * lParam: nativeint -> nativeint
  
  [<Struct; StructLayout(LayoutKind.Sequential)>]
  type MsllHookStruct =
    val pt: System.Drawing.Point
    val mouseData: nativeint
    val flags: int
    val time: int
    val dwExtraInfo: nativeint
      
  [<Struct; StructLayout(LayoutKind.Sequential)>]
  type Rect =
    val mutable left: int
    val mutable top: int
    val mutable right: int
    val mutable bottom:int  

  [<Struct; StructLayout(LayoutKind.Sequential)>]
  type WindowInfo =
    val mutable cbSize: int
    val rcWindow: Rect
    val rcClient: Rect
    val dwStyle: int
    val dwExStyle: int
    val dwWindowStatus: int
    val cxWindwBorders: uint
    val cyWindwBorders: int
    val atomWindowType: int16
    val wCreatorCersion: int16

  [<Struct; StructLayout(LayoutKind.Sequential)>]
  type DevMode =
    [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)>]
    val dmDeviceName: string
    val dmSpecVersion: uint16
    val dmDriverVersion: uint16
    val dmSize: uint16
    val dmDriverExtra: uint16
    val dmFields: uint32
    val dmPositionX: int32
    val dmPositionY: int32
    val dmDisplayOrientation: uint32
    val dmDisplayOrientationFixedOutput: uint32
    val dmColor: int16
    val dmDuplex: int16
    val dmYResolution: int16
    val dmTTOption: int16
    val dmCollate: int16
    [<MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)>]
    val dmFormName: string
    val dmLogPixels: uint16
    val dmBitsPerPel: uint32
    val dmPelsWidth: uint32
    val dmPelsHeight: uint32
    val dmDisplayFlags: uint32
    val dmDisplayFrequency: uint32
    val dmICMMethod: uint32
    val dmICMIntent: uint32
    val dmMediaType: uint32
    val dmDitherType: uint32
    val dmReserved1: uint32
    val dmReserved2: uint32
    val dmPanningWidth: uint32
    val dmPanningHeight: uint32

  [<DllImport("user32.dll", EntryPoint="WindowFromPoint"); CompiledName("WindowFromPoint")>]
  extern nativeint windowFromPoint(System.Drawing.Point point)
  
  [<DllImport("user32.dll", EntryPoint="WindowFromPhysicalPoint"); CompiledName("WindowFromPhysicalPoint")>]
  extern nativeint windowFromPhysicalPoint([<In>] System.Drawing.Point point)
  
  [<DllImport("user32.dll", EntryPoint="GetWindowThreadProcessId"); CompiledName("GetWindowThreadProcessId")>]
  extern uint getWindowThreadProcessId(nativeint hWnd, [<Out>] uint& lp);
  
  [<DllImport("user32.dll", EntryPoint="SetWindowsHookEx"); CompiledName("SetWindowsHookEx")>]
  extern nativeint setWindowsHookEx(int idHook, HookProc lpfn, nativeint hMod, uint dwThreadId)
  
  [<DllImport("user32.dll", EntryPoint="UnhookWindowsHookEx"); CompiledName("UnhookWindowsHookEx")>]
  extern bool unhookWindowsHookEx(nativeint hhk)
  
  [<DllImport("user32.dll", EntryPoint="CallNextHookEx"); CompiledName("CallNextHookEx")>]
  extern nativeint callNextHookEx(nativeint hhk, int nCode, nativeint wParam, nativeint lParam)
  
  [<DllImport("user32.dll", EntryPoint="GetWindowRect"); CompiledName("GetWindowRect")>]
  extern bool getWindowRect(nativeint hWnd, [<Out>] Rect& lpRect);
    
  [<DllImport("user32.dll", EntryPoint="GetWindowInfo"); CompiledName("GetWindowInfo")>]
  extern bool getWindowInfo(nativeint hWnd, [<Out>] WindowInfo& pwi);
    
  [<DllImport("user32.dll", EntryPoint="GetDpiForWindow"); CompiledName("GetDpiForWindow")>]
  extern int getDpiForWindow([<In>] nativeint hWnd);

  [<DllImport("user32.dll", EntryPoint="GetDpiForSystem"); CompiledName("GetDpiForSystem")>]
  extern int getDpiForSystem();
  
  [<DllImport("user32.dll", EntryPoint="EnumDisplaySettings"); CompiledName("EnumDisplaySettings")>]
  extern bool enumDisplaySettings(string lpszDeviceName, int iModeNum, [<Out>] DevMode& lpDevMode);

  [<DllImport("user32.dll", EntryPoint="SetThreadDpiAwarenessContext"); CompiledName("SetThreadDpiAwarenessContext")>]
  extern uint setThreadDpiAwarenessContext([<In>] int dpiCntext);

  [<DllImport("user32.dll", EntryPoint="GetForegroundWindow"); CompiledName("GetForegroundWindow")>]
  extern nativeint getForegroundWindow();