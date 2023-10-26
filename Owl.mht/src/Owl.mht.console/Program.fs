open Owl.mht
open Owl.mht.sonic
open System.IO
open System.Text.RegularExpressions

// let jpg = "base64 value"
// let save_as = fpath "./output.jpg"
// let target = jpg |> (base64 >> decode)

// task {
//   do! output save_as target
// }
// |> System.Threading.Tasks.Task.WaitAll

// let mht = 
//   Mht.fpath "../assets/sample.mht"

// mht
// |> Mht.search_boundary
// |> printfn "%s"

// mht
// |> Mht.split
// |> Seq.take 1
// |> printfn "%A"

// mht
// |> Mht.load
// // |> Seq.take 2
// |> Seq.skip 6
// |> Seq.map Mime.parse
// |> printfn "%A"

// let a = "Content-Type: text/html; charset=\"UTF-8\""
// let b = "Content-Type: image/jpeg"
// let c = "Content-Type: image/quoted-printable; charset=\"UTF-8\""

// let m = Regex.Matches(a, "Content-Type:\s*((application|audio|example|font|image|model|text|video|message|multipart)/([a-zA-Z-]*)).*")
// if 0 < m.Count
//   then
//     m[0].Groups[1].Value |> printfn "%s"  // image/jpeg
//     m[0].Groups[2].Value |> printfn "%s"  // image
//     m[0].Groups[3].Value |> printfn "%s"  // jpeg
//   else
//     printfn "not found"


// let x = "Content-Transfer-Encoding: base64;"
// let mc = Regex.Matches(x, "Content-Transfer-Encoding:\s*(7bit|8bit|binary|base64|quoted_printable).*")
// if 0 < mc.Count
//   then
//     mc[0].Groups[1].Value |> printfn "%s"
//   else
//     printfn "not found"

// let src = "<a class=\"left33 align-center\" title=\"Return to first slide\" href=\"pslide0001.htm\">Return to first slide</a>"
// let pattern = "(.*href=\")(pslide.*\.htm)(\".*)"
// let result = Regex.Replace(src, pattern, "$1pslide/$2$3")

// Regex.IsMatch ("pslide0001.htm", "pslide[0-9]*\.htm")
// |> printfn "%b"

// printfn "%s" result


let mht = Path.GetFullPath "../assets/sample.mht"
let handle = Mht.open_read mht
let output_dir = Path.GetFullPath "./out"

let combine a b = Path.Combine(a, b)







if Directory.Exists output_dir
  then Directory.Delete(path = output_dir, recursive = true)
Directory.CreateDirectory(output_dir) |> ignore

try
  let pages = Mht.read handle
  for page in pages do
    match Mht.get_mime page.header with
    | "text" ->
      let location = Mht.get_location page.header
      let charset = Mht.get_charset page.header
      use fs = File.Create(combine output_dir location)
      fs.Write (charset.GetBytes page.body)
    | "image" ->
      let location = Mht.get_location page.header
      match Mht.get_ctencode page.header with
      | "base64" ->
        page.body
        |> (base64 >> decode >> write (combine output_dir location))
      | _ -> ()
    | _ -> ()
finally
  ()


printfn "=== end ==="