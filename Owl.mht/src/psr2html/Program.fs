open Owl.mht
open Owl.mht.sonic
open System.IO
open System.Diagnostics
open System.Text.RegularExpressions
open System.Threading.Tasks
open System.Collections.Concurrent

let debug_log msg = System.Diagnostics.Debug.WriteLine(msg)
let combine a b = Path.Combine(a, b)
let indicator i =
  System.Console.Clear()
  let n = i % 4
  System.Console.WriteLine $"Processing%s{System.String('.', n)}{System.String(' ', 3 - n)}"

// // 出力フォルダの作成
// // もしすでに作成されている場合、再起的に削除してから再作成する
// let output_dir = Path.GetFullPath "./out"
// let img_dir = combine output_dir "img"
// let slide_dir = combine output_dir "slide"
// let pslide_dir = combine output_dir "pslide"
// if Directory.Exists output_dir
//   then Directory.Delete(path = output_dir, recursive = true)
// Directory.CreateDirectory(output_dir) |> ignore
// Directory.CreateDirectory(img_dir) |> ignore
// Directory.CreateDirectory(slide_dir) |> ignore
// Directory.CreateDirectory(pslide_dir) |> ignore

// // コマンドライン オプションの取得
// let args = System.Environment.GetCommandLineArgs()
// debug_log $"%A{args}"

// let sw = Stopwatch()
// sw.Start()

// try
//   let mht = Mht.fpath args[1]
//   let pages = mht |> Mht.load |> Seq.map Mime.parse
//   let src_pattern = "(.*src=\")(.*\.JPEG)(\".*)"
//   let slide_pattern = "(.*href=\")(slide[0-9]*\.htm)(\".*)"
//   let pslide_pattern = "(.*href=\")(pslide[0-9]*\.htm)(\".*)"
//   let mutable fin = false
//   let t = Task.Run(fun () -> 
//     let mutable i = 0
//     while not fin do
//       indicator i; i <- i + 1
//       Task.Delay(1000).Wait() )

//   for page: Mime in pages do
//     match page with
//     // text データの場合, 指定のエンコードでファイル保存する
//     | Mime.text (content, encode) ->
//       let output_dir =
//         if Regex.IsMatch (content.location, "pslide[0-9]*\.htm")
//           then pslide_dir
//         else if Regex.IsMatch (content.location, "slide[0-9]*\.htm")
//           then slide_dir
//           else output_dir

//       let body = Regex.Replace(content.body, src_pattern, "$1img/$2$3")
//       let body = Regex.Replace(body, pslide_pattern, "$1pslide/$2$3")
//       let body =
//         if content.location = "main.htm"
//           then Regex.Replace(body, slide_pattern, "$1slide/$2$3")
//           else Regex.Replace(body, slide_pattern, "$1../slide/$2$3")

//       use fs = File.Create(combine output_dir content.location)
//       fs.Write (encode.GetBytes body)
//     | Mime.image (content, encode) ->
//       match encode with
//       | ContentTransferEncode.base64 ->
//         content.body 
//         |> (base64 >> decode >> write (combine img_dir content.location))
//       | _ -> raise (exn "Not supported encoding types yet.")
//     | _ -> raise (exn "Not supported MIMEs yet.")
//   fin <- true
// with
//   e -> printfn "%s" e.Message

// コマンドライン オプションの取得
let args = System.Environment.GetCommandLineArgs()
debug_log $"%A{args}"

let mht =  args[1]
let handle = Mht.open_read mht
let output_dir = Path.GetFullPath "./out"
let img_dir = Path.GetFullPath "./out/img"
let slide_dir = Path.GetFullPath "./out/slide"
let pslide_dir = Path.GetFullPath "./out/pslide"

if Directory.Exists output_dir
  then Directory.Delete(path = output_dir, recursive = true)
Directory.CreateDirectory(output_dir) |> ignore
Directory.CreateDirectory(img_dir) |> ignore
Directory.CreateDirectory(slide_dir) |> ignore
Directory.CreateDirectory(pslide_dir) |> ignore

let inline is_pslide (location: string) = Regex.IsMatch (location, "pslide[0-9]*\.htm")
let inline is_slide (location: string) = Regex.IsMatch (location, "slide[0-9]*\.htm")
let inline replace (input: string, pattern: string, replacement: string) = Regex.Replace(input, pattern, replacement)

let [<Literal>] src_pattern = "(.*src=\")(.*\.JPEG)(\".*)"
let [<Literal>] href_pattern = "(.*href=\")(.*\.JPEG)(\".*)"
let [<Literal>] slide_pattern = "(.*href=\")(slide[0-9]*\.htm)(\".*)"
let [<Literal>] pslide_pattern = "(.*href=\")(pslide[0-9]*\.htm)(\".*)"

let inline img_save_as dst = base64 >> decode >> write dst

let sw = Stopwatch()
sw.Start()


let mutable fin = false
let t = Task.Run(fun () -> 
  let mutable i = 0
  while not fin do
    indicator i; i <- i + 1
    Task.Delay(500).Wait() )

try
  let pages = Mht.read handle
  Parallel.ForEach(
    Partitioner.Create(0, pages.Count), 
    fun (partition: int * int) ->
      let (start', end') = partition
      try
        for i = start' to end' do
          let page = pages[i]
          match Mht.get_mime page.header with
          | "text" ->
            let location = Mht.get_location page.header
            let charset = Mht.get_charset page.header
            let (output_dir, body) = 
              if location = "main.htm" || location = "main.html"
                then
                  let body = replace (page.body, src_pattern, "$1img/$2$3")
                  let body = replace (body, href_pattern, "$1img/$2$3")
                  let body = replace (body, pslide_pattern, "$1pslide/$2$3")
                  let body = replace (body, slide_pattern, "$1slide/$2$3")
                  (output_dir, body)
                elif location = "main.css" then
                  (output_dir, page.body)
                else
                  let body = replace (page.body, src_pattern, "$1img/$2$3")
                  let body = replace (body, href_pattern, "$1img/$2$3")
                  if is_pslide location
                    then
                      let body = replace (body, slide_pattern, "$1slide/$2$3")
                      (pslide_dir, body)
                    else
                      let body = replace (body, pslide_pattern, "$1pslide/$2$3")
                      (slide_dir, body)
                      
            use fs = File.Create(combine output_dir location)
            fs.Write (charset.GetBytes body)
          | "image" ->
            let location = Mht.get_location page.header
            match Mht.get_ctencode page.header with
            | "base64" -> page.body |> img_save_as (combine img_dir location)
            | _ -> printfn "Not supported encoding types yet." // raise (exn "Not supported encoding types yet.")
          | _ -> printfn "Not supported MIME types yet." // raise (exn "Not supported MIME types yet.")
        with 
          e -> printfn "%s" e.Message
    )
  |> ignore
finally
  fin <- true


sw.Stop()
System.Console.Clear()
printfn "### Finished ###"
printfn $"Processed time: {sw.Elapsed}"
printfn "--- press any key."
System.Console.ReadKey() |> ignore