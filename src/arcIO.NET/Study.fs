namespace arcIO.NET

open ISADotNet
open ISADotNet.XLSX
open System.IO 


module Study = 

    let rootFolderName = "studies"

    let studyFileName = "isa.study.xlsx"

    let subFolderPaths = 
        ["resources";"protocol"]

    module StudyFolder =
        
        /// Checks if an study folder exists in the ARC.
        let exists (arc : string) (identifier : string) =
            Path.Combine([|arc;rootFolderName;identifier|])
            |> System.IO.Directory.Exists

    let readFromFolder (arc : string) (folderPath : string) =
        let sp = Path.Combine(folderPath,studyFileName).Replace(@"\","/")
        let study = StudyFile.Study.fromFile sp
        match study.Assays with
        | Some assays ->
            let contacts,ps,assays = 
                assays
                |> List.fold (fun (contacts,processSequence,assays) a -> 
                    let c,a = Assay.readByFileName arc a.FileName.Value               
                    contacts @ c, processSequence @ (a.ProcessSequence |> Option.defaultValue []), assays @ [a]
                ) (study.Contacts |> Option.defaultValue [],study.ProcessSequence |> Option.defaultValue [],[])
            let ref = ps |> ProcessSequence.updateByItself
            let updatedAssays =
                assays
                |> List.map (fun a ->
                    {a with ProcessSequence = a.ProcessSequence |> Option.map (ProcessSequence.updateByRef ref)}
                )
            {study with 
                ProcessSequence = study.ProcessSequence |> Option.map (ProcessSequence.updateByRef ref)
                Assays = Some updatedAssays
                Contacts = Option.fromValueWithDefault [] (contacts |> List.distinct)
            }
        | None -> 
            {study with ProcessSequence = study.ProcessSequence |> Option.map ProcessSequence.updateByItself}

    let readByIdentifier (arc : string) (studyIdentifier : string) =
        Path.Combine ([|arc;rootFolderName;studyIdentifier|])
        |> readFromFolder arc

    let writeToFolder (folderPath : string) (study : Study) =
        let sp = Path.Combine (folderPath,studyFileName)
        StudyFile.Study.toFile sp study        

    let write (arc : string) (study : Study) =
        if study.FileName.IsNone then
            failwith "Cannot write study to arc, as it has no filename"
        let sp = Path.Combine ([|arc;rootFolderName;study.FileName.Value|])
        StudyFile.Study.toFile sp study    


    let init (arc : string) (study : Study) =
        
        if study.Identifier.IsNone || study.FileName.IsNone then
            failwith "Given study does not contain identifier or filename"

        let studyIdentifier = study.Identifier.Value

        if StudyFolder.exists arc studyIdentifier then
            printfn $"Study folder with identifier {studyIdentifier} already exists."
        else
            subFolderPaths 
            |> List.iter (fun n ->
                let dp = Path.Combine([|arc;rootFolderName;studyIdentifier;n|])
                let dir = Directory.CreateDirectory(dp)
                File.Create(Path.Combine(dir.FullName, ".gitkeep")).Close()
            )

            let studyFilePath = Path.Combine([|arc;rootFolderName;study.FileName.Value|])

            StudyFile.Study.toFile studyFilePath study


    let initFromName  (arc : string) (studyName : string) =
        
        let studyFileName = Path.Combine(studyName,studyFileName).Replace(@"\","/")

        let study = Study.create(FileName = studyFileName, Identifier = studyName)

        init arc study