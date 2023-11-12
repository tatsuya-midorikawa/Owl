namespace Owl.mht.core

type CheckedPath = CheckedPath of string

module File =
  let is_valid (path: string) = 
    if 0 <= path.IndexOfAny(System.IO.Path.GetInvalidPathChars())
      then invalidArg "file path" "The path inculdes invalid path chars."
      else System.IO.Path.GetFullPath path |> CheckedPath
