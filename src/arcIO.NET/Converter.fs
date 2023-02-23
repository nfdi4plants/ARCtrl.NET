namespace arcIO.NET.Converter

open ISADotNet
open ISADotNet.QueryModel
open FsSpreadsheet.DSL
open LitXml


type ARCconverter =
| ARCtoCSV      of (QInvestigation -> QStudy -> QAssay -> SheetEntity<Workbook>)
| ARCtoTSV      of (QInvestigation -> QStudy -> QAssay -> SheetEntity<Workbook>)
| ARCtoXLSX     of (QInvestigation -> QStudy -> QAssay -> SheetEntity<Workbook>)
| ARCtoXML      of (QInvestigation -> QStudy -> QAssay -> LitXml.XmlPart)
//| ARCtoJSON     of QInvestigation -> QStudy -> QAssay -> 