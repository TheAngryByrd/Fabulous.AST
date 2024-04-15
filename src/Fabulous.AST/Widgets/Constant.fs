namespace Fabulous.AST

open Fabulous.AST.StackAllocatedCollections.StackList
open Fantomas.Core.SyntaxOak
open Fantomas.FCS.Text

module Constant =
    let Value = Attributes.defineScalar<StringOrWidget<Constant>> "Value"
    let Measure = Attributes.defineWidget "Measure"

    let WidgetKey =
        Widgets.register "ConstantFromText" (fun widget ->
            let value = Widgets.getScalarValue widget Value

            match value with
            | StringOrWidget.StringExpr value ->
                Constant.FromText(SingleTextNode.Create(StringParsing.normalizeIdentifierQuotes(value)))
            | StringOrWidget.WidgetExpr value ->
                let measure = Widgets.getNodeFromWidget<Measure> widget Measure

                Constant.Measure(
                    ConstantMeasureNode(
                        value,
                        UnitOfMeasureNode(SingleTextNode.lessThan, measure, SingleTextNode.greaterThan, Range.Zero),
                        Range.Zero
                    )

                ))

    let WidgetUnitKey =
        Widgets.register "ConstantUnit" (fun _ ->
            Constant.Unit(UnitNode(SingleTextNode.leftParenthesis, SingleTextNode.rightParenthesis, Range.Zero)))

[<AutoOpen>]
module ConstantBuilders =
    type Ast with
        static member Constant(value: StringVariant) =
            WidgetBuilder<Constant>(
                Constant.WidgetKey,
                AttributesBundle(
                    StackList.one(Constant.Value.WithValue(StringOrWidget.StringExpr value)),
                    Array.empty,
                    Array.empty
                )
            )

        static member ConstantMeasure(constant: WidgetBuilder<Constant>, measure: WidgetBuilder<Measure>) =
            WidgetBuilder<Constant>(
                Constant.WidgetKey,
                AttributesBundle(
                    StackList.one(Constant.Value.WithValue(StringOrWidget.WidgetExpr(Gen.mkOak(constant)))),
                    [| Constant.Measure.WithValue(measure.Compile()) |],
                    Array.empty
                )
            )

        static member ConstantUnit() =
            WidgetBuilder<Constant>(
                Constant.WidgetUnitKey,
                AttributesBundle(StackList.empty(), Array.empty, Array.empty)
            )
