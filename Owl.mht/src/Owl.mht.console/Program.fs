open Owl.mht
open System.IO
open System.Text.RegularExpressions

// let jpg = "base64 value"
// let save_as = fpath "./output.jpg"
// let target = jpg |> (base64 >> decode)

// task {
//   do! output save_as target
// }
// |> System.Threading.Tasks.Task.WaitAll

let mht = 
  Mht.fpath "../assets/sample.mht"

// mht
// |> Mht.search_boundary
// |> printfn "%s"

// mht
// |> Mht.split
// |> Seq.take 1
// |> printfn "%A"

// mht
// |> Mht.load
// |> printfn "%A"

let a = "Content-Type: text/html; charset=\"UTF-8\""
let b = "Content-Type: image/jpeg"
let c = "Content-Type: image/quoted-printable; charset=\"UTF-8\""

let m = Regex.Matches(a, "Content-Type:\s*((application|audio|example|font|image|model|text|video|message|multipart)/([a-zA-Z-]*)).*")
if 0 < m.Count
  then
    m[0].Groups[1].Value |> printfn "%s"  // image/jpeg
    m[0].Groups[2].Value |> printfn "%s"  // image
    m[0].Groups[3].Value |> printfn "%s"  // jpeg
  else
    printfn "not found"

printfn "=== end ==="