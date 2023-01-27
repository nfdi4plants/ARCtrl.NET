namespace arcIO.NET

open ISADotNet
open ISADotNet.XLSX
open System.IO 

module Assay = 

    let rootFolderName = "assays"

    let assayFileName = "isa.assay.xlsx"

    let subFolderPaths = 
        ["dataset";"protocol"]

    let nameToFileName (n:string) =
        if n.Contains("/") || n.Contains("\\") then n else
        Path.Combine(n,assayFileName)

    module AssayFolder =
        
        /// Checks if an assay folder exists in the ARC.
        let exists (arc : string) (identifier : string) =
            Path.Combine([|arc;rootFolderName;identifier|])
            |> System.IO.Directory.Exists

    let readFromFolder (folderPath : string) =
        let ap = Path.Combine(folderPath,assayFileName).Replace(@"\","/")
        let c,a = AssayFile.Assay.fromFile ap
        c,a

    let readByFileName (arc : string) (assayFileName : string) =
        let ap = Path.Combine([|arc;rootFolderName;assayFileName|]).Replace(@"\","/")
        let c,a = AssayFile.Assay.fromFile ap
        c,a

    let readByName (arc : string) (assayName : string) =
        Path.Combine([|arc;rootFolderName;assayName|]).Replace(@"\","/")
        |> readFromFolder

    let tryReadFromFolder (folderPath : string) =
        try 
            readFromFolder folderPath |> Some
        with | _ -> None

    let tryReadByFileName (arc : string) (assayFileName : string) =
        try 
            readByFileName arc assayFileName |> Some
        with | _ -> None

    let tryReadByName (arc : string) (assayName : string) =
        try 
            readByName arc assayName |> Some
        with | _ -> None

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
                File.Create(Path.Combine(dir.FullName, ".gitkeep")).Close()
            )

            let assayFilePath = Path.Combine([|arc;rootFolderName;assay.FileName.Value|])

            AssayFile.Assay.toFile assayFilePath [] assay


    let initFromName (arc : string) (assayName : string) =

        let assay = Assay.create(FileName = nameToFileName assayName)

        init arc assay

[<AutoOpen>]
module AssayExtensions =
    type Assay with
        static member create (?Id,?Name,?MeasurementType,?TechnologyType,?TechnologyPlatform,?DataFiles,?Materials,?CharacteristicCategories,?UnitCategories,?ProcessSequence,?Comments) : Assay =
            let Filename = 
                Name
                |> Option.map Assay.nameToFileName
            Assay.make Id Filename MeasurementType TechnologyType TechnologyPlatform DataFiles Materials CharacteristicCategories UnitCategories ProcessSequence Comments
