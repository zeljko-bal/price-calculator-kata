namespace PriceCalculator

open System
open PriceCalculation

module PriceReportGenerator = 
    let private listFilterSome = 
        List.choose id

    let private someWhenGreaterThan0 amount someValue = 
        if amount > 0m then Some someValue else None

    let generatePriceReport price = 
        [
            sprintf "Tax amount = $%M" price.TaxAmount |> someWhenGreaterThan0 price.TaxAmount
            sprintf "Discount amount = $%M" price.DiscountAmount |> someWhenGreaterThan0 price.DiscountAmount
            sprintf "Price before = $%M, price after = $%M" price.BaseAmount price.FinalAmount |> Some
        ]
            |> listFilterSome
            |> String.concat Environment.NewLine