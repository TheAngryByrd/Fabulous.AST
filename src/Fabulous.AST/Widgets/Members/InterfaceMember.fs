namespace Fabulous.AST

open System.Runtime.CompilerServices
open Fabulous.AST.StackAllocatedCollections
open Fabulous.AST.StackAllocatedCollections.StackList
open Fantomas.Core.SyntaxOak
open Fantomas.FCS.Text

type InterfaceMemberNode(``type``: Type, members: MemberDefn list) =
    inherit
        MemberDefnInterfaceNode(
            SingleTextNode.``interface``,
            ``type``,
            Some(SingleTextNode.``with``),
            members,
            Range.Zero
        )

module InterfaceMember =
    let Type = Attributes.defineScalar<Type> "Type"

    let Members = Attributes.defineWidgetCollection "Members"

    let WidgetKey =
        Widgets.register "InterfaceMember" (fun widget ->
            let ``type`` = Helpers.getScalarValue widget Type
            let members = Helpers.tryGetNodesFromWidgetCollection<MemberDefn> widget Members

            let members =
                match members with
                | Some members -> members
                | None -> []

            InterfaceMemberNode(``type``, members))

[<AutoOpen>]
module InterfaceMemberBuilders =
    type Ast with

        static member InterfaceMember(``type``: Type) =
            CollectionBuilder<InterfaceMemberNode, MemberDefn>(
                InterfaceMember.WidgetKey,
                InterfaceMember.Members,
                AttributesBundle(StackList.one(InterfaceMember.Type.WithValue(``type``)), ValueNone, ValueNone)
            )

[<Extension>]
type InterfaceMemberYieldExtensions =
    [<Extension>]
    static member inline Yield
        (
            _: CollectionBuilder<InterfaceMemberNode, MemberDefn>,
            x: MethodMemberNode
        ) : CollectionContent =
        let widget = Ast.EscapeHatch(MemberDefn.Member(x)).Compile()
        { Widgets = MutStackArray1.One(widget) }

    [<Extension>]
    static member inline Yield
        (
            this: CollectionBuilder<InterfaceMemberNode, MemberDefn>,
            x: WidgetBuilder<MethodMemberNode>
        ) : CollectionContent =
        let node = Tree.compile x
        InterfaceMemberYieldExtensions.Yield(this, node)

    [<Extension>]
    static member inline Yield
        (
            _: CollectionBuilder<InterfaceMemberNode, MemberDefn>,
            x: PropertyMemberNode
        ) : CollectionContent =
        let widget = Ast.EscapeHatch(MemberDefn.Member(x)).Compile()
        { Widgets = MutStackArray1.One(widget) }

    [<Extension>]
    static member inline Yield
        (
            this: CollectionBuilder<InterfaceMemberNode, MemberDefn>,
            x: WidgetBuilder<PropertyMemberNode>
        ) : CollectionContent =
        let node = Tree.compile x
        InterfaceMemberYieldExtensions.Yield(this, node)