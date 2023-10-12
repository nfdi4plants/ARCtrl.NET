module ARCtrl.NET.Contract

open ARCtrl.Contract
open FsSpreadsheet
open FsSpreadsheet.ExcelIO

open System.IO
open DocumentFormat.OpenXml

module Package =

    let tryGetApplication (package : Packaging.Package) =
        let uri = new System.Uri("/docProps/app.xml", System.UriKind.Relative);
        if package.PartExists(uri) then
            let part = package.GetPart(uri)
            use stream = part.GetStream()
            use reader = System.Xml.XmlReader.Create(stream)
            let ns = System.Xml.Linq.XNamespace.Get("http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")
            let root = System.Xml.Linq.XElement.Load(reader)
            let app = root.Element(ns + "Application")
            if app <> null then
                Some app.Value
            else None
        else 
            None

    let fixLibrePackage (package : Packaging.Package) =

        let uri = new System.Uri("/xl/webextensions/taskpanes.xml", System.UriKind.Relative);

        package.DeletePart(uri)
        package.CreatePart(uri,contentType = "application/vnd.ms-office.webextensiontaskpanes+xml")
        |> ignore


    let isLibrePackage (package : Packaging.Package) =
        match tryGetApplication package with
        | Some app -> app.Contains "LibreOffice"
        | None -> false
     

type FsWorkbook with
        
    /// <summary>
    /// Creates an FsWorkbook from a given SpreadsheetDocument.
    /// </summary>
    static member fromSpreadsheetDocument (doc : Packaging.SpreadsheetDocument) =
        let sst = Spreadsheet.tryGetSharedStringTable doc
        let xlsxWorkbookPart = Spreadsheet.getWorkbookPart doc        
        let xlsxSheets = 
            try
                let xlsxWorkbook = Workbook.get xlsxWorkbookPart
                Sheet.Sheets.get xlsxWorkbook
                |> Sheet.Sheets.getSheets
            with 
            | _ -> []
        let xlsxWorksheetParts = 
            xlsxSheets
            |> Seq.map (
                fun s -> 
                    let sid = Sheet.getID s
                    sid, Worksheet.WorksheetPart.getByID sid xlsxWorkbookPart
            )
        let xlsxTables = 
            xlsxWorksheetParts 
            |> Seq.map (fun (sid, wsp) -> sid, Worksheet.WorksheetPart.getTables wsp)

        let sheets =
            xlsxSheets
            |> Seq.map (
                fun xlsxSheet ->
                    let sheetIndex = Sheet.getSheetIndex xlsxSheet
                    let sheetId = Sheet.getID xlsxSheet
                    let xlsxCells = 
                        Spreadsheet.getCellsBySheetID sheetId doc
                        |> Seq.map (FsCell.ofXlsxCell sst)
                    let assocXlsxTables = 
                        xlsxTables 
                        |> Seq.tryPick (fun (sid,ts) -> if sid = sheetId then Some ts else None)
                    let fsTables =
                        match assocXlsxTables with
                        | Some ts -> ts |> Seq.map FsTable.fromXlsxTable |> List.ofSeq
                        | None -> []
                    let fsWorksheet = FsWorksheet(xlsxSheet.Name)
                    fsWorksheet
                    |> FsWorksheet.addCells xlsxCells
                    |> FsWorksheet.addTables fsTables
            )

        sheets
        |> Seq.fold (
            fun wb sheet -> 
                sheet.RescanRows()      // we need this to have all FsRows present in the final FsWorksheet
                FsWorkbook.addWorksheet sheet wb
        ) (new FsWorkbook())

    /// <summary>
    /// Creates an FsWorkbook from a given Packaging.Package xlsx package.
    /// </summary>
    static member fromPackage(package:Packaging.Package) =
        if Package.isLibrePackage package then
            Package.fixLibrePackage package
        let doc = Packaging.SpreadsheetDocument.Open(package)
        FsWorkbook.fromSpreadsheetDocument doc

    static member fromXlsxFile(path:string) =
        use package = Packaging.Package.Open(path)


        FsWorkbook.fromPackage package

let fulfillReadContract basePath (c : Contract) =
    let log = Logging.createLogger("ReadContractHandler")
    match c.DTOType with
    | Some DTOType.ISA_Assay 
    | Some DTOType.ISA_Investigation 
    | Some DTOType.ISA_Study ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        let wb = FsWorkbook.fromXlsxFile path |> box |> DTO.Spreadsheet
        {c with DTO = Some wb}
    | Some DTOType.PlainText ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        let text = System.IO.File.ReadAllText(path) |> DTO.Text
        {c with DTO = Some text}
    | _ -> 
        log.Info(sprintf "Contract %s is not an ISA contract" c.Path) 
        c

let fulfillWriteContract basePath (c : Contract) =
    let log = Logging.createLogger("WriteContractHandler")
    match c.DTO with
    | Some (DTO.Spreadsheet wb) ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        Path.ensureDirectory path
        FsWorkbook.toFile path (wb :?> FsWorkbook)
    | Some (DTO.Text t) ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        Path.ensureDirectory path
        System.IO.File.WriteAllText(path,t)
    | None -> 
        let path = System.IO.Path.Combine(basePath, c.Path)
        Path.ensureDirectory path
        System.IO.File.Create(path).Close()
    | _ -> 
        log.Info(sprintf "Contract %s is not an ISA contract" c.Path)

//let fulfillExecuteContract basePath (c : Contract) =
//    let log = Logging.createLogger("ExecuteContractHandler")
//    match c.DTO with
//    | Some (DTO.CLITool tool) ->
//        let path = System.IO.Path.Combine(basePath, c.Path)
//        Path.ensureDirectory path
//        FsWorkbook.toFile path (wb :?> FsWorkbook)
//    | _ -> log.Info(sprintf "Contract %O is not an Execute contract" c)