module ARCtrl.NET.Path

open System.IO

let ensureDirectory (filePath : string) =
    let file = new System.IO.FileInfo(filePath);
    file.Directory.Create()

let getAllFilePaths (directoryPath : string) =
    let rec allFiles dirs =
        if Seq.isEmpty dirs then Seq.empty else
            seq { yield! dirs |> Seq.collect Directory.EnumerateFiles
                  yield! dirs |> Seq.collect Directory.EnumerateDirectories |> allFiles }

    allFiles [directoryPath] |> Seq.toArray
    |> Array.map (fun p -> p.Replace(directoryPath, ""))