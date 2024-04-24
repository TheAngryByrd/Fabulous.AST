namespace Fabulous.AST.Tests.Expressions

open Xunit
open Fabulous.AST.Tests

open Fabulous.AST

open type Ast

module App =

    [<Fact>]
    let ``let value with a App expression``() =
        Oak() { AnonymousModule() { Value("x", AppExpr("printfn", ConstantExpr(DoubleQuoted "a"))) } }
        |> produces
            """

let x = printfn "a"
"""
