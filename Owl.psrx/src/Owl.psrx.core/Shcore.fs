namespace Owl.psrx.core

open System.Runtime.InteropServices

module Shcore =
  [<Literal>]
  let PROCESS_PER_MONITOR_DPI_AWARE = 2

  [<DllImport("Shcore.dll", EntryPoint="SetProcessDpiAwareness"); CompiledName("SetProcessDpiAwareness")>]
  extern int setProcessDpiAwareness ([<In>] int processDpiAwareness);