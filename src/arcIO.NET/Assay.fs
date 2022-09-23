namespace arcIO.Net

open ISADotNet.XLSX
open System.IO 

module Assay = 

    let assayFileName = "isa.assay.xlsx"

    let readFromFolder (folderPath : string) =
        let ap = Path.Combine (folderPath,assayFileName)
        let _,_,c,a = AssayFile.Assay.fromFile ap
        c,a

    let readByFileName (arc : string) (assayFileName : string) =
        let ap = Path.Combine ([|arc;"assays";assayFileName|])
        let _,_,c,a = AssayFile.Assay.fromFile ap
        c,a

    let readByName (arc : string) (assayName : string) =
        Path.Combine ([|arc;"assays";assayName|])
        |> readFromFolder