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

mht
|> Mht.load
|> printfn "%A"
