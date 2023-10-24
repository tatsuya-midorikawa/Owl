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
type MimeType =
  | application of content: MhtPage
  | audio of content: MhtPage
  | example of content: MhtPage
  | font of content: MhtPage
  | image of content: MhtPage * encode: ContentTransferEncode
  | model of content: MhtPage
  | text of content: MhtPage * charset: System.Text.Encoding
  | video of content: MhtPage
  | message of content: MhtPage
  | multipart of content: MhtPage


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

    pages