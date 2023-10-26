open Owl.mht
open System.IO
open System.Diagnostics
open System.Text.RegularExpressions
open System.Threading.Tasks

let debug_log msg = System.Diagnostics.Debug.WriteLine(msg)
let combine a b = Path.Combine(a, b)
let indicator i =
  System.Console.Clear()
  let n = i % 4
  System.Console.WriteLine $"Processing%s{System.String('.', n)}{System.String(' ', 3 - n)}"

// 出力フォルダの作成
// もしすでに作成されている場合、再起的に削除してから再作成する
let output_dir = Path.GetFullPath "./out"
let img_dir = combine output_dir "img"
let slide_dir = combine output_dir "slide"
let pslide_dir = combine output_dir "pslide"
if Directory.Exists output_dir
  then Directory.Delete(path = output_dir, recursive = true)
Directory.CreateDirectory(output_dir) |> ignore
Directory.CreateDirectory(img_dir) |> ignore
Directory.CreateDirectory(slide_dir) |> ignore
Directory.CreateDirectory(pslide_dir) |> ignore

// コマンドライン オプションの取得
let args = System.Environment.GetCommandLineArgs()
debug_log $"%A{args}"

let sw = Stopwatch()
sw.Start()

try
  let mht = Mht.fpath args[1]
  let pages = mht |> Mht.load |> Seq.map Mime.parse
  let src_pattern = "(.*src=\")(.*\.JPEG)(\".*)"
  let slide_pattern = "(.*href=\")(slide[0-9]*\.htm)(\".*)"
  let pslide_pattern = "(.*href=\")(pslide[0-9]*\.htm)(\".*)"
  let mutable fin = false
  let t = Task.Run(fun () -> 
    let mutable i = 0
    while not fin do
      indicator i; i <- i + 1
      Task.Delay(1000).Wait() )

  for page: Mime in pages do
    match page with
    // text データの場合, 指定のエンコードでファイル保存する
    | Mime.text (content, encode) ->
      let output_dir =
        if Regex.IsMatch (content.location, "pslide[0-9]*\.htm")
          then pslide_dir
        else if Regex.IsMatch (content.location, "slide[0-9]*\.htm")
          then slide_dir
          else output_dir

      let body = Regex.Replace(content.body, src_pattern, "$1img/$2$3")
      let body = Regex.Replace(body, pslide_pattern, "$1pslide/$2$3")
      let body =
        if content.location = "main.htm"
          then Regex.Replace(body, slide_pattern, "$1slide/$2$3")
          else Regex.Replace(body, slide_pattern, "$1../slide/$2$3")

      use fs = File.Create(combine output_dir content.location)
      fs.Write (encode.GetBytes body)
    | Mime.image (content, encode) ->
      match encode with
      | ContentTransferEncode.base64 ->
        content.body 
        |> (base64 >> decode >> write (combine img_dir content.location))
      | _ -> raise (exn "Not supported encoding types yet.")
    | _ -> raise (exn "Not supported MIMEs yet.")
  fin <- true
with
  e -> printfn "%s" e.Message

sw.Stop()
System.Console.Clear()
printfn "### Finished ###"
printfn $"Processed time: {sw.Elapsed}"
printfn "--- press any key."
System.Console.ReadKey() |> ignore