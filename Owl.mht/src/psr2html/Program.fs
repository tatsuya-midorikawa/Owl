open Owl.mht

let args = System.Environment.GetCommandLineArgs()
printfn "%A" args

try
  let mht = Mht.fpath args[1]
  let pages = mht |> Mht.load |> Seq.map Mime.parse
  for page in pages do
    match page with
    | Mime.text (content, encode) -> ()
    | Mime.image (content, encode) -> ()
    | _ -> ()


with
  e -> printfn "%s" e.Message