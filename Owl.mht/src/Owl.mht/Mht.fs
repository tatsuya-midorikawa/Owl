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
    let xs = split' mht
    let ys =
      xs
      |> Seq.take 1
      |> Seq.toArray
    let zs =
      ys[0].Split(System.Environment.NewLine)
    zs
    |> Array.iter (printfn "%s")

    ()