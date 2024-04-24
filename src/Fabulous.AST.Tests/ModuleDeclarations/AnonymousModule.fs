namespace Fabulous.AST.Tests.ModuleDeclarations

open Fabulous.AST.Tests
open Fantomas.Core.SyntaxOak
open Xunit

open Fabulous.AST

open type Ast

module AnonymousModule =
    [<Fact>]
    let ``Produces a simple hello world console app``() =
        Oak() { AnonymousModule() { AppExpr("printfn", ConstantExpr(DoubleQuoted("hello, world"))) } }
        |> produces
            """

printfn "hello, world"

"""

    [<Fact>]
    let ``Produces Hello world with a let binding``() =
        Oak() {
            AnonymousModule() {
                Value("x", DoubleQuoted "hello, world")

                AppExpr("printfn", [ ConstantExpr(DoubleQuoted "%s"); ConstantExpr(Unquoted "x") ])

            }
        }

        |> produces
            """

let x = "hello, world"
printfn "%s" x

"""

    [<Fact>]
    let ``Produces several Call nodes``() =
        Oak() {
            AnonymousModule() {
                for i = 0 to 2 do
                    AppExpr("printfn", [ ConstantExpr(DoubleQuoted "%s"); ConstantExpr(Unquoted $"{i}") ])
            }
        }

        |> produces
            """

printfn "%s" 0
printfn "%s" 1
printfn "%s" 2

"""
