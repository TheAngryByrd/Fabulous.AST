namespace Fabulous.AST

open System.Runtime.CompilerServices
open Fantomas.FCS.Text
open Fabulous.AST.StackAllocatedCollections
open Fantomas.Core.SyntaxOak
open Fabulous.AST.StackAllocatedCollections.StackList

module TypeDefnAbbrevNode =

    let Name = Attributes.defineScalar<string> "Name"

    let PowerType = Attributes.defineScalar<StringOrWidget<Type>> "PowerType"

    let XmlDocs = Attributes.defineScalar<string list> "XmlDoc"

    let XmlDocsAbbrev = Attributes.defineScalar<string list> "XmlDoc"

    let WidgetKey =
        Widgets.register "TypeDefnAbbrevNode" (fun widget ->
            let name = Widgets.getScalarValue widget Name
            let lines = Widgets.tryGetScalarValue widget XmlDocs

            let xmlDocs =
                match lines with
                | ValueSome values ->
                    let xmlDocNode = XmlDocNode.Create(values)
                    Some xmlDocNode
                | ValueNone -> None

            TypeNameNode(
                xmlDocs,
                Some(
                    MultipleAttributeListNode(
                        [ AttributeListNode(
                              SingleTextNode.leftAttribute,
                              [ AttributeNode(
                                    IdentListNode([ IdentifierOrDot.Ident(SingleTextNode.measure) ], Range.Zero),
                                    None,
                                    None,
                                    Range.Zero
                                ) ],
                              SingleTextNode.rightAttribute,
                              Range.Zero
                          ) ],
                        Range.Zero
                    )
                ),
                SingleTextNode.``type``,
                Some(SingleTextNode.Create(name)),
                IdentListNode([], Range.Zero),
                None,
                [],
                None,
                None,
                None,
                Range.Zero
            ))

    let WidgetAbbrevKey =
        Widgets.register "TypeDefnAbbrevNode" (fun widget ->
            let name = Widgets.getScalarValue widget Name
            let powerType = Widgets.getScalarValue widget PowerType

            let powerType =
                match powerType with
                | StringOrWidget.StringExpr value ->
                    Type.LongIdent(
                        IdentListNode([ IdentifierOrDot.Ident(SingleTextNode.Create(value.Normalize())) ], Range.Zero)
                    )
                | StringOrWidget.WidgetExpr value -> value

            let lines = Widgets.tryGetScalarValue widget XmlDocsAbbrev

            let xmlDocs =
                match lines with
                | ValueSome values ->
                    let xmlDocNode = XmlDocNode.Create(values)
                    Some xmlDocNode
                | ValueNone -> None

            TypeDefnAbbrevNode(
                TypeNameNode(
                    xmlDocs,
                    Some(
                        MultipleAttributeListNode(
                            [ AttributeListNode(
                                  SingleTextNode.leftAttribute,
                                  [ AttributeNode(
                                        IdentListNode([ IdentifierOrDot.Ident(SingleTextNode.measure) ], Range.Zero),
                                        None,
                                        None,
                                        Range.Zero
                                    ) ],
                                  SingleTextNode.rightAttribute,
                                  Range.Zero
                              ) ],
                            Range.Zero
                        )
                    ),
                    SingleTextNode.``type``,
                    Some(SingleTextNode.Create(name)),
                    IdentListNode([ IdentifierOrDot.Ident(SingleTextNode.equals) ], Range.Zero),
                    None,
                    [],
                    None,
                    None,
                    None,
                    Range.Zero
                ),
                powerType,
                [],
                Range.Zero
            ))

[<AutoOpen>]
module TypeDefnAbbrevNodeBuilders =
    type Ast with

        static member inline Measure(name: string) =
            WidgetBuilder<TypeNameNode>(
                TypeDefnAbbrevNode.WidgetKey,
                AttributesBundle(StackList.one(TypeDefnAbbrevNode.Name.WithValue(name)), Array.empty, Array.empty)
            )

        static member inline Measure(name: string, powerType: WidgetBuilder<Type>) =
            WidgetBuilder<TypeDefnAbbrevNode>(
                TypeDefnAbbrevNode.WidgetAbbrevKey,
                AttributesBundle(
                    StackList.two(
                        TypeDefnAbbrevNode.Name.WithValue(name),
                        TypeDefnAbbrevNode.PowerType.WithValue(StringOrWidget.WidgetExpr(Gen.mkOak powerType))
                    ),
                    Array.empty,
                    Array.empty
                )
            )

        static member inline Measure(name: string, powerType: string) =
            WidgetBuilder<TypeDefnAbbrevNode>(
                TypeDefnAbbrevNode.WidgetAbbrevKey,
                AttributesBundle(
                    StackList.two(
                        TypeDefnAbbrevNode.Name.WithValue(name),
                        TypeDefnAbbrevNode.PowerType.WithValue(StringOrWidget.StringExpr(Unquoted powerType))
                    ),
                    Array.empty,
                    Array.empty
                )
            )

type TypeNameNodeModifiers =
    [<Extension>]
    static member xmlDocs(this: WidgetBuilder<TypeNameNode>, comments: string list) =
        this.AddScalar(TypeDefnAbbrevNode.XmlDocs.WithValue(comments))

type UnitsOfTypeDefnAbbrevNodeAbbrevModifiers =

    [<Extension>]
    static member xmlDocs(this: WidgetBuilder<TypeDefnAbbrevNode>, comments: string list) =
        this.AddScalar(TypeDefnAbbrevNode.XmlDocsAbbrev.WithValue(comments))

type TypeDefnAbbrevNodeYieldExtensions =
    [<Extension>]
    static member inline Yield
        (_: CollectionBuilder<'parent, ModuleDecl>, x: WidgetBuilder<TypeNameNode>)
        : CollectionContent =
        let node = Gen.mkOak x
        let typeDefn = TypeDefn.None(node)
        let typeDefn = ModuleDecl.TypeDefn(typeDefn)
        let widget = Ast.EscapeHatch(typeDefn).Compile()
        { Widgets = MutStackArray1.One(widget) }

    [<Extension>]
    static member inline Yield(_: CollectionBuilder<'parent, ModuleDecl>, x: TypeNameNode) : CollectionContent =
        let moduleDecl = ModuleDecl.TypeDefn(TypeDefn.None(x))
        let widget = Ast.EscapeHatch(moduleDecl).Compile()
        { Widgets = MutStackArray1.One(widget) }
