﻿namespace arcIO.NET

open ISADotNet
open ISADotNet.XLSX
open System.IO 

module Assay = 

    let rootFolderName = "assays"

    let assayFileName = "isa.assay.xlsx"

    let subFolderPaths = 
        ["dataset";"protocol"]

    module AssayFolder =
        
        /// Checks if an assay folder exists in the ARC.
        let exists (arc : string) (identifier : string) =
            Path.Combine([|arc;rootFolderName;identifier|])
            |> System.IO.Directory.Exists

    let readFromFolder (folderPath : string) =
        let ap = Path.Combine (folderPath,assayFileName)
        let c,a = AssayFile.Assay.fromFile ap
        c,a

    let readByFileName (arc : string) (assayFileName : string) =
        let ap = Path.Combine ([|arc;rootFolderName;assayFileName|])
        let c,a = AssayFile.Assay.fromFile ap
        c,a

    let readByName (arc : string) (assayName : string) =
        Path.Combine ([|arc;rootFolderName;assayName|])
        |> readFromFolder

    let writeToFolder (folderPath : string) (contacts : Person list) (assay : Assay) =
        let ap = Path.Combine (folderPath,assayFileName)
        AssayFile.Assay.toFile ap contacts assay        

    let write (arc : string) (contacts : Person list) (assay : Assay) =
        if assay.FileName.IsNone then
            failwith "Cannot write assay to arc, as it has no filename"
        let ap = Path.Combine ([|arc;"assays";assay.FileName.Value|])
        AssayFile.Assay.toFile ap contacts assay  

    let identifierFromFileName (fileName : string) = 
        let regex = $@".+(?=[/\\]{assayFileName})"
        System.Text.RegularExpressions.Regex.Match(fileName,regex).Value

    let init (arc : string) (assay : Assay) =
        
        if assay.FileName.IsNone then
            failwith "Given assay does not contain filename"

        let assayIdentifier = identifierFromFileName assay.FileName.Value

        if AssayFolder.exists arc assayIdentifier then
            printfn $"Assay folder with identifier {assayIdentifier} already exists."
        else
            subFolderPaths 
            |> List.iter (fun n ->
                let dp = Path.Combine([|arc;rootFolderName;assayIdentifier;n|])
                let dir = Directory.CreateDirectory(dp)
                File.Create(Path.Combine(dir.FullName, ".gitkeep")) |> ignore 
            )

            let assayFilePath = Path.Combine([|arc;rootFolderName;assay.FileName.Value|])

            AssayFile.Assay.toFile assayFilePath [] assay


    let initFromName (arc : string) (assayName : string) =
        
        let assayFileName = Path.Combine(assayName,assayFileName)

        let assay = Assay.create(FileName = assayFileName)

        init arc assay
