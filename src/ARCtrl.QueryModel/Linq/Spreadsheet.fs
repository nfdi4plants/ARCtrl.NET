﻿namespace ARCtrl.QueryModel.Linq

open System.Collections.Generic
open System.Collections
open Microsoft.FSharp.Linq.RuntimeHelpers
open Microsoft.FSharp.Quotations.Patterns
open FsSpreadsheet.DSL
open ARCtrl.QueryModel
open Errors
open Helpers

/// Should be opened for using ISA-Value querying DSL in combindation with FsSpreadsheet DSL.
module Spreadsheet =

    type CellBuilder() =

        inherit ISAQueryBuilder()

        //let mutable isOptional = false
        //let index = None
        
        /// Returns the value or value collection as FsSpreadsheet Cells. If the expression does not evaluate, return them es Missing and Required.
        [<CustomOperation("required")>] 
        member this.Required (source) =
            this.AddMessage $"as required"
            //isOptional <- false
            RequiredSource(source)

        /// Returns the value or value collection as FsSpreadsheet Cells. If the expression does not evaluate, return them es Missing and Optional.
        [<CustomOperation("optional")>] 
        member this.Optional (source: 'T)  =
            this.AddMessage $"as optional"
            //isOptional <- true
            OptionSource(source)

        //member this.IsOptional = isOptional

    [<AutoOpen>]
    module ValueExtensions =
        type CellBuilder with
            [<CompiledName("RunQueryAsCell")>]
            member this.Run (q: Microsoft.FSharp.Quotations.Expr<'T>) = 
                this.Reset()
                try 
                    let value = eval<'T> q |> FsSpreadsheet.DataType.InferCellValue
                    let cell = CellElement(value,None)
                    SheetEntity.some cell         
                with
                //| err when this.IsOptional -> MissingOptional([this.FormatError err])
                | :? MissingCategoryException           as exc -> NoneRequired([message exc])
                | :? MissingParameterException          as exc -> NoneRequired([message exc])
                | :? MissingCharacteristicException     as exc -> NoneRequired([message exc])
                | :? MissingFactorException             as exc -> NoneRequired([message exc])

                | :? MissingProtocolException           as exc -> NoneRequired([message exc])
                | :? MissingProtocolWithTypeException   as exc -> NoneRequired([message exc])
                | :? ProtocolHasNoDescriptionException  as exc -> NoneRequired([message exc])

                | :? MissingValueException              as exc -> NoneRequired([message exc])
                | :? MissingUnitException               as exc -> NoneRequired([message exc])
                | :? NoSynonymInTargOntologyException   as exc -> NoneRequired([message exc])
                | err -> NoneRequired([message err])            

    [<AutoOpen>]
    module OptionCellExtensions =
        type CellBuilder with
            [<CompiledName("RunQueryAsOptionalCell")>]
            member this.Run (q: Microsoft.FSharp.Quotations.Expr<OptionSource<'T>>) = 
                this.Reset()
                let subExpr = 
                    match q with
                    | Call(exprOpt, methodInfo, [subExpr]) -> Result.Ok subExpr
                    | Call(exprOpt, methodInfo, [ValueWithName(a,b,c);subExpr]) -> Result.Ok subExpr    
                    | x ->                     
                        Result.Error $"could not parse option expression as it was not a call: {x}"
                match subExpr with
                | Result.Ok subExpr -> 
                    try 
                        let value = eval<'T> subExpr |> FsSpreadsheet.DataType.InferCellValue
                        let cell = CellElement(value,None)
                        SheetEntity.some cell 
                    with 
                    | :? MissingCategoryException           as exc -> NoneOptional([message exc])
                    | :? MissingParameterException          as exc -> NoneOptional([message exc])
                    | :? MissingCharacteristicException     as exc -> NoneOptional([message exc])
                    | :? MissingFactorException             as exc -> NoneOptional([message exc])

                    | :? MissingProtocolException           as exc -> NoneOptional([message exc])
                    | :? MissingProtocolWithTypeException   as exc -> NoneOptional([message exc])
                    | :? ProtocolHasNoDescriptionException  as exc -> NoneOptional([message exc])

                    | :? MissingValueException              as exc -> NoneOptional([message exc])
                    | :? MissingUnitException               as exc -> NoneOptional([message exc])
                    | :? NoSynonymInTargOntologyException   as exc -> NoneOptional([message exc]) 
                    | err -> NoneOptional([message err])   
                | Result.Error err -> NoneOptional([message (this.FormatError err)])    
       
    [<AutoOpen>]
    module RequiredCellExtensions =
        type CellBuilder with
            [<CompiledName("RunQueryAsRequiredCell")>]
            member this.Run (q: Microsoft.FSharp.Quotations.Expr<RequiredSource<'T>>) = 
                this.Reset()
                let subExpr = 
                    match q with
                    | Call(exprOpt, methodInfo, [subExpr]) -> Result.Ok subExpr
                    | Call(exprOpt, methodInfo, [ValueWithName(a,b,c);subExpr]) -> Result.Ok subExpr    
                    | x ->                     
                        Result.Error $"could not parse option expression as it was not a call: {x}"
                match subExpr with
                | Result.Ok subExpr -> 
                    try 
                        let value = eval<'T> subExpr |> FsSpreadsheet.DataType.InferCellValue
                        let cell = CellElement(value,None)
                        SheetEntity.some cell 
                    with 
                    | :? MissingCategoryException           as exc -> NoneRequired([message exc])
                    | :? MissingParameterException          as exc -> NoneRequired([message exc])
                    | :? MissingCharacteristicException     as exc -> NoneRequired([message exc])
                    | :? MissingFactorException             as exc -> NoneRequired([message exc])

                    | :? MissingProtocolException           as exc -> NoneRequired([message exc])
                    | :? MissingProtocolWithTypeException   as exc -> NoneRequired([message exc])
                    | :? ProtocolHasNoDescriptionException  as exc -> NoneRequired([message exc])

                    | :? MissingValueException              as exc -> NoneRequired([message exc])
                    | :? MissingUnitException               as exc -> NoneRequired([message exc])
                    | :? NoSynonymInTargOntologyException   as exc -> NoneRequired([message exc])
                    | err -> NoneRequired([message err])   
                | Result.Error err -> NoneRequired([message (this.FormatError err)])


    [<AutoOpen>]
    module EnumerableExtensions =
        type CellBuilder with
            [<CompiledName("RunQueryAsCellCollection")>]
            member this.Run (q: Quotations.Expr<QuerySource<'T, IEnumerable>>) = 
                this.Reset()
                try
                    this.RunQueryAsEnumerable q
                    |> Seq.map (fun v ->                     
                        let value = v |> FsSpreadsheet.DataType.InferCellValue
                        let cell = CellElement(value,None)
                        SheetEntity.some cell)
                    |> Seq.toList
                with
                | :? MissingCategoryException           as exc -> [NoneRequired([message exc])]
                | :? MissingParameterException          as exc -> [NoneRequired([message exc])]
                | :? MissingCharacteristicException     as exc -> [NoneRequired([message exc])]
                | :? MissingFactorException             as exc -> [NoneRequired([message exc])]
                                                                  
                | :? MissingProtocolException           as exc -> [NoneRequired([message exc])]
                | :? MissingProtocolWithTypeException   as exc -> [NoneRequired([message exc])]
                | :? ProtocolHasNoDescriptionException  as exc -> [NoneRequired([message exc])]
                                                                  
                | :? MissingValueException              as exc -> [NoneRequired([message exc])]
                | :? MissingUnitException               as exc -> [NoneRequired([message exc])]
                | :? NoSynonymInTargOntologyException   as exc -> [NoneRequired([message exc])]
                | err -> [NoneRequired([message err])]  

    [<AutoOpen>]
    module OptionEnumerableExtensions =
        type CellBuilder with
            [<CompiledName("RunQueryAsOptionalCollection")>]
            member this.Run (q: Quotations.Expr<OptionSource<QuerySource<'T, IEnumerable>>>) = 
                this.Reset()
                let subExpr = 
                    match q with
                    | Call(exprOpt, methodInfo, [subExpr]) -> Result.Ok subExpr
                    | Call(exprOpt, methodInfo, [ValueWithName(a,b,c);subExpr]) -> Result.Ok subExpr    
                    | x ->                     
                        Result.Error $"could not parse option expression as it was not a call: {x}"
                match subExpr with
                | Result.Ok subExpr -> 
                    try 
                        (LeafExpressionConverter.EvaluateQuotation subExpr :?> QuerySource<'T, IEnumerable>).Source
                        |> Seq.map (fun v ->                     
                            let value = v |> FsSpreadsheet.DataType.InferCellValue
                            let cell = CellElement(value,None)
                            SheetEntity.some cell) 
                        |> Seq.toList
                    with 
                    | :? MissingCategoryException           as exc -> [NoneOptional([message exc])]
                    | :? MissingParameterException          as exc -> [NoneOptional([message exc])]
                    | :? MissingCharacteristicException     as exc -> [NoneOptional([message exc])]
                    | :? MissingFactorException             as exc -> [NoneOptional([message exc])]
                                                                      
                    | :? MissingProtocolException           as exc -> [NoneOptional([message exc])]
                    | :? MissingProtocolWithTypeException   as exc -> [NoneOptional([message exc])]
                    | :? ProtocolHasNoDescriptionException  as exc -> [NoneOptional([message exc])]
                                                                      
                    | :? MissingValueException              as exc -> [NoneOptional([message exc])]
                    | :? MissingUnitException               as exc -> [NoneOptional([message exc])]
                    | :? NoSynonymInTargOntologyException   as exc -> [NoneOptional([message exc])]
                    | err -> [NoneOptional([message err])]   
                | Result.Error err -> [NoneOptional([message (this.FormatError err)])]    


    [<AutoOpen>]
    module RequiredEnumerableExtensions =
        type CellBuilder with
            [<CompiledName("RunQueryAsRequiredCollection")>]
            member this.Run (q: Quotations.Expr<RequiredSource<QuerySource<'T, IEnumerable>>>) = 
                this.Reset()
                let subExpr = 
                    match q with
                    | Call(exprOpt, methodInfo, [subExpr]) -> Result.Ok subExpr
                    | Call(exprOpt, methodInfo, [ValueWithName(a,b,c);subExpr]) -> Result.Ok subExpr    
                    | x ->                     
                        Result.Error $"could not parse option expression as it was not a call: {x}"
                match subExpr with
                | Result.Ok subExpr -> 
                    try 
                        (LeafExpressionConverter.EvaluateQuotation subExpr :?> QuerySource<'T, IEnumerable>).Source
                        |> Seq.map (fun v ->                     
                            let value = v |> FsSpreadsheet.DataType.InferCellValue
                            let cell = CellElement(value,None)
                            SheetEntity.some cell)        
                        |> Seq.toList
                    with 
                    | :? MissingCategoryException           as exc -> [NoneRequired([message exc])]
                    | :? MissingParameterException          as exc -> [NoneRequired([message exc])]
                    | :? MissingCharacteristicException     as exc -> [NoneRequired([message exc])]
                    | :? MissingFactorException             as exc -> [NoneRequired([message exc])]
                                                                      
                    | :? MissingProtocolException           as exc -> [NoneRequired([message exc])]
                    | :? MissingProtocolWithTypeException   as exc -> [NoneRequired([message exc])]
                    | :? ProtocolHasNoDescriptionException  as exc -> [NoneRequired([message exc])]
                                                                      
                    | :? MissingValueException              as exc -> [NoneRequired([message exc])]
                    | :? MissingUnitException               as exc -> [NoneRequired([message exc])]
                    | :? NoSynonymInTargOntologyException   as exc -> [NoneRequired([message exc])]
                    | err -> [NoneRequired([message err])]   
                | Result.Error err -> [NoneRequired([message (this.FormatError err)])]   

    /// Computation expression for querying ISA values for consumption of FsSpreadsheet DSL.
    let cells = CellBuilder()
