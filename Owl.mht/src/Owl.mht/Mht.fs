namespace Owl.mht
open System.Text.RegularExpressions

type [<Struct>] MhtFile = internal MhtFile of string
type MhtPage = internal { header: string; location: string; body: string }
type ContentTransferEncode =
  | bit7 = 0              // 7bit
  | bit8 = 1              // 8bit
  | binary = 2            // binary
  | base64 = 3            // base64
  | quoted_printable = 4  // quoted-printable

[<RequireQualifiedAccess>]
type Mime =
  | application of content: MhtPage   // TBD
  | audio of content: MhtPage         // TBD
  | example of content: MhtPage       // TBD
  | font of content: MhtPage          // TBD
  | image of content: MhtPage * encode: ContentTransferEncode
  | model of content: MhtPage         // TBD
  | text of content: MhtPage * charset: System.Text.Encoding
  | video of content: MhtPage         // TBD
  | message of content: MhtPage       // TBD
  | multipart of content: MhtPage     // TBD
with
  static member parse (page: MhtPage) =
    let rec loop = function
      | [] -> raise (exn "Mime-type not found")
      | x::tail ->
        let mc = Regex.Matches(x, "Content-Type:\s*((application|audio|example|font|image|model|text|video|message|multipart)/([a-zA-Z-]*)).*")
        if 0 < mc.Count
          then
            match mc[0].Groups[2].Value with
            | "application" -> Mime.application page
            | "audio" -> Mime.audio page
            | "example" -> Mime.example page
            | "font" -> Mime.font page
            | "image" -> Mime.image
              // header 情報から to_ctenc を使って ContentTransferEncode を拾う
              (page)
            | "model" -> Mime.model page
            | "text" ->
              // header 情報から System.Text.Encoding.GetEncoding を使って Encoding 情報を拾う
              Mime.text (page)
            | "video" -> Mime.video page
            | "message" -> Mime.message page
            | "multipart" -> Mime.multipart page
            | _ -> raise (exn "Invalid Mime-type")
          else loop tail
      
    for l in page.header.Split(System.Environment.NewLine) do
      ()
    ()


module Mht =
  let private to_ctenc (value: string) =
    value.Replace("Content-Transfer-Encoding:", "").Replace(" ", "")
    |> function
      | "7bit" -> ContentTransferEncode.bit7
      | "8bit" -> ContentTransferEncode.bit8
      | "binary" -> ContentTransferEncode.binary
      | "base64" -> ContentTransferEncode.base64
      | "quoted_printable" -> ContentTransferEncode.quoted_printable
      | _ -> raise (exn "Invalid encode type")

  let fpath (path: string) =
    if not (path.EndsWith ".mht") 
      then raise (invalidArg "Invalid file" $"path is not mht file: {path}")
    if not (System.IO.File.Exists path)
      then raise (invalidArg "File not found" $"the file is not found: {path}")
    MhtFile path

  let private search_boundary' mht =   
    let lines = System.IO.File.ReadLines mht
    use e = lines.GetEnumerator()

    let mutable boundary = ""
    while (System.String.IsNullOrWhiteSpace boundary) && e.MoveNext() do
      let m = Regex.Matches(e.Current, ".*?boundary=\"(.*?)\"")
      if 0 < m.Count
        then boundary <- m[0].Groups[1].Value

    if System.String.IsNullOrWhiteSpace boundary
      then raise (exn "boundary not found")
      else $"--{boundary}"

  let search_boundary (MhtFile mht) = search_boundary' mht

  let private split' mht =
    let boundary = mht |> (fpath >> search_boundary)
    let lines = System.IO.File.ReadLines mht
    let acc = System.Text.StringBuilder(1_024)
    seq {
      use e = lines.GetEnumerator()
      while e.MoveNext() do
        if e.Current = boundary
          then 
            yield acc.ToString()
            acc.Clear() |> ignore
          else
            acc.Append(e.Current).Append(System.Environment.NewLine) |> ignore
    }

  let split (MhtFile mht) = split' mht

  let load (MhtFile mht) =
    // Skip header field
    let pages = split' mht |> Seq.skip 1

    let parse (s: string) =
      let lines = s.Split(System.Environment.NewLine)

      let header = System.Text.StringBuilder(256)
      let body = System.Text.StringBuilder(512)
      let mutable location = ""
      let mutable is_header = true
      
      System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance)
      let mutable encode = System.Text.Encoding.UTF8

      for l in lines do

        if is_header
          then
            let m = Regex.Matches(l, "Content-Location:\s*(.*)")
            if 0 < m.Count
              then location <- m[0].Groups[1].Value

            let m = Regex.Matches(l, "Content-Type:.*?charset=\"(.*?)\"")
            if 0 < m.Count
              then encode <- System.Text.Encoding.GetEncoding(m[0].Groups[1].Value)

            if l = System.String.Empty
              then is_header <- false
              else header.Append(l).Append(System.Environment.NewLine) |> ignore
          else
            body.Append(l).Append(System.Environment.NewLine) |> ignore

      { header = header.ToString(); body = body.ToString(); location = location }

    pages
    |> Seq.map parse