namespace Fabulous.AST

open System.Runtime.CompilerServices
open Fabulous.AST.StackAllocatedCollections.StackList
open Fantomas.Core.SyntaxOak
open Fantomas.FCS.Text

module AttributeNode =
    let TypeName = Attributes.defineScalar<string> "TypeName"

    let Value = Attributes.defineScalar<StringOrWidget<Expr>> "Value"

    let Target = Attributes.defineScalar<string> "Target"

    let HasQuotes = Attributes.defineScalar<bool> "HasQuotes"

    let WidgetKey =
        Widgets.register "AttributeNode" (fun widget ->
            let expr = Widgets.tryGetScalarValue widget Value
            let target = Widgets.tryGetScalarValue widget Target

            let hasQuotes =
                Widgets.tryGetScalarValue widget HasQuotes |> ValueOption.defaultValue true

            let expr =
                match expr with
                | ValueNone -> None
                | ValueSome expr ->
                    match expr with
                    | StringOrWidget.StringExpr value ->
                        Expr.Constant(
                            Constant.FromText(
                                SingleTextNode.Create(StringParsing.normalizeIdentifierQuotes(value, hasQuotes))
                            )
                        )
                        |> Some
                    | StringOrWidget.WidgetExpr value -> Some value

            let target =
                match target with
                | ValueNone -> None
                | ValueSome target -> Some(SingleTextNode.Create(target))

            let typeName = Widgets.getScalarValue widget TypeName

            AttributeNode(
                IdentListNode([ IdentifierOrDot.Ident(SingleTextNode.Create typeName) ], Range.Zero),
                expr,
                target,
                Range.Zero
            ))

[<AutoOpen>]
module AttributeNodeBuilders =
    type Ast with

        static member Attribute(value: string) =
            WidgetBuilder<AttributeNode>(
                AttributeNode.WidgetKey,
                AttributesBundle(StackList.one(AttributeNode.TypeName.WithValue(value)), ValueNone, ValueNone)
            )

        static member Attribute(value: string, target: string) =
            WidgetBuilder<AttributeNode>(
                AttributeNode.WidgetKey,
                AttributesBundle(
                    StackList.two(AttributeNode.TypeName.WithValue(value), AttributeNode.Target.WithValue(target)),
                    ValueNone,
                    ValueNone
                )
            )

        static member Attribute(value: string, expr: WidgetBuilder<Expr>) =
            WidgetBuilder<AttributeNode>(
                AttributeNode.WidgetKey,
                AttributesBundle(
                    StackList.two(
                        AttributeNode.TypeName.WithValue(value),
                        AttributeNode.Value.WithValue(StringOrWidget.WidgetExpr(Gen.mkOak expr))
                    ),
                    ValueNone,
                    ValueNone
                )
            )

        static member Attribute(value: string, expr: WidgetBuilder<Expr>, target: string) =
            WidgetBuilder<AttributeNode>(
                AttributeNode.WidgetKey,
                AttributesBundle(
                    StackList.three(
                        AttributeNode.TypeName.WithValue(value),
                        AttributeNode.Value.WithValue(StringOrWidget.WidgetExpr(Gen.mkOak expr)),
                        AttributeNode.Target.WithValue(target)
                    ),
                    ValueNone,
                    ValueNone
                )
            )

[<Extension>]
type AttributeModifiers =
    [<Extension>]
    static member inline hasQuotes(this: WidgetBuilder<AttributeNode>, value: bool) =
        this.AddScalar(AttributeNode.HasQuotes.WithValue(value))
