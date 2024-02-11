namespace Owl.psrx.core

open System.Drawing
open System.Drawing.Imaging
open System.Windows.Forms
open System.Runtime.InteropServices

module Window =
  [<Literal>]
  let ENUM_CURRENT_SETTINGS = -1

  let all_screens = Screen.AllScreens
  let system_dpi = User32.getDpiForSystem() |> float
  let inline get_window_dpi hwnd = User32.getDpiForWindow(hwnd)

  let max_range =
    let max_range (r: Rectangle) (s: Screen) =
      let mutable dm = User32.DevMode()
      User32.enumDisplaySettings(s.DeviceName,  ENUM_CURRENT_SETTINGS, &dm) |> ignore
      Rectangle(
        min r.X dm.dmPositionX, min r.Y dm.dmPositionY,
        r.Width + (int dm.dmPelsWidth), max r.Height (int dm.dmPelsHeight))

    all_screens
    |> Array.fold max_range Rectangle.Empty

  let inline capture (rectangle: Rectangle)  =
    let bitmap = Bitmap (rectangle.Width, rectangle.Height)
    use graphics = Graphics.FromImage(bitmap)
    graphics.CopyFromScreen(rectangle.X, rectangle.Y, 0, 0, rectangle.Size)
    bitmap

  let inline capture_all_screen () =
    capture max_range