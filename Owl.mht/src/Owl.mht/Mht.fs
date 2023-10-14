namespace Owl.mht

module Mht =
  let load (FilePath path) =
    if not (path.EndsWith ".mht") 
      then raise (invalidArg "Invalid file" $"path is not mht file: {path}")
    if not (System.IO.File.Exists path)
      then raise (invalidArg "File not found" $"the file is not found: {path}")

    let lines = System.IO.File.ReadLines path

    ()