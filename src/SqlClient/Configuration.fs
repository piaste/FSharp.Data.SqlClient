namespace FSharp.Data.SqlClient.Internals

//this is mess. Clean up later.
type FsharpDataSqlClientConfiguration = {
    ResultsetRuntimeVerification: bool
}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<AutoOpen>]
module Configuration = 
    let internal guard = obj()
    let mutable internal current = { FsharpDataSqlClientConfiguration.ResultsetRuntimeVerification = false }

    type FsharpDataSqlClientConfiguration with
        static member Current 
            with get() = lock guard <| fun() -> current
            and set value = lock guard <| fun() -> current <- value