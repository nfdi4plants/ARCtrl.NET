module ARCtrl.Contract

open ARCtrl.Contract
open Path
open FsSpreadsheet
open FsSpreadsheet.ExcelIO

let fulfillReadContract basePath (c : Contract) =
    let log = Logging.createLogger("ReadContractHandler")
    match c.DTOType with
    | Some DTOType.ISA_Assay 
    | Some DTOType.ISA_Investigation 
    | Some DTOType.ISA_Study ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        let wb = FsWorkbook.fromXlsxFile path |> box |> DTO.Spreadsheet
        {c with DTO = Some wb}
    | _ -> 
        log.Info(sprintf "Contract %s is not an ISA contract" c.Path) 
        c

let fulfillWriteContract basePath (c : Contract) =
    let log = Logging.createLogger("WriteContractHandler")
    match c.DTO with
    | Some (DTO.Spreadsheet wb) ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        ensureDirectory path
        FsWorkbook.toFile path (wb :?> FsWorkbook)
    | Some (DTO.Text t) ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        ensureDirectory path
        System.IO.File.WriteAllText(path,t)
    | None -> 
        let path = System.IO.Path.Combine(basePath, c.Path)
        ensureDirectory path
        System.IO.File.Create(path).Close()
    | _ -> 
        log.Info(sprintf "Contract %s is not an ISA contract" c.Path)