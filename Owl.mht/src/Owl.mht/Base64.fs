namespace Owl.mht

type [<Struct>] Base64String = internal Base64String of string
type [<Struct>] Base64Data = internal Base64Data of byte[]

[<AutoOpen>]
module Base64 =
  let base64 (data: string) =
    Base64String data

  let decode (Base64String src) =
    Base64Data (System.Convert.FromBase64String src)
    
  let write (save_as: string) (Base64Data src) =
    use fs = System.IO.File.Create(save_as)
    fs.Write src