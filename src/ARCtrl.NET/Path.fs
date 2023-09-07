module ARCtrl.NET.Path

open System.IO

let ensureDirectory (filePath : string) =
    let file = new System.IO.FileInfo(filePath);
    file.Directory.Create()

/// Return the absolute path relative to the directoryPath
let makeRelative directoryPath (path : string) = 
    if directoryPath = "." || directoryPath = "/" || directoryPath = "" then path
    else
        if path.StartsWith(directoryPath) then 
            path.Substring(directoryPath.Length)
        else path

let standardizeSlashes (path : string) = 
    path.Replace("\\","/")              

let getAllFilePaths (directoryPath : string) =
    let rec allFiles dirs =
        if Seq.isEmpty dirs then Seq.empty else
            seq { yield! dirs |> Seq.collect Directory.EnumerateFiles
                  yield! dirs |> Seq.collect Directory.EnumerateDirectories |> allFiles }
    
    allFiles [directoryPath] |> Seq.toArray
    |> Array.map (makeRelative directoryPath >> standardizeSlashes)
