namespace Fabulous.AST.Tests.OpenDirectives

open Fabulous.AST.Tests
open Xunit

open Fabulous.AST

open type Ast

module Open =

    [<Fact>]
    let ``Produces a simple open directive from a string``() =
        Oak() { AnonymousModule() { Open("ABC") } }
        |> produces
            """

open ABC

"""
