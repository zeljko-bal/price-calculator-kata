namespace PriceCalculator

open FSharp.Data
open PriceDefinition
open FSharp.Data.JsonExtensions
open Model

module Configuration = 

    let private throwUnsupportedType entityName unsupportedType = 
        invalidOp (sprintf "Unsupported %s type: %s." entityName unsupportedType)

    let private configureUsing (configFuncs: (JsonValue -> PriceDefinition -> PriceDefinition) seq) (configuration: JsonValue) (priceDefinition: PriceDefinition) = 
        configFuncs
        |> Seq.fold (fun currentDefinition configFunc -> configFunc configuration currentDefinition) priceDefinition

    let private whenDefined (propertyName: string) (configFunc: JsonValue -> PriceDefinition -> PriceDefinition) (configuration: JsonValue) (priceDefinition: PriceDefinition) = 
        match configuration.TryGetProperty(propertyName) with
        | Some jsonValue -> configFunc jsonValue priceDefinition
        | None -> priceDefinition

    let private parseMoney configuration = 
        Money.Of (configuration?Amount.AsDecimal()) (configuration?Currency.AsString())

    let private configurePrecision = 
        whenDefined "Precision" (fun precision -> withPrecision (precision.AsInteger()))

    let private configureTaxRate = 
        whenDefined "TaxRate" (fun taxRate -> withTax (taxRate.AsInteger()))

    let rec private parseDiscount configuration = 
        match configuration?Type.AsString() with
        | discountType when discountType = "UniversalDiscount" -> UniversalDiscount { Rate = configuration?Rate.AsInteger() }
        | discountType when discountType = "UPCDiscount" -> UPCDiscount { Rate = configuration?Rate.AsInteger(); UPC = configuration?UPC.AsInteger() }
        | discountType when discountType = "AdditiveDiscounts" -> AdditiveDiscounts (configuration?Discounts.AsArray() |> Seq.map parseDiscount)
        | discountType when discountType = "MultiplicativeDiscounts" -> AdditiveDiscounts (configuration?Discounts.AsArray() |> Seq.map parseDiscount)
        | unsupportedType -> throwUnsupportedType "discount" unsupportedType
    
    let private parseDiscountCap configuration = 
        match configuration?Type.AsString() with
        | capType when capType = "Percentage" -> Percentage (configuration?Percentage.AsInteger())
        | capType when capType = "Absolute" -> Absolute (parseMoney configuration?Amount)
        | unsupportedType -> throwUnsupportedType "discount cap" unsupportedType

    let private configureDiscounts = 
        whenDefined 
            "Discounts"
            (configureUsing [
                whenDefined "BeforeTax" (fun discount -> withDiscountsBeforeTax (parseDiscount discount))
                whenDefined "AfterTax" (fun discount -> withDiscountsAfterTax (parseDiscount discount))
                whenDefined "Cap" (fun cap -> withDisountCap (parseDiscountCap cap))
            ])

    let private parseExpense configuration = 
        match configuration?Type.AsString() with
        | expenseType when expenseType = "AbsoluteExpense" -> AbsoluteExpense { Name = configuration?Name.AsString(); Amount = parseMoney configuration?Amount }
        | expenseType when expenseType = "PercentageExpense" -> PercentageExpense { Name = configuration?Name.AsString(); Percentage = configuration?Percentage.AsInteger(); }
        | unsupportedType -> throwUnsupportedType "expense" unsupportedType

    let private configureExpenses = 
        whenDefined "Expenses" (fun expenses -> withExpenses (expenses.AsArray() |> Seq.map parseExpense))

    let fromJson jsonConfiguration = 
        let configuration = JsonValue.Parse(jsonConfiguration)
        (configuration, definePrice) ||> configureUsing [
            configurePrecision
            configureTaxRate
            configureDiscounts
            configureExpenses
        ]
