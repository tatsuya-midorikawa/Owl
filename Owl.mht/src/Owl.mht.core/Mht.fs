namespace Owl.mht.core

open System
open System.IO
open System.Text
open System.Text.RegularExpressions
open System.Runtime.CompilerServices
open System.ComponentModel

module Mht =

  /// <summary>
  /// MHT ファイルの個別ページクラス
  /// </summary>
  [<IsReadOnly;Struct;>]
  type Page internal (raw': string) =
    member __.raw = raw'

  /// <summary>
  /// MHT ファイルのバウンダリー文字列の取得を試みる関数
  /// </summary>
  let inline private try_get_boudary (line) =
    let mc = Regex.Matches(line, ".*?boundary=\"(.*?)\"")
    if 0 < mc.Count
      then Some mc[0].Groups[1].Value
      else None

  /// <summary>
  /// MHT ファイルの個別ページを列挙する関数
  /// </summary>
  let enumerate (CheckedPath path) =
    let lines = File.ReadLines path
    let mutable boundary = ""
    let acc = StringBuilder 1024
    seq {
      for line in lines do
        // boudary 文字列が検索できていない場合、boudary 文字列が見つかるまで try する
        if boundary = ""
          then match try_get_boudary line with Some s -> boundary <- $"--%s{s}" | None -> ()
          else ()

        if line = boundary
          then
            // boundary まで来たら、キャッシュを yield return
            yield Page (acc.ToString())
            acc.Clear() |> ignore
          else
            // キャッシュに文字列を追加しつつ、改行文字も追加して元の行区切りを維持する
            acc.Append line 
            |> fun (sb) -> sb.Append Environment.NewLine
            |> ignore
    }

  /// <summary>
  /// MHT ファイルの個別ページクラスに対して非同期処理を行う関数
  /// </summary>
  let task_iter (task: Page -> unit) (CheckedPath path) = 
    ()