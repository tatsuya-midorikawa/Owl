﻿namespace Owl.psrx.core

open System.Drawing
open System.Drawing.Imaging

module Jpg =
  
  let inline draw_rect (hwnd, img, wi: User32.Rect) =
    let dpi = Window.get_window_dpi(hwnd)
    let scale = Window.system_dpi / (float dpi)
    use g = Graphics.FromImage(img)
    let r = User32.Rect(
      top= int ((float wi.top) / scale),
      left= int ((float wi.left) / scale),
      right= int ((float wi.right) / scale),
      bottom= int ((float wi.bottom) / scale) )
    ()



  // 'quality' is specified between 0 and 100.
  // 0 is the highest compression and 100 is the highest quality.
  let inline save_as (path: string) (bitmap: Bitmap, quality: int64) =
    let dir = System.IO.Path.GetDirectoryName path
    if not <| System.IO.Directory.Exists(dir) then
      System.IO.Directory.CreateDirectory(dir) |> ignore

    let path = match System.IO.Path.GetExtension path with ".jpg" | ".jpeg" -> path | _ -> $"{path}.jpg"

    let codec = ImageCodecInfo.GetImageEncoders() |> Array.find (fun e -> e.MimeType = "image/jpeg")
    let enc = Encoder.Quality
    use parameters = new EncoderParameters(1)
    use p = new EncoderParameter(enc, match quality with q when q <= 0L -> 0L | q when 100L <= q -> 100L | q -> q)
    parameters.Param[0] <- p
    bitmap.Save(path, codec, parameters)
