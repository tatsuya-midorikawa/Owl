namespace Owl.mht

type [<Struct>] FilePath = internal FilePath of string

[<AutoOpen>]
module File =
  let fpath (path: string) =
    if 0 <= path.IndexOfAny(System.IO.Path.GetInvalidPathChars())
      then invalidArg "file path" "The path inculdes invalid path chars."
      else System.IO.Path.GetFullPath path |> FilePath

  let output (FilePath save_as) (Base64Data data) =
    use stream = new System.IO.FileStream(save_as, System.IO.FileMode.Create)
    task {
      return! stream.WriteAsync (data, 0, data.Length)
    }
