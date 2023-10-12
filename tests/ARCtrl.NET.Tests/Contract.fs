module Contract.Tests

open ARCtrl.NET.Contract
open ARCtrl.Contract
open Expecto
open System.IO
open FsSpreadsheet
open FsSpreadsheet.ExcelIO
open ARCtrl.ISA.Spreadsheet

let testInputFolder = System.IO.Path.Combine(__SOURCE_DIRECTORY__,@"TestObjects/Contracts")
let testOutputFolder = System.IO.Path.Combine(__SOURCE_DIRECTORY__,@"TestResults/Contracts")

let testRead =

    testList "Read" [
        testCase "TextFile" (fun () -> 
            let fileName = "TestReadMe.txt"
            let contract = Contract.createRead(fileName,DTOType.PlainText)
            let dto = DTO.Text "This is a test"
            let expected = 
                {contract with DTO = Some dto}
            let result = fulfillReadContract testInputFolder contract
            Expect.equal result expected $"Text was not read correctly"
        )
        testCase "XLSXFile" (fun () ->
            let fileName = "TestWorkbook.xlsx"
            let contract = Contract.createRead(fileName,DTOType.ISA_Study)
            let result = fulfillReadContract testInputFolder contract
            let dto = Expect.wantSome result.DTO "DTO was not read correctly"
            Expect.isTrue dto.isSpreadsheet "DTO was not read correctly"
            let wb = dto.AsSpreadsheet() :?> FsSpreadsheet.FsWorkbook
            let ws = Expect.wantSome (wb.TryGetWorksheetByName "TestSheet") "Workbook does not contain worksheet"
            let row1 = Expect.wantSome (ws.TryGetRowValuesAt 1) "Worksheet does not contain row 1"
            let expected = ["1";"2";"3"]
            Expect.sequenceEqual row1 expected "Worksheet does not contain correct values"
            let row2 = Expect.wantSome (ws.TryGetRowValuesAt 2) "Worksheet does not contain row 2"
            let expected = ["A";"B";"C"]
            Expect.sequenceEqual row2 expected "Worksheet does not contain correct values"      
        )
    ]

let testAssayRead =
    let assayIdentifier = "measurement1"
    let tableNames = 
        [
            "Cell Lysis"
            "Protein Extraction"
            "Protein Measurement"
            "Computational Proteome Analysis"
        ]
    testList "AssayRead" [
        
        testCase "FsSpreadsheet" (fun () -> 
            let fileName = "TestAssayFsSpreadsheet.xlsx"
            let contract = Contract.createRead(fileName,DTOType.ISA_Assay)
            let result = fulfillReadContract testInputFolder contract
            let dto = Expect.wantSome result.DTO "DTO was not read correctly"
            let wb = dto.AsSpreadsheet() :?> FsWorkbook
            let assay = ArcAssay.fromFsWorkbook wb
            Expect.equal assay.Identifier assayIdentifier "Assay identifier was not read correctly"
            Expect.sequenceEqual assay.TableNames tableNames "Assay table names were not read correctly"
            Expect.equal assay.Tables.[0].ColumnCount 6 "Assay table column count was not read correctly"
            Expect.equal assay.Tables.[0].RowCount 7 "Assay table row count was not read correctly"
        )
        testCase "Excel" (fun () ->
            let fileName = "TestAssayExcel.xlsx"
            let contract = Contract.createRead(fileName,DTOType.ISA_Assay)
            let result = fulfillReadContract testInputFolder contract
            let dto = Expect.wantSome result.DTO "DTO was not read correctly"
            let wb = dto.AsSpreadsheet() :?> FsWorkbook
            let assay = ArcAssay.fromFsWorkbook wb
            Expect.equal assay.Identifier assayIdentifier "Assay identifier was not read correctly"
            Expect.sequenceEqual assay.TableNames tableNames "Assay table names were not read correctly"
            Expect.equal assay.Tables.[0].ColumnCount 6 "Assay table column count was not read correctly"
            Expect.equal assay.Tables.[0].RowCount 7 "Assay table row count was not read correctly"
        )
        testCase "Libre" (fun () ->
            let fileName = "TestAssayLibre.xlsx"
            let contract = Contract.createRead(fileName,DTOType.ISA_Assay)
            let result = fulfillReadContract testInputFolder contract
            let dto = Expect.wantSome result.DTO "DTO was not read correctly"
            let wb = dto.AsSpreadsheet() :?> FsWorkbook
            let assay = ArcAssay.fromFsWorkbook wb
            Expect.equal assay.Identifier assayIdentifier "Assay identifier was not read correctly"
            Expect.sequenceEqual assay.TableNames tableNames "Assay table names were not read correctly"
            Expect.equal assay.Tables.[0].ColumnCount 6 "Assay table column count was not read correctly"
            Expect.equal assay.Tables.[0].RowCount 7 "Assay table row count was not read correctly"
        )
    ]

let testWrite =

    testList "Write" [
        testCase "TextFileEmpty" (fun () -> 
            let fileName = "TestEmpty.txt"
            let contract = Contract.createCreate(fileName,DTOType.PlainText)

            fulfillWriteContract testOutputFolder contract

            let filePath = Path.Combine(testOutputFolder,fileName)
            Expect.isTrue (System.IO.File.Exists filePath) $"File {filePath} was not created"
            Expect.equal (File.ReadAllText filePath) "" $"File {filePath} was not empty"
        )
        testCase "TextFile" (fun () -> 

            let testText = "This is a test"
            let fileName = "TestReadMe.txt"
            let dto = DTO.Text testText
            let contract = Contract.createCreate(fileName,DTOType.PlainText,dto)

            fulfillWriteContract testOutputFolder contract

            let filePath = Path.Combine(testOutputFolder,fileName)
            Expect.isTrue (System.IO.File.Exists filePath) $"File {filePath} was not created"
            Expect.equal (File.ReadAllText filePath) testText $"File {filePath} was not empty"
        )
        testCase "XLSXFile" (fun () -> 

            let worksheetName = "TestSheet"
            let testWB = new FsWorkbook()
            let testSheet = testWB.InitWorksheet (worksheetName)
            testSheet.Row(1).Item(1).Value <- "A1"
            testSheet.Row(1).Item(2).Value <- "B1"
            testSheet.Row(1).Item(3).Value <- "C1"
            let fileName = "TestWorkbook.xlsx"
            let dto = DTO.Spreadsheet testWB
            let contract = Contract.createCreate(fileName,DTOType.ISA_Assay,dto)

            fulfillWriteContract testOutputFolder contract

            let filePath = Path.Combine(testOutputFolder,fileName)
            
            let wb = FsWorkbook.fromXlsxFile filePath
            let ws = Expect.wantSome (wb.TryGetWorksheetByName worksheetName) "Workbook does not contain worksheet"
            let row1 = Expect.wantSome (ws.TryGetRowValuesAt 1) "Worksheet does not contain row 1"
            let expected = ["A1";"B1";"C1"]
            Expect.sequenceEqual row1 expected "Worksheet does not contain correct values"      
        )
    ]

let testExecute =

    testList "Write" [
        testCase "Implement" (fun () -> 
            Expect.isTrue false "ImplementTest"           
        )
    ]

[<Tests>]
let main = 
    testList "ContractTests" [
        testAssayRead
        testRead
        testWrite
    ]
