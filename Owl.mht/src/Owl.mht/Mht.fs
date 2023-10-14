namespace Owl.mht
open System.Text.RegularExpressions

type [<Struct>] MhtFile = internal MhtFile of string

module Mht =
  let fpath (path: string) =
    if not (path.EndsWith ".mht") 
      then raise (invalidArg "Invalid file" $"path is not mht file: {path}")
    if not (System.IO.File.Exists path)
      then raise (invalidArg "File not found" $"the file is not found: {path}")
    MhtFile path

  let search_boundary (MhtFile mht) =    
    let lines = System.IO.File.ReadLines mht
    use e = lines.GetEnumerator()

    let mutable boundary = ""
    while (System.String.IsNullOrWhiteSpace boundary) && e.MoveNext() do
      let m = Regex.Matches(e.Current, ".*?boundary=\"(.*?)\"")
      if 0 < m.Count
        then boundary <- m[0].Groups[1].Value

    if System.String.IsNullOrWhiteSpace boundary
      then raise (exn "boundary not found")
      else boundary

  let load (mht: MhtFile) =
    let boundary = search_boundary mht
    let mht = match mht with MhtFile s -> s
    let lines = System.IO.File.ReadLines mht
    use a = lines.GetEnumerator()
    a.MoveNext() |> ignore
    a.Current |> printfn "%s"