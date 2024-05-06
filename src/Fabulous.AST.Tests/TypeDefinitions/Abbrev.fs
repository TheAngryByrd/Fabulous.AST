namespace Fabulous.AST.Tests.TypeDefinitions

open Fabulous.AST.Tests
open Fantomas.FCS.Text
open Fantomas.Core.SyntaxOak
open Xunit

open Fabulous.AST

open type Fabulous.AST.Ast

module Abbrev =

    [<Fact>]
    let ``Produces type Abbrev``() =
        Oak() { AnonymousModule() { Abbrev("MyInt", Int()) } }

        |> produces
            """

type MyInt = int

"""

    [<Fact>]
    let ``Produces type Abbrev using an escape hatch``() =
        let alias =
            TypeDefnAbbrevNode(
                TypeNameNode(
                    None,
                    None,
                    SingleTextNode("type", Range.Zero),
                    Some(SingleTextNode("MyFloat", Range.Zero)),
                    IdentListNode([ IdentifierOrDot.Ident(SingleTextNode("=", Range.Zero)) ], Range.Zero),
                    None,
                    [],
                    None,
                    None,
                    None,
                    Range.Zero
                ),
                Type.LongIdent(IdentListNode([ IdentifierOrDot.Ident(SingleTextNode.Create("float")) ], Range.Zero)),
                [],
                Range.Zero
            )

        Oak() {
            AnonymousModule() {
                Abbrev("MyInt", Int())
                EscapeHatch(alias)
            }
        }

        |> produces
            """

type MyInt = int
type MyFloat = float

"""

    [<Fact>]
    let ``Produces type Abbrev with TypeDefnAbbrevNode``() =
        Oak() {
            AnonymousModule() {
                Abbrev("MyInt", Int())

                Abbrev("MyString", LongIdent "string")

                TypeDefnAbbrevNode(
                    TypeNameNode(
                        None,
                        None,
                        SingleTextNode("type", Range.Zero),
                        Some(SingleTextNode("MyFloat", Range.Zero)),
                        IdentListNode([ IdentifierOrDot.Ident(SingleTextNode("=", Range.Zero)) ], Range.Zero),
                        None,
                        [],
                        None,
                        None,
                        None,
                        Range.Zero
                    ),
                    Type.LongIdent(IdentListNode([ IdentifierOrDot.Ident(SingleTextNode.Create("float")) ], Range.Zero)),
                    [],
                    Range.Zero
                )
            }
        }

        |> produces
            """

type MyInt = int
type MyString = string
type MyFloat = float

"""
