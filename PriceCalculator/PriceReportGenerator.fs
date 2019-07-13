namespace PriceCalculator

open System
open PriceCalculation
open Model

module PriceReportGenerator = 
    let private filterSome = 
        Seq.choose id

    let private someWhenGreaterThan0 (amount: Money) someValue = 
        if amount > amount.ZeroOfSameCurrency then Some someValue else None

    let generatePriceReport price = 
        [
            [
                sprintf "Tax amount = %s" (price.TaxAmount.ToString()) |> someWhenGreaterThan0 price.TaxAmount
                sprintf "Discount amount = %s" (price.DiscountAmount.ToString()) |> someWhenGreaterThan0 price.DiscountAmount
            ]
            price.Expenses
                |> Seq.map (fun expense -> sprintf "%s = %s" expense.Name (expense.Amount.ToString()) |> Some)
                |> Seq.toList
            [
                sprintf "Price before = %s, price after = %s" (price.BaseAmount.ToString()) (price.FinalAmount.ToString()) |> Some
            ]
        ]
        |> Seq.concat
        |> filterSome
        |> String.concat Environment.NewLine