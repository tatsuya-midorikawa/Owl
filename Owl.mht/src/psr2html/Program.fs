open Owl.mht
open System.IO

let debug_log msg = System.Diagnostics.Debug.WriteLine(msg)
let combine a b = Path.Combine(a, b)

// 出力フォルダの作成
// もしすでに作成されている場合、再起的に削除してから再作成する
let output_dir = Path.GetFullPath "./out"
if Directory.Exists output_dir
  then Directory.Delete(path = output_dir, recursive = true)
Directory.CreateDirectory(output_dir) |> ignore

// コマンドライン オプションの取得
let args = System.Environment.GetCommandLineArgs()
debug_log $"%A{args}"

try
  let mht = Mht.fpath args[1]
  let pages = mht |> Mht.load |> Seq.map Mime.parse
  for page in pages do
    match page with
    // text データの場合, 指定のエンコードでファイル保存する
    | Mime.text (content, encode) ->  
      let dir = combine output_dir "src"
      use fs = File.Create(combine dir content.location)
      fs.Write (encode.GetBytes content.body)
    | Mime.image (content, encode) ->
      match encode with
      | ContentTransferEncode.base64 ->
        let dir = combine output_dir "src"
        content.body 
        |> (base64 >> decode >> write (combine dir content.location))
      | _ -> raise (exn "Not supported encoding types yet.")
    | _ -> raise (exn "Not supported MIMEs yet.")
with
  e -> printfn "%s" e.Message