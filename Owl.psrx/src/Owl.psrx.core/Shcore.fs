namespace Owl.psrx.core

open System.Runtime.InteropServices

module Shcore =
  [<Literal>]
  let PROCESS_PER_MONITOR_DPI_AWARE = 2

  [<DllImport("shcore.dll", EntryPoint="SetProcessDpiAwareness"); CompiledName("SetProcessDpiAwareness")>]
  extern bool setProcessDpiAwareness([<In>] int value);