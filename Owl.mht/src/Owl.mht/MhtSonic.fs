namespace Owl.mht.sonic

open System
open System.Text
open System.Text.RegularExpressions
open System.IO
open System.Collections.Generic
open System.Collections.ObjectModel

type MhtFile = MhtFile of IEnumerable<string>
type MhtPage = { header: string[]; body: string }

module Mht =
  let private is_empty s = String.IsNullOrWhiteSpace s

  let open_read mht = File.ReadLines mht |> MhtFile
  
  let private get_boundary' (file: IEnumerable<string>) =
    let rec fn (e: IEnumerator<string>) =
      if e.MoveNext()
        then
          match Regex.Matches(e.Current, ".*?boundary=\"(.*?)\"") with
          | mc when 0 < mc.Count -> $"--{mc[0].Groups[1].Value}"
          | _ -> fn e
        else
          raise (exn "boundary not found")
    use e = file.GetEnumerator()
    fn e

  let get_boundary (MhtFile file) = get_boundary' file

  let get_ctencode (lines: string[]) =
    let rec f i =
      if lines.Length <= i 
        then raise (exn "ContentTransferEncode is not found")
        else
          let line = lines[i]
          match Regex.Matches(line, "Content-Transfer-Encoding:\s*(7bit|8bit|binary|base64|quoted_printable).*") with
          | mc when 0 < mc.Count -> mc[0].Groups[1].Value
          | _ -> f (i+1)
    f 0

  let get_mime (lines: string[]) =
    let rec f i =
      if lines.Length <= i 
        then raise (exn "MIME-type is not found")
        else
          let line = lines[i]
          match Regex.Matches(line, "Content-Type:\s*((application|audio|example|font|image|model|text|video|message|multipart)/([a-zA-Z-]*)).*") with
          | mc when 0 < mc.Count -> mc[0].Groups[2].Value
          | _ -> f (i+1)
    f 0

  let get_charset (lines: string[]) =
    let rec f i =
      if lines.Length <= i 
        then Encoding.UTF8
        else
          let line = lines[i]
          match Regex.Matches(line, "Content-Type:.*?charset=\"(.*?)\".*") with
          | mc when 0 < mc.Count -> Encoding.GetEncoding(mc[0].Groups[1].Value)
          | _ -> f (i+1)
    f 0

  let get_location (lines: string[]) =
    let rec f i =
      if lines.Length <= i 
        then raise (exn "Content-Location is not found")
        else
          let line = lines[i]
          match Regex.Matches(line, "Content-Location:\s*(.*)") with
          | mc when 0 < mc.Count -> mc[0].Groups[1].Value
          | _ -> f (i+1)
    f 0

  let read (MhtFile file) =
    let bounary = get_boundary' file
    let acc = ResizeArray<MhtPage>(128)
    let header = ResizeArray<string>(8)
    let body = StringBuilder(256)
    let mutable is_header = true

    use e = file.GetEnumerator()
    while e.MoveNext() do
      let c = e.Current
      if c.StartsWith bounary
        then 
          acc.Add { header = header.ToArray(); body = body.ToString() }
          header.Clear()
          body.Clear() |> ignore
          is_header <- true
        else
          if is_header
            then 
              if is_empty c then is_header <- false else header.Add c
            else
              body.Append $"{c}{Environment.NewLine}" |> ignore
    acc.AsReadOnly()