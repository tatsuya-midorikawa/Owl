namespace Owl.mht.sonic

open System
open System.Text
open System.Text.RegularExpressions
open System.IO
open System.Collections.Generic
open System.Collections.ObjectModel

type MhtFile = MhtFile of IEnumerator<string>
type MhtPage = { header: string[]; body: string[] }

module Mht =
  let private is_empty s = String.IsNullOrWhiteSpace s

  let open_read mht = (File.ReadLines mht).GetEnumerator() |> MhtFile
  
  let private get_boundary' (file: IEnumerator<string>) =
    file.Reset()
    let rec fn (e: IEnumerator<string>) =
      if e.MoveNext()
        then
          match Regex.Matches(e.Current, ".*?boundary=\"(.*?)\"") with
          | mc when 0 < mc.Count -> $"--{mc[0].Groups[1].Value}"
          | _ -> fn e
        else
          raise (exn "boundary not found")
    fn file

  let get_boundary (MhtFile file) = get_boundary' file

  let get_ctencode (lines: string[]) =
    let rec f i =
      if lines.Length <= i then raise (exn "ContentTransferEncode is not found")
      let line = lines[i]
      match Regex.Matches(line, "Content-Transfer-Encoding:\s*(7bit|8bit|binary|base64|quoted_printable).*") with
      | mc when 0 < mc.Count -> mc[0].Groups[1].Value
      | _ -> f (i+1)
    f 0



  let read (MhtFile file) =
    file.Reset()
    
    let bounary = get_boundary' file
    file.Reset()

    let acc = ResizeArray<MhtPage>(128)
    let header = ResizeArray<string>(8)
    let body = ResizeArray<string>(32)
    let mutable is_header = true
    while file.MoveNext() do
      let c = file.Current
      if c.StartsWith bounary
        then 
          acc.Add { header = header.ToArray(); body = body.ToArray() }
          header.Clear()
          body.Clear()
          is_header <- true
        else
          if is_header
            then 
              if is_empty c then is_header <- false else header.Add c
            else
              body.Add c
    acc.AsReadOnly()