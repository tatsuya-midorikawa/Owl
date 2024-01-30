namespace Owl.psrx.core

open System.Drawing
open System.Drawing.Imaging
open System.Windows.Forms
open System.Runtime.InteropServices

module Window =
  [<Literal>]
  let ENUM_CURRENT_SETTINGS = -1

  let all_screens = Screen.AllScreens

  let inline get_max_range() =
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
    //let max_range (r: Rectangle) (s: Screen) =
    //  let mutable dm = User32.DevMode()
    //  User32.enumDisplaySettings(s.DeviceName,  ENUM_CURRENT_SETTINGS, &dm) |> ignore
    //  System.Diagnostics.Debug.WriteLine($"(x= {dm.dmPositionX}, y= {dm.dmPositionY}) (W= {dm.dmPelsWidth}, H= {dm.dmPelsHeight})")
    //  Rectangle(
    //    min r.X dm.dmPositionX, min r.Y dm.dmPositionY,
    //    r.Width + (int dm.dmPelsWidth), max r.Height (int dm.dmPelsHeight))

    //all_screens
    //|> Array.fold max_range Rectangle.Empty
    get_max_range()
    |> capture