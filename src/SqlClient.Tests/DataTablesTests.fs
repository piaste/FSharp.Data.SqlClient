﻿module FSharp.Data.DataTablesTests

open System
open System.Configuration
open System.Transactions
open System.Data.SqlClient
open System.Data
open FSharp.Data
open Xunit
open FsUnit.Xunit

type Settings = FSharp.Configuration.AppSettings<"app.config">

type AdventureWorks = SqlProgrammabilityProvider<"name=AdventureWorks2012">

//Tables types structured as: [TypeAlias].[Namespace].Tables.[TableName]
type ShiftTable = AdventureWorks.HumanResources.Tables.Shift

[<Fact>]
let NewRowAndBulkCopy() = 
    let t = new ShiftTable()
    use conn = new SqlConnection(connectionString = Settings.ConnectionStrings.AdventureWorks2012)
    conn.Open()
    use tran = conn.BeginTransaction()
    
    let rows: DataRow[] = 
        [|
            //erased method to provide static typing
            t.NewRow("French coffee break", StartTime = TimeSpan.FromHours 10., EndTime = TimeSpan.FromHours 12., ModifiedDate = DateTime.Now.Date)
            t.NewRow("Spanish siesta", TimeSpan.FromHours 13., TimeSpan.FromHours 16., DateTime.Now.Date)
        |]
    let bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran)
    let rowsCopied = ref 0L
    bulkCopy.NotifyAfter <- rows.Length
    bulkCopy.SqlRowsCopied.Add(fun args -> rowsCopied := args.RowsCopied)
    //table name is there
    bulkCopy.DestinationTableName <- t.TableName
    bulkCopy.WriteToServer(rows)

    Assert.Equal(int64 rows.Length, !rowsCopied)

[<Fact>]
let AddRowAndBulkCopy() = 
    let t = new ShiftTable()
    use conn = new SqlConnection(connectionString = Settings.ConnectionStrings.AdventureWorks2012)
    conn.Open()
    use tran = conn.BeginTransaction()
    
    //erased method to provide static typing
    t.AddRow("French coffee break", StartTime = TimeSpan.FromHours 10., EndTime = TimeSpan.FromHours 12., ModifiedDate = DateTime.Now.Date)
    t.AddRow("Spanish siesta", TimeSpan.FromHours 13., TimeSpan.FromHours 16., DateTime.Now.Date)

    let bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran)
    let rowsCopied = ref 0L
    bulkCopy.NotifyAfter <- t.Rows.Count
    bulkCopy.SqlRowsCopied.Add(fun args -> rowsCopied := args.RowsCopied)
    bulkCopy.DestinationTableName <- t.TableName
    bulkCopy.WriteToServer(t)

    Assert.Equal(int64 t.Rows.Count, !rowsCopied)










