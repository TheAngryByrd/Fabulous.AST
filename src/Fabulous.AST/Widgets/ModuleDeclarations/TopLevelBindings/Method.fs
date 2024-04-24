namespace Fabulous.AST

open Fabulous.AST.StackAllocatedCollections.StackList
open Fantomas.FCS.Text
open Fantomas.Core.SyntaxOak

open type Fabulous.AST.Ast

module BindingMethodNode =
    let WidgetKey =
        Widgets.register "MethodMember" (fun widget ->
            let name = Widgets.getScalarValue widget BindingNode.Name

            let name =
                match name with
                | StringOrWidget.StringExpr name ->
                    Some(SingleTextNode.Create(StringParsing.normalizeIdentifierQuotes name))
                | StringOrWidget.WidgetExpr _ -> None

            let parameters = Widgets.getScalarValue widget BindingNode.Parameters
            let bodyExpr = Widgets.getScalarValue widget BindingNode.BodyExpr

            let bodyExpr =
                match bodyExpr with
                | StringOrWidget.StringExpr value ->
                    Expr.Constant(
                        Constant.FromText(SingleTextNode.Create(StringParsing.normalizeIdentifierQuotes(value)))
                    )
                | StringOrWidget.WidgetExpr value -> value

            let isInlined = Widgets.tryGetScalarValue widget BindingNode.IsInlined

            let isStatic =
                Widgets.tryGetScalarValue widget BindingNode.IsStatic
                |> ValueOption.defaultValue false

            let returnType = Widgets.tryGetScalarValue widget BindingNode.Return

            let accessControl =
                Widgets.tryGetScalarValue widget BindingNode.Accessibility
                |> ValueOption.defaultValue AccessControl.Unknown

            let accessControl =
                match accessControl with
                | Public -> Some(SingleTextNode.``public``)
                | Private -> Some(SingleTextNode.``private``)
                | Internal -> Some(SingleTextNode.``internal``)
                | Unknown -> None

            let lines = Widgets.tryGetScalarValue widget BindingNode.XmlDocs

            let xmlDocs =
                match lines with
                | ValueSome values ->
                    let xmlDocNode = XmlDocNode.Create(values)
                    Some xmlDocNode
                | ValueNone -> None

            let returnType =
                match returnType with
                | ValueNone -> None
                | ValueSome value ->
                    match value with
                    | StringOrWidget.StringExpr value ->
                        let value = StringParsing.normalizeIdentifierBackticks value

                        let returnType =
                            Type.LongIdent(
                                IdentListNode([ IdentifierOrDot.Ident(SingleTextNode.Create(value)) ], Range.Zero)
                            )

                        Some(BindingReturnInfoNode(SingleTextNode.colon, returnType, Range.Zero))
                    | StringOrWidget.WidgetExpr returnType ->
                        Some(BindingReturnInfoNode(SingleTextNode.colon, returnType, Range.Zero))

            let attributes = Widgets.tryGetScalarValue widget BindingNode.MultipleAttributes

            let multipleAttributes =
                match attributes with
                | ValueSome values ->
                    Some(
                        MultipleAttributeListNode(
                            [ AttributeListNode(
                                  SingleTextNode.leftAttribute,
                                  values,
                                  SingleTextNode.rightAttribute,
                                  Range.Zero
                              ) ],
                            Range.Zero
                        )
                    )
                | ValueNone -> None

            let inlineNode =
                match isInlined with
                | ValueSome true -> Some(SingleTextNode.``inline``)
                | ValueSome false -> None
                | ValueNone -> None

            let typeParams = Widgets.tryGetScalarValue widget BindingNode.TypeParams

            let typeParams =
                match typeParams with
                | ValueSome values ->
                    TyparDeclsPostfixListNode(
                        SingleTextNode.lessThan,
                        [ for v in values do
                              TyparDeclNode(None, SingleTextNode.Create v, [], Range.Zero) ],
                        [],
                        SingleTextNode.greaterThan,
                        Range.Zero
                    )
                    |> TyparDecls.PostfixList
                    |> Some
                | ValueNone -> None

            let multipleTextsNode =
                [ if isStatic then
                      SingleTextNode.``static``
                      SingleTextNode.``member``
                  else
                      SingleTextNode.``member`` ]

            BindingNode(
                xmlDocs,
                multipleAttributes,
                MultipleTextsNode(multipleTextsNode, Range.Zero),
                false,
                inlineNode,
                accessControl,
                Choice1Of2(
                    IdentListNode(
                        [ match name with
                          | None -> ()
                          | Some value -> IdentifierOrDot.Ident(value) ],
                        Range.Zero
                    )
                ),
                typeParams,
                parameters,
                returnType,
                SingleTextNode.equals,
                bodyExpr,
                Range.Zero
            ))

[<AutoOpen>]
module BindingMethodBuilders =
    type Ast with

        static member Method(name: string, parameters: WidgetBuilder<Pattern>, body: StringVariant) =
            WidgetBuilder<BindingNode>(
                BindingMethodNode.WidgetKey,
                AttributesBundle(
                    StackList.three(
                        BindingNode.Name.WithValue(StringOrWidget.StringExpr(Unquoted(name))),
                        BindingNode.BodyExpr.WithValue(StringOrWidget.StringExpr(body)),
                        BindingNode.Parameters.WithValue([ Gen.mkOak parameters ])
                    ),
                    [||],
                    Array.empty
                )
            )

        static member Method(name: string, parameters: WidgetBuilder<Pattern> list, body: StringVariant) =
            let parameters = parameters |> List.map(Gen.mkOak)

            WidgetBuilder<BindingNode>(
                BindingMethodNode.WidgetKey,
                AttributesBundle(
                    StackList.three(
                        BindingNode.Name.WithValue(StringOrWidget.StringExpr(Unquoted(name))),
                        BindingNode.BodyExpr.WithValue(StringOrWidget.StringExpr(body)),
                        BindingNode.Parameters.WithValue(parameters)
                    ),
                    [||],
                    Array.empty
                )
            )

        static member Method(name: string, parameters: WidgetBuilder<Pattern> list, body: WidgetBuilder<Expr>) =
            let parameters = parameters |> List.map(Gen.mkOak)

            WidgetBuilder<BindingNode>(
                BindingMethodNode.WidgetKey,
                AttributesBundle(
                    StackList.three(
                        BindingNode.Name.WithValue(StringOrWidget.StringExpr(Unquoted(name))),
                        BindingNode.BodyExpr.WithValue(StringOrWidget.WidgetExpr(Gen.mkOak body)),
                        BindingNode.Parameters.WithValue(parameters)
                    ),
                    Array.empty,
                    Array.empty
                )
            )

        static member Method(name: string, parameters: WidgetBuilder<Pattern>, body: WidgetBuilder<Expr>) =
            WidgetBuilder<BindingNode>(
                BindingMethodNode.WidgetKey,
                AttributesBundle(
                    StackList.three(
                        BindingNode.Name.WithValue(StringOrWidget.StringExpr(Unquoted(name))),
                        BindingNode.BodyExpr.WithValue(StringOrWidget.WidgetExpr(Gen.mkOak body)),
                        BindingNode.Parameters.WithValue([ Gen.mkOak parameters ])
                    ),
                    Array.empty,
                    Array.empty
                )
            )
